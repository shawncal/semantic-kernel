﻿// Copyright (c) Microsoft. All rights reserved.

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.AI;
using Microsoft.SemanticKernel.AI.TextCompletion;
using Microsoft.SemanticKernel.Connectors.AI.OpenAI;
using Microsoft.SemanticKernel.Orchestration;
using Microsoft.SemanticKernel.TemplateEngine.Prompt;
using SemanticKernel.IntegrationTests.Fakes;
using Xunit;
using Xunit.Abstractions;

namespace SemanticKernel.IntegrationTests.Extensions;

public sealed class KernelSemanticFunctionExtensionsTests : IDisposable
{
    public KernelSemanticFunctionExtensionsTests(ITestOutputHelper output)
    {
        this._logger = new RedirectOutput(output);
        this._target = new PromptTemplateEngine();
    }

    [Fact]
    public async Task ItSupportsFunctionCallsAsync()
    {
        var builder = Kernel.Builder
                .WithAIService<ITextCompletion>(null, new RedirectTextCompletion(), true)
                .WithLoggerFactory(this._logger);
        IKernel target = builder.Build();

        var emailFunctions = target.ImportFunctions(new EmailPluginFake());

        var prompt = $"Hey {{{{{FunctionCollection.GlobalFunctionsPluginName}.GetEmailAddress}}}}";

        // Act
        KernelResult actual = await target.InvokeSemanticFunctionAsync(prompt, requestSettings: new OpenAIRequestSettings() { MaxTokens = 150 });

        // Assert
        Assert.Equal("Hey johndoe1234@example.com", actual.GetValue<string>());
    }

    [Fact]
    public async Task ItSupportsFunctionCallsWithInputAsync()
    {
        var builder = Kernel.Builder
                .WithAIService<ITextCompletion>(null, new RedirectTextCompletion(), true)
                .WithLoggerFactory(this._logger);
        IKernel target = builder.Build();

        var emailFunctions = target.ImportFunctions(new EmailPluginFake());

        var prompt = $"Hey {{{{{FunctionCollection.GlobalFunctionsPluginName}.GetEmailAddress \"a person\"}}}}";

        // Act
        KernelResult actual = await target.InvokeSemanticFunctionAsync(prompt, requestSettings: new OpenAIRequestSettings() { MaxTokens = 150 });

        // Assert
        Assert.Equal("Hey a person@example.com", actual.GetValue<string>());
    }

    private readonly RedirectOutput _logger;
    private readonly PromptTemplateEngine _target;

    public void Dispose()
    {
        this._logger.Dispose();
    }

    private sealed class RedirectTextCompletion : ITextCompletion
    {
        Task<IReadOnlyList<ITextResult>> ITextCompletion.GetCompletionsAsync(string text, AIRequestSettings? requestSettings, CancellationToken cancellationToken)
        {
            return Task.FromResult<IReadOnlyList<ITextResult>>(new List<ITextResult> { new RedirectTextCompletionResult(text) });
        }

        IAsyncEnumerable<ITextStreamingResult> ITextCompletion.GetStreamingCompletionsAsync(string text, AIRequestSettings? requestSettings, CancellationToken cancellationToken)
        {
            throw new NotImplementedException(); // TODO
        }
    }

    internal sealed class RedirectTextCompletionResult : ITextResult
    {
        private readonly string _completion;

        public RedirectTextCompletionResult(string completion)
        {
            this._completion = completion;
        }

        public ModelResult ModelResult => new(this._completion);

        public Task<string> GetCompletionAsync(CancellationToken cancellationToken = default)
        {
            return Task.FromResult(this._completion);
        }
    }
}
