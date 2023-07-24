// Copyright (c) Microsoft. All rights reserved.

namespace Microsoft.SemanticKernel.Context;
internal sealed class DefaultRetryContext : RetryContext
{
    public DefaultRetryContext(int retryCount, int maxRetryCount)
    {
        this.RetryCount = retryCount;
        this.MaxRetryCount = maxRetryCount;
    }

    public override int RetryCount { get; }

    public override int MaxRetryCount { get; }
}
