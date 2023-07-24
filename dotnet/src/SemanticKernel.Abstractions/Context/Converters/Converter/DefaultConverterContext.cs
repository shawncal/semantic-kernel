// Copyright (c) Microsoft. All rights reserved.

using System;
using System.Collections.Generic;

namespace Microsoft.SemanticKernel.Context.Converters;

/// <summary>
/// A type defining the information needed for an input conversion operation.
/// </summary>
internal sealed class DefaultConverterContext : ConverterContext
{
    public DefaultConverterContext(Type targetType, object? source, FunctionContext context, IReadOnlyDictionary<string, object> properties)
    {
        this.TargetType = targetType ?? throw new ArgumentNullException(nameof(context));
        this.Source = source;
        this.FunctionContext = context ?? throw new ArgumentNullException(nameof(context));
        this.Properties = properties ?? throw new ArgumentNullException(nameof(properties));
    }

    /// <inheritdoc/>
    public override Type TargetType { get; }

    /// <inheritdoc/>
    public override object? Source { get; }

    /// <inheritdoc/>
    public override FunctionContext FunctionContext { get; }

    /// <inheritdoc/>
    public override IReadOnlyDictionary<string, object> Properties { get; }
}
