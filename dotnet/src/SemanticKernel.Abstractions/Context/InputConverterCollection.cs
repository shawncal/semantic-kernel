// Copyright (c) Microsoft. All rights reserved.

using System;
using System.Collections;
using System.Collections.Generic;
using Microsoft.SemanticKernel.Context.Converters;

namespace Microsoft.SemanticKernel.Context;

/// <summary>
/// A collection of input converters.
/// </summary>
public sealed class InputConverterCollection : IEnumerable<Type>
{
    // Passing initial capacity as a tiny optimization since we know we will be registering
    // at-least 8 built-in converters to this collection shortly while bootstrapping.
    private readonly IList<Type> _converterTypes = new List<Type>(capacity: 8);

    /// <summary>
    /// Registers an input converter type.
    /// </summary>
    /// <typeparam name="T">The input converter type. This type must implement <see cref="IInputConverter"/></typeparam>
    public void Register<T>() where T : IInputConverter
    {
        this._converterTypes.Add(typeof(T));
    }

    /// <summary>
    /// Registers an input converter type at the specific index.
    /// </summary>
    /// <typeparam name="T">The input converter type. This type must implement <see cref="IInputConverter"/></typeparam>
    public void RegisterAt<T>(int index) where T : IInputConverter
    {
        this._converterTypes.Insert(index, typeof(T));
    }

    /// <summary>
    /// Removes all items from the collection.
    /// </summary>
    public void Clear()
    {
        this._converterTypes.Clear();
    }

    /// <inheritdoc />
    public IEnumerator<Type> GetEnumerator() => this._converterTypes.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();
}
