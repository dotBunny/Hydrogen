#region Copyright Notice & License Information
// 
// Convert.cs
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
    /// Additional static functions, constants and classes used to extend existing Convert support inside of Unity.
    /// </summary>
    public static class Convert
    {

        /// <summary>
        /// Converts the specified values boxed type to its corresponding unsigned
        /// type.
        /// </summary>
        /// <remarks>
        /// This allows for converting without knowing the original type.
        /// </remarks>
        /// <param name="value">The value.</param>
        /// <returns>A boxed numeric object whos type is unsigned.</returns>
        public static object ToUnsigned(object value)
        {
            switch (Type.GetTypeCode(value.GetType()))
            {
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

        ///<summary>
        /// Convert Object to Long value
        ///</summary>
        ///<param name="value">Object</param>
        ///<param name="round">Should decimals be rounded?</param>
        ///<returns>A long value</returns>
        public static long UnboxToLong(object value, bool round)
        {
            switch (Type.GetTypeCode(value.GetType()))
            {
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
                    return (round ? (long)System.Math.Round((float)value) : (long)((float)value));
                case TypeCode.Double:
                    return (round ? (long)System.Math.Round((double)value) : (long)((double)value));
                case TypeCode.Decimal:
                    return (round ? (long)System.Math.Round((decimal)value) : (long)((decimal)value));

                default:
                    return 0;
            }
        }
    }
}