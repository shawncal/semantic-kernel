// Copyright (c) Microsoft. All rights reserved.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.SemanticKernel.AI;
using Microsoft.SemanticKernel.AI.TextCompletion;
using Microsoft.SemanticKernel.Diagnostics;
using Microsoft.SemanticKernel.Orchestration;
using Microsoft.SemanticKernel.SkillDefinition;

#pragma warning disable IDE0130 // Namespace does not match folder structure

// TODO: For refactor only. Update namespace to match projec before checkin !!!!!!

namespace Microsoft.SemanticKernel.SemanticFunctions;
#pragma warning restore IDE0130 // Namespace does not match folder structure

#pragma warning disable format

/// <summary>
/// Standard Semantic Kernel callable function.
/// SKFunction is used to extend one C# <see cref="Delegate"/>, <see cref="Func{T, TResult}"/>, <see cref="Action"/>,
/// with additional methods required by the kernel.
/// </summary>
[DebuggerDisplay("{DebuggerDisplay,nq}")]
public sealed class PromptFunction : IPromptFunction, IDisposable
{
    /// <inheritdoc/>
    public string Name { get; }

    /// <inheritdoc/>
    public string SkillName { get; }

    /// <inheritdoc/>
    public string Description { get; }

    /// <inheritdoc/>
    public bool IsSemantic => true;

    /// <inheritdoc/>
    public CompleteRequestSettings RequestSettings => this._aiRequestSettings;

    /// <summary>
    /// List of function parameters
    /// </summary>
    public IList<ParameterView> Parameters { get; }

    /// <summary>
    /// Create a native function instance, given a semantic function configuration.
    /// </summary>
    /// <param name="skillName">Name of the skill to which the function to create belongs.</param>
    /// <param name="functionName">Name of the function to create.</param>
    /// <param name="functionConfig">Semantic function configuration.</param>
    /// <param name="logger">Optional logger for the function.</param>
    /// <returns>SK function instance.</returns>
    public static IPromptFunction FromSemanticConfig(
        string skillName,
        string functionName,
        SemanticFunctionConfig functionConfig,
        ILogger? logger = null)
    {
        Verify.NotNull(functionConfig);

        Task<SKContext> LocalFuncTmp(
            ITextCompletion? client,
            CompleteRequestSettings? requestSettings,
            SKContext context,
            CancellationToken cancellationToken)
        {
            return Task.FromResult(context);
        }

        var func = new PromptFunction(
            // Start with an empty delegate, so we can have a reference to func
            // to be used in the LocalFunc below
            // Before returning the delegateFunction will be updated to be LocalFunc
            delegateFunction: LocalFuncTmp,
            parameters: functionConfig.PromptTemplate.GetParameters(),
            description: functionConfig.PromptTemplateConfig.Description,
            skillName: skillName,
            functionName: functionName,
            logger: logger
        );

        async Task<SKContext> LocalFunc(
            ITextCompletion? client,
            CompleteRequestSettings? requestSettings,
            SKContext context,
            CancellationToken cancellationToken)
        {
            Verify.NotNull(client);
            Verify.NotNull(requestSettings);

            try
            {
                string renderedPrompt = await functionConfig.PromptTemplate.RenderAsync(context, cancellationToken).ConfigureAwait(false);
                var completionResults = await client.GetCompletionsAsync(renderedPrompt, requestSettings, cancellationToken).ConfigureAwait(false);
                string completion = await GetCompletionsResultContentAsync(completionResults, cancellationToken).ConfigureAwait(false);

                // Update the result with the completion
                context.Variables.Update(completion);

                context.ModelResults = completionResults.Select(c => c.ModelResult).ToArray();
            }
            catch (AIException ex)
            {
                const string Message = "Something went wrong while rendering the semantic function" +
                                       " or while executing the text completion. Function: {0}.{1}. Error: {2}. Details: {3}";
                logger?.LogError(ex, Message, skillName, functionName, ex.Message, ex.Detail);
                throw;
            }
            catch (Exception ex) when (!ex.IsCriticalException())
            {
                const string Message = "Something went wrong while rendering the semantic function" +
                                       " or while executing the text completion. Function: {0}.{1}. Error: {2}";
                logger?.LogError(ex, Message, skillName, functionName, ex.Message);
                throw;
            }

            return context;
        }

        // Update delegate function with a reference to the LocalFunc created
        func._function = LocalFunc;

        return func;
    }

    /// <inheritdoc/>
    public FunctionView Describe()
    {
        return new FunctionView
        {
            IsSemantic = this.IsSemantic,
            Name = this.Name,
            SkillName = this.SkillName,
            Description = this.Description,
            Parameters = this.Parameters,
        };
    }

