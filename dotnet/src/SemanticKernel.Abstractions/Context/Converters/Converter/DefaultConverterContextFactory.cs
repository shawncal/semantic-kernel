// Copyright (c) Microsoft. All rights reserved.

using System;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace Microsoft.SemanticKernel.Context.Converters;

/// <summary>
/// A factory for creating <see cref="ConverterContext"/> instances.
/// </summary>
internal sealed class DefaultConverterContextFactory : IConverterContextFactory
{
    /// <inheritdoc/>
    public ConverterContext Create(Type targetType, object? source, FunctionContext functionContext)
    {
        return this.Create(targetType, source, functionContext, ImmutableDictionary<string, object>.Empty);
    }

    /// <inheritdoc/>
    public ConverterContext Create(Type targetType, object? source, FunctionContext functionContext,
        IReadOnlyDictionary<string, object> properties)
    {
        return new DefaultConverterContext(targetType, source, functionContext, properties);
    }
}
