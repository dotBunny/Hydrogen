#region Copyright Notice & License Information
//
// Math.cs
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

namespace Hydrogen
{
		/// <summary>
		/// Additional static functions used to extend existing Math support inside of Unity.
		/// </summary>
		public static class Math
		{
				public const float Half = 0.5f;
				public const float Quarter = 0.25f;

				/// <summary>
				/// Clamp and neutralize an angle.
				/// </summary>
				/// <returns>The clamped and neutralized angle.</returns>
				/// <param name="angle">The source angle.</param>
				/// <param name="minimumAngle">Minimum angle.</param>
				/// <param name="maximumAngle">Maximum angle.</param>
				public static float ClampAngle (float angle, float minimumAngle, float maximumAngle)
				{
						// Clamp that angle up
						return UnityEngine.Mathf.Clamp (NeutralizeAngle (angle), minimumAngle, maximumAngle);
				}

				/// <summary>
				/// Flip a coin.
				/// </summary>
				/// <returns><c>true</c>, if toss was heads, <c>false</c> if it was tails.</returns>
				public static bool CoinToss ()
				{
						return UnityEngine.Random.Range (0, 100) <= 50;
				}

				/// <summary>
				/// Neutralizes the an angle, providing an angle below 360 degrees.
				/// </summary>
				/// <returns>The neutralized angle.</returns>
				/// <param name="angle">The source angle.</param>
				public static float NeutralizeAngle (float angle)
				{
						// Neutralize really fucked up angles
						if (angle < 0)
								return (angle % 360f) * -1;
						else
								return angle % 360f;
				}

				/// <summary>
				/// The signed equivalent of the angle.
				/// </summary>
				/// <returns>The signed angle.</returns>
				/// <param name="angle">The source angle.</param>
				public static float SignedAngle (float angle)
				{
						angle = angle % 360f;

						if (angle > 180f) {
								return (180 - ((angle % 180f))) * -1f;
						} else if (angle < -180) {
								return ((angle % 180) * -1f);
						}
						return angle;
				}

				/// <summary>
				/// Converts the specified values boxed type to its corresponding unsigned type.
				/// </summary>
				/// <returns>A boxed numeric object whos type is unsigned.</returns>
				/// <remarks>This allows for converting without knowing the original type.</remarks>
				/// <param name="value">The value.</param>
				public static object ToUnsigned (object value)
				{
						switch (Type.GetTypeCode (value.GetType ())) {
						case TypeCode.SByte:
								return (byte)((sbyte)value);
						case TypeCode.Int16:
								return (ushort)((short)value);
						case TypeCode.Int32:
								return (uint)((int)value);
						case TypeCode.Int64:
								return (ulong)((long)value);

						case TypeCode.Byte:
								return value;
						case TypeCode.UInt16:
								return value;
						case TypeCode.UInt32:
								return value;
						case TypeCode.UInt64:
								return value;

						case TypeCode.Single:
								return (UInt32)((float)value);
						case TypeCode.Double:
								return (ulong)((double)value);
						case TypeCode.Decimal:
								return (ulong)((decimal)value);

						default:
								return null;
						}
				}

				/// <summary>
				/// Convert object to long value.
				/// </summary>
				/// <returns>A long value.</returns>
				/// <param name="value">The object.</param>
				/// <param name="round">Should decimals be rounded?</param>
				public static long UnboxToLong (object value, bool round)
				{
						switch (Type.GetTypeCode (value.GetType ())) {
						case TypeCode.SByte:
								return (long)((sbyte)value);
						case TypeCode.Int16:
								return (long)((short)value);
						case TypeCode.Int32:
								return (long)((int)value);
						case TypeCode.Int64:
								return (long)value;

						case TypeCode.Byte:
								return (long)((byte)value);
						case TypeCode.UInt16:
								return (long)((ushort)value);
						case TypeCode.UInt32:
								return (long)((uint)value);
						case TypeCode.UInt64:
								return (long)((ulong)value);

						case TypeCode.Single:
								return (round ? (long)System.Math.Round ((float)value) : (long)((float)value));
						case TypeCode.Double:
								return (round ? (long)System.Math.Round ((double)value) : (long)((double)value));
						case TypeCode.Decimal:
								return (round ? (long)System.Math.Round ((decimal)value) : (long)((decimal)value));

						default:
								return 0;
						}
				}

				/// <summary>
				/// The unsigned equivalent of the angle.
				/// </summary>
				/// <returns>The unsigned angle.</returns>
				/// <param name="angle">The source angle.</param>
				public static float UnsignedAngle (float angle)
				{
						if (angle < 0f) {
								if (angle < -360f) {	
										return ((360 + (angle % 360f)) + ((angle - (angle % 360f))) * -1);
								} else {
										return 360f + angle;
								}
						}
						return angle;
				}
		}
}
