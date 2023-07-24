// Copyright (c) Microsoft. All rights reserved.

using System;

namespace Microsoft.SemanticKernel.Context;

internal interface IInvocationFeatureProvider
{
    bool TryCreate(Type type, out object? feature);
}
