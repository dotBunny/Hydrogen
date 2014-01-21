#region Copyright Notice & License Information
//
// ThreadPoolJob.cs
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
		/// A base class for ThreadPool jobs.
		/// </summary>
		public class ThreadPoolJob : JobBase
		{
				/// <summary>
				/// ThreadPool Delegate
				/// </summary>
				System.Threading.WaitCallback _callback;

				/// <summary>
				/// Sends the work process to the ThreadPool for starting when resources are available, 
				/// This needs to initialize the WaitCallback with the Run function.
				/// </summary>
				/// <param name="backgroundThread">Not Used.</param>
				/// <param name="priority">Not Used.</param>
				public override void Start (bool backgroundThread, System.Threading.ThreadPriority priority)
				{
						_callback = new System.Threading.WaitCallback (Run);
						System.Threading.ThreadPool.QueueUserWorkItem (_callback);
				}
		}
}