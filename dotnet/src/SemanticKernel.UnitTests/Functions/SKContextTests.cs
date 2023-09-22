// Copyright (c) Microsoft. All rights reserved.

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading.Tasks;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Orchestration;
using Moq;
using Xunit;

namespace SemanticKernel.UnitTests.Functions;

public class SKContextTests
{
    private readonly Mock<IReadOnlyFunctionCollection> _functions;
    private readonly Mock<IKernel> _kernel;

    public SKContextTests()
    {
        this._functions = new Mock<IReadOnlyFunctionCollection>();
        this._kernel = new Mock<IKernel>();
    }

    [Fact]
    public void ItHasHelpersForArgs()
    {
        // Arrange
        var args = new Dictionary<string, string>();
        var target = new SKContext(this._kernel.Object, args, functions: this._functions.Object);
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
    public async Task ItHasHelpersForFunctionCollectionAsync()
    {
        // Arrange
        IDictionary<string, ISKFunction> plugin = KernelBuilder.Create().ImportPlugin(new Parrot(), "test");
        this._functions.Setup(x => x.GetFunction("func")).Returns(plugin["say"]);
        var target = new SKContext(this._kernel.Object, new ContextVariables(), this._functions.Object);
        Assert.NotNull(target.Functions);

        // Act
        var say = target.Functions.GetFunction("func");

        FunctionResult result = await say.InvokeAsync("ciao", this._kernel.Object);

        // Assert
        Assert.Equal("ciao", result.Context.Result);
        Assert.Equal("ciao", result.GetValue<string>());
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
