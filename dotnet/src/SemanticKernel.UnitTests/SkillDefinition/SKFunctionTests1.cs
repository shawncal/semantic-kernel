// Copyright (c) Microsoft. All rights reserved.

using System.Collections.Generic;
using System.Threading;
using Microsoft.SemanticKernel.AI.TextCompletion;
using Microsoft.SemanticKernel.Orchestration;
using Microsoft.SemanticKernel.SemanticFunctions;
using Microsoft.SemanticKernel.SkillDefinition;
using Moq;
using Xunit;

namespace SemanticKernel.UnitTests.SkillDefinition;

public sealed class SKFunctionTests1
{
    private readonly Mock<IPromptTemplate> _promptTemplate;

    public SKFunctionTests1()
    {
        this._promptTemplate = new Mock<IPromptTemplate>();
        this._promptTemplate.Setup(x => x.RenderAsync(It.IsAny<SKContext>(), It.IsAny<CancellationToken>())).ReturnsAsync("foo");
        this._promptTemplate.Setup(x => x.GetParameters()).Returns(new List<ParameterView>());
    }

    [Fact]
    public void ItHasDefaultRequestSettings()
    {
        // Arrange
        var templateConfig = new PromptTemplateConfig();
        var functionConfig = new SemanticFunctionConfig(templateConfig, this._promptTemplate.Object);

        // Act
        var skFunction = PromptFunction.FromSemanticConfig("sk", "name", functionConfig);

        // Assert
        Assert.Equal(0, skFunction.DefaultRequestSettings.Temperature);
        Assert.Equal(null, skFunction.DefaultRequestSettings.MaxTokens);
    }

    [Fact]
    public void ItAllowsToUpdateRequestSettings()
    {
        // Arrange
        var templateConfig = new PromptTemplateConfig();
        var functionConfig = new SemanticFunctionConfig(templateConfig, this._promptTemplate.Object);
        var skFunction = PromptFunction.FromSemanticConfig("sk", "name", functionConfig);
        var settings = new CompleteRequestSettings
        {
            Temperature = 0.9,
            MaxTokens = 2001,
        };

        // Act
        skFunction.DefaultRequestSettings.Temperature = 1.3;
        skFunction.DefaultRequestSettings.MaxTokens = 130;

        // Assert
        Assert.Equal(1.3, skFunction.DefaultRequestSettings.Temperature);
        Assert.Equal(130, skFunction.DefaultRequestSettings.MaxTokens);

        // Act
        skFunction.DefaultRequestSettings.Temperature = 0.7;

        // Assert
        Assert.Equal(0.7, skFunction.DefaultRequestSettings.Temperature);
    }

    private static Mock<IPromptTemplate> MockPromptTemplate()
    {
        var promptTemplate = new Mock<IPromptTemplate>();

        promptTemplate.Setup(x => x.RenderAsync(It.IsAny<SKContext>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync("some prompt");

        promptTemplate
            .Setup(x => x.GetParameters())
            .Returns(new List<ParameterView>());

        return promptTemplate;
    }

    private static Mock<ITextCompletion> MockAIService(string result)
    {
        var aiService = new Mock<ITextCompletion>();
        var textCompletionResult = new Mock<ITextResult>();

        textCompletionResult
            .Setup(x => x.GetCompletionAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(result);

        aiService
            .Setup(x => x.GetCompletionsAsync(It.IsAny<string>(), It.IsAny<CompleteRequestSettings>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<ITextResult> { textCompletionResult.Object });

        return aiService;
    }
}
