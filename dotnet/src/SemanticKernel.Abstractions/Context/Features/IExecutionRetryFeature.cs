// Copyright (c) Microsoft. All rights reserved.

namespace Microsoft.SemanticKernel.Context;

internal interface IExecutionRetryFeature
{
    RetryContext Context { get; }
}
