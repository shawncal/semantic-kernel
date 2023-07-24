// Copyright (c) Microsoft. All rights reserved.

using System;
using System.Collections.Concurrent;
using System.IO;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.SemanticKernel.Diagnostics;

namespace Microsoft.SemanticKernel.Context.Serialization;
/// <summary>
/// An <see cref="ObjectSerializer"/> implementation that uses <see cref="JsonSerializer"/> for serialization/deserialization.
/// </summary>
public sealed class JsonObjectSerializer : ObjectSerializer, IMemberNameConverter
{
    private const int JsonIgnoreConditionAlways = 1;

    private static PropertyInfo? s_jsonIgnoreAttributeCondition;
    private static bool s_jsonIgnoreAttributeConditionInitialized;

    private readonly ConcurrentDictionary<MemberInfo, string?> _cache;
    private readonly JsonSerializerOptions _options;

    /// <summary>
    /// A shared instance of <see cref="JsonObjectSerializer"/>, initialized with the default options.
    /// </summary>
    public static JsonObjectSerializer Default { get; } = new JsonObjectSerializer();

    /// <summary>
    /// Initializes new instance of <see cref="JsonObjectSerializer"/>.
    /// </summary>
    public JsonObjectSerializer() : this(new JsonSerializerOptions())
    {
    }

    /// <summary>
    /// Initializes new instance of <see cref="JsonObjectSerializer"/>.
    /// </summary>
    /// <param name="options">The <see cref="JsonSerializerOptions"/> instance to use when serializing/deserializing.</param>
    /// <exception cref="ArgumentNullException"><paramref name="options"/> is null.</exception>
    public JsonObjectSerializer(JsonSerializerOptions options)
    {
        this._options = options ?? throw new ArgumentNullException(nameof(options));

        // TODO: Consider using WeakReference cache to allow the GC to collect if the JsonObjectSerialized is held for a long duration.
        this._cache = new ConcurrentDictionary<MemberInfo, string?>();
    }

    /// <inheritdoc />
    public override void Serialize(Stream stream, object? value, Type inputType, CancellationToken cancellationToken)
    {
        var buffer = JsonSerializer.SerializeToUtf8Bytes(value, inputType, this._options);
        stream.Write(buffer, 0, buffer.Length);
    }

    /// <inheritdoc />
    public override async ValueTask SerializeAsync(Stream stream, object? value, Type inputType, CancellationToken cancellationToken)
    {
        await JsonSerializer.SerializeAsync(stream, value, inputType, this._options, cancellationToken).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public override object? Deserialize(Stream stream, Type returnType, CancellationToken cancellationToken)
    {
        using var memoryStream = new MemoryStream();
        stream.CopyTo(memoryStream);
        return JsonSerializer.Deserialize(memoryStream.ToArray(), returnType, this._options);
    }

    /// <inheritdoc />
    public override ValueTask<object?> DeserializeAsync(Stream stream, Type returnType, CancellationToken cancellationToken)
    {
        return JsonSerializer.DeserializeAsync(stream, returnType, this._options, cancellationToken);
    }

    /// <inheritdoc />
    public override BinaryData Serialize(object? value, Type? inputType = default, CancellationToken cancellationToken = default) =>
        this.SerializeToBinaryDataInternal(value, inputType);

    /// <inheritdoc />
    public override ValueTask<BinaryData> SerializeAsync(object? value, Type? inputType = default, CancellationToken cancellationToken = default) =>
         new(this.SerializeToBinaryDataInternal(value, inputType));

    private BinaryData SerializeToBinaryDataInternal(object? value, Type? inputType)
    {
        byte[] bytes = JsonSerializer.SerializeToUtf8Bytes(value, inputType ?? value?.GetType() ?? typeof(object), this._options);
        return new BinaryData(bytes);
    }

    /// <inheritdoc/>
    string? IMemberNameConverter.ConvertMemberName(MemberInfo member)
    {
        Verify.NotNull(member);

        return this._cache.GetOrAdd(member, m =>
        {
            // Mimics property enumeration based on:
            // * https://github.com/dotnet/corefx/blob/v3.1.0/src/System.Text.Json/src/System/Text/Json/Serialization/JsonClassInfo.cs#L130-L191
            // * TODO: Add support for fields when .NET 5 GAs (https://github.com/Azure/azure-sdk-for-net/issues/13627)

            if (m is PropertyInfo propertyInfo)
            {
                // Ignore indexers.
                if (propertyInfo.GetIndexParameters().Length > 0)
                {
                    return null;
                }

                // Only support public getters and/or setters.
                if (propertyInfo.GetMethod?.IsPublic == true ||
                    propertyInfo.SetMethod?.IsPublic == true)
                {
                    JsonIgnoreAttribute? attr = propertyInfo.GetCustomAttribute<JsonIgnoreAttribute>();
                    if (attr != null && GetCondition(attr) == JsonIgnoreConditionAlways)
                    {
                        return null;
                    }

                    // Ignore - but do not assert correctness - for JsonExtensionDataAttribute based on
                    // https://github.com/dotnet/corefx/blob/v3.1.0/src/System.Text.Json/src/System/Text/Json/Serialization/JsonClassInfo.cs#L244-L261
                    if (propertyInfo.GetCustomAttribute<JsonExtensionDataAttribute>() != null)
                    {
                        return null;
                    }

                    // No need to validate collisions since they are based on the serialized name.
                    return this.GetPropertyName(propertyInfo);
                }
            }

            // The member is unsupported or ignored.
            return null;
        });
    }

    private static int GetCondition(JsonIgnoreAttribute attribute)
    {
        if (!s_jsonIgnoreAttributeConditionInitialized)
        {
            s_jsonIgnoreAttributeCondition = typeof(JsonIgnoreAttribute).GetProperty("Condition", BindingFlags.Public | BindingFlags.Instance);
            s_jsonIgnoreAttributeConditionInitialized = true;
        }

        if (s_jsonIgnoreAttributeCondition != null)
        {
            return (int)s_jsonIgnoreAttributeCondition.GetValue(attribute)!;
        }

        // Return the default value in net5.0.
        return JsonIgnoreConditionAlways;
    }

    private string GetPropertyName(MemberInfo memberInfo)
    {
        // Mimics property name determination based on
        // https://github.com/dotnet/runtime/blob/dc8b6f90/src/libraries/System.Text.Json/src/System/Text/Json/Serialization/JsonPropertyInfo.cs#L53-L90

        JsonPropertyNameAttribute? nameAttribute = memberInfo.GetCustomAttribute<JsonPropertyNameAttribute>(false);
        if (nameAttribute != null)
        {
            return nameAttribute.Name
                ?? throw new InvalidOperationException($"The JSON property name for '{memberInfo.DeclaringType}.{memberInfo.Name}' cannot be null.");
        }
        else if (this._options.PropertyNamingPolicy != null)
        {
            return this._options.PropertyNamingPolicy.ConvertName(memberInfo.Name)
                ?? throw new InvalidOperationException($"The JSON property name for '{memberInfo.DeclaringType}.{memberInfo.Name}' cannot be null.");
        }
        else
        {
            return memberInfo.Name;
        }
    }
}
