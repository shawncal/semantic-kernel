// Copyright (c) Microsoft. All rights reserved.

using System;
using System.ComponentModel;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.SemanticKernel.Memory;
using Microsoft.SemanticKernel.Orchestration;
using Microsoft.SemanticKernel.Services;
using Microsoft.SemanticKernel.SkillDefinition;
using Microsoft.SemanticKernel.TemplateEngine;

namespace Microsoft.SemanticKernel;

/// <summary>
/// Interface for the semantic kernel.
/// </summary>
public interface IKernel
{
    /// <summary>
    /// Settings required to execute functions, including details about AI dependencies, e.g. endpoints and API keys.
    /// </summary>
    KernelConfig Config { get; }

    /// <summary>
    /// App logger
    /// </summary>
    ILogger Logger { get; }

    /// <summary>
    /// Semantic memory instance
    /// </summary>
    ISemanticTextMemory Memory { get; }

    /// <summary>
    /// Reference to the engine rendering prompt templates
    /// </summary>
    IPromptTemplateEngine PromptTemplateEngine { get; }

    /// <summary>
    /// Reference to the read-only skill collection containing all the imported functions
    /// </summary>
    IReadOnlySkillCollection Skills { get; }

    /// <summary>
    /// Registers a custom function in the internal skill collection.
    /// </summary>
    /// <param name="skFunction">The <see cref="ISKFunction"/> function to register.</param>
    /// <returns>A C# function wrapping the function execution logic.</returns>
    ISKFunction RegisterFunction(ISKFunction skFunction);

    /// <summary>
    /// Set the semantic memory to use
    /// </summary>
    /// <param name="memory">Semantic memory instance</param>
    void RegisterMemory(ISemanticTextMemory memory);

    /// <summary>
    /// Run a single synchronous or asynchronous <see cref="ISKFunction"/>.
    /// </summary>
    /// <param name="skFunction">A Semantic Kernel function to run</param>
    /// <param name="variables">Input to process</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> to monitor for cancellation requests. The default is <see cref="CancellationToken.None"/>.</param>
    /// <returns>Result of the function</returns>
    Task<SKContext> RunAsync(
        ISKFunction skFunction,
        ContextVariables? variables = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Run a pipeline composed of synchronous and asynchronous functions.
    /// </summary>
    /// <param name="pipeline">List of functions</param>
    /// <returns>Result of the function composition</returns>
    Task<SKContext> RunAsync(
        params ISKFunction[] pipeline);

    /// <summary>
    /// Run a pipeline composed of synchronous and asynchronous functions.
    /// </summary>
    /// <param name="input">Input to process</param>
    /// <param name="pipeline">List of functions</param>
    /// <returns>Result of the function composition</returns>
    Task<SKContext> RunAsync(
        string input,
        params ISKFunction[] pipeline);

    /// <summary>
    /// Run a pipeline composed of synchronous and asynchronous functions.
    /// </summary>
    /// <param name="variables">Input to process</param>
    /// <param name="pipeline">List of functions</param>
    /// <returns>Result of the function composition</returns>
    Task<SKContext> RunAsync(
        ContextVariables variables,
        params ISKFunction[] pipeline);

    /// <summary>
    /// Run a pipeline composed of synchronous and asynchronous functions.
    /// </summary>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> to monitor for cancellation requests. The default is <see cref="CancellationToken.None"/>.</param>
    /// <param name="pipeline">List of functions</param>
    /// <returns>Result of the function composition</returns>
    Task<SKContext> RunAsync(
        CancellationToken cancellationToken,
        params ISKFunction[] pipeline);

    /// <summary>
    /// Run a pipeline composed of synchronous and asynchronous functions.
    /// </summary>
    /// <param name="input">Input to process</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> to monitor for cancellation requests. The default is <see cref="CancellationToken.None"/>.</param>
    /// <param name="pipeline">List of functions</param>
    /// <returns>Result of the function composition</returns>
    Task<SKContext> RunAsync(
        string input,
        CancellationToken cancellationToken,
        params ISKFunction[] pipeline);

    /// <summary>
    /// Run a pipeline composed of synchronous and asynchronous functions.
    /// </summary>
    /// <param name="variables">Input to process</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> to monitor for cancellation requests. The default is <see cref="CancellationToken.None"/>.</param>
    /// <param name="pipeline">List of functions</param>
    /// <returns>Result of the function composition</returns>
    Task<SKContext> RunAsync(
        ContextVariables variables,
        CancellationToken cancellationToken,
        params ISKFunction[] pipeline);

    /// <summary>
    /// Access registered functions by skill + name. Not case sensitive.
    /// The function might be native or semantic, it's up to the caller handling it.
    /// </summary>
    /// <param name="skillName">Skill name</param>
    /// <param name="functionName">Function name</param>
    /// <returns>Delegate to execute the function</returns>
    ISKFunction Func(string skillName, string functionName);

    /// <summary>
    /// Create a new instance of a context, linked to the kernel internal state.
    /// </summary>
    /// <returns>SK context</returns>
    SKContext CreateNewContext();

    /// <summary>
    /// Get one of the configured services. Currently limited to AI services.
    /// </summary>
    /// <param name="name">Optional name. If the name is not provided, returns the default T available</param>
    /// <typeparam name="T">Service type</typeparam>
    /// <returns>Instance of T</returns>
    T GetService<T>(string? name = null) where T : IAIService;

    #region Obsolete

    /// <summary>
    /// App logger
    /// </summary>
    [Obsolete("Use Logger instead. This will be removed in a future release.")]
    [EditorBrowsable(EditorBrowsableState.Never)]
    ILogger Log { get; }

    /// <summary>
    /// Create a new instance of a context, linked to the kernel internal state.
    /// </summary>
    /// <param name="cancellationToken">Optional cancellation token for operations in context.</param>
    /// <returns>SK context</returns>
    [Obsolete("SKContext no longer contains the CancellationToken. Use CreateNewContext().")]
    [EditorBrowsable(EditorBrowsableState.Never)]
    SKContext CreateNewContext(CancellationToken cancellationToken);

    /// <summary>
    /// Registers a custom function in the internal skill collection.
    /// </summary>
    /// <param name="customFunction">The custom function to register.</param>
    /// <returns>A C# function wrapping the function execution logic.</returns>
    [Obsolete("Use RegisterFunction instead. This will be removed in a future release.")]
    [EditorBrowsable(EditorBrowsableState.Never)]
    ISKFunction RegisterCustomFunction(ISKFunction customFunction);

    #endregion
}
