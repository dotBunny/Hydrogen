#region Copyright Notice & License Information
//
// JobBase.cs
//
// Author:
//       Matthew Davey <matthew.davey@dotbunny.com>
//
// Copyright (c) 2013 dotBunny Inc. (http://www.dotbunny.com)
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

namespace Hydrogen.Threading
{
		// <summary>
		/// The base for all classes which want to extend the behaviour of a Job.
		/// </summary>
		public abstract class JobBase
		{
				/// <summary>
				/// Is the Job busy working?
				/// </summary>
				internal bool _isBusy;
				/// <summary>
				/// Is the Job done?
				/// </summary>
				internal bool _isDone;
				/// <summary>
				/// Has the OnFinished been called?
				/// </summary>
				internal bool _firedOnFinished;
				/// <summary>
				/// Internal fail safe to maintain lock.
				/// </summary>
				internal object _syncRoot = new object ();

				/// <summary>
				/// Is the Job busy working?
				/// </summary>
				/// <value><c>true</c> if is; otherwise, <c>false</c>.</value>
				public bool IsBusy {
						get {
								bool tmp;
								lock (_syncRoot) {
										tmp = _isBusy;
								}
								return tmp;
						}
						protected set {
								lock (_syncRoot) {
										_isBusy = value;
								}
						}
				}

				/// <summary>
				/// Is the Job done?
				/// </summary>
				/// <value><c>true</c> if is; otherwise, <c>false</c>.</value>
				public bool IsDone {
						get {
								bool tmp;
								lock (_syncRoot) {
										tmp = _isDone;
								}
								return tmp;
						}
						protected set {
								lock (_syncRoot) {
										_isDone = value;
								}
						}
				}

				/// <summary>
				/// Has the OnFinished been called yet?
				/// </summary>
				/// <value><c>true</c> if it has; otherwise, <c>false</c>.</value>
				public bool FiredOnFinished {
						get {
								bool tmp;
								lock (_syncRoot) {
										tmp = _firedOnFinished;
								}
								return tmp;
						}
						protected set {
								lock (_syncRoot) {
										_firedOnFinished = value;
								}
						}
				}

				/// <summary>
				/// Checks if the Job is done, and facilitates calling OnFinished when completed.
				/// </summary>
				public virtual bool Check ()
				{
						if (IsDone) {

								// Only fire off our OnFinished if we haven't before.
								if (!FiredOnFinished) {

										// Fireoff our Finish Code
										OnFinished ();

										// Stop From Firing Again
										FiredOnFinished = true;
								}
								return true;
						}
						return false;
				}

				/// <summary>
				/// Abort the Job (as best we can).
				/// </summary>
				protected virtual void Abort ()
				{
						IsBusy = false;
						IsDone = false;
						FiredOnFinished = true;
				}

				/// <summary>
				/// Called once by Check when the Job has finished.
				/// </summary>
				/// <remarks>Can use Unity API.</remarks>
				protected virtual void OnFinished ()
				{
				}

				/// <summary>
				/// The launcher of the ThreadedFunction, used to handle the state as well.
				/// </summary>
				/// <param name="state">Irrelevent / Not Used. Required for the ThreadPool to be used.</param>
				protected virtual void Run (object state)
				{
						IsBusy = true;

						// I guess we can't be done now can we?
						IsDone = false;

						FiredOnFinished = false;

						// Execute our threaded function
						ThreadedFunction ();

						// Not busy anymore
						IsBusy = false;

						// Yup we are now done.
						IsDone = true;
				}

				/// <summary>
				/// Start the work process, should probably send the Run function to the thread!
				/// </summary>
				/// <param name="backgroundThread">If set to <c>true</c> the thread will be set to background.</param>
				/// <param name="priority">The thread priority.</param>
				public abstract void Start (bool backgroundThread, System.Threading.ThreadPriority priority);

				/// <summary>
				/// The work horse function that MUST BE THREAD SAFE. Do not touch the Unity API! 
				/// It will cause an exception if you do, and things will act like a 4 year old having a tantrum.
				/// Debugging should be handled via System.Console and you should be aware that exceptions will not 
				/// show up in Unity if they happen on the thread; therefore you will never know if its broken so test 
				/// your code outside of the threading first, or have copius amounts of debugging implemented.
				/// </summary>
				/// <remarks>Threading: Because when it works, its awesome.</remarks>
				protected virtual void ThreadedFunction ()
				{

				}
		}
}