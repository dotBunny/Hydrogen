#region Copyright Notice & License Information
//
// Regex.cs
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
using System.Text;
using System.Text.RegularExpressions;

namespace Hydrogen
{
		/// <summary>
		/// Additional static functions and constants used to extend existing Regex support inside of Unity.
		/// </summary>
		public static class Regex
		{
				internal static System.Text.RegularExpressions.Regex R = 
						new System.Text.RegularExpressions.Regex (@"\%(\d*\$)?([\'\#\-\+ ]*)(\d*)(?:\.(\d+))?([hl])?([dioxXucsfeEgGpn%])");

				/// <summary>
				/// Replaces all matches based on pattern with their associated replacement in the target.
				/// </summary>
				/// <returns>The replaced target string.</returns>
				/// <param name="target">The target string to search through.</param>
				/// <param name="pattern">Regular expression patterns.</param>
				/// <param name="replacements">Replacement strings.</param>
				public static string PregReplace (this string target, string[] pattern, string[] replacements)
				{
						if (replacements.Length != pattern.Length)
								throw new ArgumentException ("Replacement and Pattern Arrays must be balanced");

						for (var i = 0; i < pattern.Length; i++) {
								target = System.Text.RegularExpressions.Regex.Replace (target, pattern [i], replacements [i]);
						}

						return target;
				}

				/// <summary>
				/// Replaces the string representations of meta chars with their corresponding character values.
				/// </summary>
				/// <param name="input">The input.</param>
				/// <returns>A string with all string meta chars are replaced.</returns>
				public static string ReplaceMetaChars (string input)
				{
						return System.Text.RegularExpressions.Regex.Replace (input, @"(\\)(\d{3}|[^\d])?", new MatchEvaluator (ReplaceMetaCharsMatch));
				}

