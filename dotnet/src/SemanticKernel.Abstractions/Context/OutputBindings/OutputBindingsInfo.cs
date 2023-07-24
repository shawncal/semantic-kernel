// Copyright (c) Microsoft. All rights reserved.

namespace Microsoft.SemanticKernel.Context.OutputBindings;

/// <summary>
/// Encapsulates the information about all output bindings in a Function
/// </summary>
internal abstract class OutputBindingsInfo
{
    /// <summary>
    /// Binds output from a function <paramref name="context"/> to the output bindings
    /// </summary>
    /// <param name="context">The Function context to bind the data to.</param>
    public abstract void BindOutputInContext(FunctionContext context);
}
