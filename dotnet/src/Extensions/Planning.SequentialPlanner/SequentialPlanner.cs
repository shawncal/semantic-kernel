// Copyright (c) Microsoft. All rights reserved.

using System.Threading;
using System.Threading.Tasks;
using Microsoft.SemanticKernel.Diagnostics;
using Microsoft.SemanticKernel.Orchestration;
using Microsoft.SemanticKernel.Planning.Sequential;
using Microsoft.SemanticKernel.SkillDefinition;

#pragma warning disable IDE0130
// ReSharper disable once CheckNamespace - Using NS of Plan
namespace Microsoft.SemanticKernel.Planning;
#pragma warning restore IDE0130

/// <summary>
/// A planner that uses semantic function to create a sequential plan.
/// </summary>
public sealed class SequentialPlanner : ISequentialPlanner
{
    private const string StopSequence = "<!-- END -->";

    /// <summary>
    /// Initialize a new instance of the <see cref="SequentialPlanner"/> class.
    /// </summary>
    /// <param name="kernel">The semantic kernel instance.</param>
    /// <param name="config">The planner configuration.</param>
    /// <param name="prompt">Optional prompt override</param>
    public SequentialPlanner(
        IKernel kernel,
        SequentialPlannerConfig? config = null,
        string? prompt = null)
    {
        Verify.NotNull(kernel);
        this.Config = config ?? new();

        this.Config.ExcludedSkills.Add(RestrictedSkillName);

        this._kernel = kernel;

        string promptTemplate = prompt ?? EmbeddedResource.Read("skprompt.txt");

        this._functionFlowFunction = kernel.CreateSemanticFunction(
            promptTemplate: promptTemplate,
            skillName: RestrictedSkillName,
            description: "Given a request or command or goal generate a step by step plan to " +
                         "fulfill the request using functions. This ability is also known as decision making and function flow",
            maxTokens: this.Config.MaxTokens ?? 1024,
            temperature: 0.0,
            stopSequences: new[] { StopSequence });
    }

    /// <inheritdoc />
    public async Task<Plan> CreatePlanAsync(string goal, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrEmpty(goal))
        {
            throw new SKException("The goal specified is empty");
        }

#pragma warning disable CA2016 // Forward the 'CancellationToken' parameter to methods
        string relevantFunctionsManual = await this._kernel.CreateNewContext()
            .GetFunctionsManualAsync(goal, this.Config, cancellationToken).ConfigureAwait(false);
#pragma warning restore CA2016 // Forward the 'CancellationToken' parameter to methods

        var args = new ContextVariables();
        args["available_functions"] = relevantFunctionsManual;
        args["input"] = goal;

        var planResult = await this._kernel.RunAsync(this._functionFlowFunction, args, cancellationToken: cancellationToken).ConfigureAwait(false);

        if (planResult.ErrorOccurred)
        {
            throw new SKException($"Error creating plan for goal: {planResult.Exception?.Message}", planResult.Exception);
        }

        string planResultString = planResult.Result?.Trim()!;

        var getSkillFunction = this.Config.GetSkillFunction ?? SequentialPlanParser.GetSkillFunction(this._kernel);
        Plan plan = planResultString.ToPlanFromXml(goal, getSkillFunction, this.Config.AllowMissingFunctions);

        if (plan.Steps.Count == 0)
        {
            throw new SKException($"Not possible to create plan for goal with available functions.\nGoal:{goal}\nFunctions:\n{relevantFunctionsManual}");
        }

        return plan;
    }

    private SequentialPlannerConfig Config { get; }

    private readonly IKernel _kernel;

    /// <summary>
    /// the function flow semantic function, which takes a goal and creates an xml plan that can be executed
    /// </summary>
    private readonly ISKFunction _functionFlowFunction;

    /// <summary>
    /// The name to use when creating semantic functions that are restricted from plan creation
    /// </summary>
    private const string RestrictedSkillName = "SequentialPlanner_Excluded";
}
