// Copyright (c) Microsoft. All rights reserved.

using System;
using System.Threading.Tasks;

namespace Microsoft.SemanticKernel.Context.Converters;

internal class TypeConverter : IInputConverter
{
    public ValueTask<ConversionResult> ConvertAsync(ConverterContext context)
    {
        Type? sourceType = context.Source?.GetType();

        if (sourceType is not null &&
            context.TargetType.IsAssignableFrom(sourceType))
        {
            var conversionResult = ConversionResult.Success(context.Source);
            return new ValueTask<ConversionResult>(conversionResult);
        }

        return new ValueTask<ConversionResult>(ConversionResult.Unhandled());
    }
}
