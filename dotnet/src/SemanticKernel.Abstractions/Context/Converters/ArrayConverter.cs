// Copyright (c) Microsoft. All rights reserved.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Microsoft.SemanticKernel.Context.Converters;

// Converting IEnumerable<> to Array
internal class ArrayConverter : IInputConverter
{
    // Convert IEnumerable to array
    public ValueTask<ConversionResult> ConvertAsync(ConverterContext context)
    {
        object? target = null;
        // Ensure requested type is an array
        if (context.TargetType.IsArray)
        {
            Type? elementType = context.TargetType.GetElementType();
            if (elementType is not null)
            {
                // Ensure that we can assign from source to parameter type
                if (elementType.Equals(typeof(string))
                    || elementType.Equals(typeof(byte[]))
                    || elementType.Equals(typeof(ReadOnlyMemory<byte>))
                    || elementType.Equals(typeof(long))
                    || elementType.Equals(typeof(double)))
                {
                    target = context.Source switch
                    {
                        IEnumerable<string> source => source.ToArray(),
                        IEnumerable<ReadOnlyMemory<byte>> source => GetBinaryData(source, elementType!),
                        IEnumerable<double> source => source.ToArray(),
                        IEnumerable<long> source => source.ToArray(),
                        _ => null
                    };
                }
            }
        }

        return new ValueTask<ConversionResult>((target is not null)
            ? ConversionResult.Success(target)
            : ConversionResult.Unhandled());
    }

    private static object? GetBinaryData(IEnumerable<ReadOnlyMemory<byte>> source, Type targetType)
        => targetType.IsAssignableFrom(typeof(ReadOnlyMemory<byte>))
               ? source.ToArray()
               : source.Select(i => i.ToArray()).ToArray();
}
