#region Copyright Notice & License Information
//
// Forker.cs
//
// Author:
//       Matthew Davey <matthew.davey@dotbunny.com>
//       Marc Gravell <marc.gravell@gmail.com>
//
// Copyright (c) 2014 dotBunny Inc. (http://www.dotbunny.com)
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.
#endregion

using System;
using System.Threading;

namespace Hydrogen.Threading
{
		/// <summary>
		/// Provides a caller-friendly wrapper around parallel actions.
		/// </summary>
		public sealed class Forker
		{
				/// <summary>
				/// All Complete Event.
				/// </summary>
				EventHandler _allComplete;
				/// <summary>
				/// The event lock object used for safe threading.
				/// </summary>
				readonly object _eventLock = new object ();
				/// <summary>
				/// Event fired when an item is complete.
				/// </summary>
				EventHandler<ParallelEventArgs> _itemComplete;
				/// <summary>
				/// The join lock object used for safe threading.
				/// </summary>
				readonly object _joinLock = new object ();
				/// <summary>
				/// The number of items running across the Forker.
				/// </summary>
				int _runningCount;

				/// <summary>
				/// Raised when all operations have completed.
				/// </summary>
				public event EventHandler AllComplete {
						add {
								lock (_eventLock) {
										_allComplete += value;
								}
						}
						remove {
								lock (_eventLock) {
										_allComplete -= value;
								}
						}
				}

				/// <summary>
				/// Raised when each operation completes.
				/// </summary>
				public event EventHandler<ParallelEventArgs> ItemComplete {
						add {
								lock (_eventLock) {
										_itemComplete += value;
								}
						}
						remove {
								lock (_eventLock) {
										_itemComplete -= value;
								}
						}
				}

				/// <summary>
				/// Indicates the number of incomplete operations.
				/// </summary>
				/// <returns>The number of incomplete operations.</returns>
				public int CountRunning ()
				{
						return Interlocked.CompareExchange (ref _runningCount, 0, 0);
				}

				/// <summary>
				/// Enqueues an operation.
				/// </summary>
				/// <param name="action">The operation to perform.</param>
				/// <returns>The current instance (for fluent API).</returns>
				public Forker Fork (ThreadStart action)
				{
						return Fork (action, null);
				}

				/// <summary>
				/// Enqueues an operation.
				/// </summary>
				/// <param name="action">The operation to perform.</param>
				/// <param name="state">An opaque object, allowing the caller to identify operations.</param>
				/// <returns>The current instance (for fluent API).</returns>
				public Forker Fork (ThreadStart action, object state)
				{
						if (action == null)
								throw new ArgumentNullException ("action");
						Interlocked.Increment (ref _runningCount);
						ThreadPool.QueueUserWorkItem (delegate {
								Exception exception = null;
								try {
										action ();
								} catch (Exception ex) {
										exception = ex;
								}
								OnItemComplete (state, exception);
						});
						return this;
				}

				/// <summary>
				/// Waits for all operations to complete.
				/// </summary>
				public void Join ()
				{
						Join (-1);
				}

				/// <summary>
				/// Waits (with timeout) for all operations to complete.
				/// </summary>
				/// <returns>Whether all operations had completed before the timeout.</returns>
				public bool Join (int millisecondsTimeout)
				{
						lock (_joinLock) {
								if (CountRunning () == 0)
										return true;
								Thread.SpinWait (1); // try our luck...
								return (CountRunning () == 0) ||
								Monitor.Wait (_joinLock, millisecondsTimeout);
						}
				}

				/// <summary>
				/// Adds a callback to invoke when each operation completes.
				/// </summary>
				/// <returns>Current instance (for fluent API).</returns>
				public Forker OnItemComplete (EventHandler<ParallelEventArgs> handler)
				{
						if (handler == null)
								throw new ArgumentNullException ("handler");
						ItemComplete += handler;
						return this;
				}

				/// <summary>
				/// Adds a callback to invoke when all operations are complete.
				/// </summary>
				/// <returns>Current instance (for fluent API).</returns>
				public Forker OnAllComplete (EventHandler handler)
				{
						if (handler == null)
								throw new ArgumentNullException ("handler");
						AllComplete += handler;
						return this;
				}

				/// <summary>
				/// Raised when an item is completed.
				/// </summary>
				/// <param name="state">Item State.</param>
				/// <param name="exception">Exception.</param>
				void OnItemComplete (object state, Exception exception)
				{
						EventHandler<ParallelEventArgs> itemHandler = _itemComplete; // don't need to lock
						if (itemHandler != null)
								itemHandler (this, new ParallelEventArgs (state, exception));
						if (Interlocked.Decrement (ref _runningCount) == 0) {
								EventHandler allHandler = _allComplete; // don't need to lock
								if (allHandler != null)
										allHandler (this, EventArgs.Empty);
								lock (_joinLock) {
										Monitor.PulseAll (_joinLock);
								}
						}
				}

				/// <summary>
				/// Event arguments representing the completion of a parallel action.
				/// </summary>
				public class ParallelEventArgs : EventArgs
				{
						/// <summary>
						/// The System.Exception
						/// </summary>
						readonly Exception _exception;
						/// <summary>
						/// The State
						/// </summary>
						readonly object _state;

						/// <summary>
						/// Initializes a new instance of the <see cref="Forker+ParallelEventArgs"/> class.
						/// </summary>
						/// <param name="state">Object State.</param>
						/// <param name="exception">The System.Exeception.</param>
						internal ParallelEventArgs (object state, Exception exception)
						{
								_state = state;
								_exception = exception;
						}

						/// <summary>
						/// The opaque state object that identifies the action (null otherwise).
						/// </summary>
						public object State { get { return _state; } }

						/// <summary>T
						/// he exception thrown by the parallel action, or null if it completed without exception.
						/// </summary>
						public Exception Exception { get { return _exception; } }
				}
		}
}