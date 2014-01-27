﻿#region Copyright Notice & License Information
//
// hConsole.cs
//
// Author:
//       Matthew Davey <matthew.davey@dotbunny.com>
//       Robin Southern <betajaen@ihoed.com>
//
// Copyright (c) 2014 dotBunny Inc. (http://www.dotbunny.com)
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

using System.Collections.Generic;
using UnityEngine;

//TODO CODE REVIEW AND CLEAN UP
/// <summary>
/// A developer console useful for in-game debugging.
/// </summary>
/// <remarks>This component does not follow our normal component/base model.</remarks>
public class hConsole : MonoBehaviour
{
		public bool CreateCamera = true;
		public DisplayMode Mode = DisplayMode.Off;
		public DisplayLocation Location = DisplayLocation.Top;
		public KeyCode ToggleKey = KeyCode.BackQuote;
		const int DebugLogLines = 24;
		const int DebugLogLinesMax = 10000;
		const int StatsPadding = 10;
		const int StatsLineSpacing = 7;
		const int FontBegin = 33;
		const int FontEnd = 127;
		const int GlyphHeight = 8;
		const int GlyphWidth = 8;
		const int GlyphCount = 94;
		const string ShaderText = "Shader \"hDebug/Text\"\r\n{\r\nProperties\r\n{\r\n_MainTex (\"Main\", 2D) = \"white\" {}\r\n}\r\n\r\nCategory\r\n{\r\n                        Tags\r\n                        {\r\n                            \"Queue\" = \"Transparent\"\r\n                        }\r\n                        \r\n                        Blend SrcAlpha OneMinusSrcAlpha\r\n                        AlphaTest Greater .01\r\n                        ColorMask RGB\r\n                        Cull Off\r\n                        Lighting Off\r\n                        ZWrite On\r\n                        \r\n                        Fog\r\n                        {\r\n                            Color(0, 0, 0, 0)\r\n                        }\r\n                        \r\n                        BindChannels\r\n                        {\r\n                            Bind \"Color\", color\r\n                            Bind \"Vertex\", vertex\r\n                            Bind \"TexCoord\", texcoord\r\n                        }\r\n                        \r\n                        SubShader\r\n                        {\r\n                            Pass\r\n                            {\r\n                                SetTexture [_MainTex]\r\n                                {\r\n                                    combine texture * primary\r\n                                }\r\n                            }\r\n                        }\r\n                    }\r\n                }";
		public static Color Shadow = Color.cyan;
		static readonly char[] _newLineCharacters = new char[] { '\n' };
		/// <summary>
		/// The font to be rendered to the screen, stored as a byte array.
		/// </summary>
		/// <remarks>It's a black and white image stored as a 1-bit PNG. Genius Robin! Genius!</remarks>
		static readonly byte[] _fontImageData = {
				0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A, 0x00, 0x00, 0x00, 0x0D, 0x49, 0x48, 0x44, 0x52,
				0x00, 0x00, 0x00, 0x80, 0x00, 0x00, 0x00, 0x80, 0x01, 0x03, 0x00, 0x00, 0x00, 0xF9, 0xF0, 0xF3,
				0x88, 0x00, 0x00, 0x00, 0x06, 0x50, 0x4C, 0x54, 0x45, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0x55,
				0x7C, 0xF5, 0x6C, 0x00, 0x00, 0x00, 0x01, 0x74, 0x52, 0x4E, 0x53, 0x00, 0x40, 0xE6, 0xD8, 0x66,
				0x00, 0x00, 0x02, 0x15, 0x49, 0x44, 0x41, 0x54, 0x78, 0x5E, 0xED, 0xD2, 0x41, 0x8A, 0xD5, 0x40,
				0x10, 0x06, 0xE0, 0x5A, 0xF5, 0xAA, 0x70, 0x5D, 0x60, 0x78, 0x27, 0x70, 0x51, 0x30, 0x12, 0x1F,
				0x4C, 0x91, 0x8B, 0xB8, 0x29, 0x18, 0xF8, 0x5D, 0xBC, 0xF0, 0x66, 0x44, 0x18, 0x03, 0x36, 0xFD,
				0x6E, 0xE0, 0x99, 0x04, 0x0F, 0xE0, 0x56, 0x9C, 0x4B, 0x78, 0x03, 0xAD, 0xEE, 0x24, 0x4E, 0x66,
				0x46, 0x17, 0xCA, 0x2C, 0x5C, 0xCC, 0xBF, 0xC8, 0xE2, 0xE3, 0x4F, 0x77, 0x85, 0x0A, 0xFD, 0x2E,
				0xF2, 0x00, 0xAE, 0xB6, 0x20, 0xE8, 0xED, 0xEB, 0x5E, 0x58, 0x1B, 0xA4, 0x1D, 0x19, 0x7A, 0x7C,
				0x1B, 0x45, 0x64, 0x86, 0xB3, 0x80, 0x93, 0x5F, 0x57, 0xE8, 0x2B, 0xF1, 0x2B, 0x32, 0xEA, 0x95,
				0xF7, 0xA4, 0xDC, 0x2D, 0x20, 0xD4, 0x8B, 0x78, 0xC0, 0xA9, 0x50, 0x21, 0x39, 0xAB, 0xC0, 0x7A,
				0xB9, 0x36, 0xE4, 0x3C, 0x1E, 0xA7, 0x74, 0x44, 0x40, 0x3B, 0x43, 0xCF, 0x6B, 0x03, 0xAF, 0x2B,
				0x10, 0x75, 0xD4, 0x69, 0x7B, 0xC5, 0x9E, 0x87, 0x4B, 0x03, 0xDF, 0x51, 0x44, 0x9E, 0x51, 0x83,
				0x28, 0xF9, 0xAF, 0x6F, 0x61, 0x0D, 0xA4, 0xC7, 0x0A, 0x9B, 0x69, 0xE9, 0x8A, 0x19, 0x51, 0x22,
				0xB7, 0x42, 0x1D, 0xA0, 0x2E, 0x09, 0xA8, 0xD7, 0x2A, 0x3E, 0x53, 0x06, 0x7A, 0x57, 0x06, 0xBA,
				0x4E, 0x48, 0x02, 0x38, 0xA5, 0xDE, 0x33, 0x5F, 0x07, 0x68, 0x61, 0xFE, 0x42, 0xCC, 0x5D, 0x9F,
				0x21, 0x86, 0x3A, 0x66, 0x92, 0x1B, 0x62, 0x49, 0x48, 0x90, 0xE3, 0x40, 0x14, 0x8D, 0x0A, 0x8A,
				0x53, 0x82, 0x82, 0xEB, 0xE8, 0x42, 0xF1, 0x8A, 0x23, 0x71, 0x80, 0x74, 0x1D, 0x93, 0xCA, 0x27,
				0xE2, 0x62, 0x69, 0x32, 0xB5, 0x7D, 0xD7, 0x25, 0x72, 0x39, 0x6D, 0x47, 0x7C, 0xB8, 0xD1, 0x47,
				0x8B, 0x64, 0x9B, 0x4A, 0x31, 0x58, 0x82, 0x5F, 0x5C, 0x58, 0x26, 0x03, 0x46, 0x77, 0x40, 0x1A,
				0x00, 0x84, 0xC8, 0x0C, 0xA3, 0xBF, 0x7F, 0xD7, 0xC0, 0x03, 0xBC, 0xC1, 0xE1, 0x43, 0x85, 0xEC,
				0xC8, 0xD9, 0x8B, 0xA4, 0xC9, 0x0F, 0x97, 0xC8, 0x54, 0x5A, 0xE3, 0x38, 0x37, 0xDE, 0xC0, 0x6F,
				0xCF, 0xC0, 0x18, 0x87, 0xCE, 0x30, 0xCE, 0x80, 0x05, 0xEA, 0x1C, 0x3E, 0xC0, 0x0C, 0x25, 0xE6,
				0x70, 0x7A, 0xF4, 0x08, 0xED, 0xEF, 0x82, 0x05, 0x59, 0xB6, 0x02, 0x5C, 0x00, 0xC5, 0xDC, 0x40,
				0x4C, 0x00, 0xA4, 0x41, 0x52, 0x67, 0x8A, 0x00, 0x5E, 0xE1, 0x65, 0x80, 0x2E, 0xA0, 0x01, 0x07,
				0x01, 0xAF, 0x90, 0xA5, 0x81, 0x89, 0xCA, 0x0C, 0x23, 0x57, 0x78, 0x21, 0xBA, 0x02, 0x52, 0x40,
				0x0F, 0x71, 0xE5, 0x05, 0x20, 0xB0, 0x0D, 0x18, 0x4C, 0x4C, 0x02, 0x8A, 0x26, 0xBE, 0xBB, 0xF3,
				0x05, 0xD2, 0x2D, 0x10, 0xDF, 0xFF, 0x10, 0xA3, 0x1F, 0xF4, 0x17, 0x11, 0x5E, 0xC1, 0x29, 0xD1,
				0x8E, 0x5C, 0xD8, 0xA7, 0x5B, 0xD0, 0xE0, 0xB0, 0x06, 0x96, 0x6D, 0x30, 0x1D, 0xF2, 0x64, 0x90,
				0x92, 0x2D, 0x53, 0x42, 0x44, 0x01, 0x61, 0xC8, 0x01, 0x40, 0x80, 0x03, 0xA5, 0xC2, 0x38, 0xC3,
				0x10, 0x50, 0x5A, 0x63, 0x9A, 0x01, 0x01, 0xAE, 0x9B, 0x06, 0x22, 0xBE, 0x3D, 0x63, 0x98, 0x6F,
				0x41, 0xE1, 0xD8, 0x3E, 0x2C, 0xD3, 0x9C, 0xB4, 0xFE, 0xFB, 0xFE, 0x27, 0x88, 0xE6, 0xB4, 0xC0,
				0xBF, 0x45, 0x67, 0x60, 0xD1, 0xB7, 0x77, 0x40, 0x44, 0x6E, 0x02, 0x06, 0x0C, 0xCB, 0xFF, 0x11,
				0xF0, 0x31, 0x00, 0x47, 0x57, 0xE0, 0x00, 0xA4, 0x00, 0xAA, 0x70, 0xD5, 0xC0, 0xC0, 0xEB, 0xB2,
				0xDD, 0x2A, 0x08, 0xC4, 0x25, 0xCD, 0x90, 0xE6, 0x86, 0xD6, 0xC6, 0x0A, 0xD6, 0x03, 0xBE, 0x9C,
				0x31, 0x78, 0xDE, 0x0D, 0xD2, 0xC3, 0x4A, 0xC0, 0x76, 0xD9, 0x4C, 0xF7, 0x40, 0xEA, 0xA4, 0x5B,
				0xF8, 0x5E, 0xED, 0xBF, 0xC8, 0x53, 0x9E, 0xF2, 0x13, 0xCE, 0xA7, 0xD4, 0x2D, 0x81, 0x1E, 0xBF,
				0x8C, 0x00, 0x00, 0x00, 0x00, 0x49, 0x45, 0x4E, 0x44, 0xAE, 0x42, 0x60, 0x82
		};
		static readonly Color _logBackgroundColor = new Color (0.1337f, 0.1337f, 0.1337f, 0.95f);
		static readonly List<hConsole.LogMessage> _logMessages = new List<hConsole.LogMessage> ();
		static readonly Color[] _logTypeColors = { Color.white, Color.yellow, Color.red };
		static readonly List<hConsole.PrintedText> _printedText = new List<hConsole.PrintedText> ();
		static readonly Color _statsBackgroundColor = new Color (0.1337f, 0.1337f, 0.1337f, 0.95f);
		static readonly Color _statsForegroundColor = new Color (1f, 1f, 1f, 0.95f);
		static readonly Dictionary<string, WatchedItem> _watchedItems = new Dictionary<string, WatchedItem> ();
		static int _debugLogCount;
		Camera _fontCamera;
		Texture2D _fontImage;
		Material _fontMaterial;
		float[] _glyphLeft;
		float[] _glyphRight;
		float[] _glyphTop;
		float[] _glyphBottom;
		bool _init;
		int _logOffsetH;
		int _logOffsetV;
		int _watchWidth = 100;

