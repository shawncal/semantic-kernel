﻿// Copyright (c) Microsoft. All rights reserved.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Azure;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.AI;
using Microsoft.SemanticKernel.AI.TextCompletion;
using Microsoft.SemanticKernel.Connectors.AI.OpenAI.ChatCompletion;
using RepoUtils;

#pragma warning disable RCS1192 // (Unnecessary usage of verbatim string literal)

// ReSharper disable once InconsistentNaming
public static class Example43_GetModelResult
{
    public static async Task RunAsync()
    {
        Console.WriteLine("======== Inline Function Definition + Result ========");

        IKernel kernel = new KernelBuilder()
            .WithOpenAITextCompletionService(
                modelId: TestConfiguration.OpenAI.ModelId,
                apiKey: TestConfiguration.OpenAI.ApiKey)
            .Build();

        // Function defined using few-shot design pattern
        const string FunctionDefinition = "Hi, give me 5 book suggestions about: {{$input}}";

        var myFunction = kernel.CreateSemanticFunction(FunctionDefinition);

        // Using InvokeAsync with 3 results (Currently invoke only supports 1 result, but you can get the other results from the ModelResults)
        var textResult = await myFunction.InvokeAsync("Sci-fi",
            settings: new CompleteRequestSettings { ResultsPerPrompt = 3, MaxTokens = 500, Temperature = 1, TopP = 0.5 });
        Console.WriteLine(textResult);
        Console.WriteLine(textResult.ModelResults.Select(result => result.GetOpenAITextResult()).AsJson());
        Console.WriteLine();

        // Using the Kernel RunAsync
        textResult = await kernel.RunAsync("sorry I forgot your birthday", myFunction);
        Console.WriteLine(textResult);
        Console.WriteLine(textResult.ModelResults.LastOrDefault()?.GetOpenAITextResult()?.Usage.AsJson());
        Console.WriteLine();

        // Using Chat Completion directly
        var chatCompletion = new OpenAIChatCompletion(
            modelId: TestConfiguration.OpenAI.ChatModelId,
            apiKey: TestConfiguration.OpenAI.ApiKey);
        var prompt = FunctionDefinition.Replace("{{$input}}", $"Translate this date {DateTimeOffset.Now:f} to French format", StringComparison.InvariantCultureIgnoreCase);

        IReadOnlyList<ITextResult> completionResults = await chatCompletion.GetCompletionsAsync(prompt, new CompleteRequestSettings() { MaxTokens = 500, Temperature = 1, TopP = 0.5 });

        Console.WriteLine(await completionResults[0].GetCompletionAsync());
        Console.WriteLine(completionResults[0].ModelResult.GetOpenAIChatResult().Usage.AsJson());
        Console.WriteLine();

        // Getting the error details
        kernel = new KernelBuilder()
            .WithOpenAITextCompletionService("text-davinci-003", "Invalid Key")
            .Build();
        var errorFunction = kernel.CreateSemanticFunction(FunctionDefinition);
        var failedContext = await kernel.RunAsync("sorry I forgot your birthday", errorFunction);

        if (failedContext.ErrorOccurred)
        {
            Console.WriteLine(OutputExceptionDetail(failedContext.LastException?.InnerException));
        }

        string OutputExceptionDetail(Exception? exception)
        {
            return exception switch
            {
                RequestFailedException requestException => new { requestException.Status, requestException.Message }.AsJson(),
                AIException aiException => new { ErrorCode = aiException.ErrorCode.ToString(), aiException.Message, aiException.Detail }.AsJson(),
                { } e => new { e.Message }.AsJson(),
                _ => string.Empty
            };
        }
    }
}
