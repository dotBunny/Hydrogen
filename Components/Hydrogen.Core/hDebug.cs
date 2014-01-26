#region Copyright Notice & License Information
//
// hDebug.cs
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

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// TODO: Make as a drop in
// TODO: Add "Watch" system to watch something (maybe you register actions for it to pull a string from?
/// <summary>
/// A developer console useful for in-game debugging.
/// </summary>
public class hDebug : MonoBehaviour
{
		const int _debugLogLines = 25;
		const int _debugLogLinesMax = 10000;
		const int _statsPadding = 10;
		const int _statsLineSpacing = 6;
		const int FontBegin = 33;
		const int FontEnd = 127;
		const int GlyphHeight = 15;
		const int GlyphWidth = 8;
		const int NbGlyphs = 94;
		const string ShaderText = "Shader \"hDebug/Text\"\r\n{\r\nProperties\r\n{\r\n_MainTex (\"Main\", 2D) = \"white\" {}\r\n}\r\n\r\nCategory\r\n{\r\n                        Tags\r\n                        {\r\n                            \"Queue\" = \"Transparent\"\r\n                        }\r\n                        \r\n                        Blend SrcAlpha OneMinusSrcAlpha\r\n                        AlphaTest Greater .01\r\n                        ColorMask RGB\r\n                        Cull Off\r\n                        Lighting Off\r\n                        ZWrite On\r\n                        \r\n                        Fog\r\n                        {\r\n                            Color(0, 0, 0, 0)\r\n                        }\r\n                        \r\n                        BindChannels\r\n                        {\r\n                            Bind \"Color\", color\r\n                            Bind \"Vertex\", vertex\r\n                            Bind \"TexCoord\", texcoord\r\n                        }\r\n                        \r\n                        SubShader\r\n                        {\r\n                            Pass\r\n                            {\r\n                                SetTexture [_MainTex]\r\n                                {\r\n                                    combine texture * primary\r\n                                }\r\n                            }\r\n                        }\r\n                    }\r\n                }";
		public static Color Shadow = Color.cyan;
		public bool CreateCamera = true;
		public DisplayMode Mode = DisplayMode.Off;
		public DisplayLocation Location = DisplayLocation.Top;
		public KeyCode Toggle = KeyCode.BackQuote;
		public KeyCode ScrollUp = KeyCode.UpArrow;
		public KeyCode ScrollDown = KeyCode.DownArrow;
		public KeyCode ScrollLeft = KeyCode.LeftArrow;
		public KeyCode ScrollRight = KeyCode.RightArrow;
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
		static readonly List<hDebug.LogMessage> _logMessages = new List<hDebug.LogMessage> ();
		static readonly Color[] _logTypeColors = { Color.white, Color.yellow, Color.red };
		static readonly List<hDebug.PrintedText> _printedText = new List<hDebug.PrintedText> ();
		static readonly Color _statsBackgroundColor = new Color (0.1337f, 0.1337f, 0.1337f, 0.95f);
		static readonly Color _statsForegroundColor = new Color (1f, 1f, 1f, 0.95f);
		static readonly Dictionary<string, WatchedItem> _watchedItems = new Dictionary<string, WatchedItem> ();
		static int _debugLogCount;
		Camera _fontCamera;
		Texture2D _fontImage;
		UnityEngine.Material _fontMaterial;
		float[] _glyphLeft;
		float[] _glyphRight;
		float[] _glyphTop;
		float[] _glyphBottom;
		bool _init;
		bool _keyIsUp;
		bool _keyWasUp;
		int _logOffsetH;
		int _logOffsetV;
		bool _showLog;
		int _watchWidth = 100;
		hDebug.UVRectangle _whiteUV;

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
				#if UNITY_EDITOR
				UnityEngine.Debug.LogError (obj);
				#endif
				hDebug.PushLog (obj.ToString (), 2);
		}

		public static void Error (string text, params object[] args)
		{
				#if UNITY_EDITOR
				UnityEngine.Debug.LogError (string.Format (text, args));
				#endif
				hDebug.PushLog (string.Format (text, args), 2);
		}

		public static void Error (UnityEngine.Object context, string text, params object[] args)
		{
				#if UNITY_EDITOR
				UnityEngine.Debug.LogError (string.Format (text, args), context);
				#endif
				hDebug.PushLog (string.Format (text, args), 2);
		}

		public static void Log (object obj)
		{
				#if UNITY_EDITOR
				UnityEngine.Debug.Log (obj);
				#endif
				hDebug.PushLog (obj.ToString (), 0);
		}

		public static void Log (string text, params object[] args)
		{
				#if UNITY_EDITOR
				UnityEngine.Debug.Log (string.Format (text, args));
				#endif
				hDebug.PushLog (string.Format (text, args), 0);
		}

		public static void Log (UnityEngine.Object context, string text, params object[] args)
		{
				#if UNITY_EDITOR
				UnityEngine.Debug.Log (string.Format (text, args), context);
				#endif
				hDebug.PushLog (string.Format (text, args), 0);
		}

		public static void Print (int x, int y, object obj)
		{
				hDebug._printedText.Add (new hDebug.PrintedText (obj.ToString (), x, y, Color.white));
		}

		public static void Print (int x, int y, string text)
		{
				hDebug._printedText.Add (new hDebug.PrintedText (text, x, y, Color.white));
		}

		public static void Print (int x, int y, string text, params object[] args)
		{
				hDebug._printedText.Add (new hDebug.PrintedText (string.Format (text, args), x, y, Color.white));
		}

		public static void Print (int x, int y, Color colour, object obj)
		{
				hDebug._printedText.Add (new hDebug.PrintedText (obj.ToString (), x, y, colour));
		}

		public static void Print (int x, int y, Color colour, string text)
		{
				hDebug._printedText.Add (new hDebug.PrintedText (text, x, y, colour));
		}

		public static void Print (int x, int y, Color colour, string text, params object[] args)
		{
				hDebug._printedText.Add (new hDebug.PrintedText (string.Format (text, args), x, y, colour));
		}

		public static void Warn (object obj)
		{
				#if UNITY_EDITOR
				UnityEngine.Debug.LogWarning (obj);
				#endif
				hDebug.PushLog (obj.ToString (), 1);
		}

		public static void Warn (string text, params object[] args)
		{
				#if UNITY_EDITOR
				UnityEngine.Debug.LogWarning (string.Format (text, args));
				#endif
				hDebug.PushLog (string.Format (text, args), 1);
		}

		public static void Warn (UnityEngine.Object context, string text, params object[] args)
		{
				#if UNITY_EDITOR
				UnityEngine.Debug.LogWarning (string.Format (text, args), context);
				#endif
				hDebug.PushLog (string.Format (text, args), 1);
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
				hDebug.PushLog (string.Format (text, args), 2);
		}

		public static void UnityLog (string text, params object[] args)
		{
				hDebug.PushLog (string.Format (text, args), 0);
		}

		public static void UnityWarn (string text, params object[] args)
		{
				hDebug.PushLog (string.Format (text, args), 1);
		}

		public static void UnWatch (string key)
		{
				if (_watchedItems.ContainsKey (key))
						_watchedItems.Remove (key);
		}

		static void PushLog (string text, int type)
		{
				if (hDebug._logMessages.Count >= _debugLogLinesMax) {
						hDebug._logMessages.RemoveAt (0);
				}
				string[] array = text.Split (new char[] {
						'\n'
				});
				string[] array2 = array;
				for (int i = 0; i < array2.Length; i++) {
						string m = array2 [i];
						hDebug._logMessages.Add (
								new hDebug.LogMessage (
										string.Format ("[{0}] {1}", Time.time.ToString ().PadRight (9, '0'), m), type));
						hDebug._debugLogCount++;
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
						hDebug.UnityWarn (condition, new object[0]);
						break;
				case LogType.Log:
						hDebug.UnityLog (condition, new object[0]);
						break;
				default:
						hDebug.UnityError (condition, new object[0]);
						hDebug.UnityError (stackTrace, new object[0]);
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
						foreach (hDebug.PrintedText current in hDebug._printedText) {
								PrintString (current.Text, current.X, current.Y, current.Color);
						}
						if (Mode == DisplayMode.Stats) {
								int itemCount = _watchedItems.Count;
								int watchHeight = (itemCount * 9) + (itemCount * _statsLineSpacing) + (_statsPadding * 2);
								int lineOffset = _statsPadding;
								int lastFrameWidth = _watchWidth;

								if (Location == DisplayLocation.Top) {
										// Stats
										SolidQuad (0, 0, 92, 50, _statsBackgroundColor);
										PrintString ((1 / Time.smoothDeltaTime).ToString ("#,##0.00") + " FPS", 10, 10, _statsForegroundColor);
										PrintString ((System.GC.GetTotalMemory (true) / 1048576f).ToString ("#,##0.00") + " MB", 10, 25, _statsForegroundColor);

										// Watch
										SolidQuad (Screen.width - lastFrameWidth, 0, Screen.width, watchHeight, _statsBackgroundColor);
										foreach (KeyValuePair<string, WatchedItem> entry in _watchedItems) {
												int testLength = (entry.Value.Output.Length * GlyphWidth) + (_statsPadding * 2);
												if (testLength > _watchWidth) {
														_watchWidth = testLength;
												}
												PrintString (entry.Value.Output, Screen.width - lastFrameWidth + _statsPadding, lineOffset, _statsForegroundColor);
												lineOffset += _statsLineSpacing + 9;
										}
								} else {
										SolidQuad (0, Screen.height - 50, 92, Screen.height, _statsBackgroundColor);
										PrintString ((1 / Time.smoothDeltaTime).ToString ("#,##0.00") + " FPS", 10, Screen.height - 40, _statsForegroundColor);
										PrintString ((System.GC.GetTotalMemory (true) / 1048576f).ToString ("#,##0.00") + " MB", 10, Screen.height - 25, _statsForegroundColor);

										// Watch
										lineOffset = Screen.height - _statsPadding - (_statsLineSpacing * (_watchedItems.Count - 1)) - (GlyphHeight * (_watchedItems.Count - 1));
										SolidQuad (Screen.width - lastFrameWidth, Screen.height - watchHeight, Screen.width, Screen.height, _statsBackgroundColor);
										foreach (KeyValuePair<string, WatchedItem> entry in _watchedItems) {
												int testLength = (entry.Value.Output.Length * GlyphWidth) + (_statsPadding * 2);
												if (testLength > _watchWidth) {
														_watchWidth = testLength;
												}
												PrintString (entry.Value.Output, Screen.width - lastFrameWidth + _statsPadding, lineOffset, _statsForegroundColor);
												lineOffset += _statsLineSpacing + 9;
										}
								}
						} else if (Mode == DisplayMode.Console) {
								SolidQuad (0f, -4f, (float)Screen.width, 375f, _logBackgroundColor);
								int num = 0;
								int num2 = hDebug._logMessages.Count - _logOffsetV;
								if (num2 > hDebug._debugLogCount) {
										num2 = hDebug._debugLogCount;
								} else {
										if (num2 < 25) {
												num2 = 25;
										}
								}
								int num3 = num2 - 25;
								if (num3 < 0) {
										num3 = 0;
								}
								for (int i = num3; i < num2; i++) {
										if (i >= 0 && i < hDebug._logMessages.Count) {
												hDebug.LogMessage logMessage = hDebug._logMessages [i];
												PrintString (logMessage.Message, 1 + _logOffsetH * 8, num, _logTypeColors [logMessage.Type]);
												num += 15;
										}
								}
								string text = string.Format ("{0}-{1}", num3, num2);
								PrintString (text, Screen.width - 8 * text.Length, 0, Color.gray);

						}
						GL.End ();
						GL.PopMatrix ();
						hDebug._printedText.Clear ();
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
								if (colour == hDebug.Shadow) {
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

		void SolidQuad (float left, float top, float right, float bottom, Color colour)
		{
				top = (float)Screen.height - top;
				bottom = (float)Screen.height - bottom;
				GL.Color (colour);
				GL.TexCoord2 (_whiteUV.U0, _whiteUV.V0);
				GL.Vertex3 (left, bottom, 0f);
				GL.Color (colour);
				GL.TexCoord2 (_whiteUV.U0, _whiteUV.V0);
				GL.Vertex3 (left, top, 0f);
				GL.Color (colour);
				GL.TexCoord2 (_whiteUV.U0, _whiteUV.V0);
				GL.Vertex3 (right, top, 0f);
				GL.Color (colour);
				GL.TexCoord2 (_whiteUV.U0, _whiteUV.V0);
				GL.Vertex3 (left, bottom, 0f);
				GL.Color (colour);
				GL.TexCoord2 (_whiteUV.U0, _whiteUV.V0);
				GL.Vertex3 (right, top, 0f);
				GL.Color (colour);
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
						UnityEngine.Material material = new UnityEngine.Material ("Shader \"hDebug/Text\"\r\n                {\r\n                      Properties\r\n                      {\r\n                          _MainTex (\"Main\", 2D) = \"white\" {}\r\n                      }\r\n                      \r\n                      Category\r\n                      {\r\n                        Tags\r\n                        {\r\n                            \"Queue\" = \"Transparent\"\r\n                        }\r\n                        \r\n                        Blend SrcAlpha OneMinusSrcAlpha\r\n                        AlphaTest Greater .01\r\n                        ColorMask RGB\r\n                        Cull Off\r\n                        Lighting Off\r\n                        ZWrite On\r\n                        \r\n                        Fog\r\n                        {\r\n                            Color(0, 0, 0, 0)\r\n                        }\r\n                        \r\n                        BindChannels\r\n                        {\r\n                            Bind \"Color\", color\r\n                            Bind \"Vertex\", vertex\r\n                            Bind \"TexCoord\", texcoord\r\n                        }\r\n                        \r\n                        SubShader\r\n                        {\r\n                            Pass\r\n                            {\r\n                                SetTexture [_MainTex]\r\n                                {\r\n                                    combine texture * primary\r\n                                }\r\n                            }\r\n                        }\r\n                    }\r\n                }");
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
						_whiteUV = hDebug.UVRectangle.CreateFromTexture (3, 5, 1, 1, _fontImage.width, _fontImage.height);
				}
		}

		void Update ()
		{
				if (CreateCamera && !_init) {
						_init = true;
						gameObject.camera.enabled = true;
				}
				_keyWasUp = _keyIsUp;
				_keyIsUp = Input.GetKeyUp (Toggle);
				if (_keyWasUp && !_keyIsUp) {
						_showLog = !_showLog;
				}
				if (_showLog) {
						if (Input.GetKeyUp (ScrollUp)) {
								_logOffsetV += 25;
								int num = hDebug._logMessages.Count - 25;
								if (_logOffsetV > num) {
										_logOffsetV = num;
								}
						} else {
								if (Input.GetKeyUp (ScrollDown)) {
										_logOffsetV -= 25;
										if (_logOffsetV < 0) {
												_logOffsetV = 0;
										}
								} else {
										if (Input.GetKeyUp (ScrollLeft)) {
												_logOffsetH--;
										} else {
												if (Input.GetKeyUp (ScrollRight)) {
														_logOffsetH++;
												}
										}
								}
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

				public static hDebug.UVRectangle CreateFromTexture (int left, int top, int width, int height, int sourceWidth, int sourceHeight)
				{
						float num = 1f / (float)sourceWidth;
						float num2 = 1f / (float)sourceHeight;
						return new hDebug.UVRectangle {
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
				public string Data;
				public string Key;

				public string Output { 
						get {
								return Key + ": " + Data;
						}
				}

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