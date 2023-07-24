// Copyright (c) Microsoft. All rights reserved.

using System.IO;
using System.Net;

namespace Microsoft.SemanticKernel.Context;

/// <summary>
/// A representation of the outgoing HTTP response.
/// </summary>
public abstract class FunctionResponseData
{
    /// <summary>
    /// Initializes a new instance of the <see cref="FunctionResponseData"/> class.
    /// </summary>
    /// <param name="functionContext">The <see cref="FunctionContext"/> for this response.</param>
    protected FunctionResponseData(FunctionContext functionContext)
    {
        this.FunctionContext = functionContext ?? throw new System.ArgumentNullException(nameof(functionContext));
    }

    /// <summary>
    /// Gets or sets the status code for the response.
    /// </summary>
    public abstract HttpStatusCode StatusCode { get; set; }

    ///// <summary>
    ///// Gets or sets a <see cref="HttpHeadersCollection"/> containing the response headers
    ///// </summary>
    //public abstract HttpHeadersCollection Headers { get; set; }

    /// <summary>
    /// Gets or sets the response body stream.
    /// </summary>
    public abstract Stream Body { get; set; }

    /// <summary>
    /// Gets the <see cref="FunctionContext"/> for this response.
    /// </summary>
    public FunctionContext FunctionContext { get; }

    /// <summary>
    /// Creates an HTTP response for the provided request.
    /// </summary>
    /// <param name="request">The request for which we need to create a response.</param>
    /// <returns>An <see cref="FunctionResponseData"/> that represens the response for the provided request.</returns>
    public static FunctionResponseData CreateResponse(FunctionRequestData request)
    {
        if (request is null)
        {
            throw new System.ArgumentNullException(nameof(request));
        }

        return request.CreateResponse();
    }
}
