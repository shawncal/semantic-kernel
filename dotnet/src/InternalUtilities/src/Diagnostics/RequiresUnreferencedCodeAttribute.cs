
// Copyright (c) Microsoft. All rights reserved.

// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

// This was copied from https://github.com/dotnet/aspnetcore/blob/main/src/Shared/TrimmingAttributes.cs
// and updated to have the scope of the attributes be internal.

// ReSharper disable once CheckNamespace
namespace System.Diagnostics.CodeAnalysis;

/// <summary>
/// Indicates that the specified method requires dynamic access to code that is not referenced
/// statically, for example through <see cref="System.Reflection"/>.
/// </summary>
/// <remarks>
/// This allows tools to understand which methods are unsafe to call when removing unreferenced
/// code from an application.
/// </remarks>
[AttributeUsage(AttributeTargets.Method | AttributeTargets.Constructor | AttributeTargets.Class, Inherited = false)]
internal sealed class RequiresUnreferencedCodeAttribute : Attribute
{
    /// <summary>
    /// Initializes a new instance of the <see cref="RequiresUnreferencedCodeAttribute"/> class
    /// with the specified message.
    /// </summary>
    /// <param name="message">
    /// A message that contains information about the usage of unreferenced code.
    /// </param>
    public RequiresUnreferencedCodeAttribute(string message)
    {
        this.Message = message;
    }

    /// <summary>
    /// Gets a message that contains information about the usage of unreferenced code.
    /// </summary>
    public string Message { get; }

    /// <summary>
    /// Gets or sets an optional URL that contains more information about the method,
    /// why it requires unreferenced code, and what options a consumer has to deal with it.
    /// </summary>
    public string? Url { get; set; }
}
