﻿// Copyright (c) Microsoft. All rights reserved.

using System;
using System.Text.Json.Nodes;
using Microsoft.SemanticKernel.Diagnostics;
using Microsoft.SemanticKernel.Functions.OpenAPI.Model;

namespace Microsoft.SemanticKernel.Functions.OpenAPI.Builders.Serialization;

/// <summary>
/// Serializes REST API operation parameter of the 'PipeDelimited' style.
/// </summary>
internal static class PipeDelimitedStyleParameterSerializer
{
    /// <summary>
    /// Serializes a REST API operation `PipeDelimited` style parameter.
    /// </summary>
    /// <param name="parameter">The REST API operation parameter to serialize.</param>
    /// <param name="argument">The parameter argument.</param>
    /// <returns>The serialized parameter.</returns>
    public static string Serialize(RestApiOperationParameter parameter, string argument)
    {
        const string ArrayType = "array";

        if (parameter is null)
        {
            throw new ArgumentNullException(nameof(parameter));
        }

        if (parameter.Style != RestApiOperationParameterStyle.PipeDelimited)
        {
            throw new SKException($"Unexpected Rest Api operation parameter style `{parameter.Style}`. Parameter name `{parameter.Name}`.");
        }

        if (parameter.Type != ArrayType)
        {
            throw new SKException($"Serialization of Rest API operation parameters of type `{parameter.Type}` is not supported for the `{RestApiOperationParameterStyle.PipeDelimited}` style parameters. Parameter name `{parameter.Name}`.");
        }

        return SerializeArrayParameter(parameter, argument);
    }

    /// <summary>
    /// Serializes an array-type parameter.
    /// </summary>
    /// <param name="parameter">The REST API operation parameter to serialize.</param>
    /// <param name="argument">The argument value.</param>
    /// <returns>The serialized parameter string.</returns>
    private static string SerializeArrayParameter(RestApiOperationParameter parameter, string argument)
    {
        if (JsonNode.Parse(argument) is not JsonArray array)
        {
            throw new SKException($"Can't deserialize parameter name `{parameter.Name}` argument `{argument}` to JSON array.");
        }

        if (parameter.Expand)
        {
            return ArrayParameterValueSerializer.SerializeArrayAsSeparateParameters(parameter.Name, array, delimiter: "&"); //id=1&id=2&id=3
        }

        return $"{parameter.Name}={ArrayParameterValueSerializer.SerializeArrayAsDelimitedValues(array, delimiter: "|")}"; //id=1|2|3
    }
}
