// Copyright (c) Microsoft. All rights reserved.

using System;

namespace Microsoft.SemanticKernel.Orchestration;

/// <summary>
/// The result of a <see cref="IKernel"/> RunAsync execution.
/// </summary>
public sealed class KernelResult
{
    public string? Result { get; private set; }

    /// <summary>
    /// When an error occurs, this is the most recent exception.
    /// </summary>
    public Exception? Exception { get; private set; }

    public bool ErrorOccurred => this.Exception != null;

    [Obsolete("Use 'Exception' property instead")]
    public Exception? LastException => this.Exception;

    internal KernelResult(string? result)
    {
        this.Result = result;
    }

    internal KernelResult(Exception exception)
    {
        this.Exception = exception;
    }

    public static implicit operator SKContext(KernelResult result)
    {
        var resultValue = result.Result; // TODO: Fix this before checkin
        var variables = new ContextVariables(resultValue);
        return new SKContext(variables);
    }

    //public static implicit operator KernelResult(SKContext context)
    //{
    //    if (context.LastException != null)
    //    {
    //        return new KernelResult(context.LastException);
    //    }

    //    return new KernelResult(context.Result);
    //}
}
