// Copyright (c) Microsoft. All rights reserved.

using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.SemanticKernel.Context.Converters;

internal class CancellationTokenConverter : IInputConverter
{
    public ValueTask<ConversionResult> ConvertAsync(ConverterContext context)
    {
        if (context.TargetType == typeof(CancellationToken) || context.TargetType == typeof(CancellationToken?))
        {
            return new ValueTask<ConversionResult>(ConversionResult.Success(context.FunctionContext.CancellationToken));
        }

        return new ValueTask<ConversionResult>(ConversionResult.Unhandled());
    }
}
