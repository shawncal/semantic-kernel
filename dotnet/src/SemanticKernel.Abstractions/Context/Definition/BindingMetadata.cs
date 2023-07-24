// Copyright (c) Microsoft. All rights reserved.

namespace Microsoft.SemanticKernel.Context.Worker;

/// <summary>
/// Specifies the direction of the binding.
/// </summary>
public enum BindingDirection
{
    /// <summary>
    /// Identifies an input binding; a binding that provides data to the function.
    /// </summary>
    In,
    /// <summary>
    /// Identifies an output binding; a binding that receives data from the function.
    /// </summary>
    Out
}

/// <summary>
/// Contains metadata about an Azure Functions binding.
/// </summary>
public abstract class BindingMetadata
{
    /// <summary>
    /// Gets the name of the binding metadata entry.
    /// </summary>
    public abstract string Name { get; }

    /// <summary>
    /// Gets the type of the binding. For example, "httpTrigger".
    /// </summary>
    public abstract string Type { get; }

    /// <summary>
    /// Gets the binding direction.
    /// </summary>
    public abstract BindingDirection Direction { get; }
}
