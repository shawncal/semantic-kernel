// Copyright (c) Microsoft. All rights reserved.

using System.Threading.Tasks;

namespace Microsoft.SemanticKernel.Context.Converters;

/// <summary>
/// Provides a mechanism for input conversion.
/// </summary>
public interface IInputConverter
{
    /// <summary>
    /// Executes an input conversion operation using the converter context provided.
    /// </summary>
    /// <param name="context">The converter context instance.</param>
    /// <returns>An instance of <see cref="ConversionResult"/> representing the result of the convert operation.</returns>
    ValueTask<ConversionResult> ConvertAsync(ConverterContext context);
}