		public int ConsoleHeight {
				get { return _consoleHeight; }
				set { _consoleHeight = value; }
		}

		int _consoleHeight = 382;
		hConsole.UVRectangle _whiteUV;

		public enum DisplayLocation
		{
				Top = 1,
				Bottom = 2,
		}

		public enum DisplayMode
		{
				Off = 0,
				Stats = 1,
				Console = 2
		}

		public static void Error (object obj)
		{
				Debug.LogError (obj);
				hConsole.PushLog (obj.ToString (), 2);
		}

		public static void Error (string text, params object[] args)
		{
				Debug.LogError (string.Format (text, args));

				hConsole.PushLog (string.Format (text, args), 2);
		}

		public static void Error (Object context, string text, params object[] args)
		{

				Debug.LogError (string.Format (text, args), context);

				hConsole.PushLog (string.Format (text, args), 2);
		}

		public static void Initialize ()
		{
				var foundConsole = FindObjectOfType (typeof(hConsole)) as hConsole;
				if (foundConsole == null) {
						var go = GameObject.Find (Hydrogen.Components.DefaultSingletonName) ??
						         new GameObject (Hydrogen.Components.DefaultSingletonName);
						go.AddComponent<hConsole> ();
				}
		}

		public static void Log (object obj)
		{

				Debug.Log (obj);
				hConsole.PushLog (obj.ToString (), 0);
		}

