#region Copyright Notice & License Information
//
// Job.cs
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
		public abstract class JobBase
		{
				internal bool _isBusy;
				internal bool _isDone;
				internal object _syncRoot = new object ();

				public bool IsDone {
						get {
								bool tmp;
								lock (_syncRoot) {
										tmp = _isDone;
								}
								return tmp;
						}
						set {
								lock (_syncRoot) {
										_isDone = value;
								}
						}
				}

				public bool IsBusy {
						get {
								bool tmp;
								lock (_syncRoot) {
										tmp = _isBusy;
								}
								return tmp;
						}
						set {
								lock (_syncRoot) {
										_isBusy = value;
								}
						}
				}

				public virtual bool Check ()
				{
						if (IsDone) {
								OnFinished ();
								return true;
						}
						return false;
				}

				public abstract void Start (bool backgroundThread, System.Threading.ThreadPriority priority);

				protected virtual void Run (object state)
				{
						IsBusy = true;

						// I guess we can't be done now can we?
						IsDone = false;

						// Execute our threaded function
						ThreadedFunction ();

						IsBusy = false;

						// Yup we are now done.
						IsDone = true;
				}

				protected virtual void ThreadedFunction ()
				{

				}

				protected virtual void OnFinished ()
				{

				}

				protected virtual void Abort ()
				{
						IsBusy = false;
						IsDone = false;
				}
		}
}