// Copyright (c) Microsoft. All rights reserved.

namespace Microsoft.SemanticKernel.Context.Converters;

/// <summary>
/// Specifies the fallback behavior for a converter.
/// The default behavior is <see cref="ConverterFallbackBehavior.Allow"/>.
/// </summary>
[System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1027:Mark enums with FlagsAttribute")]
public enum ConverterFallbackBehavior
{
    /// <summary>
    /// Allows fallback to built-in converters. This is the default behavior.
    /// </summary>
    Allow = 0,

    /// <summary>
    /// Disallows fallback to built-in converters.
    /// </summary>
    Disallow = 1,

    /// <summary>
    /// Specifies the default fallback behavior as <see cref="ConverterFallbackBehavior.Allow"/>
    /// </summary>
    Default = Allow
}
