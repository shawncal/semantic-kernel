// Copyright (c) Microsoft. All rights reserved.

using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.SemanticKernel.Orchestration;

#pragma warning disable IDE0130
// ReSharper disable once CheckNamespace - Using NS of ISKFunction
namespace Microsoft.SemanticKernel.SkillDefinition;
#pragma warning restore IDE0130

/// <summary>
/// Class with extension methods for semantic functions.
/// </summary>
public static class SKFunctionTextExtensions
{
    /// <summary>
    /// Extension method to aggregate partitioned results of a semantic function.
    /// </summary>
    /// <param name="func">Semantic Kernel function</param>
    /// <param name="partitionedInput">Input to aggregate.</param>
    /// <param name="context">Semantic Kernel context.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> to monitor for cancellation requests. The default is <see cref="CancellationToken.None"/>.</param>
    /// <returns>Aggregated results.</returns>
    public static async Task<SKContext> AggregatePartitionedResultsAsync(
        this ISKFunction func,
        List<string> partitionedInput,
        SKContext context,
        CancellationToken cancellationToken = default)
    {
        StringBuilder results = new();
        foreach (var partition in partitionedInput)
        {
            context.Variables.Update(partition);
            FunctionResult stepResult = await func.InvokeAsync(context, cancellationToken: cancellationToken).ConfigureAwait(false);
            string? resultContent = await stepResult.ReadContentAsync(cancellationToken).ConfigureAwait(false);

            if (!string.IsNullOrEmpty(resultContent))
            {
                results.AppendLine(resultContent);
            }
        }

        context.Variables.Update(results.ToString());
        return context;
    }
}
