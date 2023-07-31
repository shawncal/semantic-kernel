// Copyright (c) Microsoft. All rights reserved.

using System.Globalization;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.SemanticKernel.Orchestration;
using Microsoft.SemanticKernel.SkillDefinition;

#pragma warning disable IDE0130 // Namespace does not match folder structure
namespace Microsoft.SemanticKernel;
#pragma warning restore IDE0130 // Namespace does not match folder structure

/// <summary>
/// Class that holds extension methods for objects implementing ISKFunction.
/// </summary>
public static class SKFunctionExtensions
{
    /// <summary>
    /// Execute a function allowing to pass the main input separately from the rest of the context.
    /// </summary>
    /// <param name="function">Function to execute</param>
    /// <param name="variables">Input variables for the function</param>
    /// <param name="skills">Skills that the function can access</param>
    /// <param name="culture">Culture to use for the function execution</param>
    /// <param name="logger">Logger to use for the function execution</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> to monitor for cancellation requests. The default is <see cref="CancellationToken.None"/>.</param>
    /// <returns>The result of the function execution</returns>
    public static Task<SKContext> InvokeAsync(this ISKFunction function,
        ContextVariables? variables = null,
        IReadOnlySkillCollection? skills = null,
        CultureInfo? culture = null,
        ILogger? logger = null,
        CancellationToken cancellationToken = default)
    {
        var context = new SKContext(variables, skills, logger)
        {
            Culture = culture!
        };

        return function.InvokeAsync(context, cancellationToken);
    }

    /// <summary>
    /// Execute a function allowing to pass the main input separately from the rest of the context.
    /// </summary>
    /// <param name="function">Function to execute</param>
    /// <param name="input">Input string for the function</param>
    /// <param name="skills">Skills that the function can access</param>
    /// <param name="culture">Culture to use for the function execution</param>
    /// <param name="logger">Logger to use for the function execution</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> to monitor for cancellation requests. The default is <see cref="CancellationToken.None"/>.</param>
    /// <returns>The result of the function execution</returns>
    public static Task<SKContext> InvokeAsync(this ISKFunction function,
        string input,
        IReadOnlySkillCollection? skills = null,
        CultureInfo? culture = null,
        ILogger? logger = null,
        CancellationToken cancellationToken = default)
        => function.InvokeAsync(new ContextVariables(input), skills, culture, logger, cancellationToken);

    /// <summary>
    /// Returns decorated instance of <see cref="ISKFunction"/> with enabled instrumentation.
    /// </summary>
    /// <param name="function">Instance of <see cref="ISKFunction"/> to decorate.</param>
    /// <param name="logger">Optional logger.</param>
    public static ISKFunction WithInstrumentation(this ISKFunction function, ILogger? logger = null)
    {
        return new InstrumentedSKFunction(function, logger);
    }
}
