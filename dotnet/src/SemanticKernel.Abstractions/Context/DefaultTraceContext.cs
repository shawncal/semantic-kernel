// Copyright (c) Microsoft. All rights reserved.

namespace Microsoft.SemanticKernel.Context;

internal sealed class DefaultTraceContext : TraceContext
{
    public DefaultTraceContext(string traceParent, string traceState)
    {
        this.TraceParent = traceParent;
        this.TraceState = traceState;
    }

    public override string TraceParent { get; }

    public override string TraceState { get; }
}