				/// <summary>
				/// Returns a string produced according to the formatting string format.
				/// </summary>
				/// <param name="format">Format.</param>
				/// <param name="parameters">Parameters.</param>
				public static string Sprintf (string format, params object[] parameters)
				{

						var f = new StringBuilder ();
						Match m;
						string w;
						int defaultParamIx = 0;
						int paramIx;
						object o;

						bool flagLeft2Right;
						bool flagAlternate;
						bool flagPositiveSign;
						bool flagPositiveSpace;
						bool flagZeroPadding;
						bool flagGroupThousands;

						int fieldLength;
						int fieldPrecision;
						char shortLongIndicator;
						char formatSpecifier;
						char paddingCharacter;

						// find all format parameters in format string
						f.Append (format);
						m = R.Match (f.ToString ());
						while (m.Success) {

								paramIx = defaultParamIx;
								if (m.Groups [1] != null && m.Groups [1].Value.Length > 0) {
										var val = m.Groups [1].Value.Substring (0, m.Groups [1].Value.Length - 1);
										paramIx = Convert.ToInt32 (val) - 1;
								}

								// extract format flags
								flagAlternate = false;
								flagLeft2Right = false;
								flagPositiveSign = false;
								flagPositiveSpace = false;
								flagZeroPadding = false;
								flagGroupThousands = false;

								if (m.Groups [2] != null && m.Groups [2].Value.Length > 0) {
										var flags = m.Groups [2].Value;

										flagAlternate = (flags.IndexOf ('#') >= 0);
										flagLeft2Right = (flags.IndexOf ('-') >= 0);
										flagPositiveSign = (flags.IndexOf ('+') >= 0);
										flagPositiveSpace = (flags.IndexOf (' ') >= 0);
										flagGroupThousands = (flags.IndexOf ('\'') >= 0);

										// positive + indicator overrides a positive space character
										flagPositiveSpace &= !flagPositiveSign || !flagPositiveSpace;
								}

								// Extract field length and pading character
								paddingCharacter = ' ';
								fieldLength = int.MinValue;

								if (m.Groups [3] != null && m.Groups [3].Value.Length > 0) {
										fieldLength = Convert.ToInt32 (m.Groups [3].Value);
										flagZeroPadding = (m.Groups [3].Value [0] == '0');
								}

								if (flagZeroPadding)
										paddingCharacter = '0';

								// Left2Right allignment overrides zero padding
								if (flagLeft2Right && flagZeroPadding) {
										paddingCharacter = ' ';
								}

								// Extract field precision
								fieldPrecision = int.MinValue;
								if (m.Groups [4] != null && m.Groups [4].Value.Length > 0)
										fieldPrecision = Convert.ToInt32 (m.Groups [4].Value);

								// Extract short / long indicator
								shortLongIndicator = Char.MinValue;
								if (m.Groups [5] != null && m.Groups [5].Value.Length > 0)
										shortLongIndicator = m.Groups [5].Value [0];

								// Extract format
								formatSpecifier = Char.MinValue;
								if (m.Groups [6] != null && m.Groups [6].Value.Length > 0)
										formatSpecifier = m.Groups [6].Value [0];

								// Default precision is 6 digits if none is specified except
								if (fieldPrecision == int.MinValue && formatSpecifier != 's' && formatSpecifier != 'c' &&
								    Char.ToUpper (formatSpecifier) != 'X' && formatSpecifier != 'o')
										fieldPrecision = 6;

								// Get next value parameter and convert value parameter depending on short / long indicator
								if (parameters == null || paramIx >= parameters.Length)
										o = null;
								else {
										o = parameters [paramIx];

										if (shortLongIndicator == 'h') {
												if (o is int)
														o = (short)((int)o);
												else if (o is long)
														o = (short)((long)o);
												else if (o is uint)
														o = (ushort)((uint)o);
												else if (o is ulong)
														o = (ushort)((ulong)o);
										} else if (shortLongIndicator == 'l') {
												if (o is short)
														o = (long)((short)o);
												else if (o is int)
														o = (long)((int)o);
												else if (o is ushort)
														o = (ulong)((ushort)o);
												else if (o is uint)
														o = (ulong)((uint)o);
										}
								}

								// convert value parameters to a string depending on the formatSpecifier
								w = String.Empty;
								switch (formatSpecifier) {
								case '%':  // % character
										w = "%";
										break;
								case 'd':   // integer
										w = FormatNumber ((flagGroupThousands ? "n" : "d"), fieldLength, int.MinValue, 
												flagLeft2Right, flagPositiveSign, flagPositiveSpace, paddingCharacter, o);
										defaultParamIx++;
										break;
								case 'i':   // integer
										goto case 'd';
								case 'o':   // octal integer - no leading zero
										w = FormatOct (flagAlternate, fieldLength, flagLeft2Right, paddingCharacter, o);
										defaultParamIx++;
										break;
								case 'x':   // hex integer - no leading zero
										w = FormatHex ("x", flagAlternate, fieldLength, fieldPrecision, flagLeft2Right,
												paddingCharacter, o);
										defaultParamIx++;
										break;
								case 'X':   // same as x but with capital hex characters
										w = FormatHex ("X", flagAlternate, fieldLength, fieldPrecision, flagLeft2Right,
												paddingCharacter, o);
										defaultParamIx++;
										break;
								case 'u':   // unsigned integer
										w = FormatNumber ((flagGroupThousands ? "n" : "d"), fieldLength, int.MinValue,
												flagLeft2Right, false, false, paddingCharacter, Math.ToUnsigned (o));
										defaultParamIx++;
										break;
								case 'c':   // character
										if (o.IsNumericType ())
												w = Convert.ToChar (o).ToString ();
										else if (o is char)
												w = ((char)o).ToString ();
										else if (o is string && ((string)o).Length > 0)
												w = ((string)o) [0].ToString ();
										defaultParamIx++;
										break;
								case 's':   // string
										if (o == null)
												break;

										w = o.ToString ();
										if (fieldPrecision >= 0)
												w = w.Substring (0, fieldPrecision);

										if (fieldLength != int.MinValue)
										if (flagLeft2Right)
												w = w.PadRight (fieldLength, paddingCharacter);
										else
												w = w.PadLeft (fieldLength, paddingCharacter);
										defaultParamIx++;
										break;
								case 'f':   // double
										w = FormatNumber ((flagGroupThousands ? "n" : "f"), fieldLength, fieldPrecision, 
												flagLeft2Right, flagPositiveSign, flagPositiveSpace, paddingCharacter, o);
										defaultParamIx++;
										break;
								case 'e':   // double / exponent
										w = FormatNumber ("e",
												fieldLength, fieldPrecision, flagLeft2Right, flagPositiveSign, 
												flagPositiveSpace, paddingCharacter, o);
										defaultParamIx++;
										break;
								case 'E':   // double / exponent
										w = FormatNumber ("E",
												fieldLength, fieldPrecision, flagLeft2Right, flagPositiveSign, 
												flagPositiveSpace, paddingCharacter, o);
										defaultParamIx++;
										break;
								case 'g':   // double / exponent
										w = FormatNumber ("g", fieldLength, fieldPrecision, flagLeft2Right,
												flagPositiveSign, flagPositiveSpace, paddingCharacter, o);
										defaultParamIx++;
										break;
								case 'G':   // double / exponent
										w = FormatNumber ("G", fieldLength, fieldPrecision, flagLeft2Right,
												flagPositiveSign, flagPositiveSpace, paddingCharacter, o);
										defaultParamIx++;
										break;
								case 'p':   // pointer
										if (o is IntPtr)
#if UNITY_XBOX360
												w = ( (IntPtr)o ).ToString();
#else
												w = "0x" + ((IntPtr)o).ToString ("x");
#endif
										defaultParamIx++;
										break;
								case 'n':   // number of characters so far
										w = FormatNumber ("d", fieldLength, int.MinValue, flagLeft2Right,
												flagPositiveSign, flagPositiveSpace, paddingCharacter, m.Index);
										break;
								default:
										w = String.Empty;
										defaultParamIx++;
										break;
								}

								// replace format parameter with parameter value
								// and start searching for the next format parameter
								// AFTER the position of the current inserted value
								// to prohibit recursive matches if the value also
								// includes a format specifier
								f.Remove (m.Index, m.Length);
								f.Insert (m.Index, w);
								m = R.Match (f.ToString (), m.Index + w.Length);
						}

						return f.ToString ();
				}

