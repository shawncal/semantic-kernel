// Copyright (c) Microsoft. All rights reserved.

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;

namespace Microsoft.SemanticKernel.Context.Converters;

/// <summary>
/// An implementation of <see cref="IInputConverterProvider"/> to get IInputConverter instances.
///  - Provides IInputConverter instances from what is defined in WorkerOptions.InputConverters
///  - Provides IInputConverter instances when requested for a specific type explicitly.
///  - Internally caches the instances created.
/// </summary>
internal sealed class DefaultInputConverterProvider : IInputConverterProvider
{
    private readonly IServiceProvider _serviceProvider;
    private readonly WorkerOptions _workerOptions;
    private readonly Type _inputConverterInterfaceType = typeof(IInputConverter);

    /// <summary>
    /// Stores all input converters.
    /// Key is assembly qualified name of the Converter implementation and value is the instance of it.
    /// </summary>
    private readonly ConcurrentDictionary<string, IInputConverter> _converterCache = new();

    public DefaultInputConverterProvider(WorkerOptions workerOptions, IServiceProvider serviceProvider)
    {
        this._workerOptions = workerOptions ?? throw new ArgumentNullException(nameof(workerOptions));
        this._serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));

        if (!this._workerOptions.InputConverters.Any())
        {
            throw new InvalidOperationException("No input converters found in worker options.");
        }
    }

    /// <summary>
    /// Get a collection of registered converter instances.
    /// </summary>
    public IEnumerable<IInputConverter> RegisteredInputConverters
    {
        get
        {
            foreach (var converterType in this._workerOptions.InputConverters!)
            {
                yield return this._converterCache.GetOrAdd(converterType.AssemblyQualifiedName!, (key) =>
                {
                    return this.GetOrCreateConverterInstance(converterType);
                });
            }
        }
    }

    /// <summary>
    /// Gets an instance of the converter for the type requested.
    /// </summary>
    /// <param name="converterType">The type for which we are requesting an IInputConverter instance.</param>
    /// <exception cref="InvalidOperationException">Throws when the converterType param is null.</exception>
    /// <returns>IConverter instance of the requested type.</returns>
    public IInputConverter GetOrCreateConverterInstance(Type converterType)
    {
        if (converterType is null)
        {
            throw new ArgumentNullException(nameof(converterType), $"Could not create an instance of {(nameof(converterType))}.");
        }

        this.EnsureTypeCanBeAssigned(converterType);

        return (IInputConverter)ActivatorUtilities.GetServiceOrCreateInstance(this._serviceProvider, converterType);
    }

    /// <summary>
    /// Gets an instance of the converter for the type requested.
    /// </summary>
    /// <param name="converterTypeName">The assembly qualified name of the type for which we are requesting an IInputConverter instance.</param>
    /// <exception cref="ArgumentNullException">Throws when the converterTypeName param is null.</exception>
    /// <returns>IConverter instance of the requested type.</returns>
    public IInputConverter GetOrCreateConverterInstance(string converterTypeName)
    {
        if (converterTypeName is null)
        {
            throw new ArgumentNullException((nameof(converterTypeName)));
        }

        // Get from cache or create the instance and cache
        return this._converterCache.GetOrAdd(converterTypeName, (key, converterTypeAssemblyQualifiedName) =>
        {
            // Create the instance and cache that against the assembly qualified name of the type.
            var converterType = Type.GetType(converterTypeAssemblyQualifiedName);

            if (converterType is null)
            {
                throw new InvalidOperationException($"Could not create an instance of {converterTypeAssemblyQualifiedName}.");
            }

            return this.GetOrCreateConverterInstance(converterType);
        }, converterTypeName);
    }

    /// <summary>
    /// Make sure the converter type is a type which has implemented <see cref="IInputConverter"/> interface
    /// </summary>
    private void EnsureTypeCanBeAssigned(Type converterType)
    {
        if (!this._inputConverterInterfaceType.IsAssignableFrom(converterType))
        {
            throw new InvalidOperationException(
                $"{converterType.Name} must implement {this._inputConverterInterfaceType.FullName} to be used as an input converter.");
        }
    }
}
