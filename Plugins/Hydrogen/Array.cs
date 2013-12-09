#region Copyright Notice & License Information 
//
// Array.cs
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

using System.Linq;

namespace Hydrogen
{
		/// <summary>
		/// Additional static functions used to extend existing Array support inside of Unity.
		/// </summary>
		public static class Array
		{
				/// <summary>
				/// Add a unique item to an array.
				/// </summary>
				/// <returns><c>true</c>, if something was added, <c>false</c> otherwise.</returns>
				/// <param name="array">The source array</param>
				/// <param name="newObject">Object to be added.</param>
				/// <param name="forceUnique">Should it check to see if there is an existing reference in the array?</param>
				/// <typeparam name="T">Object Type.</typeparam>
				public static bool Add<T> (ref T[] array, T newObject, bool forceUnique)
				{
						return AddAt (ref array, array.Length, newObject, forceUnique);
				}

				/// <summary>
				/// Add an item to an array.
				/// </summary>
				/// <returns>Was anything added?</returns>
				/// <param name="array">The source array</param>
				/// <param name="newObject">Object to be added.</param>
				/// <typeparam name="T">Object Type.</typeparam>
				public static bool Add<T> (ref T[] array, T newObject)
				{
						return AddAt (ref array, array.Length, newObject, false);
				}

				/// <summary>
				/// Add a unique item to an array.
				/// </summary>
				/// <returns><c>true</c>, if something was added, <c>false</c> otherwise.</returns>
				/// <param name="array">The source array.</param>
				/// <param name="position">The position (index) where to insert the object.</param>
				/// <param name="newObject">Object to be added.</param>
				/// <param name="forceUnique">Should it check to see if there is an existing reference in the array?</param>
				/// <typeparam name="T">Object Type.</typeparam>
				public static bool AddAt<T> (ref T[] array, int position, T newObject, bool forceUnique)
				{
						// First Addition - Just Quick Add
						if (array.Length == 0) {
								array = new T[1];
								array [0] = newObject;
								return true;
						}

						// Check if we already have this target and unique is required	
						if (forceUnique) {
								if (array.Any (t => t.Equals (newObject))) {
										return false;
								}
						}

						// Create new array
						var newArray = new T[array.Length + 1];

						for (var x = 0; x <= array.Length; x++) {
								if (x < position) {
										newArray [x] = array [x];
								} else if (x == position) {
										newArray [x] = newObject;
								} else if (x > position) {
										newArray [x] = array [x - 1];
								}
						}

						// Assign array
						array = newArray;

						return true;
				}

				/// <summary>
				/// Determine if the specified array contains the targetObject.
				/// </summary>
				/// <returns><c>true</c>, if something was found, <c>false</c> otherwise.</returns>
				/// <param name="array">The source array.</param>
				/// <param name="targetObject">Object to test for.</param>
				/// <typeparam name="T">Object Type</typeparam>
				public static bool Contains<T> (ref T[] array, T targetObject)
				{
						return array.Any (t => t.Equals (targetObject));
				}

				/// <summary>
				/// Removes all references for an object from array.
				/// </summary>
				/// <returns><c>true</c>, if something was removed, <c>false</c> otherwise.</returns>
				/// <param name="array">The source array.</param>
				/// <param name="oldObject">Object to be removed.</param>
				/// <typeparam name="T">Object Type.</typeparam>
				public static bool Remove<T> (ref T[] array, T oldObject)
				{

						// Make sure we're not trying to remove nothing
						if (oldObject.Equals (null)) {
								return false;
						}

						var newArray = new T[array.Length - 1];
						var counter = 0;
						var found = false;

						foreach (var singleObject in array) {
								if (!singleObject.Equals (oldObject)) {
										newArray [counter] = singleObject;
										counter++;
								} else {
										found = true;
								}
						}

						if (found) {
								array = newArray;
						}

						return found;
				}

				/// <summary>
				/// Removes an object in an array at the provided index.
				/// </summary>
				/// <returns><c>true</c>, if something was removed, <c>false</c> otherwise.</returns>
				/// <param name="array">The source array.</param>
				/// <param name="index">The index of the item to be removed.</param>
				/// <typeparam name="T">Object Type.</typeparam>
				public static bool RemoveAt<T> (ref T[] array, int index)
				{
						// Failsafe
						if (index >= array.Length || index < 0)
								return false;

						var newArray = new T[array.Length - 1];
						var counter = 0;

						for (int x = 0; x < array.Length; x++) {
								if (x != index) {
										newArray [counter] = array [x];
										counter++;
								}
						}
						array = newArray;
						return true;
				}
		}
}