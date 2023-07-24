// Copyright (c) Microsoft. All rights reserved.

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.SemanticKernel.Diagnostics;
using Microsoft.SemanticKernel.Context.Worker;

namespace Microsoft.SemanticKernel.Context;

internal sealed class DefaultFunctionContext : FunctionContext, IAsyncDisposable
{
    private readonly IServiceScopeFactory _serviceScopeFactory;
    private readonly FunctionInvocation _invocation;
    private IServiceScope? _instanceServicesScope;
    private IServiceProvider? _instanceServices;

    public DefaultFunctionContext(IServiceScopeFactory serviceScopeFactory, IInvocationFeatures features, CancellationToken cancellationToken = default)
    {
        Verify.NotNull(serviceScopeFactory);
        Verify.NotNull(features);

        this._serviceScopeFactory = serviceScopeFactory;
        this.Features = features;
        this.CancellationToken = cancellationToken;

        this._invocation = features.Get<FunctionInvocation>() ?? throw new InvalidOperationException($"The '{nameof(FunctionInvocation)}' feature is required.");
        this.FunctionDefinition = features.Get<FunctionDefinition>() ?? throw new InvalidOperationException($"The {nameof(Worker.FunctionDefinition)} feature is required.");
    }

    public override string InvocationId => this._invocation.Id;

    public override string FunctionId => this._invocation.FunctionId;

    public override FunctionDefinition FunctionDefinition { get; }

    public override IDictionary<object, object> Items { get; set; } = new Dictionary<object, object>();

    public override IInvocationFeatures Features { get; }

    public override CancellationToken CancellationToken { get; }

    public override IServiceProvider InstanceServices
    {
        get
        {
            if (this._instanceServicesScope == null)
            {
                this._instanceServicesScope = this._serviceScopeFactory.CreateScope();
                this._instanceServices = this._instanceServicesScope.ServiceProvider;
            }

            return this._instanceServices!;
        }

        set => this._instanceServices = value;
    }

    //public override TraceContext TraceContext => this._invocation.TraceContext;

    public override TraceContext TraceContext => throw new NotImplementedException();

    public override BindingContext BindingContext => throw new NotImplementedException();

    public override RetryContext RetryContext => this.Features.GetRequired<IExecutionRetryFeature>().Context;

    public async ValueTask DisposeAsync()
    {
        if (this._instanceServicesScope is IAsyncDisposable asyncServiceScope)
        {
            await asyncServiceScope.DisposeAsync().ConfigureAwait(false);
        }

        this._instanceServicesScope?.Dispose();

        this._instanceServicesScope = null;
        this._instanceServices = null;
    }
}
