// Copyright (c) Microsoft. All rights reserved.

using System;
using System.Threading.Tasks;

namespace Microsoft.SemanticKernel.Context.Converters;

/// <summary>
/// Converter to bind Guid/Guid? type parameters.
/// </summary>
internal class GuidConverter : IInputConverter
{
    public ValueTask<ConversionResult> ConvertAsync(ConverterContext context)
    {
        if (context.TargetType == typeof(Guid) || context.TargetType == typeof(Guid?))
        {
            if (context.Source is string sourceString && Guid.TryParse(sourceString, out Guid parsedGuid))
            {
                return new ValueTask<ConversionResult>(ConversionResult.Success(parsedGuid));
            }
        }

        return new ValueTask<ConversionResult>(ConversionResult.Unhandled());
    }
}