		public static void Log (string text, params object[] args)
		{

				Debug.Log (string.Format (text, args));
				hConsole.PushLog (string.Format (text, args), 0);
		}

		public static void Log (Object context, string text, params object[] args)
		{

				Debug.Log (string.Format (text, args), context);
				hConsole.PushLog (string.Format (text, args), 0);
		}

		public static void Print (int x, int y, object obj)
		{
				hConsole._printedText.Add (new hConsole.PrintedText (obj.ToString (), x, y, Color.white));
		}

		public static void Print (int x, int y, string text)
		{
				hConsole._printedText.Add (new hConsole.PrintedText (text, x, y, Color.white));
		}

		public static void Print (int x, int y, string text, params object[] args)
		{
				hConsole._printedText.Add (new hConsole.PrintedText (string.Format (text, args), x, y, Color.white));
		}

		public static void Print (int x, int y, Color colour, object obj)
		{
				hConsole._printedText.Add (new hConsole.PrintedText (obj.ToString (), x, y, colour));
		}

		public static void Print (int x, int y, Color colour, string text)
		{
				hConsole._printedText.Add (new hConsole.PrintedText (text, x, y, colour));
		}

		public static void Print (int x, int y, Color colour, string text, params object[] args)
		{
				hConsole._printedText.Add (new hConsole.PrintedText (string.Format (text, args), x, y, colour));
		}

