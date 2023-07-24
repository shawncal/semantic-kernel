// Copyright (c) Microsoft. All rights reserved.

using System.Threading.Tasks;

namespace Microsoft.SemanticKernel.Context.Converters;

internal class FunctionContextConverter : IInputConverter
{
    public ValueTask<ConversionResult> ConvertAsync(ConverterContext context)
    {
        // Special handling for the context.
        if (context.TargetType == typeof(FunctionContext))
        {
            return new ValueTask<ConversionResult>(ConversionResult.Success(context.FunctionContext));
        }

        return new ValueTask<ConversionResult>(ConversionResult.Unhandled());
    }
}
