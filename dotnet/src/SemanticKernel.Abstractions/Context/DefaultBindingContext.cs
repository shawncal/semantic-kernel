// Copyright (c) Microsoft. All rights reserved.

using System;
using System.Collections.Generic;

namespace Microsoft.SemanticKernel.Context;

internal sealed class DefaultBindingContext : BindingContext
{
    private readonly FunctionContext _functionContext;
    private IFunctionBindingsFeature? _functionBindings;

    public DefaultBindingContext(FunctionContext functionContext)
    {
        this._functionContext = functionContext ?? throw new ArgumentNullException(nameof(functionContext));
    }

    public override IReadOnlyDictionary<string, object?> BindingData
        => (this._functionBindings ??= this._functionContext.Features.GetRequired<IFunctionBindingsFeature>()).TriggerMetadata;
}
