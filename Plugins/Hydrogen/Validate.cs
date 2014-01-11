#region Copyright Notice & License Information
//
// Validate.cs
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
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Hydrogen
{
		/// <summary>
		/// Additional static functions used to extend existing Validation support inside of Unity.
		/// </summary>
		public static class Validate
		{
				/// <summary>
				/// Calculate the percentage of similarity between two colors.
				/// </summary>
				/// <returns>The percentage of similarity (0-1)</returns>
				/// <param name="sourceColor">Left Side Color</param>
				/// <param name="targetColor">Right Side Color</param>
				public static float ColorPercentage (this Color sourceColor, Color targetColor)
				{
						return ((targetColor.r / sourceColor.r) +
						(targetColor.g / sourceColor.g) +
						(targetColor.b / sourceColor.b) +
						(targetColor.a / sourceColor.a) / 4f);
				}

				/// <summary>
				/// Are the colors approximately equal?
				/// </summary>
				/// <returns><c>true</c>, if colors are approximately equal, <c>false</c> otherwise.</returns>
				/// <param name="sourceColor">Left Side Color</param>
				/// <param name="targetColor">Right Side Color</param>
				public static bool IsColorApproximatelySame (this Color sourceColor, Color targetColor)
				{
						return (Mathf.Approximately (sourceColor.r, targetColor.r) &&
						Mathf.Approximately (sourceColor.g, targetColor.g) &&
						Mathf.Approximately (sourceColor.b, targetColor.b) &&
						Mathf.Approximately (sourceColor.a, targetColor.a));
				}

				/// <summary>
				/// Determines if dictionaries are equal.
				/// </summary>
				/// <returns><c>true</c> if the left side equals the right side; otherwise, <c>false</c>.</returns>
				/// <param name="first">Left Side Dictionary.</param>
				/// <param name="second">Right Side Dictionary.</param>
				/// <typeparam name="TKey">The 1st type parameter.</typeparam>
				/// <typeparam name="TValue">The 2nd type parameter.</typeparam>
				public static bool IsDictionaryEqual<TKey, TValue> (
						this IDictionary<TKey, TValue> first, IDictionary<TKey, TValue> second)
				{
						if (first == second)
								return true;
						if ((first == null) || (second == null))
								return false;
						if (first.Count != second.Count)
								return false;

						var comparer = EqualityComparer<TValue>.Default;

						foreach (KeyValuePair<TKey, TValue> kvp in first) {
								TValue secondValue;
								if (!second.TryGetValue (kvp.Key, out secondValue))
										return false;
								if (!comparer.Equals (kvp.Value, secondValue))
										return false;
						}
						return true;
				}

				/// <summary>
				/// Determines whether the specified value is of numeric type.
				/// </summary>

				/// <returns>
				/// 	<c>true</c> if value is a numeric type; otherwise, <c>false</c>.
				/// </returns>
				/// <param name="value">The object to check.</param>
				public static bool IsNumericType (this object value)
				{
						return (value is byte ||
						value is sbyte ||
						value is short ||
						value is ushort ||
						value is int ||
						value is uint ||
						value is long ||
						value is ulong ||
						value is float ||
						value is double ||
						value is decimal);
				}

				/// <summary>
				/// Determines whether the specified value is positive.
				/// </summary>
				/// <returns>
				/// 	<c>true</c> if the specified value is positive; otherwise, <c>false</c>.
				/// </returns>
				/// <param name="value">The value.</param>
				/// <param name="zeroIsPositive">if set to <c>true</c> treats 0 as positive. Defaults to true.</param>
				public static bool IsPositive (this object value, bool zeroIsPositive = true)
				{
						switch (Type.GetTypeCode (value.GetType ())) {
						case TypeCode.SByte:
								return (zeroIsPositive ? (sbyte)value >= 0 : (sbyte)value > 0);
						case TypeCode.Int16:
								return (zeroIsPositive ? (short)value >= 0 : (short)value > 0);
						case TypeCode.Int32:
								return (zeroIsPositive ? (int)value >= 0 : (int)value > 0);
						case TypeCode.Int64:
								return (zeroIsPositive ? (long)value >= 0 : (long)value > 0);
						case TypeCode.Single:
								return (zeroIsPositive ? (float)value >= 0 : (float)value > 0);
						case TypeCode.Double:
								return (zeroIsPositive ? (double)value >= 0 : (double)value > 0);
						case TypeCode.Decimal:
								return (zeroIsPositive ? (decimal)value >= 0 : (decimal)value > 0);
						case TypeCode.Byte:
								return (zeroIsPositive || (byte)value > 0);
						case TypeCode.UInt16:
								return (zeroIsPositive || (ushort)value > 0);
						case TypeCode.UInt32:
								return (zeroIsPositive || (uint)value > 0);
						case TypeCode.UInt64:
								return (zeroIsPositive || (ulong)value > 0);
						case TypeCode.Char:
								return (zeroIsPositive || (char)value != '\0');
						default:
								return false;
						}
				}

				/// <summary>
				/// Determines whether the two scrambled enumerable objects are equal.
				/// </summary>
				/// <returns><c>true</c>, if they are equal, <c>false</c> otherwise.</returns>
				/// <param name="list1">List1.</param>
				/// <param name="list2">List2.</param>
				/// <typeparam name="T">The 1st type parameter.</typeparam>
				public static bool ScrambledEquals<T> (IEnumerable<T> list1, IEnumerable<T> list2)
				{
						var cnt = new Dictionary<T, int> ();

						foreach (T s in list1) {
								if (cnt.ContainsKey (s)) {
										cnt [s]++;
								} else {
										cnt.Add (s, 1);
								}
						}

						foreach (T s in list2) {
								if (cnt.ContainsKey (s)) {
										cnt [s]--;
								} else {
										return false;
								}
						}

						return cnt.Values.All (c => c == 0);
				}
		}
}