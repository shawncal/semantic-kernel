﻿// Copyright (c) Microsoft. All rights reserved.

using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Microsoft.SemanticKernel.Diagnostics;

#pragma warning disable IDE0130
// ReSharper disable once CheckNamespace - Using the main namespace
namespace Microsoft.SemanticKernel;
#pragma warning restore IDE0130

/// <summary>
/// Read-only function collection interface.
/// </summary>
[SuppressMessage("Naming", "CA1711:Identifiers should not have incorrect suffix")]
public interface IReadOnlyFunctionCollection
{
    /// <summary>
    /// Gets the function stored in the collection.
    /// </summary>
    /// <param name="functionName">The name of the function to retrieve.</param>
    /// <returns>The function retrieved from the collection.</returns>
    /// <exception cref="SKException">The specified function could not be found in the collection.</exception>
    ISKFunction GetFunction(string functionName);

    /// <summary>
    /// Gets the function stored in the collection.
    /// </summary>
    /// <param name="pluginName">The name of the plugin with which the function is associated.</param>
    /// <param name="functionName">The name of the function to retrieve.</param>
    /// <returns>The function retrieved from the collection.</returns>
    /// <exception cref="SKException">The specified function could not be found in the collection.</exception>
    ISKFunction GetFunction(string pluginName, string functionName);

    /// <summary>
    /// Check if a function is available in the current context, and return it.
    /// </summary>
    /// <param name="functionName">The name of the function to retrieve.</param>
    /// <param name="availableFunction">When this method returns, the function that was retrieved if one with the specified name was found; otherwise, <see langword="null"/>.</param>
    /// <returns><see langword="true"/> if the function was found; otherwise, <see langword="false"/>.</returns>
    bool TryGetFunction(string functionName, [NotNullWhen(true)] out ISKFunction? availableFunction);

    /// <summary>
    /// Check if a function is available in the current context, and return it.
    /// </summary>
    /// <param name="pluginName">The name of the plugin with which the function is associated.</param>
    /// <param name="functionName">The name of the function to retrieve.</param>
    /// <param name="availableFunction">When this method returns, the function that was retrieved if one with the specified name was found; otherwise, <see langword="null"/>.</param>
    /// <returns><see langword="true"/> if the function was found; otherwise, <see langword="false"/>.</returns>
    bool TryGetFunction(string pluginName, string functionName, [NotNullWhen(true)] out ISKFunction? availableFunction);

    /// <summary>
    /// Get a snapshot all registered functions details, minus the delegates
    /// </summary>
    /// <returns>An object containing all the functions details</returns>
    IReadOnlyList<FunctionView> GetFunctionViews();
}
