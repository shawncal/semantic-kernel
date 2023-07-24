// Copyright (c) Microsoft. All rights reserved.

#nullable disable

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

internal static class TaskExtensions
{
    public static WithCancellationTaskAwaitable AwaitWithCancellation(this Task task, CancellationToken cancellationToken)
        => new(task, cancellationToken);

    public static WithCancellationTaskAwaitable<T> AwaitWithCancellation<T>(this Task<T> task, CancellationToken cancellationToken)
        => new(task, cancellationToken);

    public static WithCancellationValueTaskAwaitable<T> AwaitWithCancellation<T>(this ValueTask<T> task, CancellationToken cancellationToken)
        => new(task, cancellationToken);

    public static T EnsureCompleted<T>(this Task<T> task)
    {
#if DEBUG
        VerifyTaskCompleted(task.IsCompleted);
#endif
        return task.GetAwaiter().GetResult();
    }

    public static void EnsureCompleted(this Task task)
    {
#if DEBUG
        VerifyTaskCompleted(task.IsCompleted);
#endif
        task.GetAwaiter().GetResult();
    }

    public static T EnsureCompleted<T>(this ValueTask<T> task)
    {
#if DEBUG
        VerifyTaskCompleted(task.IsCompleted);
#endif
        return task.GetAwaiter().GetResult();
    }

    public static void EnsureCompleted(this ValueTask task)
    {
#if DEBUG
        VerifyTaskCompleted(task.IsCompleted);
#endif
        task.GetAwaiter().GetResult();
    }

    public static Enumerable<T> EnsureSyncEnumerable<T>(this IAsyncEnumerable<T> asyncEnumerable) => new(asyncEnumerable);

    public static ConfiguredValueTaskAwaitable<T> EnsureCompleted<T>(this ConfiguredValueTaskAwaitable<T> awaitable, bool async)
    {
        if (!async)
        {
#if DEBUG
            VerifyTaskCompleted(awaitable.GetAwaiter().IsCompleted);
#endif
        }
        return awaitable;
    }

    public static ConfiguredValueTaskAwaitable EnsureCompleted(this ConfiguredValueTaskAwaitable awaitable, bool async)
    {
        if (!async)
        {
#if DEBUG
            VerifyTaskCompleted(awaitable.GetAwaiter().IsCompleted);
#endif
        }
        return awaitable;
    }

    [Conditional("DEBUG")]
    private static void VerifyTaskCompleted(bool isCompleted)
    {
        if (!isCompleted)
        {
            if (Debugger.IsAttached)
            {
                Debugger.Break();
            }
            // Throw an InvalidOperationException instead of using
            // Debug.Assert because that brings down nUnit immediately
            throw new InvalidOperationException("Task is not completed");
        }
    }

    /// <summary>
    /// Both <see cref="Enumerable{T}"/> and <see cref="Enumerator{T}"/> are defined as public structs so that foreach can use duck typing
    /// to call <see cref="Enumerable{T}.GetEnumerator"/> and avoid heap memory allocation.
    /// Please don't delete this method and don't make these types private.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public readonly struct Enumerable<T> : IEnumerable<T>
    {
        private readonly IAsyncEnumerable<T> _asyncEnumerable;

        public Enumerable(IAsyncEnumerable<T> asyncEnumerable) => this._asyncEnumerable = asyncEnumerable;

        public Enumerator<T> GetEnumerator() => new(this._asyncEnumerable.GetAsyncEnumerator());

        IEnumerator<T> IEnumerable<T>.GetEnumerator() => new Enumerator<T>(this._asyncEnumerable.GetAsyncEnumerator());

        IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();
    }

    public readonly struct Enumerator<T> : IEnumerator<T>
    {
        private readonly IAsyncEnumerator<T> _asyncEnumerator;

        public Enumerator(IAsyncEnumerator<T> asyncEnumerator) => this._asyncEnumerator = asyncEnumerator;

        public bool MoveNext() => this._asyncEnumerator.MoveNextAsync().EnsureCompleted();

        public void Reset() => throw new NotSupportedException($"{this.GetType()} is a synchronous wrapper for {this._asyncEnumerator.GetType()} async enumerator, which can't be reset, so IEnumerable.Reset() calls aren't supported.");

        public T Current => this._asyncEnumerator.Current;

        object IEnumerator.Current => this.Current;

        public void Dispose() => this._asyncEnumerator.DisposeAsync().EnsureCompleted();
    }

    public readonly struct WithCancellationTaskAwaitable
    {
        private readonly CancellationToken _cancellationToken;
        private readonly ConfiguredTaskAwaitable _awaitable;

        public WithCancellationTaskAwaitable(Task task, CancellationToken cancellationToken)
        {
            this._awaitable = task.ConfigureAwait(false);
            this._cancellationToken = cancellationToken;
        }

        public WithCancellationTaskAwaiter GetAwaiter() => new(this._awaitable.GetAwaiter(), this._cancellationToken);
    }

    public readonly struct WithCancellationTaskAwaitable<T>
    {
        private readonly CancellationToken _cancellationToken;
        private readonly ConfiguredTaskAwaitable<T> _awaitable;

        public WithCancellationTaskAwaitable(Task<T> task, CancellationToken cancellationToken)
        {
            this._awaitable = task.ConfigureAwait(false);
            this._cancellationToken = cancellationToken;
        }

        public WithCancellationTaskAwaiter<T> GetAwaiter() => new(this._awaitable.GetAwaiter(), this._cancellationToken);
    }

