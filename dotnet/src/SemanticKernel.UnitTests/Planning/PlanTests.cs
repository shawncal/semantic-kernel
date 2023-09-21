// Copyright (c) Microsoft. All rights reserved.

using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.AI;
using Microsoft.SemanticKernel.Orchestration;
using Microsoft.SemanticKernel.Planning;
using Microsoft.SemanticKernel.SkillDefinition;
using Moq;
using Xunit;

namespace SemanticKernel.UnitTests.Planning;

public sealed class PlanTests
{
    [Fact]
    public Task CanCreatePlanAsync()
    {
        // Arrange
        var goal = "Write a poem or joke and send it in an e-mail to Kai.";
        // Act
        var plan = new Plan(goal);

        // Assert
        Assert.Equal(goal, plan.Description);
        Assert.Empty(plan.Steps);
        return Task.CompletedTask;
    }

    [Fact]
    public async Task CanExecutePlanAsync()
    {
        // Arrange
        var kernel = new Mock<IKernel>();
        var goal = "Write a poem or joke and send it in an e-mail to Kai.";
        var plan = new Plan(goal);

        // Act
        var result = await plan.InvokeAsync("Some input", kernel.Object);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Some input", result.Result);
    }

    [Fact]
    public async Task CanExecutePlanWithContextAsync()
    {
        // Arrange
        var goal = "Write a poem or joke and send it in an e-mail to Kai.";
        var plan = new Plan(goal);
        var kernel = new Mock<IKernel>();

        var context = new SKContext(kernel.Object, new ContextVariables("Some input"));

        // Act
        var result = await plan.InvokeAsync(context);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Some input", result.Result);

        plan = new Plan(goal);
        // Act
        context.Args["input"] = "other input";
        result = await plan.InvokeAsync(context);
        // Assert
        Assert.NotNull(result);
        Assert.Equal("other input", result.Result);
    }

