// Copyright (c) Microsoft. All rights reserved.

using System.Collections.Immutable;

namespace Microsoft.SemanticKernel.Context.Worker;

/// <summary>
/// A representation of a function.
/// </summary>
public abstract class FunctionDefinition
{
    /// <summary>
    /// Gets the parameters for the function.
    /// </summary>
    public abstract ImmutableArray<FunctionParameter> Parameters { get; }

    /// <summary>
    /// Gets the path to the assembly that contains the function.
    /// </summary>
    public abstract string PathToAssembly { get; }

    /// <summary>
    /// Gets the method entry point to the function.
    /// </summary>
    public abstract string EntryPoint { get; }

    /// <summary>
    /// Gets the unique function id, assigned by the Functions host.
    /// </summary>
    public abstract string Id { get; }

    /// <summary>
    /// Gets the unique function name.
    /// </summary>
    public abstract string Name { get; }

    /// <summary>
    /// Gets the input binding metadata.
    /// </summary>
    public abstract IImmutableDictionary<string, BindingMetadata> InputBindings { get; }

    /// <summary>
    /// Gets the output binding metadata.
    /// </summary>
    public abstract IImmutableDictionary<string, BindingMetadata> OutputBindings { get; }
}