		public static void Warn (object obj)
		{
				Debug.LogWarning (obj);
				hConsole.PushLog (obj.ToString (), 1);
		}

		public static void Warn (string text, params object[] args)
		{
				Debug.LogWarning (string.Format (text, args));
				hConsole.PushLog (string.Format (text, args), 1);
		}

		public static void Warn (Object context, string text, params object[] args)
		{
				Debug.LogWarning (string.Format (text, args), context);
				hConsole.PushLog (string.Format (text, args), 1);
		}

		public static void Watch (string key, bool value)
		{
				if (_watchedItems.ContainsKey (key) && _watchedItems [key].Type == WatchedItem.ItemType.Boolean) {
						_watchedItems [key].Data = value.ToString ();

				} else {
						_watchedItems [key] = new WatchedItem (WatchedItem.ItemType.Boolean, key, value.ToString ());
				}
		}

		public static void Watch (string key, string value)
		{
				if (_watchedItems.ContainsKey (key) && _watchedItems [key].Type == WatchedItem.ItemType.String) {
						_watchedItems [key].Data = value;
				} else {
						_watchedItems [key] = new WatchedItem (WatchedItem.ItemType.String, key, value);
				}
		}

		public static void Watch (string key, int value)
		{
				if (_watchedItems.ContainsKey (key) && _watchedItems [key].Type == WatchedItem.ItemType.Integer) {
						_watchedItems [key].Data = value.ToString ();
				} else {
						_watchedItems [key] = new WatchedItem (WatchedItem.ItemType.Integer, key, value.ToString ());
				}
		}

		public static void Watch (string key, float value)
		{
				if (_watchedItems.ContainsKey (key) && _watchedItems [key].Type == WatchedItem.ItemType.Float) {
						_watchedItems [key].Data = value.ToString ();
				} else {
						_watchedItems [key] = new WatchedItem (WatchedItem.ItemType.Float, key, value.ToString ());
				}
		}

		public static void UnityError (string text, params object[] args)
		{
				hConsole.PushLog (string.Format (text, args), 2);
		}

		public static void UnityLog (string text, params object[] args)
		{
				hConsole.PushLog (string.Format (text, args), 0);
		}

		public static void UnityWarn (string text, params object[] args)
		{
				hConsole.PushLog (string.Format (text, args), 1);
		}

		public static void UnWatch (string key)
		{
				if (_watchedItems.ContainsKey (key))
						_watchedItems.Remove (key);
		}

		static void PushLog (string text, int type)
		{
				if (hConsole._logMessages.Count >= DebugLogLinesMax) {
						hConsole._logMessages.RemoveAt (0);
				}
						
				foreach (var lineItem in text.Split (_newLineCharacters)) {
						hConsole._logMessages.Add (
								new hConsole.LogMessage (
										string.Format ("[{0}] {1}", Time.time.ToString ().PadRight (9, '0'), lineItem), type));
						hConsole._debugLogCount++;
				}
		}

		void Awake ()
		{
				Application.RegisterLogCallback (new Application.LogCallback (HandleException));
		}

