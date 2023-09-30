// Copyright (c) Microsoft. All rights reserved.

using System.Diagnostics;
using System.Globalization;
using Microsoft.Extensions.Logging;
using Microsoft.SemanticKernel.Diagnostics;

namespace Microsoft.SemanticKernel.Orchestration;

/// <summary>
/// Semantic Kernel context.
/// </summary>
[DebuggerDisplay("{DebuggerDisplay,nq}")]
public sealed class DefaultSKContext : SKContext
{
    /// <summary>
    /// Print the processed input, aka the current data after any processing occurred.
    /// </summary>
    /// <returns>Processed input, aka result</returns>
    public override string Result => this.Variables.ToString();

    /// <summary>
    /// The culture currently associated with this context.
    /// </summary>
    public override CultureInfo Culture
    {
        get => this._culture;
        set => this._culture = value ?? CultureInfo.CurrentCulture;
    }

    /// <summary>
    /// User variables
    /// </summary>
    public override ContextVariables Variables { get; }

    /// <summary>
    /// Read only functions collection
    /// </summary>
    public override IReadOnlyFunctionCollection Functions { get; }

    /// <summary>
    /// App logger
    /// </summary>
    public override ILoggerFactory LoggerFactory { get; }

    /// <summary>
    /// Kernel context reference
    /// </summary>
    public override IKernel Kernel => this.GetKernelContext();

    /// <summary>
    /// Create a clone of the current context, using the same kernel references (memory, functions, logger)
    /// and a new set variables, so that variables can be modified without affecting the original context.
    /// </summary>
    /// <returns>A new context copied from the current one</returns>
    public override SKContext Clone()
    {
        return new DefaultSKContext(
            kernel: this._originalKernel,
            variables: this.Variables.Clone())
        {
            Culture = this.Culture,
        };
    }

    /// <summary>
    /// Constructor for the context.
    /// </summary>
    /// <param name="kernel">Kernel reference</param>
    /// <param name="variables">Context variables to include in context.</param>
    /// <param name="functions">Functions to include in context.</param>
    internal DefaultSKContext(
        IKernel kernel,
        ContextVariables? variables = null,
        IReadOnlyFunctionCollection? functions = null)
    {
        Verify.NotNull(kernel, nameof(kernel));

        this._originalKernel = kernel;
        this.Variables = variables ?? new();
        this.Functions = functions ?? NullReadOnlyFunctionCollection.Instance;
        this.LoggerFactory = kernel.LoggerFactory;
        this._culture = CultureInfo.CurrentCulture;
    }

    /// <summary>
    /// Constructor for the context.
    /// </summary>
    /// <param name="kernel">Kernel instance parameter</param>
    /// <param name="variables">Context variables to include in context.</param>
    internal DefaultSKContext(
        IKernel kernel,
        ContextVariables? variables = null)
            : this(kernel, variables, kernel.Functions)
    {
    }

    /// <summary>
    /// Constructor for the context.
    /// </summary>
    /// <param name="kernel">Kernel instance parameter</param>
    /// <param name="functions">Functions to include in context.</param>
    internal DefaultSKContext(
        IKernel kernel,
        IReadOnlyFunctionCollection? functions = null)
            : this(kernel, null, functions)
    {
    }

    /// <summary>
    /// Constructor for the context.
    /// </summary>
    /// <param name="kernel">Kernel instance parameter</param>
    internal DefaultSKContext(IKernel kernel)
        : this(kernel, null, kernel.Functions)
    {
    }

    /// <summary>
    /// The culture currently associated with this context.
    /// </summary>
    private CultureInfo _culture;

    /// <summary>
    /// Kernel instance reference for this context.
    /// </summary>
    private readonly IKernel _originalKernel;

    /// <summary>
    /// Spawns the kernel for the context.
    /// </summary>
    /// <remarks>
    /// The kernel context is a lightweight instance of the main kernel with its services.
    /// </remarks>
    /// <returns>Kernel reference</returns>
    private IKernel GetKernelContext()
        => this._originalKernel; // TODO: Clone a lightweight kernel instead of returning the same instance

    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    private string DebuggerDisplay
    {
        get
        {
            string display = this.Variables.DebuggerDisplay;

            if (this.Functions is IReadOnlyFunctionCollection functions)
            {
                var view = functions.GetFunctionViews();
                display += $", Functions = {view.Count}";
            }

            display += $", Culture = {this.Culture.EnglishName}";

            return display;
        }
    }
}
