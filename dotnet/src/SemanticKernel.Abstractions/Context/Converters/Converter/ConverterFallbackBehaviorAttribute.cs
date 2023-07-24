// Copyright (c) Microsoft. All rights reserved.

using System;
using System.Diagnostics.CodeAnalysis;

namespace Microsoft.SemanticKernel.Context.Converters;

/// <summary>
/// An attribute that specifies if converter fallback is allowed or disallowed.
/// Converter fallback refers to the ability to use built-in converters when custom converters
/// cannot handle a given request.
/// The default converter fallback behavior is <see cref="ConverterFallbackBehavior.Allow"/>.
/// </summary>
[AttributeUsage(AttributeTargets.Class)]
public sealed class ConverterFallbackBehaviorAttribute : Attribute
{
    /// <summary>
    /// Gets the value of the converter fallback behavior.
    /// </summary>
    public ConverterFallbackBehavior Behavior { get; }

    /// <summary>
    /// Creates a new instance of <see cref="ConverterFallbackBehaviorAttribute"/>
    /// </summary>
    /// <param name="fallbackBehavior">The value to indicate if converter fallback is allowed or disallowed.</param>
    [SuppressMessage("Design", "CA1019:Define accessors for attribute arguments")]
    public ConverterFallbackBehaviorAttribute(ConverterFallbackBehavior fallbackBehavior)
        => this.Behavior = fallbackBehavior;
}
