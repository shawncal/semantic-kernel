// Copyright (c) Microsoft. All rights reserved.

using System.Collections.Generic;
using Microsoft.SemanticKernel.Context.OutputBindings;

namespace Microsoft.SemanticKernel.Context;

internal interface IFunctionBindingsFeature
{
    public IReadOnlyDictionary<string, object?> TriggerMetadata { get; }

    public IReadOnlyDictionary<string, object?> InputData { get; }

    public IDictionary<string, object?> OutputBindingData { get; }

    public OutputBindingsInfo OutputBindingsInfo { get; }

    public object? InvocationResult { get; set; }
}
