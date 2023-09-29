﻿// Copyright (c) Microsoft. All rights reserved.

using System.Collections.Generic;
using System.Linq;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Connectors.AI.OpenAI.AzureSdk;
using Xunit;

namespace SemanticKernel.Connectors.UnitTests.OpenAI.FunctionCalling;
public sealed class FunctionViewExtensionsTests
{
    [Fact]
    public void ItCanConvertToOpenAIFunctionNoParameters()
    {
        // Arrange
        var sut = new FunctionView(
            Name: "foo",
            PluginName: "bar",
            Description: "baz");

        // Act
        var result = sut.ToOpenAIFunction();

        // Assert
        Assert.Equal(sut.Name, result.FunctionName);
        Assert.Equal(sut.PluginName, result.PluginName);
        Assert.Equal(sut.Description, result.Description);
        Assert.Equal($"{sut.PluginName}-{sut.Name}", result.FullyQualifiedName);
    }

    [Fact]
    public void ItCanConvertToOpenAIFunctionNoPluginName()
    {
        // Arrange
        var sut = new FunctionView(
            Name: "foo",
            PluginName: string.Empty,
            Description: "baz");

        // Act
        var result = sut.ToOpenAIFunction();

        // Assert
        Assert.Equal(sut.Name, result.FunctionName);
        Assert.Equal(sut.PluginName, result.PluginName);
        Assert.Equal(sut.Description, result.Description);
        Assert.Equal(sut.Name, result.FullyQualifiedName);
    }

    [Fact]
    public void ItCanConvertToOpenAIFunctionWithParameter()
    {
        // Arrange
        var param1 = new ParameterView(
            Name: "param1",
            Description: "This is param1",
            DefaultValue: "1",
            Type: new ParameterViewType("int"),
            IsRequired: false);

        var sut = new FunctionView(
            Name: "foo",
            PluginName: "bar",
            Description: "baz",
            Parameters: new List<ParameterView> { param1 });

        // Act
        var result = sut.ToOpenAIFunction();
        var outputParam = result.Parameters.First();

        // Assert
        Assert.Equal("int", outputParam.Type);
        Assert.Equal(param1.Name, outputParam.Name);
        Assert.Equal("This is param1 (default value: 1)", outputParam.Description);
        Assert.Equal(param1.IsRequired, outputParam.IsRequired);
    }

    [Fact]
    public void ItCanConvertToOpenAIFunctionWithParameterNoType()
    {
        // Arrange
        var param1 = new ParameterView(
            Name: "param1",
            Description: "This is param1",
            Type: null,
            IsRequired: false);

        var sut = new FunctionView(
            Name: "foo",
            PluginName: "bar",
            Description: "baz",
            Parameters: new List<ParameterView> { param1 });

        // Act
        var result = sut.ToOpenAIFunction();
        var outputParam = result.Parameters.First();

        // Assert
        Assert.Equal("string", outputParam.Type);
        Assert.Equal(param1.Name, outputParam.Name);
        Assert.Equal(param1.Description, outputParam.Description);
        Assert.Equal(param1.IsRequired, outputParam.IsRequired);
    }
}
