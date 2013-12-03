#region Copyright Notice & License Information
//
// JSON.cs
//
// Author:
//       Patrick van Bergen
//       Matthew Davey <matthew.davey@dotbunny.com>
//       Calvin Rien
//       WyrmTale Games <info@wyrmtale.com>
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
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Hydrogen.Serialization
{
		/// <summary>
		/// This class encodes and decodes JSON strings.
		/// Spec. details, see http://www.json.org/
		///
		/// JSON uses Arrays and Objects. These correspond here to the datatypes IList and IDictionary.
		/// All numbers are parsed to doubles.
		/// </summary>
		sealed class JSON
		{
				/// <summary>
				/// Parses the string json into a value
				/// </summary>
				/// <param name="json">A JSON string.</param>
				/// <returns>An List&lt;object&gt;, a Dictionary&lt;string, object&gt;, a double, an integer,a string, null, true, or false</returns>
				/// <example>
				///  var dict = Json.Deserialize(jsonString) as Dictionary<string,object>;
				/// 
				///   Debug.Log("deserialized: " + dict.GetType());
				///   Debug.Log("dict['array'][0]: " + ((List<object>) dict["array"])[0]);
				///	  Debug.Log("dict['string']: " + (string) dict["string"]);
				///   Debug.Log("dict['float']: " + (double) dict["float"]); // floats come out as doubles
				///   Debug.Log("dict['int']: " + (long) dict["int"]); // ints come out as longs
				///   Debug.Log("dict['unicode']: " + (string) dict["unicode"]);
				/// </example>
				public static JSONObject Deserialize (string json)
				{
						// save the string for debug information
						return json == null ? null : Parser.Parse (json);
			
				}

				sealed class Parser : IDisposable
				{
						const string WHITESPACE = " \t\n\r";
						const string WORDBREAK = " \t\n\r{}[],:\"";

						enum TOKEN
						{
								NONE,
								CURLYOPEN,
								CURLYCLOSE,
								SQUAREDOPEN,
								SQUAREDCLOSE,
								COLON,
								COMMA,
								STRING,
								NUMBER,
								TRUE,
								FALSE,
								NULL}
						;

						StringReader json;

						Parser (string jsonString)
						{
								json = new StringReader (jsonString);
						}

						public static JSONObject Parse (string jsonString)
						{
								using (var instance = new Parser (jsonString)) {
										return (instance.ParseValue () as JSONObject);
								}
						}

						public void Dispose ()
						{
								json.Dispose ();
								json = null;
						}

						JSONObject ParseObject ()
						{
								var table = new Dictionary<string, object> ();
								var obj = new JSONObject ();
								obj.Fields = table;
					
								// ditch opening brace
								json.Read ();
					
								// {
								while (true) {
										switch (NextToken) {
										case TOKEN.NONE:
												return null;
										case TOKEN.COMMA:
												continue;
										case TOKEN.CURLYCLOSE:
												return obj;
										default:
							// name
												string name = ParseString ();
												if (name == null) {
														return null;
												}
							
							// :
												if (NextToken != TOKEN.COLON) {
														return null;
												}
							// ditch the colon
												json.Read ();
							
							// value
												table [name] = ParseValue ();
												break;
										}
								}												
						}

						List<object> ParseArray ()
						{
								var array = new List<object> ();
					
								// ditch opening bracket
								json.Read ();
					
								// [
								var parsing = true;
								while (parsing) {
										TOKEN nextToken = NextToken;
						
										switch (nextToken) {
										case TOKEN.NONE:
												return null;
										case TOKEN.COMMA:
												continue;
										case TOKEN.SQUAREDCLOSE:
												parsing = false;
												break;
										default:
												object value = ParseByToken (nextToken);
							
												array.Add (value);
												break;
										}
								}
					
								return array;
						}

						object ParseValue ()
						{
								TOKEN nextToken = NextToken;
								return ParseByToken (nextToken);
						}

						object ParseByToken (TOKEN token)
						{
								switch (token) {
								case TOKEN.STRING:
										return ParseString ();
								case TOKEN.NUMBER:
										return ParseNumber ();
								case TOKEN.CURLYOPEN:
										return ParseObject ();
								case TOKEN.SQUAREDOPEN:
										return ParseArray ();
								case TOKEN.TRUE:
										return true;
								case TOKEN.FALSE:
										return false;
								case TOKEN.NULL:
										return null;
								default:
										return null;
								}
						}

						string ParseString ()
						{
								var s = new StringBuilder ();
								char c;

								// ditch opening quote
								json.Read ();

								bool parsing = true;
								while (parsing) {

										if (json.Peek () == -1) {
												break;
										}

										c = NextChar;
										switch (c) {
										case '"':
												parsing = false;
												break;
										case '\\':
												if (json.Peek () == -1) {
														parsing = false;
														break;
												}

												c = NextChar;
												switch (c) {
												case '"':
												case '\\':
												case '/':
														s.Append (c);
														break;
												case 'b':
														s.Append ('\b');
														break;
												case 'f':
														s.Append ('\f');
														break;
												case 'n':
														s.Append ('\n');
														break;
												case 'r':
														s.Append ('\r');
														break;
												case 't':
														s.Append ('\t');
														break;
												case 'u':
														var hex = new StringBuilder ();
								
														for (int i = 0; i < 4; i++) {
																hex.Append (NextChar);
														}
								
														s.Append ((char)Convert.ToInt32 (hex.ToString (), 16));
														break;
												}
												break;
										default:
												s.Append (c);
												break;
										}
								}
					
								return s.ToString ();
						}

						object ParseNumber ()
						{
								string number = NextWord;
					
								if (number.IndexOf ('.') == -1) {
										long parsedInt;
										Int64.TryParse (number, out parsedInt);
										return parsedInt;
								}
					
								double parsedDouble;
								Double.TryParse (number, out parsedDouble);
								return parsedDouble;
						}

						void EatWhitespace ()
						{
								while (WHITESPACE.IndexOf (PeekChar) != -1) {
										json.Read ();
						
										if (json.Peek () == -1) {
												break;
										}
								}
						}

						char PeekChar {
								get {
										return Convert.ToChar (json.Peek ());
								}
						}

						char NextChar {
								get {
										return Convert.ToChar (json.Read ());
								}
						}

						string NextWord {
								get {
										var word = new StringBuilder ();
						
										while (WORDBREAK.IndexOf (PeekChar) == -1) {
												word.Append (NextChar);
							
												if (json.Peek () == -1) {
														break;
												}
										}
						
										return word.ToString ();
								}
						}

						TOKEN NextToken {
								get {
										EatWhitespace ();
						
										if (json.Peek () == -1) {
												return TOKEN.NONE;
										}
						
										char c = PeekChar;
										switch (c) {
										case '{':
												return TOKEN.CURLYOPEN;
										case '}':
												json.Read ();
												return TOKEN.CURLYCLOSE;
										case '[':
												return TOKEN.SQUAREDOPEN;
										case ']':
												json.Read ();
												return TOKEN.SQUAREDCLOSE;
										case ',':
												json.Read ();
												return TOKEN.COMMA;
										case '"':
												return TOKEN.STRING;
										case ':':
												return TOKEN.COLON;
										case '0':
										case '1':
										case '2':
										case '3':
										case '4':
										case '5':
										case '6':
										case '7':
										case '8':
										case '9':
										case '-':
												return TOKEN.NUMBER;
										}
						
										string word = NextWord;
						
										switch (word) {
										case "false":
												return TOKEN.FALSE;
										case "true":
												return TOKEN.TRUE;
										case "null":
												return TOKEN.NULL;
										}
						
										return TOKEN.NONE;
								}
						}
				}

				/// <summary>
				/// Converts a IDictionary / IList object or a simple type (string, int, etc.) into a JSON string
				/// </summary>
				/// <param name="jsonObject">A Dictionary&lt;string, object&gt; / List&lt;object&gt;</param>
				/// <returns>A JSON encoded string, or null if object 'json' is not serializable</returns>
				/// <example>
				/// 	string jsonString = Hydrogen.Serialization.JSON.Serialize(object);
				/// </example>
				public static string Serialize (JSONObject jsonObject)
				{
						return Serializer.Serialize (jsonObject);
				}

				sealed class Serializer
				{
						StringBuilder builder;

						Serializer ()
						{
								builder = new StringBuilder ();
						}

						public static string Serialize (JSONObject obj)
						{
								var instance = new Serializer ();
					
								instance.SerializeValue (obj);
					
								return instance.builder.ToString ();
						}

						void SerializeValue (object value)
						{
								if (value == null) {
										builder.Append ("null");
								} else if (value as string != null) {
										SerializeString (value as string);
								} else if (value is bool) {
										builder.Append (value.ToString ().ToLower ());
								} else if (value as JSONObject != null) {
										SerializeObject (value as JSONObject);
								} else if (value as IDictionary != null) {
										SerializeDictionary (value as IDictionary);
								} else if (value as IList != null) {
										SerializeArray (value as IList);
								} else if (value is char) {
										SerializeString (value.ToString ());
								} else {
										SerializeOther (value);
								}
						}

						void SerializeObject (JSONObject obj)
						{
								SerializeDictionary (obj.Fields);
						}

						void SerializeDictionary (IDictionary obj)
						{
								bool first = true;
					
								builder.Append ('{');
					
								foreach (object e in obj.Keys) {
										if (!first) {
												builder.Append (',');
										}
						
										SerializeString (e.ToString ());
										builder.Append (':');
						
										SerializeValue (obj [e]);
						
										first = false;
								}
					
								builder.Append ('}');
						}

						void SerializeArray (IList anArray)
						{
								builder.Append ('[');
					
								bool first = true;
					
								foreach (object obj in anArray) {
										if (!first) {
												builder.Append (',');
										}
						
										SerializeValue (obj);
						
										first = false;
								}
					
								builder.Append (']');
						}

						void SerializeString (string str)
						{
								builder.Append ('\"');
					
								char[] charArray = str.ToCharArray ();
								foreach (var c in charArray) {
										switch (c) {
										case '"':
												builder.Append ("\\\"");
												break;
										case '\\':
												builder.Append ("\\\\");
												break;
										case '\b':
												builder.Append ("\\b");
												break;
										case '\f':
												builder.Append ("\\f");
												break;
										case '\n':
												builder.Append ("\\n");
												break;
										case '\r':
												builder.Append ("\\r");
												break;
										case '\t':
												builder.Append ("\\t");
												break;
										default:
												int codepoint = Convert.ToInt32 (c);
												if ((codepoint >= 32) && (codepoint <= 126)) {
														builder.Append (c);
												} else {
														builder.Append ("\\u" + Convert.ToString (codepoint, 16).PadLeft (4, '0'));
												}
												break;
										}
								}
					
								builder.Append ('\"');
						}

						void SerializeOther (object value)
						{
								if (value is float
								    || value is int
								    || value is uint
								    || value is long
								    || value is double
								    || value is sbyte
								    || value is byte
								    || value is short
								    || value is ushort
								    || value is ulong
								    || value is decimal) {
										builder.Append (value.ToString ());
								} else {
										SerializeString (value.ToString ());
								}
						}
				}
		}
}


	