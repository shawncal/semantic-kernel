// Copyright (c) Microsoft. All rights reserved.

using System;
using System.Threading;
using Microsoft.Extensions.DependencyInjection;

namespace Microsoft.SemanticKernel.Context;

internal class DefaultFunctionContextFactory : IFunctionContextFactory
{
    private readonly IServiceScopeFactory _serviceScopeFactory;

    public DefaultFunctionContextFactory(IServiceScopeFactory serviceScopeFactory)
    {
        this._serviceScopeFactory = serviceScopeFactory ?? throw new ArgumentNullException(nameof(serviceScopeFactory));
    }

    public FunctionContext Create(IInvocationFeatures features, CancellationToken cancellationToken = default)
    {
        return new DefaultFunctionContext(this._serviceScopeFactory, features, cancellationToken);
    }
}
