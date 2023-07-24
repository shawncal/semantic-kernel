// Copyright (c) Microsoft. All rights reserved.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.SemanticKernel.Context;

internal class InvocationFeatures : IInvocationFeatures
{
    private readonly Dictionary<Type, object> _features = new();
    private readonly IEnumerable<IInvocationFeatureProvider> _featureProviders;

    public InvocationFeatures(IEnumerable<IInvocationFeatureProvider> featureProviders)
    {
        this._featureProviders = featureProviders;
    }

    public T? Get<T>()
    {
        var type = typeof(T);
        if (!this._features.TryGetValue(type, out object? feature))
        {
            if (this._featureProviders.Any(t => t.TryCreate(type, out feature)) && !this._features.TryAdd(type, feature!))
            {
                feature = this._features[type];
            }
        }

        return feature is null ? default : (T)feature;
    }

    public IEnumerator<KeyValuePair<Type, object>> GetEnumerator()
    {
        return this._features.GetEnumerator();
    }

    public void Set<T>(T instance)
    {
        if (instance is null)
        {
            throw new ArgumentNullException(nameof(instance));
        }

        this._features[typeof(T)] = instance;
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return this._features.GetEnumerator();
    }
}