				/// <summary>
				/// Formats a hex string.
				/// </summary>
				/// <returns>The formatted hex string</returns>
				/// <param name="nativeFormat">Native format</param>
				/// <param name="alternate">If set to <c>true</c> alternate.</param>
				/// <param name="fieldLength">Field length.</param>
				/// <param name="fieldPrecision">Field precision.</param>
				/// <param name="left2Right">If set to <c>true</c> left2 right.</param>
				/// <param name="padding">Padding.</param>
				/// <param name="value">Value.</param>
				static string FormatHex (string nativeFormat, bool alternate, int fieldLength, int fieldPrecision, 
				                         bool left2Right, char padding, object value)
				{
						string w = String.Empty;
						string lengthFormat = "{0" + (fieldLength != int.MinValue ?
								"," + (left2Right ?
										"-" :
										String.Empty) + fieldLength :
								String.Empty) + "}";
						string numberFormat = "{0:" + nativeFormat + (fieldPrecision != int.MinValue ?
								fieldPrecision.ToString () :
								String.Empty) + "}";

						if (value.IsNumericType ()) {
								w = String.Format (numberFormat, value);

								if (left2Right || padding == ' ') {
										if (alternate)
												w = (nativeFormat == "x" ? "0x" : "0X") + w;
										w = String.Format (lengthFormat, w);
								} else {
										if (fieldLength != int.MinValue)
												w = w.PadLeft (fieldLength - (alternate ? 2 : 0), padding);
										if (alternate)
												w = (nativeFormat == "x" ? "0x" : "0X") + w;
								}
						}

						return w;
				}

