// Copyright (c) Microsoft. All rights reserved.

using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.SemanticKernel.Orchestration;

namespace Microsoft.SemanticKernel.SkillDefinition;

public sealed class FunctionResult
{
    internal IAsyncEnumerable<string> ContentStream { get; private set; }

    internal Task<string> ReadContentAsync(CancellationToken cancellationToken = default)
    {
        this.AccumulateBufferTask ??= this.AccumulateBufferAsync(cancellationToken);
        return this.AccumulateBufferTask;
    }

    internal FunctionResult(IAsyncEnumerable<string> resultStream)
    {
        this.ContentStream = resultStream;
    }

    internal FunctionResult(string result)
    {
        this.ContentStream = new[] { result }.ToAsyncEnumerable();
    }

    public static implicit operator SKContext(FunctionResult result)
    {
        var resultValue = result.AccumulateBufferAsync().Result; // TODO: Fix this before checkin
        var variables = new ContextVariables(resultValue);
        return new SKContext(variables);
    }

    public static implicit operator FunctionResult(SKContext context)
    {
        return new FunctionResult(context.Result);
    }

    #region private

    private Task<string>? AccumulateBufferTask = null;

    private async Task<string> AccumulateBufferAsync(CancellationToken cancellationToken = default)
    {
        // Accumulates all the content from the ContentStream and then returns a single string.
        var sb = new StringBuilder();
        await foreach (var content in this.ContentStream.WithCancellation(cancellationToken))
        {
            sb.Append(content);
        }

        return sb.ToString();
    }

    #endregion
}
