#region Copyright Notice & License Information
// 
// Validate.cs
//  
// Author:
//   Matthew Davey <matthew.davey@dotbunny.com>
//
// Copyright (C) 2013 dotBunny Inc. (http://www.dotbunny.com)
//
// Permission is hereby granted, free of charge, to any person obtaining a copy of
// this software and associated documentation files (the "Software"), to deal in
// the Software without restriction, including without limitation the rights to
// use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of
// the Software, and to permit persons to whom the Software is furnished to do so,
// subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS
// FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR
// COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER
// IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN
// CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
#endregion

using System;

namespace Hydrogen
{

    /// <summary>
    /// Additional static functions, constants and classes used to extend existing Validation support inside of Unity.
    /// </summary>
	//TODO: Need proper documentation
    public static class Validate
    {
		
		/// <summary>
		/// Checks if a GameObject is in a LayerMask
		/// </summary>
		/// <param name="obj">GameObject to test</param>
		/// <param name="layerMask">LayerMask with all the layers to test against</param>
		/// <returns>True if in any of the layers in the LayerMask</returns>
		public static bool IsInLayerMask(UnityEngine.GameObject obj, UnityEngine.LayerMask layerMask)
		{
		    // Convert the object's layer to a bitfield for comparison
		    int objLayerMask = (1 << obj.layer);
		    if ((layerMask.value & objLayerMask) > 0)  // Extra round brackets required!
			{
		        return true;
			}
		    else
			{
		        return false;
			}
		}
				
		
        /// <summary>
        /// Determines whether the specified value is of numeric type.
        /// </summary>
        /// <param name="value">The object to check.</param>
        /// <returns>
        /// 	<c>true</c> if value is a numeric type; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsNumericType(object value)
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
        /// <param name="value">The value.</param>
        /// <param name="zeroIsPositive">if set to <c>true</c> treats 0 as positive.</param>
        /// <returns>
        /// 	<c>true</c> if the specified value is positive; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsPositive(object value, bool zeroIsPositive)
        {
            switch (Type.GetTypeCode(value.GetType()))
            {
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
                    return (zeroIsPositive ? true : (byte)value > 0);
                case TypeCode.UInt16:
                    return (zeroIsPositive ? true : (ushort)value > 0);
                case TypeCode.UInt32:
                    return (zeroIsPositive ? true : (uint)value > 0);
                case TypeCode.UInt64:
                    return (zeroIsPositive ? true : (ulong)value > 0);
                case TypeCode.Char:
                    return (zeroIsPositive ? true : (char)value != '\0');
                default:
                    return false;
            }
        }
    }
}