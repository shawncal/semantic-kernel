// Copyright (c) Microsoft. All rights reserved.

using System.Threading;
using System.Threading.Tasks;
using Microsoft.SemanticKernel.AI.TextCompletion;
using Microsoft.SemanticKernel.Orchestration;
using Microsoft.SemanticKernel.SkillDefinition;

namespace Microsoft.SemanticKernel.SemanticFunctions;

/// <summary>
/// Semantic Kernel callable function interface
/// </summary>
public interface IPromptFunction : ISKFunction
{
    /// <summary>
    /// AI service settings
    /// </summary>
    CompleteRequestSettings DefaultRequestSettings { get; }

    /// <summary>
    /// Invoke the <see cref="ISKFunction"/>.
    /// </summary>
    /// <param name="context">SK context</param>
    /// <param name="settings">LLM completion settings (for semantic functions only)</param>
    /// <returns>The updated context, potentially a new one if context switching is implemented.</returns>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> to monitor for cancellation requests. The default is <see cref="CancellationToken.None"/>.</param>
    Task<SKContext> InvokeAsync(
        SKContext context,
        CompleteRequestSettings? settings,
        CancellationToken cancellationToken = default);
}