		/// <summary>
		/// Registered Callback for Unity's own Debug.Log call
		/// </summary>
		/// <param name="condition">Condition.</param>
		/// <param name="stackTrace">Stack trace.</param>
		/// <param name="type">Type of log even being passed.</param>
		void HandleException (string condition, string stackTrace, LogType type)
		{
				switch (type) {
				case LogType.Warning:
						hConsole.UnityWarn (condition, new object[0]);
						break;
				case LogType.Log:
						hConsole.UnityLog (condition, new object[0]);
						break;
				default:
						hConsole.UnityError (condition, new object[0]);
						hConsole.UnityError (stackTrace, new object[0]);
						break;
				}
		}

		void OnPostRender ()
		{
				if (CreateCamera) {
						GL.PushMatrix ();
						_fontMaterial.SetPass (0);
						GL.LoadPixelMatrix ();
						GL.Begin (4);
						foreach (hConsole.PrintedText current in hConsole._printedText) {
								PrintString (current.Text, current.X, current.Y, current.Color);
						}
						if (Mode == DisplayMode.Stats) {
								int itemCount = _watchedItems.Count;
								int watchHeight = (itemCount * GlyphHeight) + (itemCount * StatsLineSpacing) + (StatsPadding * 2);
								int lineOffset = StatsPadding;
								int lastFrameWidth = _watchWidth;

								if (Location == DisplayLocation.Top) {
										// Output the stats background quad
										SolidQuad (0, 0, 92, 50, _statsBackgroundColor);

										// Framerate
										PrintString ((1 / Time.smoothDeltaTime).ToString ("#,##0.00") + " FPS", 10, 10, _statsForegroundColor);

										// Approximated Memory
										PrintString (string.Format ("{0:#,##0.00} MB", (System.GC.GetTotalMemory (true) / 1048576f)), 10, 25, _statsForegroundColor);

										// Output the watch list background quad using the calculated width based on content.
										SolidQuad (Screen.width - lastFrameWidth, 0, Screen.width, watchHeight, _statsBackgroundColor);

										// Output all WatchedItems
										foreach (KeyValuePair<string, WatchedItem> entry in _watchedItems) {

												// Determine the width of the background quad for the future.
												if (entry.Value.OutputPaddedSize > _watchWidth) {
														_watchWidth = entry.Value.OutputPaddedSize;
												}

												// Output WatchedItem
												PrintString (entry.Value.Output, Screen.width - lastFrameWidth + StatsPadding, lineOffset, _statsForegroundColor);

												// Increment Line Offset
												lineOffset += StatsLineSpacing + GlyphHeight;
										}
								} else {
										// Output the stats background quad
										SolidQuad (0, Screen.height - 50, 92, Screen.height, _statsBackgroundColor);

										// Framerate
										PrintString ((1 / Time.smoothDeltaTime).ToString ("#,##0.00") + " FPS", 10, Screen.height - 40, _statsForegroundColor);

										// Approximated Memory
										PrintString (string.Format ("{0:#,##0.00} MB", (System.GC.GetTotalMemory (true) / 1048576f)), 10, Screen.height - 25, _statsForegroundColor);

										// Determine a height offset to start outputing the watches
										lineOffset = Screen.height - watchHeight + StatsPadding;

										// Output the watch list background quad using the calculated width based on content.
										SolidQuad (Screen.width - lastFrameWidth, Screen.height - watchHeight, Screen.width, Screen.height, _statsBackgroundColor);

										// Output all WatchedItems
										foreach (KeyValuePair<string, WatchedItem> entry in _watchedItems) {

												// Determine the width of the background quad for the future.
												if (entry.Value.OutputPaddedSize > _watchWidth) {
														_watchWidth = entry.Value.OutputPaddedSize;
												}

												// Output WatchedItem
												PrintString (entry.Value.Output, Screen.width - lastFrameWidth + StatsPadding, lineOffset, _statsForegroundColor);

												// Increment Line Offset
												lineOffset += StatsLineSpacing + GlyphHeight;
										}
								}
						} else if (Mode == DisplayMode.Console) {

								int num = StatsPadding;
								int num2 = hConsole._logMessages.Count - _logOffsetV;
								if (num2 > hConsole._debugLogCount) {
										num2 = hConsole._debugLogCount;
								} else {
										if (num2 < DebugLogLines) {
												num2 = DebugLogLines;
										}
								}
								int num3 = num2 - DebugLogLines;
								if (num3 < 0) {
										num3 = 0;
								}

								if (Location == DisplayLocation.Top) {
										SolidQuad (0f, -4f, (float)Screen.width, ConsoleHeight, _logBackgroundColor);

										for (int i = num3; i < num2; i++) {
												if (i >= 0 && i < hConsole._logMessages.Count) {
														hConsole.LogMessage logMessage = hConsole._logMessages [i];
														PrintString (logMessage.Message, StatsPadding + (_logOffsetH * 8), num, _logTypeColors [logMessage.Type]);
														num += 15;
												}
										}

										string text = string.Format ("{0}-{1}", num3, num2);
										PrintString (text, Screen.width - StatsPadding - (text.Length * 8), StatsPadding, Color.gray);
								} else {
										SolidQuad (0f, Screen.height - ConsoleHeight, (float)Screen.width, Screen.height + 4, _logBackgroundColor);

										num = Screen.height - ConsoleHeight + StatsPadding;
										for (int i = num3; i < num2; i++) {
												if (i >= 0 && i < hConsole._logMessages.Count) {
														hConsole.LogMessage logMessage = hConsole._logMessages [i];
														PrintString (logMessage.Message, StatsPadding + _logOffsetH * 8, num, _logTypeColors [logMessage.Type]);
														num += 15;
												}
										}
										string text = string.Format ("{0}-{1}", num3, num2);

										PrintString (text, Screen.width - StatsPadding - (text.Length * 8), (Screen.height - ConsoleHeight) + StatsPadding, Color.gray);
								}

						}
						GL.End ();
						GL.PopMatrix ();
						hConsole._printedText.Clear ();
				}
		}