    [Fact]
    public async Task CanExecutePlanWithPlanStepAsync()
    {
        // Arrange
        var goal = "Write a poem or joke and send it in an e-mail to Kai.";
        var planInput = "Some input";
        var stepOutput = "Output: The input was: ";
        var plan = new Plan(goal);

        // Arrange
        var kernel = new Mock<IKernel>();

        var returnContext = new SKContext(kernel.Object, new ContextVariables(stepOutput));

        var mockFunction = new Mock<ISKFunction>();
        mockFunction.Setup(x => x.InvokeAsync(It.IsAny<SKContext>(), null, It.IsAny<CancellationToken>()))
            .Callback<SKContext, AIRequestSettings, CancellationToken>((c, s, ct) =>
                returnContext.Args["input"] += c.Args["input"])
            .Returns(() => Task.FromResult(returnContext));
        mockFunction.Setup(x => x.Describe()).Returns(() => new FunctionView("functionName", "skillName"));

        plan.AddSteps(new Plan(mockFunction.Object));

        // Act
        var result = await plan.InvokeAsync(planInput, kernel.Object);

        // Assert
        Assert.NotNull(result);
        Assert.Equal($"{stepOutput}{planInput}", result.Result);
        mockFunction.Verify(x => x.InvokeAsync(It.IsAny<SKContext>(), null, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task CanExecutePlanWithFunctionStepAsync()
    {
        // Arrange
        var goal = "Write a poem or joke and send it in an e-mail to Kai.";
        var planInput = "Some input";
        var stepOutput = "Output: The input was: ";
        var plan = new Plan(goal);

        // Arrange
        var kernel = new Mock<IKernel>();

        var returnContext = new SKContext(kernel.Object, new ContextVariables(stepOutput));

        var mockFunction = new Mock<ISKFunction>();
        mockFunction.Setup(x => x.InvokeAsync(It.IsAny<SKContext>(), null, It.IsAny<CancellationToken>()))
            .Callback<SKContext, AIRequestSettings, CancellationToken>((c, s, ct) =>
                returnContext.Args["input"] += c.Args["input"])
            .Returns(() => Task.FromResult(returnContext));
        mockFunction.Setup(x => x.Describe()).Returns(() => new FunctionView("functionName", "skillName"));

        plan.AddSteps(mockFunction.Object);

        // Act
        var result = await plan.InvokeAsync(planInput, kernel.Object);

        // Assert
        Assert.NotNull(result);
        Assert.Equal($"{stepOutput}{planInput}", result.Result);
        mockFunction.Verify(x => x.InvokeAsync(It.IsAny<SKContext>(), null, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task CanExecutePlanWithFunctionStepsAsync()
    {
        // Arrange
        var goal = "Write a poem or joke and send it in an e-mail to Kai.";
        var planInput = "Some input";
        var stepOutput = "Output: The input was: ";
        var plan = new Plan(goal);

        // Arrange
        var kernel = new Mock<IKernel>();

        var returnContext = new SKContext(kernel.Object, new ContextVariables(stepOutput));

        var mockFunction = new Mock<ISKFunction>();
        mockFunction.Setup(x => x.InvokeAsync(It.IsAny<SKContext>(), null, It.IsAny<CancellationToken>()))
            .Callback<SKContext, AIRequestSettings, CancellationToken>((c, s, ct) =>
                returnContext.Args["input"] += c.Args["input"])
            .Returns(() => Task.FromResult(returnContext));
        mockFunction.Setup(x => x.Describe()).Returns(() => new FunctionView("functionName", "skillName"));

        plan.AddSteps(mockFunction.Object, mockFunction.Object);

        // Act
        var result = await plan.InvokeAsync(planInput, kernel.Object);

        // Assert
        Assert.NotNull(result);
        Assert.Equal($"{stepOutput}{planInput}{stepOutput}{planInput}", result.Result);
        mockFunction.Verify(x => x.InvokeAsync(It.IsAny<SKContext>(), null, It.IsAny<CancellationToken>()), Times.Exactly(2));
    }

    [Fact]
    public async Task CanExecutePlanWithStepsAndFunctionAsync()
    {
        // Arrange
        var goal = "Write a poem or joke and send it in an e-mail to Kai.";
        var planInput = "Some input";
        var stepOutput = "Output: The input was: ";
        var plan = new Plan(goal);

        // Arrange
        var kernel = new Mock<IKernel>();

        var returnContext = new SKContext(kernel.Object, new ContextVariables(stepOutput));

        var mockFunction = new Mock<ISKFunction>();
        mockFunction.Setup(x => x.InvokeAsync(It.IsAny<SKContext>(), null, It.IsAny<CancellationToken>()))
            .Callback<SKContext, AIRequestSettings, CancellationToken>((c, s, ct) =>
                returnContext.Args["input"] += c.Args["input"])
            .Returns(() => Task.FromResult(returnContext));
        mockFunction.Setup(x => x.Describe()).Returns(() => new FunctionView("functionName", "skillName"));

        plan.AddSteps(new Plan(mockFunction.Object), mockFunction.Object);

        // Act
        var result = await plan.InvokeAsync(planInput, kernel.Object);

        // Assert
        Assert.NotNull(result);
        Assert.Equal($"{stepOutput}{planInput}{stepOutput}{planInput}", result.Result);
        mockFunction.Verify(x => x.InvokeAsync(It.IsAny<SKContext>(), null, It.IsAny<CancellationToken>()), Times.Exactly(2));
    }

    [Fact]
    public async Task CanExecutePlanWithStepsAsync()
    {
        // Arrange
        var goal = "Write a poem or joke and send it in an e-mail to Kai.";
        var planInput = "Some input";
        var stepOutput = "Output: The input was: ";
        var plan = new Plan(goal);

        // Arrange
        var kernel = new Mock<IKernel>();

        var returnContext = new SKContext(kernel.Object, new ContextVariables(stepOutput));

        var mockFunction = new Mock<ISKFunction>();
        mockFunction.Setup(x => x.InvokeAsync(It.IsAny<SKContext>(), null, It.IsAny<CancellationToken>()))
            .Callback<SKContext, AIRequestSettings, CancellationToken>((c, s, ct) =>
                returnContext.Args["input"] += c.Args["input"])
            .Returns(() => Task.FromResult(returnContext));
        mockFunction.Setup(x => x.Describe()).Returns(() => new FunctionView("functionName", "skillName"));

        plan.AddSteps(new Plan(mockFunction.Object), new Plan(mockFunction.Object));

        // Act
        var result = await plan.InvokeAsync(planInput, kernel.Object);

        // Assert
        Assert.NotNull(result);
        Assert.Equal($"{stepOutput}{planInput}{stepOutput}{planInput}", result.Result);
        mockFunction.Verify(x => x.InvokeAsync(It.IsAny<SKContext>(), null, It.IsAny<CancellationToken>()), Times.Exactly(2));
    }

    [Fact]
    public async Task CanStepPlanWithStepsAsync()
    {
        // Arrange
        var goal = "Write a poem or joke and send it in an e-mail to Kai.";
        var planInput = "Some input";
        var stepOutput = "Output: The input was: ";
        var plan = new Plan(goal);

        // Arrange
        var kernel = new Mock<IKernel>();

        var returnContext = new SKContext(kernel.Object, new ContextVariables(stepOutput)
        );

        var mockFunction = new Mock<ISKFunction>();
        mockFunction.Setup(x => x.InvokeAsync(It.IsAny<SKContext>(), null, It.IsAny<CancellationToken>()))
            .Callback<SKContext, AIRequestSettings, CancellationToken>((c, s, ct) =>
                returnContext.Args["input"] += c.Args["input"])
            .Returns(() => Task.FromResult(returnContext));
        mockFunction.Setup(x => x.Describe()).Returns(() => new FunctionView("functionName", "skillName"));

        plan.AddSteps(mockFunction.Object, mockFunction.Object);

        // Act
        var result = await kernel.Object.StepAsync(planInput, plan);

        // Assert
        Assert.NotNull(result);
        Assert.Equal($"{stepOutput}{planInput}", result.State.ToString());

        // Act
        result = await kernel.Object.StepAsync(result);

        // Assert
        Assert.NotNull(result);
        Assert.Equal($"{stepOutput}{planInput}{stepOutput}{planInput}", result.State.ToString());
        mockFunction.Verify(x => x.InvokeAsync(It.IsAny<SKContext>(), null, It.IsAny<CancellationToken>()), Times.Exactly(2));
    }

    [Fact]
    public async Task CanStepPlanWithStepsAndContextAsync()
    {
        // Arrange
        var goal = "Write a poem or joke and send it in an e-mail to Kai.";
        var planInput = "Some input";
        var stepOutput = "Output: The input was: ";
        var plan = new Plan(goal);

        // Arrange
        var kernel = new Mock<IKernel>();

        var returnContext = new SKContext(kernel.Object, new ContextVariables(stepOutput));

        var mockFunction = new Mock<ISKFunction>();
        mockFunction.Setup(x => x.InvokeAsync(It.IsAny<SKContext>(), null, It.IsAny<CancellationToken>()))
            .Callback<SKContext, AIRequestSettings, CancellationToken>((c, s, ct) =>
            {
                c.Args.TryGetValue("variables", out string? v);
                returnContext.Args["input"] += c.Args["input"] + v;
            })
            .Returns(() => Task.FromResult(returnContext));
        mockFunction.Setup(x => x.Describe()).Returns(new FunctionView("functionName", "skillName", "description")
        {
            Parameters = new ParameterView[] { new("variables") }
        });

        plan.AddSteps(mockFunction.Object, mockFunction.Object);

        // Act
        var cv = new ContextVariables(planInput);
        cv.Set("variables", "foo");
        plan = await kernel.Object.StepAsync(cv, plan);

        // Assert
        Assert.NotNull(plan);
        Assert.Equal($"{stepOutput}{planInput}foo", plan.State.ToString());

        // Act
        cv.Set("variables", "bar");
        cv["input"] = string.Empty;
        plan = await kernel.Object.StepAsync(cv, plan);

        // Assert
        Assert.NotNull(plan);
        Assert.Equal($"{stepOutput}{planInput}foo{stepOutput}{planInput}foobar", plan.State.ToString());
        mockFunction.Verify(x => x.InvokeAsync(It.IsAny<SKContext>(), null, It.IsAny<CancellationToken>()), Times.Exactly(2));
    }

    [Fact]
    public async Task StepExceptionIsThrownAsync()
    {
        // Arrange
        var goal = "Write a poem or joke and send it in an e-mail to Kai.";
        var planInput = "Some input";
        var stepOutput = "Output: The input was: ";
        var plan = new Plan(goal);

        // Arrange
        var kernel = new Mock<IKernel>();

        var returnContext = new SKContext(kernel.Object, new ContextVariables(stepOutput));

        var mockFunction = new Mock<ISKFunction>();
        mockFunction.Setup(x => x.InvokeAsync(It.IsAny<SKContext>(), null, It.IsAny<CancellationToken>()))
            .Throws(new ArgumentException("Error message"));
        mockFunction.Setup(x => x.Describe()).Returns(() => new FunctionView("functionName", "skillName"));

        plan.AddSteps(mockFunction.Object, mockFunction.Object);

        // Act
        var cv = new ContextVariables(planInput);
        await Assert.ThrowsAsync<ArgumentException>(async () => await kernel.Object.StepAsync(cv, plan));
        mockFunction.Verify(x => x.InvokeAsync(It.IsAny<SKContext>(), null, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task PlanStepExceptionIsThrownAsync()
    {
        // Arrange
        var goal = "Write a poem or joke and send it in an e-mail to Kai.";
        var planInput = "Some input";
        var plan = new Plan(goal);

        // Arrange
        var kernel = new Mock<IKernel>();
        var logger = new Mock<ILogger>();
        var skills = new Mock<ISkillCollection>();

        var returnContext = new SKContext(kernel.Object);

        var mockFunction = new Mock<ISKFunction>();
        mockFunction.Setup(x => x.InvokeAsync(It.IsAny<SKContext>(), null, It.IsAny<CancellationToken>()))
            .Throws(new ArgumentException("Error message"));
        mockFunction.Setup(x => x.Describe()).Returns(() => new FunctionView("functionName", "skillName"));

        plan.AddSteps(new Plan(mockFunction.Object), new Plan(mockFunction.Object));

        // Act
        var cv = new ContextVariables(planInput);
        await Assert.ThrowsAsync<ArgumentException>(async () => await kernel.Object.StepAsync(cv, plan));
        mockFunction.Verify(x => x.InvokeAsync(It.IsAny<SKContext>(), null, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task CanExecutePanWithTreeStepsAsync()
    {
        // Arrange
        var goal = "Write a poem or joke and send it in an e-mail to Kai.";
        var plan = new Plan(goal);
        var subPlan = new Plan("Write a poem or joke");

        // Arrange
        var kernel = new Mock<IKernel>();

        var returnContext = new SKContext(kernel.Object);

        var childFunction1 = new Mock<ISKFunction>();
        childFunction1.Setup(x => x.InvokeAsync(It.IsAny<SKContext>(), null, It.IsAny<CancellationToken>()))
            .Callback<SKContext, AIRequestSettings, CancellationToken>((c, s, ct) =>
                returnContext.Args["input"] = "Child 1 output!" + c.Args["Input"])
            .Returns(() => Task.FromResult(returnContext));
        childFunction1.Setup(x => x.Describe()).Returns(() => new FunctionView("child1", "skillName"));

        var childFunction2 = new Mock<ISKFunction>();
        childFunction2.Setup(x => x.InvokeAsync(It.IsAny<SKContext>(), null, It.IsAny<CancellationToken>()))
            .Callback<SKContext, AIRequestSettings, CancellationToken>((c, s, ct) =>
                returnContext.Args["input"] = "Child 2 is happy about " + c.Args["input"])
            .Returns(() => Task.FromResult(returnContext));
        childFunction2.Setup(x => x.Describe()).Returns(() => new FunctionView("child2", "skillName"));

        var childFunction3 = new Mock<ISKFunction>();
        childFunction3.Setup(x => x.InvokeAsync(It.IsAny<SKContext>(), null, It.IsAny<CancellationToken>()))
            .Callback<SKContext, AIRequestSettings, CancellationToken>((c, s, ct) =>
                returnContext.Args["input"] = "Child 3 heard " + c.Args["input"])
            .Returns(() => Task.FromResult(returnContext));
        childFunction3.Setup(x => x.Describe()).Returns(() => new FunctionView("child3", "skillName"));

        var nodeFunction1 = new Mock<ISKFunction>();
        nodeFunction1.Setup(x => x.InvokeAsync(It.IsAny<SKContext>(), null, It.IsAny<CancellationToken>()))
            .Callback<SKContext, AIRequestSettings, CancellationToken>((c, s, ct) =>
                returnContext.Args["input"] = c.Args["input"] + " - this just happened.")
            .Returns(() => Task.FromResult(returnContext));
        nodeFunction1.Setup(x => x.Describe()).Returns(() => new FunctionView("node1", "skillName"));

        subPlan.AddSteps(childFunction1.Object, childFunction2.Object, childFunction3.Object);
        plan.AddSteps(subPlan);
        plan.AddSteps(nodeFunction1.Object);

        // Act
        while (plan.HasNextStep)
        {
            plan = await kernel.Object.StepAsync(plan);
        }

        // Assert
        Assert.NotNull(plan);
        Assert.Equal("Child 3 heard Child 2 is happy about Child 1 output!Write a poem or joke - this just happened.", plan.State.ToString());
        nodeFunction1.Verify(x => x.InvokeAsync(It.IsAny<SKContext>(), null, It.IsAny<CancellationToken>()), Times.Once);
        childFunction1.Verify(x => x.InvokeAsync(It.IsAny<SKContext>(), null, It.IsAny<CancellationToken>()), Times.Once);
        childFunction2.Verify(x => x.InvokeAsync(It.IsAny<SKContext>(), null, It.IsAny<CancellationToken>()), Times.Once);
        childFunction3.Verify(x => x.InvokeAsync(It.IsAny<SKContext>(), null, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public void CanCreatePlanWithGoalAndSteps()
    {
        // Arrange
        var goal = "Write a poem or joke and send it in an e-mail to Kai.";
        var plan = new Plan(goal, new Mock<ISKFunction>().Object, new Mock<ISKFunction>().Object);

        // Assert
        Assert.NotNull(plan);
        Assert.Equal(goal, plan.Description);
        Assert.Equal(2, plan.Steps.Count);
    }

    [Fact]
    public void CanCreatePlanWithGoalAndSubPlans()
    {
        // Arrange
        var goal = "Write a poem or joke and send it in an e-mail to Kai.";
        var plan = new Plan(goal, new Plan("Write a poem or joke"), new Plan("Send it in an e-mail to Kai"));

        // Assert
        Assert.NotNull(plan);
        Assert.Equal(goal, plan.Description);
        Assert.Equal(2, plan.Steps.Count);
    }

    [Fact]
    public async Task CanExecutePlanWithOneStepAndStateAsync()
    {
        // Arrange
        var kernel = new Mock<IKernel>();

        var returnContext = new SKContext(kernel.Object);

        var mockFunction = new Mock<ISKFunction>();
        mockFunction.Setup(x => x.InvokeAsync(It.IsAny<SKContext>(), null, It.IsAny<CancellationToken>()))
            .Callback<SKContext, AIRequestSettings, CancellationToken>((c, s, ct) =>
                returnContext.Args["input"] = "Here is a poem about " + c.Args["input"])
            .Returns(() => Task.FromResult(returnContext));

        var plan = new Plan(mockFunction.Object);
        plan.State["input"] = "Cleopatra";

        // Act
        var result = await plan.InvokeAsync(kernel.Object);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Here is a poem about Cleopatra", result.Result);
        mockFunction.Verify(x => x.InvokeAsync(It.IsAny<SKContext>(), null, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task CanExecutePlanWithStateAsync()
    {
        // Arrange
        var kernel = new Mock<IKernel>();

        var returnContext = new SKContext(kernel.Object);

        var mockFunction = new Mock<ISKFunction>();
        mockFunction.Setup(x => x.InvokeAsync(It.IsAny<SKContext>(), null, It.IsAny<CancellationToken>()))
            .Callback<SKContext, AIRequestSettings, CancellationToken>((c, s, ct) =>
            {
                c.Args.TryGetValue("type", out string? t);
                returnContext.Args["input"] = $"Here is a {t} about " + c.Args["input"];
            })
            .Returns(() => Task.FromResult(returnContext));
        mockFunction.Setup(x => x.Describe()).Returns(() => new FunctionView("functionName", "skillName"));

        var planStep = new Plan(mockFunction.Object);
        planStep.Parameters["type"] = string.Empty;
        var plan = new Plan(string.Empty);
        plan.AddSteps(planStep);
        plan.State["input"] = "Cleopatra";
        plan.State["type"] = "poem";

        // Act
        var result = await plan.InvokeAsync(kernel.Object);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Here is a poem about Cleopatra", result.Result);
        mockFunction.Verify(x => x.InvokeAsync(It.IsAny<SKContext>(), null, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task CanExecutePlanWithCustomContextAsync()
    {
        // Arrange
        var kernel = new Mock<IKernel>();

        var returnContext = new SKContext(kernel.Object);

        var mockFunction = new Mock<ISKFunction>();
        mockFunction.Setup(x => x.InvokeAsync(It.IsAny<SKContext>(), null, It.IsAny<CancellationToken>()))
            .Callback<SKContext, AIRequestSettings?, CancellationToken>((c, s, ct) =>
            {
                c.Args.TryGetValue("type", out string? t);
                returnContext.Args["input"] = $"Here is a {t} about " + c.Args["input"];
            })
            .Returns(() => Task.FromResult(returnContext));

        var plan = new Plan(mockFunction.Object);
        plan.State["input"] = "Cleopatra";
        plan.State["type"] = "poem";

        // Act
        var result = await plan.InvokeAsync(kernel.Object);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Here is a poem about Cleopatra", result.Result);
        mockFunction.Verify(x => x.InvokeAsync(It.IsAny<SKContext>(), null, It.IsAny<CancellationToken>()), Times.Once);

        plan = new Plan(mockFunction.Object);
        plan.State["input"] = "Cleopatra";
        plan.State["type"] = "poem";

        var contextOverride = new SKContext(kernel.Object);
        contextOverride.Args["type"] = "joke";
        contextOverride.Args["input"] = "Medusa";

        // Act
        result = await plan.InvokeAsync(contextOverride);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Here is a joke about Medusa", result.Result);
        mockFunction.Verify(x => x.InvokeAsync(It.IsAny<SKContext>(), null, It.IsAny<CancellationToken>()), Times.Exactly(2));
    }

    [Fact]
    public async Task CanExecutePlanWithCustomStateAsync()
    {
        // Arrange
        var kernel = new Mock<IKernel>();

        var returnContext = new SKContext(kernel.Object);

        var mockFunction = new Mock<ISKFunction>();
        mockFunction.Setup(x => x.InvokeAsync(It.IsAny<SKContext>(), null, It.IsAny<CancellationToken>()))
            .Callback<SKContext, AIRequestSettings, CancellationToken>((c, s, ct) =>
            {
                c.Args.TryGetValue("type", out string? t);
                returnContext.Args["input"] = $"Here is a {t} about " + c.Args["input"];
            })
            .Returns(() => Task.FromResult(returnContext));
        mockFunction.Setup(x => x.Describe()).Returns(() => new FunctionView("functionName", "skillName"));

        var planStep = new Plan(mockFunction.Object);
        planStep.Parameters["type"] = string.Empty;
        var plan = new Plan("A plan");
        plan.State["input"] = "Medusa";
        plan.State["type"] = "joke";
        plan.AddSteps(planStep);

        // Act
        var result = await plan.InvokeAsync(kernel.Object);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Here is a joke about Medusa", result.Result);
        mockFunction.Verify(x => x.InvokeAsync(It.IsAny<SKContext>(), null, It.IsAny<CancellationToken>()), Times.Once);

        planStep = new Plan(mockFunction.Object);
        plan = new Plan("A plan");
        planStep.Parameters["input"] = "Medusa";
        planStep.Parameters["type"] = "joke";
        plan.State["input"] = "Cleopatra"; // state input will not override parameter
        plan.State["type"] = "poem";
        plan.AddSteps(planStep);

        // Act
        result = await plan.InvokeAsync(kernel.Object);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Here is a poem about Medusa", result.Result);
        mockFunction.Verify(x => x.InvokeAsync(It.IsAny<SKContext>(), null, It.IsAny<CancellationToken>()), Times.Exactly(2));

        planStep = new Plan(mockFunction.Object);
        plan = new Plan("A plan");
        planStep.Parameters["input"] = "Cleopatra";
        planStep.Parameters["type"] = "poem";
        plan.AddSteps(planStep);
        var contextOverride = new SKContext(kernel.Object);
        contextOverride.Args["type"] = "joke";
        contextOverride.Args["input"] = "Medusa"; // context input will not override parameters

        // Act
        result = await plan.InvokeAsync(contextOverride);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Here is a joke about Cleopatra", result.Result);
        mockFunction.Verify(x => x.InvokeAsync(It.IsAny<SKContext>(), null, It.IsAny<CancellationToken>()), Times.Exactly(3));
    }

    [Fact]
    public async Task CanExecutePlanWithJoinedResultAsync()
    {
        // Arrange
        var kernel = new Mock<IKernel>();

        var returnContext = new SKContext(kernel.Object);

        var outlineMock = new Mock<ISKFunction>();
        outlineMock.Setup(x => x.InvokeAsync(It.IsAny<SKContext>(), null, It.IsAny<CancellationToken>()))
            .Callback<SKContext, AIRequestSettings, CancellationToken>((c, s, ct) =>
                returnContext.Args["input"] = $"Here is a {c.Args["chapterCount"]} chapter outline about " + c.Args["input"])
            .Returns(() => Task.FromResult(returnContext));
        outlineMock.Setup(x => x.Describe()).Returns(() => new FunctionView("outline", "skillName"));

        var elementAtIndexMock = new Mock<ISKFunction>();
        elementAtIndexMock.Setup(x => x.InvokeAsync(It.IsAny<SKContext>(), null, It.IsAny<CancellationToken>()))
            .Callback<SKContext, AIRequestSettings, CancellationToken>((c, s, ct) =>
            {
                returnContext.Args["input"] = $"Outline section #{c.Args["index"]} of {c.Args["count"]}: " + c.Args["input"];
            })
            .Returns(() => Task.FromResult(returnContext));
        elementAtIndexMock.Setup(x => x.Describe()).Returns(() => new FunctionView("elementAt", "skillName"));

        var novelChapterMock = new Mock<ISKFunction>();
        novelChapterMock.Setup(x => x.InvokeAsync(It.IsAny<SKContext>(), null, It.IsAny<CancellationToken>()))
            .Callback<SKContext, AIRequestSettings, CancellationToken>((c, s, ct) =>
            {
                returnContext.Args["input"] =
                    $"Chapter #{c.Args["chapterIndex"]}: {c.Args["input"]}\nTheme:{c.Args["theme"]}\nPreviously:{c.Args["previousChapter"]}";
            })
            .Returns(() => Task.FromResult(returnContext));
        novelChapterMock.Setup(x => x.Describe()).Returns(() => new FunctionView("novelChapter", "skillName"));

        var plan = new Plan("A plan with steps that alternate appending to the plan result.");

        // Steps:
        // - WriterSkill.NovelOutline chapterCount='3' INPUT='A group of kids in a club called 'The Thinking Caps' that solve mysteries and puzzles using their creativity and logic.' endMarker='<!--===ENDPART===-->' => OUTLINE
        // - MiscSkill.ElementAtIndex count='3' INPUT='$OUTLINE' index='0' => CHAPTER_1_SYNOPSIS
        // - WriterSkill.NovelChapter chapterIndex='1' previousChapter='' INPUT='$CHAPTER_1_SYNOPSIS' theme='Children's mystery' => RESULT__CHAPTER_1
        // - MiscSkill.ElementAtIndex count='3' INPUT='$OUTLINE' index='1' => CHAPTER_2_SYNOPSIS
        // - WriterSkill.NovelChapter chapterIndex='2' previousChapter='$CHAPTER_1_SYNOPSIS' INPUT='$CHAPTER_2_SYNOPSIS' theme='Children's mystery' => RESULT__CHAPTER_2
        // - MiscSkill.ElementAtIndex count='3' INPUT='$OUTLINE' index='2' => CHAPTER_3_SYNOPSIS
        // - WriterSkill.NovelChapter chapterIndex='3' previousChapter='$CHAPTER_2_SYNOPSIS' INPUT='$CHAPTER_3_SYNOPSIS' theme='Children's mystery' => RESULT__CHAPTER_3
        var planStep = new Plan(outlineMock.Object);
        planStep.Parameters["input"] = "NovelOutline function input.";
        planStep.Parameters["chapterCount"] = "3";
        planStep.Outputs.Add("OUTLINE");
        plan.AddSteps(planStep);

        planStep = new Plan(elementAtIndexMock.Object);
        planStep.Parameters["count"] = "3";
        planStep.Parameters["input"] = "$OUTLINE";
        planStep.Parameters["index"] = "0";
        planStep.Outputs.Add("CHAPTER_1_SYNOPSIS");
        plan.AddSteps(planStep);

        planStep = new Plan(novelChapterMock.Object);
        planStep.Parameters["chapterIndex"] = "1";
        planStep.Parameters["previousChapter"] = " ";
        planStep.Parameters["input"] = "$CHAPTER_1_SYNOPSIS";
        planStep.Parameters["theme"] = "Children's mystery";
        planStep.Outputs.Add("RESULT__CHAPTER_1");
        plan.Outputs.Add("RESULT__CHAPTER_1");
        plan.AddSteps(planStep);

        planStep = new Plan(elementAtIndexMock.Object);
        planStep.Parameters["count"] = "3";
        planStep.Parameters["input"] = "$OUTLINE";
        planStep.Parameters["index"] = "1";
        planStep.Outputs.Add("CHAPTER_2_SYNOPSIS");
        plan.AddSteps(planStep);

        planStep = new Plan(novelChapterMock.Object);
        planStep.Parameters["chapterIndex"] = "2";
        planStep.Parameters["previousChapter"] = "$CHAPTER_1_SYNOPSIS";
        planStep.Parameters["input"] = "$CHAPTER_2_SYNOPSIS";
        planStep.Parameters["theme"] = "Children's mystery";
        planStep.Outputs.Add("RESULT__CHAPTER_2");
        plan.Outputs.Add("RESULT__CHAPTER_2");
        plan.AddSteps(planStep);

        planStep = new Plan(elementAtIndexMock.Object);
        planStep.Parameters["count"] = "3";
        planStep.Parameters["input"] = "$OUTLINE";
        planStep.Parameters["index"] = "2";
        planStep.Outputs.Add("CHAPTER_3_SYNOPSIS");
        plan.AddSteps(planStep);

        planStep = new Plan(novelChapterMock.Object);
        planStep.Parameters["chapterIndex"] = "3";
        planStep.Parameters["previousChapter"] = "$CHAPTER_2_SYNOPSIS";
        planStep.Parameters["input"] = "$CHAPTER_3_SYNOPSIS";
        planStep.Parameters["theme"] = "Children's mystery";
        planStep.Outputs.Add("CHAPTER_3");
        plan.Outputs.Add("CHAPTER_3");
        plan.AddSteps(planStep);

        // Act
        var result = await plan.InvokeAsync(kernel.Object);

        var expected =
            @"Chapter #1: Outline section #0 of 3: Here is a 3 chapter outline about NovelOutline function input.
Theme:Children's mystery
Previously:
Chapter #2: Outline section #1 of 3: Here is a 3 chapter outline about NovelOutline function input.
Theme:Children's mystery
Previously:Outline section #0 of 3: Here is a 3 chapter outline about NovelOutline function input.
Chapter #3: Outline section #2 of 3: Here is a 3 chapter outline about NovelOutline function input.
Theme:Children's mystery
Previously:Outline section #1 of 3: Here is a 3 chapter outline about NovelOutline function input.";

        // Assert
        Assert.Equal(expected, result.Result);
    }

    [Fact]
    public async Task CanExecutePlanWithExpandedAsync()
    {
        // Arrange
        var kernel = new Mock<IKernel>();

        var returnContext = new SKContext(kernel.Object);

        var functionMock = new Mock<ISKFunction>();
        functionMock.Setup(x => x.InvokeAsync(It.IsAny<SKContext>(), null, It.IsAny<CancellationToken>()))
            .Callback<SKContext, AIRequestSettings, CancellationToken>((c, s, ct) =>
                returnContext.Args["input"] = $"Here is a payload '{c.Args["payload"]}' for " + c.Args["input"])
            .Returns(() => Task.FromResult(returnContext));
        functionMock.Setup(x => x.Describe()).Returns(() => new FunctionView("functionName", "skillName"));

        var plan = new Plan("A plan with steps that have variables with a $ in them but not associated with an output");

        var planStep = new Plan(functionMock.Object);
        planStep.Parameters["input"] = "Function input.";
        planStep.Parameters["payload"] = @"{""prop"":""value"", ""$prop"": 3, ""prop2"": ""my name is $pop and $var""}";
        plan.AddSteps(planStep);
        plan.State["var"] = "foobar";

        // Act
        var result = await plan.InvokeAsync(kernel.Object);

        var expected =
            @"Here is a payload '{""prop"":""value"", ""$prop"": 3, ""prop2"": ""my name is $pop and foobar""}' for Function input.";

        // Assert
        Assert.Equal(expected, result.Result);
    }
}
