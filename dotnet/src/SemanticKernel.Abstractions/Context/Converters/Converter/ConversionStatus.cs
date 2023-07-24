// Copyright (c) Microsoft. All rights reserved.

namespace Microsoft.SemanticKernel.Context.Converters;

/// <summary>
/// Conversion status enumeration.
/// </summary>
public enum ConversionStatus
{
    /// <summary>
    /// Converter did not act on the input to execute a conversion operation.
    /// </summary>
    Unhandled,

    /// <summary>
    /// Conversion operation was successful.
    /// </summary>
    Succeeded,

    /// <summary>
    /// Conversion operation failed.
    /// </summary>
    Failed
}