		void PrintString (string text, int x, int y, Color colour)
		{
				int num = x;
				for (int i = 0; i < text.Length; i++) {
						char c = text [i];
						int num2 = (int)c;
						if (num2 == 32) {
								num += 8;
						} else {
								if (num2 < 33 || num2 > 127) {
										num2 = 34;
								}
								num2 -= 33;
								float num3 = (float)num;
								int num4 = num + 8;
								int num5 = Screen.height - y;
								int num6 = Screen.height - (y + 15);
								if (colour == hConsole.Shadow) {
										GL.Color (Color.black);
										GL.TexCoord2 (_glyphLeft [num2], _glyphBottom [num2]);
										GL.Vertex3 (num3, (float)num6, 0f);
										GL.Color (Color.black);
										GL.TexCoord2 (_glyphLeft [num2], _glyphTop [num2]);
										GL.Vertex3 (num3, (float)num5, 0f);
										GL.Color (Color.black);
										GL.TexCoord2 (_glyphRight [num2], _glyphTop [num2]);
										GL.Vertex3 ((float)num4, (float)num5, 0f);
										GL.Color (Color.black);
										GL.TexCoord2 (_glyphLeft [num2], _glyphBottom [num2]);
										GL.Vertex3 (num3, (float)num6, 0f);
										GL.Color (Color.black);
										GL.TexCoord2 (_glyphRight [num2], _glyphTop [num2]);
										GL.Vertex3 ((float)num4, (float)num5, 0f);
										GL.Color (Color.black);
										GL.TexCoord2 (_glyphRight [num2], _glyphBottom [num2]);
										GL.Vertex3 ((float)num4, (float)num6, 0f);
										GL.Color (Color.white);
										GL.TexCoord2 (_glyphLeft [num2], _glyphBottom [num2]);
										GL.Vertex3 (num3 - 1f, (float)(num6 + 1), 0f);
										GL.Color (Color.white);
										GL.TexCoord2 (_glyphLeft [num2], _glyphTop [num2]);
										GL.Vertex3 (num3 - 1f, (float)(num5 + 1), 0f);
										GL.Color (Color.white);
										GL.TexCoord2 (_glyphRight [num2], _glyphTop [num2]);
										GL.Vertex3 ((float)(num4 - 1), (float)(num5 + 1), 0f);
										GL.Color (Color.white);
										GL.TexCoord2 (_glyphLeft [num2], _glyphBottom [num2]);
										GL.Vertex3 (num3 - 1f, (float)(num6 + 1), 0f);
										GL.Color (Color.white);
										GL.TexCoord2 (_glyphRight [num2], _glyphTop [num2]);
										GL.Vertex3 ((float)(num4 - 1), (float)(num5 + 1), 0f);
										GL.Color (Color.white);
										GL.TexCoord2 (_glyphRight [num2], _glyphBottom [num2]);
										GL.Vertex3 ((float)(num4 - 1), (float)(num6 + 1), 0f);
								} else {
										GL.Color (colour);
										GL.TexCoord2 (_glyphLeft [num2], _glyphBottom [num2]);
										GL.Vertex3 (num3, (float)num6, 0f);
										GL.Color (colour);
										GL.TexCoord2 (_glyphLeft [num2], _glyphTop [num2]);
										GL.Vertex3 (num3, (float)num5, 0f);
										GL.Color (colour);
										GL.TexCoord2 (_glyphRight [num2], _glyphTop [num2]);
										GL.Vertex3 ((float)num4, (float)num5, 0f);
										GL.Color (colour);
										GL.TexCoord2 (_glyphLeft [num2], _glyphBottom [num2]);
										GL.Vertex3 (num3, (float)num6, 0f);
										GL.Color (colour);
										GL.TexCoord2 (_glyphRight [num2], _glyphTop [num2]);
										GL.Vertex3 ((float)num4, (float)num5, 0f);
										GL.Color (colour);
										GL.TexCoord2 (_glyphRight [num2], _glyphBottom [num2]);
										GL.Vertex3 ((float)num4, (float)num6, 0f);
								}
								num += 8;
						}
				}
		}

