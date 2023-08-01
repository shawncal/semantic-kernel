﻿// Copyright (c) Microsoft. All rights reserved.

using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.SemanticKernel.AI.TextCompletion;
using Microsoft.SemanticKernel.Orchestration;
using Microsoft.SemanticKernel.SkillDefinition;

#pragma warning disable IDE0130 // Namespace does not match folder structure
namespace Microsoft.SemanticKernel.SemanticFunctions;
#pragma warning restore IDE0130 // Namespace does not match folder structure

/// <summary>
/// Semantic Kernel callable function interface
/// </summary>
public interface IPromptFunction : ISKFunction
{
    /// <summary>
    /// AI service settings
    /// </summary>
    CompleteRequestSettings RequestSettings { get; }

    /// <summary>
    /// Invoke the <see cref="ISKFunction"/>.
    /// </summary>
    /// <param name="context">SK context</param>
    /// <param name="settings">LLM completion settings (for semantic functions only)</param>
    /// <returns>The updated context, potentially a new one if context switching is implemented.</returns>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> to monitor for cancellation requests. The default is <see cref="CancellationToken.None"/>.</param>
    Task<SKContext> InvokeAsync(
        SKContext context,
        CompleteRequestSettings? settings = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Set the AI service used by the semantic function, passing a factory method.
    /// The factory allows to lazily instantiate the client and to properly handle its disposal.
    /// </summary>
    /// <param name="serviceFactory">AI service factory</param>
    /// <returns>Self instance</returns>
    IPromptFunction SetAIService(Func<ITextCompletion> serviceFactory);

    /// <summary>
    /// Set the AI completion settings used with LLM requests
    /// </summary>
    /// <param name="settings">LLM completion settings</param>
    /// <returns>Self instance</returns>
    IPromptFunction SetAIConfiguration(CompleteRequestSettings settings);
}
