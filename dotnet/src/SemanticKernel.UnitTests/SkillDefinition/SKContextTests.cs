// Copyright (c) Microsoft. All rights reserved.

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading.Tasks;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Orchestration;
using Microsoft.SemanticKernel.SkillDefinition;
using Moq;
using Xunit;

namespace SemanticKernel.UnitTests.SkillDefinition;

public class SKContextTests
{
    private readonly Mock<IReadOnlySkillCollection> _skills;
    private readonly Mock<IKernel> _kernel;

    public SKContextTests()
    {
        this._skills = new Mock<IReadOnlySkillCollection>();
        this._kernel = new Mock<IKernel>();
    }

    [Fact]
    public void ItHasHelpersForArgs()
    {
        // Arrange
        var args = new Dictionary<string, string>();
        var target = new SKContext(this._kernel.Object, args, skills: this._skills.Object);
        args["foo1"] = "bar1";

        // Act
        target.Args["foo2"] = "bar2";
        target.Args["INPUT"] = Guid.NewGuid().ToString("N");

        // Assert
        Assert.Equal("bar1", target.Args["foo1"]);
        Assert.Equal("bar1", target.Args["foo1"]);
        Assert.Equal("bar2", target.Args["foo2"]);
        Assert.Equal("bar2", target.Args["foo2"]);
        Assert.Equal(target.Args["INPUT"], target.Result);
        Assert.Equal(target.Args["INPUT"], target.ToString());
        Assert.Equal(target.Args["INPUT"], target.Args["input"]);
        Assert.Equal(target.Args["INPUT"], target.Args.ToString());
    }

    [Fact]
    public async Task ItHasHelpersForSkillCollectionAsync()
    {
        // Arrange
        IDictionary<string, ISKFunction> skill = KernelBuilder.Create().ImportSkill(new Parrot(), "test");
        this._skills.Setup(x => x.GetFunction("func")).Returns(skill["say"]);
        var target = new SKContext(this._kernel.Object, new ContextVariables(), this._skills.Object);
        Assert.NotNull(target.Skills);

        // Act
        var say = target.Skills.GetFunction("func");
        SKContext result = await say.InvokeAsync("ciao", this._kernel.Object);

        // Assert
        Assert.Equal("ciao", result.Result);
    }

    private sealed class Parrot
    {
        [SKFunction, Description("say something")]
        // ReSharper disable once UnusedMember.Local
        public string Say(string input)
        {
            return input;
        }
    }
}