		void SolidQuad (float left, float top, float right, float bottom, Color color)
		{
				top = (float)Screen.height - top;
				bottom = (float)Screen.height - bottom;
				GL.Color (color);
				GL.TexCoord2 (_whiteUV.U0, _whiteUV.V0);
				GL.Vertex3 (left, bottom, 0f);
				GL.Color (color);
				GL.TexCoord2 (_whiteUV.U0, _whiteUV.V0);
				GL.Vertex3 (left, top, 0f);
				GL.Color (color);
				GL.TexCoord2 (_whiteUV.U0, _whiteUV.V0);
				GL.Vertex3 (right, top, 0f);
				GL.Color (color);
				GL.TexCoord2 (_whiteUV.U0, _whiteUV.V0);
				GL.Vertex3 (left, bottom, 0f);
				GL.Color (color);
				GL.TexCoord2 (_whiteUV.U0, _whiteUV.V0);
				GL.Vertex3 (right, top, 0f);
				GL.Color (color);
				GL.TexCoord2 (_whiteUV.U0, _whiteUV.V0);
				GL.Vertex3 (right, bottom, 0f);
		}

		void Start ()
		{
				if (CreateCamera) {
						_fontImage = new Texture2D (128, 128);
						_fontImage.LoadImage (_fontImageData);
						_fontImage.filterMode = 0;
						_fontImage.wrapMode = (TextureWrapMode)1;
						_fontImage.hideFlags = (HideFlags)13;
						Material material = new Material ("Shader \"hDebug/Text\"\r\n                {\r\n                      Properties\r\n                      {\r\n                          _MainTex (\"Main\", 2D) = \"white\" {}\r\n                      }\r\n                      \r\n                      Category\r\n                      {\r\n                        Tags\r\n                        {\r\n                            \"Queue\" = \"Transparent\"\r\n                        }\r\n                        \r\n                        Blend SrcAlpha OneMinusSrcAlpha\r\n                        AlphaTest Greater .01\r\n                        ColorMask RGB\r\n                        Cull Off\r\n                        Lighting Off\r\n                        ZWrite On\r\n                        \r\n                        Fog\r\n                        {\r\n                            Color(0, 0, 0, 0)\r\n                        }\r\n                        \r\n                        BindChannels\r\n                        {\r\n                            Bind \"Color\", color\r\n                            Bind \"Vertex\", vertex\r\n                            Bind \"TexCoord\", texcoord\r\n                        }\r\n                        \r\n                        SubShader\r\n                        {\r\n                            Pass\r\n                            {\r\n                                SetTexture [_MainTex]\r\n                                {\r\n                                    combine texture * primary\r\n                                }\r\n                            }\r\n                        }\r\n                    }\r\n                }");
						material.mainTexture = _fontImage;
						material.hideFlags = (HideFlags)13;
						material.shader.hideFlags = (HideFlags)13;
						_fontMaterial = material;
						_fontCamera = (Camera)gameObject.AddComponent ("Camera");
						_fontCamera.nearClipPlane = 0.1f;
						_fontCamera.farClipPlane = 1f;
						_fontCamera.clearFlags = (CameraClearFlags)4;
						_fontCamera.depth = 99f;
						_fontCamera.cullingMask = 0;
						_fontCamera.useOcclusionCulling = false;
						_fontCamera.isOrthoGraphic = true;
						_glyphLeft = new float[94];
						_glyphTop = new float[94];
						_glyphRight = new float[94];
						_glyphBottom = new float[94];
						int num = 0;
						int num2 = 0;
						int width = _fontImage.width;
						float num3 = 1f / (float)_fontImage.width;
						float num4 = 1f / (float)_fontImage.height;
						for (int i = 0; i < 94; i++) {
								_glyphLeft [i] = (float)num;
								_glyphTop [i] = (float)num2;
								_glyphRight [i] = (float)(num + 8);
								_glyphBottom [i] = (float)(num2 + 15);
								_glyphLeft [i] *= num3;
								_glyphRight [i] *= num3;
								_glyphTop [i] *= num4;
								_glyphBottom [i] *= num4;
								_glyphTop [i] = 1f - _glyphTop [i];
								_glyphBottom [i] = 1f - _glyphBottom [i];
								num += 8;
								if (num == width) {
										num = 0;
										num2 += 15;
								}
						}
						_whiteUV = hConsole.UVRectangle.CreateFromTexture (3, 5, 1, 1, _fontImage.width, _fontImage.height);
				}
		}

