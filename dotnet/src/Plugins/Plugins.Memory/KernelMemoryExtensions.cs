// Copyright (c) Microsoft. All rights reserved.

using Microsoft.SemanticKernel.AI.Embeddings;
using Microsoft.SemanticKernel.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using Microsoft.SemanticKernel.Memory;

#pragma warning disable IDE0130
// ReSharper disable once CheckNamespace - Using NS of IKernel
namespace Microsoft.SemanticKernel;
#pragma warning restore IDE0130

public static class KernelMemoryExtensions
{
    /// <inheritdoc/>
    public static IKernel AddTextMemoryPlugin(this IKernel kernel, TextMemoryPlugin memoryPlugin)
    {
        kernel.ImportSkill(memoryPlugin);
        return kernel;
    }

    /// <inheritdoc/>
    public static IKernel AddTextMemoryPlugin(this IKernel kernel, ISemanticTextMemory memory)
    {
        kernel.ImportSkill(new TextMemoryPlugin(memory));
        return kernel;
    }

    /// <summary>
    /// Set the semantic memory to use the given memory storage and embeddings service.
    /// </summary>
    /// <param name="kernel">Kernel instance</param>
    /// <param name="storage">Memory storage</param>
    /// <param name="embeddingsServiceId">Kernel service id for embedding generation</param>
    public static IKernel AddTextMemoryPlugin(this IKernel kernel, IMemoryStore storage, string? embeddingsServiceId = null)
    {
        var embeddingGenerator = kernel.GetService<ITextEmbeddingGeneration>(embeddingsServiceId);
        AddTextMemoryPlugin(kernel, storage, embeddingGenerator);
        return kernel;
    }

    /// <summary>
    /// Set the semantic memory to use the given memory storage and embedding generator.
    /// </summary>
    /// <param name="kernel">Kernel instance</param>
    /// <param name="storage">Memory storage</param>
    /// <param name="embeddingGenerator">Embedding generator</param>
    [SuppressMessage("Reliability", "CA2000:Dispose objects before losing scope", Justification = "The embeddingGenerator object is disposed by the kernel")]
    public static IKernel AddTextMemoryPlugin(this IKernel kernel, IMemoryStore storage, ITextEmbeddingGeneration embeddingGenerator)
    {
        Verify.NotNull(storage);
        Verify.NotNull(embeddingGenerator);

        kernel.AddTextMemoryPlugin(new SemanticTextMemory(storage, embeddingGenerator));
        return kernel;
    }

    /// <inheritdoc/>
    public static ISemanticTextMemory? GetMemory(this IKernel kernel)
    {
        return null;
    }
}
