// Copyright (c) Microsoft. All rights reserved.

using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Security.Claims;
using System.Web;

namespace Microsoft.SemanticKernel.Context;

/// <summary>
/// A request object representing the input to a function
/// </summary>
public abstract class FunctionRequestData
{
    private NameValueCollection? _query;

    /// <summary>
    /// Initializes a new instance of the <see cref="FunctionRequestData"/> class.
    /// </summary>
    /// <param name="functionContext">The <see cref="FunctionContext"/> for this request.</param>
    protected FunctionRequestData(FunctionContext functionContext)
    {
        this.FunctionContext = functionContext ?? throw new ArgumentNullException(nameof(functionContext));
    }

    /// <summary>
    /// A <see cref="Stream"/> containing the HTTP body data.
    /// </summary>
    public abstract Stream Body { get; }

    ///// <summary>
    ///// Gets a <see cref="HttpHeadersCollection"/> containing the request headers.
    ///// </summary>
    //public abstract HttpHeadersCollection Headers { get; }

    /// <summary>
    /// Gets the <see cref="Uri"/> for this request.
    /// </summary>
    public abstract Uri Url { get; }

    /// <summary>
    /// Gets an <see cref="IEnumerable{ClaimsIdentity}"/> containing the request identities.
    /// </summary>
    public abstract IEnumerable<ClaimsIdentity> Identities { get; }

    /// <summary>
    /// Gets the HTTP method for this request.
    /// </summary>
    public abstract string Method { get; }

    /// <summary>
    /// Gets the <see cref="FunctionContext"/> for this request.
    /// </summary>
    public FunctionContext FunctionContext { get; }

    /// <summary>
    /// Creates a response for this request.
    /// </summary>
    /// <returns>The response instance.</returns>
    public abstract FunctionResponseData CreateResponse();

    /// <summary>
    /// Gets the <see cref="NameValueCollection"/> containing the request query.
    /// </summary>
    public virtual NameValueCollection Query => this._query ??= HttpUtility.ParseQueryString(this.Url.Query);
}
