﻿// Copyright (c) Microsoft. All rights reserved.

using Microsoft.SemanticKernel.SkillDefinition;
using Moq;
using Xunit;

namespace SemanticKernel.UnitTests.SkillDefinition;

public class SkillCollectionTests
{
    [Fact]
    public void ItAllowsToReplaceFunctions()
    {
        // Arrange
        var functionOne = new Mock<ISKFunction>();
        functionOne.SetupGet(x => x.Name).Returns("fName");
        functionOne.SetupGet(x => x.SkillName).Returns("sName");
        functionOne.SetupGet(x => x.Description).Returns("ONE");

        var functionTwo = new Mock<ISKFunction>();
        functionTwo.SetupGet(x => x.Name).Returns("fName");
        functionTwo.SetupGet(x => x.SkillName).Returns("sName");
        functionTwo.SetupGet(x => x.Description).Returns("TWO");

        var target = new SkillCollection();

        // Act
        target.AddFunction(functionOne.Object);

        // Assert
        Assert.True(target.TryGetFunction("sName", "fName", out var func));
        Assert.Equal("ONE", func.Description);

        // Act
        target.AddFunction(functionTwo.Object);

        // Assert
        Assert.True(target.TryGetFunction("sName", "fName", out func));
        Assert.Equal("TWO", func.Description);
    }
}
