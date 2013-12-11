#region Copyright Notice & License Information
//
// XML.cs
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
		/// Additional static functions used to extend existing XML support inside of Unity.
		/// </summary>
		public class XML
		{
				/// <summary>
				/// Converts the String to UTF8 Byte array and is used in Deserialization.
				/// </summary>
				/// <returns>A byte array.</returns>
				/// <param name="pXmlString">The source string.</param>
				public static Byte[] StringToUTF8ByteArray (String pXmlString)
				{
						var encoding = new System.Text.UTF8Encoding ();
						return encoding.GetBytes (pXmlString);
				}

				/// <summary>
				/// To convert a Byte Array of Unicode values (UTF-8 encoded) to a complete String.
				/// </summary>
				/// <returns>String converted from Unicode Byte Array.</returns>
				/// <param name="characters">Unicode Byte Array to be converted to String.</param>
				public static String UTF8ByteArrayToString (Byte[] characters)
				{
						var encoding = new System.Text.UTF8Encoding ();
						return encoding.GetString (characters);
				}
		}
}