		public void ToggleMode ()
		{
				switch (Mode) {
				case DisplayMode.Off:
						Mode = DisplayMode.Stats;
						break;
				case DisplayMode.Stats:
						Mode = DisplayMode.Console;
						break;
				case DisplayMode.Console:
						Mode = DisplayMode.Off;
						break;
				}
		}

		public void PageUp ()
		{
				_logOffsetV += DebugLogLines;
				int num = hConsole._logMessages.Count - DebugLogLines;
				if (_logOffsetV > num) {
						_logOffsetV = num;
				}
		}

		public void PageDown ()
		{
				_logOffsetV -= DebugLogLines;
				if (_logOffsetV < 0) {
						_logOffsetV = 0;
				}
		}

		public void ScrollLeft ()
		{
				_logOffsetH--;
		}

		public void ScrollRight ()
		{
				_logOffsetH++;
		}

		void Update ()
		{
				if (CreateCamera && !_init) {
						_init = true;
						gameObject.camera.enabled = true;
				}
						
				if ((Input.GetKey (KeyCode.LeftShift) || Input.GetKey (KeyCode.RightShift)) && Input.GetKeyDown (ToggleKey)) {
						Mode = DisplayMode.Console;
				} else {
						if (Input.GetKeyDown (ToggleKey)) {
								ToggleMode ();
						}
				}

				if (Mode == DisplayMode.Console) {
						if (Input.GetKeyDown (KeyCode.UpArrow)) {
								PageUp ();
						} else if (Input.GetKeyDown (KeyCode.DownArrow)) {
								PageDown ();
						} else if (Input.GetKeyDown (KeyCode.LeftArrow)) {
								ScrollLeft ();
						} else if (Input.GetKeyDown (KeyCode.RightArrow)) {
								ScrollRight ();
						}
				}
		}

		struct PrintedText
		{
				public readonly Color Color;
				public readonly string Text;
				public readonly int X;
				public readonly int Y;

				public PrintedText (string text, int x, int y, Color color)
				{
						Text = text;
						X = x;
						Y = y;
						Color = color;
				}
		}

		struct UVRectangle
		{
				public float U0;
				public float U1;
				public float V0;
				public float V1;

				public UVRectangle (float u0, float v0, float u1, float v1)
				{
						U0 = u0;
						V0 = v0;
						U1 = u1;
						V1 = v1;
				}

				public static hConsole.UVRectangle CreateFromTexture (int left, int top, int width, int height, int sourceWidth, int sourceHeight)
				{
						float num = 1f / (float)sourceWidth;
						float num2 = 1f / (float)sourceHeight;
						return new hConsole.UVRectangle {
								U0 = (float)left * num,
								V0 = 1f - (float)top * num2,
								U1 = (float)(left + width) * num,
								V1 = 1f - (float)(top + height) * num2
						};
				}
		}

		public class WatchedItem
		{
				public enum ItemType
				{
						String = 0,
						Integer = 1,
						Float = 2,
						Boolean = 3
				}

				public ItemType Type;

				public string Data {
						get { return _data; }
						set { 
								if (_data != value) {
										_data = value;
										OutputPaddedSize = (Output.Length * GlyphWidth) + (StatsPadding * 2);
								}
						}
				}

				public string Key;
				string _data;

				public string Output { 
						get {
								return Key + ": " + Data;
						}
				}

				public int OutputPaddedSize;

				public WatchedItem (ItemType type, string key, string data)
				{
						Type = type;
						Key = key;
						Data = data;
				}
		}

		class LogMessage
		{
				/// <summary>
				/// The Message
				/// </summary>
				public readonly string Message;
				/// <summary>
				/// The Message Type
				/// </summary>
				public readonly int Type;

				public LogMessage (string message, int type)
				{
						Message = message;
						Type = type;
				}
		}
}