				/// <summary>
				/// Formats a number string.
				/// </summary>
				/// <returns>The formatted number string.</returns>
				/// <param name="nativeFormat">Native format.</param>
				/// <param name="fieldLength">Field length.</param>
				/// <param name="fieldPrecision">Field precision.</param>
				/// <param name="left2Right">If set to <c>true</c> left2 right.</param>
				/// <param name="positiveSign">If set to <c>true</c> positive sign.</param>
				/// <param name="positiveSpace">If set to <c>true</c> positive space.</param>
				/// <param name="padding">Padding.</param>
				/// <param name="value">Value.</param>
				static string FormatNumber (string nativeFormat, int fieldLength, int fieldPrecision,
				                            bool left2Right, bool positiveSign, bool positiveSpace, char padding, object value)
				{
						string w = String.Empty;
						string lengthFormat = "{0" + (fieldLength != int.MinValue ?
								"," + (left2Right ?
										"-" :
										String.Empty) + fieldLength :
								String.Empty) + "}";
						string numberFormat = "{0:" + nativeFormat + (fieldPrecision != int.MinValue ?
								fieldPrecision.ToString () :
								"0") + "}";

						if (value.IsNumericType ()) {
								w = String.Format (numberFormat, value);

								if (left2Right || padding == ' ') {
										if (value.IsPositive (true))
												w = (positiveSign ?
														"+" : (positiveSpace ? " " : String.Empty)) + w;
										w = String.Format (lengthFormat, w);
								} else {
										if (w.StartsWith ("-", StringComparison.Ordinal))
												w = w.Substring (1);
										if (fieldLength != int.MinValue)
												w = w.PadLeft (fieldLength - 1, padding);
										if (value.IsPositive (true))
												w = (positiveSign ?
														"+" : (positiveSpace ?
																" " : (fieldLength != int.MinValue ?
																		padding.ToString () : String.Empty))) + w;
										else
												w = "-" + w;
								}
						}

						return w;
				}

				/// <summary>
				/// Formats an octet string.
				/// </summary>
				/// <returns>The formatte octet string.</returns>
				/// <param name="alternate">If set to <c>true</c> alternate.</param>
				/// <param name="fieldLength">Field length.</param>
				/// <param name="left2Right">If set to <c>true</c> left2 right.</param>
				/// <param name="padding">Padding.</param>
				/// <param name="value">Value.</param>
				static string FormatOct (bool alternate, int fieldLength, bool left2Right, char padding, object value)
				{
						string w = String.Empty;
						string lengthFormat = "{0" + (fieldLength != int.MinValue ? "," + (left2Right ? "-" : 
								String.Empty) + fieldLength : String.Empty) + "}";

						if (value.IsNumericType ()) {
								w = Convert.ToString (Math.UnboxToLong (value, true), 8);

								if (left2Right || padding == ' ') {
										if (alternate && w != "0")
												w = "0" + w;
										w = String.Format (lengthFormat, w);
								} else {
										if (fieldLength != int.MinValue)
												w = w.PadLeft (fieldLength - (alternate && w != "0" ? 1 : 0), padding);
										if (alternate && w != "0")
												w = "0" + w;
								}
						}

						return w;
				}

				/// <summary>
				/// Replaces all hex characters with thier meta character equivalent.
				/// </summary>
				/// <returns>The updated string.</returns>
				/// <param name="m">Target Match</param>
				static string ReplaceMetaCharsMatch (Match m)
				{
						// convert octal quotes (like \040)
						if (m.Groups [2].Length == 3)
								return Convert.ToChar (Convert.ToByte (m.Groups [2].Value, 8)).ToString ();

						// convert all other special meta characters
						//TODO: Add \xhhh hex and possible dec !!
						switch (m.Groups [2].Value) {
						case "0":           // null
								return "\0";
						case "a":           // alert (beep)
								return "\a";
						case "b":           // BS
								return "\b";
						case "f":           // FF
								return "\f";
						case "v":           // vertical tab
								return "\v";
						case "r":           // CR
								return "\r";
						case "n":           // LF
								return "\n";
						case "t":           // Tab
								return "\t";
						default:
								// if neither an octal quote nor a special meta character
								// so just remove the backslash
								return m.Groups [2].Value;
						}
				}
		}
}