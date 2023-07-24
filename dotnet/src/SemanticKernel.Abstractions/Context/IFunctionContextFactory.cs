// Copyright (c) Microsoft. All rights reserved.

using System.Threading;

namespace Microsoft.SemanticKernel.Context;

internal interface IFunctionContextFactory
{
    FunctionContext Create(IInvocationFeatures features, CancellationToken cancellationToken);
}
