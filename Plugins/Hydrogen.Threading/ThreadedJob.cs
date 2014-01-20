#region Copyright Notice & License Information
//
// ThreadedJob.cs
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
		/// <summary>
		/// A base class for threaded jobs.
		/// </summary>
		public class ThreadedJob : JobBase
		{
				/// <summary>
				/// The job's thread.
				/// </summary>
				System.Threading.Thread _thread;

				/// <summary>
				/// System assigned thread.
				/// </summary>
				/// <value>The job.</value>
				public System.Threading.Thread Thread {
						get { return _thread; }
				}

				/// <summary>
				/// Start the work process.
				/// This should initialize the Thread with the Run function.
				/// </summary>
				/// <param name="backgroundThread">If set to <c>true</c> background thread.</param>
				/// <param name="priority">Priority.</param>
				public override void Start (bool backgroundThread, System.Threading.ThreadPriority priority)
				{
						// Create our new thread.
						_thread = new System.Threading.Thread (Run);

						// Establish threads background.
						_thread.IsBackground = backgroundThread;

						// Establish the threads priority.
						_thread.Priority = priority;

						// Start working!
						_thread.Start ();
				}

				/// <summary>
				/// Abort the Job (as best we can).
				/// </summary>
				protected override void Abort ()
				{
						_thread.Abort ();
						base.Abort ();
				}
		}
}