    public readonly struct WithCancellationValueTaskAwaitable<T>
    {
        private readonly CancellationToken _cancellationToken;
        private readonly ConfiguredValueTaskAwaitable<T> _awaitable;

        public WithCancellationValueTaskAwaitable(ValueTask<T> task, CancellationToken cancellationToken)
        {
            this._awaitable = task.ConfigureAwait(false);
            this._cancellationToken = cancellationToken;
        }

        public WithCancellationValueTaskAwaiter<T> GetAwaiter() => new(this._awaitable.GetAwaiter(), this._cancellationToken);
    }

    public readonly struct WithCancellationTaskAwaiter : ICriticalNotifyCompletion
    {
        private readonly CancellationToken _cancellationToken;
        private readonly ConfiguredTaskAwaitable.ConfiguredTaskAwaiter _taskAwaiter;

        public WithCancellationTaskAwaiter(ConfiguredTaskAwaitable.ConfiguredTaskAwaiter awaiter, CancellationToken cancellationToken)
        {
            this._taskAwaiter = awaiter;
            this._cancellationToken = cancellationToken;
        }

        public bool IsCompleted => this._taskAwaiter.IsCompleted || this._cancellationToken.IsCancellationRequested;

        public void OnCompleted(Action continuation) => this._taskAwaiter.OnCompleted(this.WrapContinuation(continuation));

        public void UnsafeOnCompleted(Action continuation) => this._taskAwaiter.UnsafeOnCompleted(this.WrapContinuation(continuation));

        public void GetResult()
        {
            Debug.Assert(this.IsCompleted);
            if (!this._taskAwaiter.IsCompleted)
            {
                this._cancellationToken.ThrowIfCancellationRequested();
            }
            this._taskAwaiter.GetResult();
        }

        private Action WrapContinuation(in Action originalContinuation)
            => this._cancellationToken.CanBeCanceled
                ? new WithCancellationContinuationWrapper(originalContinuation, this._cancellationToken).Continuation
                : originalContinuation;
    }

    public readonly struct WithCancellationTaskAwaiter<T> : ICriticalNotifyCompletion
    {
        private readonly CancellationToken _cancellationToken;
        private readonly ConfiguredTaskAwaitable<T>.ConfiguredTaskAwaiter _taskAwaiter;

        public WithCancellationTaskAwaiter(ConfiguredTaskAwaitable<T>.ConfiguredTaskAwaiter awaiter, CancellationToken cancellationToken)
        {
            this._taskAwaiter = awaiter;
            this._cancellationToken = cancellationToken;
        }

        public bool IsCompleted => this._taskAwaiter.IsCompleted || this._cancellationToken.IsCancellationRequested;

        public void OnCompleted(Action continuation) => this._taskAwaiter.OnCompleted(this.WrapContinuation(continuation));

        public void UnsafeOnCompleted(Action continuation) => this._taskAwaiter.UnsafeOnCompleted(this.WrapContinuation(continuation));

        public T GetResult()
        {
            Debug.Assert(this.IsCompleted);
            if (!this._taskAwaiter.IsCompleted)
            {
                this._cancellationToken.ThrowIfCancellationRequested();
            }
            return this._taskAwaiter.GetResult();
        }

        private Action WrapContinuation(in Action originalContinuation)
            => this._cancellationToken.CanBeCanceled
                ? new WithCancellationContinuationWrapper(originalContinuation, this._cancellationToken).Continuation
                : originalContinuation;
    }

    public readonly struct WithCancellationValueTaskAwaiter<T> : ICriticalNotifyCompletion
    {
        private readonly CancellationToken _cancellationToken;
        private readonly ConfiguredValueTaskAwaitable<T>.ConfiguredValueTaskAwaiter _taskAwaiter;

        public WithCancellationValueTaskAwaiter(ConfiguredValueTaskAwaitable<T>.ConfiguredValueTaskAwaiter awaiter, CancellationToken cancellationToken)
        {
            this._taskAwaiter = awaiter;
            this._cancellationToken = cancellationToken;
        }

        public bool IsCompleted => this._taskAwaiter.IsCompleted || this._cancellationToken.IsCancellationRequested;

        public void OnCompleted(Action continuation) => this._taskAwaiter.OnCompleted(this.WrapContinuation(continuation));

        public void UnsafeOnCompleted(Action continuation) => this._taskAwaiter.UnsafeOnCompleted(this.WrapContinuation(continuation));

        public T GetResult()
        {
            Debug.Assert(this.IsCompleted);
            if (!this._taskAwaiter.IsCompleted)
            {
                this._cancellationToken.ThrowIfCancellationRequested();
            }
            return this._taskAwaiter.GetResult();
        }

        private Action WrapContinuation(in Action originalContinuation)
            => this._cancellationToken.CanBeCanceled
                ? new WithCancellationContinuationWrapper(originalContinuation, this._cancellationToken).Continuation
                : originalContinuation;
    }

    private class WithCancellationContinuationWrapper
    {
        private Action _originalContinuation;
        private readonly CancellationTokenRegistration _registration;

        public WithCancellationContinuationWrapper(Action originalContinuation, CancellationToken cancellationToken)
        {
            Action continuation = this.ContinuationImplementation;
            this._originalContinuation = originalContinuation;
            this._registration = cancellationToken.Register(continuation);
            this.Continuation = continuation;
        }

        public Action Continuation { get; }

        private void ContinuationImplementation()
        {
            Action originalContinuation = Interlocked.Exchange(ref this._originalContinuation, null);
            if (originalContinuation != null)
            {
                this._registration.Dispose();
                originalContinuation();
            }
        }
    }
}
