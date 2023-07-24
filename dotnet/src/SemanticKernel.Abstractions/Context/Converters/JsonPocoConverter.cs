// Copyright (c) Microsoft. All rights reserved.

using System;
using System.IO;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Microsoft.SemanticKernel.Context.Converters;

internal class JsonPocoConverter : IInputConverter
{
    public JsonPocoConverter()
    {
    }

    public async ValueTask<ConversionResult> ConvertAsync(ConverterContext context)
    {
        if (context.TargetType == typeof(string))
        {
            return ConversionResult.Unhandled();
        }

        byte[]? bytes = null;

        if (context.Source is string sourceString)
        {
            bytes = Encoding.UTF8.GetBytes(sourceString);
        }
        else if (context.Source is ReadOnlyMemory<byte> sourceMemory)
        {
            bytes = sourceMemory.ToArray();
        }

        if (bytes == null)
        {
            return ConversionResult.Unhandled();
        }

        return await this.GetConversionResultFromDeserialization(bytes, context.TargetType).ConfigureAwait(false);
    }

    private async Task<ConversionResult> GetConversionResultFromDeserialization(byte[] bytes, Type type)
    {
        Stream? stream = null;

#pragma warning disable CA1031 // Do not catch general exception types
        try
        {
            stream = new MemoryStream(bytes);

            var deserializedObject = await JsonSerializer.DeserializeAsync(stream, type).ConfigureAwait(false);
            return ConversionResult.Success(deserializedObject);
        }
        catch (Exception ex)
        {
            return ConversionResult.Failed(ex);
        }
        finally
        {
            if (stream != null)
            {
#if NET5_0_OR_GREATER

                await ((IAsyncDisposable)stream).DisposeAsync();
#else
                ((IDisposable)stream).Dispose();
#endif
            }
        }
#pragma warning restore CA1031 // Do not catch general exception types
    }
}
