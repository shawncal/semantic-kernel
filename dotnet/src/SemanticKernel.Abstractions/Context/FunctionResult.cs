// Copyright (c) Microsoft. All rights reserved.

using System.IO;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.SemanticKernel.Context;

/// <summary>
/// What surfaces to the SK caller -- not used within a function
/// </summary>
public class FunctionResult
{
    public Stream Content { get; internal set; } = Stream.Null;
    public string? ContentType { get; internal set; }

    public Task<string> ReadAsStringAsync(CancellationToken cancellationToken = default)
    {
        using var reader = new StreamReader(this.Content);
        return reader.ReadToEndAsync();
    }

    public Task<T?> ReadAsJsonAsync<T>(JsonSerializerOptions? options = null, CancellationToken cancellationToken = default)
    {
        return JsonSerializer.DeserializeAsync<T>(this.Content, options, cancellationToken).AsTask();
    }
}