    /// <inheritdoc/>
    public async Task<SKContext> InvokeAsync(
        SKContext context,
        CompleteRequestSettings? settings = null,
        CancellationToken cancellationToken = default)
    {
        this.AddDefaultValues(context.Variables);

        return await this._function(this._aiService?.Value, settings ?? this._aiRequestSettings, context, cancellationToken).ConfigureAwait(false);
    }

    /// <inheritdoc/>
    public ISKFunction SetDefaultSkillCollection(IReadOnlySkillCollection skills)
    {
        this._skillCollection = skills;
        return this;
    }

    /// <inheritdoc/>
    public IPromptFunction SetAIService(Func<ITextCompletion> serviceFactory)
    {
        Verify.NotNull(serviceFactory);
        this.VerifyIsSemantic();
        this._aiService = new Lazy<ITextCompletion>(serviceFactory);
        return this;
    }

    /// <inheritdoc/>
    public IPromptFunction SetAIConfiguration(CompleteRequestSettings settings)
    {
        Verify.NotNull(settings);
        this.VerifyIsSemantic();
        this._aiRequestSettings = settings;
        return this;
    }

    /// <summary>
    /// Dispose of resources.
    /// </summary>
    public void Dispose()
    {
        if (this._aiService is { IsValueCreated: true } aiService)
        {
            (aiService.Value as IDisposable)?.Dispose();
        }
    }

    /// <summary>
    /// JSON serialized string representation of the function.
    /// </summary>
    public override string ToString()
        => this.ToString(false);

    /// <summary>
    /// JSON serialized string representation of the function.
    /// </summary>
    public string ToString(bool writeIndented)
        => JsonSerializer.Serialize(this, options: writeIndented ? s_toStringIndentedSerialization : s_toStringStandardSerialization);

    #region private

    private static readonly JsonSerializerOptions s_toStringStandardSerialization = new();
    private static readonly JsonSerializerOptions s_toStringIndentedSerialization = new() { WriteIndented = true };
    private Func<ITextCompletion?, CompleteRequestSettings?, SKContext, CancellationToken, Task<SKContext>> _function;
    private readonly ILogger _logger;
    private IReadOnlySkillCollection? _skillCollection;
    private Lazy<ITextCompletion>? _aiService = null;
    private CompleteRequestSettings _aiRequestSettings = new();

    private struct MethodDetails
    {
        public Func<ITextCompletion?, CompleteRequestSettings?, SKContext, CancellationToken, Task<SKContext>> Function { get; set; }
        public List<ParameterView> Parameters { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
    }

    private static async Task<string> GetCompletionsResultContentAsync(IReadOnlyList<ITextResult> completions, CancellationToken cancellationToken = default)
    {
        // To avoid any unexpected behavior we only take the first completion result (when running from the Kernel)
        return await completions[0].GetCompletionAsync(cancellationToken).ConfigureAwait(false);
    }

    internal PromptFunction(
        Func<ITextCompletion?, CompleteRequestSettings?, SKContext, CancellationToken, Task<SKContext>> delegateFunction,
        IList<ParameterView> parameters,
        string skillName,
        string functionName,
        string description,
        ILogger? logger = null)
    {
        Verify.NotNull(delegateFunction);
        Verify.ValidSkillName(skillName);
        Verify.ValidFunctionName(functionName);
        Verify.ParametersUniqueness(parameters);

        this._logger = logger ?? NullLogger.Instance;

        this._function = delegateFunction;
        this.Parameters = parameters;

        this.Name = functionName;
        this.SkillName = skillName;
        this.Description = description;
    }

    /// <summary>
    /// Throw an exception if the function is not semantic, use this method when some logic makes sense only for semantic functions.
    /// </summary>
    /// <exception cref="KernelException"></exception>
    private void VerifyIsSemantic()
    {
        if (this.IsSemantic) { return; }

        this._logger.LogError("The function is not semantic");
        throw new KernelException(
            KernelException.ErrorCodes.InvalidFunctionType,
            "Invalid operation, the method requires a semantic function");
    }

    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    private string DebuggerDisplay => $"{this.Name} ({this.Description})";

    /// <summary>Add default values to the context variables if the variable is not defined</summary>
    private void AddDefaultValues(ContextVariables variables)
    {
        foreach (var parameter in this.Parameters)
        {
            if (!variables.ContainsKey(parameter.Name) && parameter.DefaultValue != null)
            {
                variables[parameter.Name] = parameter.DefaultValue;
            }
        }
    }

    public Task<SKContext> InvokeAsync(SKContext context, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    #endregion
}
