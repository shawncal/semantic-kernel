// Copyright (c) Microsoft. All rights reserved.

namespace Microsoft.SemanticKernel.Context;

/// <summary>
/// A representation of the a single function invocation.
/// </summary>
public abstract class FunctionInvocation
{
    /// <summary>
    /// The invocation id.
    /// </summary>
    public abstract string Id { get; }

    /// <summary>
    /// The function id, typically assigned by the host.
    /// </summary>
    public abstract string FunctionId { get; }

    /// <summary>
    /// Gets the distributed tracing context.
    /// </summary>
    public abstract TraceContext TraceContext { get; }
}
