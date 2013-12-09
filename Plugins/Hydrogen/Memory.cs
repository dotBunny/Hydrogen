#region Copyright Notice & License Information
//
// Memory.cs
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

using System;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

namespace Hydrogen
{
		/// <summary>
		/// Additional static functions used to extend existing Memory support inside of Unity.
		/// </summary>
		public static class Memory
		{
				/// <summary>
				/// Perform a deep Copy of the object.
				/// </summary>
				/// <returns>The copied object.</returns>
				/// <typeparam name="T">Object Type</typeparam>
				/// <param name="source">The object instance to copy.</param>
				public static T Clone<T> (this T source)
				{
						if (!typeof(T).IsSerializable) {
								throw new ArgumentException ("The type must be serializable.", "source");
						}

						// Don't serialize a null object, simply return the default for that object
						if (Object.ReferenceEquals (source, null)) {
								return default(T);
						}

						IFormatter formatter = new BinaryFormatter ();
						Stream stream = new MemoryStream ();
						using (stream) {
								formatter.Serialize (stream, source);
								stream.Seek (0, SeekOrigin.Begin);
								return (T)formatter.Deserialize (stream);
						}
				}
		}
}