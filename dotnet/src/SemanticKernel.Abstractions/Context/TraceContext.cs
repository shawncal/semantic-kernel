// Copyright (c) Microsoft. All rights reserved.

namespace Microsoft.SemanticKernel.Context;

/// <summary>
/// The trace context for the current invocation.
/// </summary>
public abstract class TraceContext
{
    /// <summary>
    /// Gets the identity of the incoming invocation in a tracing system.
    /// </summary>
    public abstract string TraceParent { get; }

    /// <summary>
    /// Gets the state data.
    /// </summary>
    public abstract string TraceState { get; }
}
