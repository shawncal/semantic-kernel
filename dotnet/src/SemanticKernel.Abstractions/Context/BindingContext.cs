// Copyright (c) Microsoft. All rights reserved.

using System.Collections.Generic;

namespace Microsoft.SemanticKernel.Context;

/// <summary>
/// Exposes binding infomation for a given function context.
/// </summary>
public abstract class BindingContext
{
    /// <summary>
    /// Gets the binding data information for the current context.
    /// This contains all of the trigger defined metadata.
    /// </summary>
    public abstract IReadOnlyDictionary<string, object?> BindingData { get; }
}
