#region Copyright Notice & License Information
//
// InputControlBase.cs
//
// Author:
//       Matthew Davey <matthew.davey@dotbunny.com>
//       Robin Southern <betajaen@ihoed.com>
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
using UnityEngine;

namespace Hydrogen.Peripherals
{
		abstract class InputControlBase
		{

				#region control names

				public static readonly Dictionary<String, String> DeltaAxes = new Dictionary<String, String> {
						{ "Mouse X", "Mouse X" },
						{ "Mouse Y", "Mouse Y" },
						{ "Mouse ScrollWheel", "Mouse ScrollWheel" }
				};
                public static readonly Dictionary<String, String[]> DoubleDeltaAxes = new Dictionary<String, String[]> {
						{ "Mouse", new []{"Mouse X", "Mouse Y"} },
				};
		        public static readonly Dictionary<String, MouseGesture> MouseGestures = new Dictionary<string, MouseGesture>()
		        {
		                {"Click", MouseGesture.Click},
		                {"DoubleClick", MouseGesture.DoubleClick},
		                {"DoubleClickDrag", MouseGesture.DoubleClickDrag},
		                {"LongClick", MouseGesture.LongClick},
		                {"LongClickDrag", MouseGesture.LongClickDrag}
		        };
                public static readonly Dictionary<String, TouchGesture> TouchGestures = new Dictionary<String, TouchGesture>
		        {
                    {"Touch",                TouchGesture.Touch},
                    {"DoubleTouch",          TouchGesture.DoubleTouch},
                    {"DoubleTouchDrag",      TouchGesture.DoubleTouchDrag},
                    {"LongPress",            TouchGesture.LongPress},
                    {"LongPressDrag",        TouchGesture.LongPressDrag},
                    {"Pinch",                TouchGesture.Pinch},
		        };
				public static readonly Dictionary<String, String> Axes = new Dictionary<String, String> {
						{ "Horizontal", "Horizontal" },
						{ "Vertical", "Vertical" }
				};
				public static readonly Dictionary<String, int> MouseButtons = new Dictionary<String, int> {
						{ "Left", 0 },
						{ "Right", 1 },
						{ "Middle", 2 },
				};
				public static readonly Dictionary<String, KeyCode> KeyboardButtons = new Dictionary<String, KeyCode> {
						{ "Backspace", KeyCode.Backspace },
						{ "Tab", KeyCode.Tab },
						{ "Clear", KeyCode.Clear },
						{ "Return", KeyCode.Return },
						{ "Pause", KeyCode.Pause },
						{ "Escape", KeyCode.Escape },
						{ "Space", KeyCode.Space },
						{ "Exclaim", KeyCode.Exclaim },
						{ "DoubleQuote", KeyCode.DoubleQuote },
						{ "Hash", KeyCode.Hash },
						{ "Dollar", KeyCode.Dollar },
						{ "Ampersand", KeyCode.Ampersand },
						{ "Quote", KeyCode.Quote },
						{ "LeftParen", KeyCode.LeftParen },
						{ "RightParen", KeyCode.RightParen },
						{ "Asterisk", KeyCode.Asterisk },
						{ "Plus", KeyCode.Plus },
						{ "Comma", KeyCode.Comma },
						{ "Minus", KeyCode.Minus },
						{ "Period", KeyCode.Period },
						{ "Slash", KeyCode.Slash },
						{ "Alpha0", KeyCode.Alpha0 },
						{ "Alpha1", KeyCode.Alpha1 },
						{ "Alpha2", KeyCode.Alpha2 },
						{ "Alpha3", KeyCode.Alpha3 },
						{ "Alpha4", KeyCode.Alpha4 },
						{ "Alpha5", KeyCode.Alpha5 },
						{ "Alpha6", KeyCode.Alpha6 },
						{ "Alpha7", KeyCode.Alpha7 },
						{ "Alpha8", KeyCode.Alpha8 },
						{ "Alpha9", KeyCode.Alpha9 },
						{ "Colon", KeyCode.Colon },
						{ "Semicolon", KeyCode.Semicolon },
						{ "Less", KeyCode.Less },
						{ "Equals", KeyCode.Equals },
						{ "Greater", KeyCode.Greater },
						{ "Question", KeyCode.Question },
						{ "At", KeyCode.At },
						{ "LeftBracket", KeyCode.LeftBracket },
						{ "Backslash", KeyCode.Backslash },
						{ "RightBracket", KeyCode.RightBracket },
						{ "Caret", KeyCode.Caret },
						{ "Underscore", KeyCode.Underscore },
						{ "BackQuote", KeyCode.BackQuote },
						{ "A", KeyCode.A },
						{ "B", KeyCode.B },
						{ "C", KeyCode.C },
						{ "D", KeyCode.D },
						{ "E", KeyCode.E },
						{ "F", KeyCode.F },
						{ "G", KeyCode.G },
						{ "H", KeyCode.H },
						{ "I", KeyCode.I },
						{ "J", KeyCode.J },
						{ "K", KeyCode.K },
						{ "L", KeyCode.L },
						{ "M", KeyCode.M },
						{ "N", KeyCode.N },
						{ "O", KeyCode.O },
						{ "P", KeyCode.P },
						{ "Q", KeyCode.Q },
						{ "R", KeyCode.R },
						{ "S", KeyCode.S },
						{ "T", KeyCode.T },
						{ "U", KeyCode.U },
						{ "V", KeyCode.V },
						{ "W", KeyCode.W },
						{ "X", KeyCode.X },
						{ "Y", KeyCode.Y },
						{ "Z", KeyCode.Z },
						{ "Delete", KeyCode.Delete },
						{ "Keypad0", KeyCode.Keypad0 },
						{ "Keypad1", KeyCode.Keypad1 },
						{ "Keypad2", KeyCode.Keypad2 },
						{ "Keypad3", KeyCode.Keypad3 },
						{ "Keypad4", KeyCode.Keypad4 },
						{ "Keypad5", KeyCode.Keypad5 },
						{ "Keypad6", KeyCode.Keypad6 },
						{ "Keypad7", KeyCode.Keypad7 },
						{ "Keypad8", KeyCode.Keypad8 },
						{ "Keypad9", KeyCode.Keypad9 },
						{ "KeypadPeriod", KeyCode.KeypadPeriod },
						{ "KeypadDivide", KeyCode.KeypadDivide },
						{ "KeypadMultiply", KeyCode.KeypadMultiply },
						{ "KeypadMinus", KeyCode.KeypadMinus },
						{ "KeypadPlus", KeyCode.KeypadPlus },
						{ "KeypadEnter", KeyCode.KeypadEnter },
						{ "KeypadEquals", KeyCode.KeypadEquals },
						{ "UpArrow", KeyCode.UpArrow },
						{ "DownArrow", KeyCode.DownArrow },
						{ "RightArrow", KeyCode.RightArrow },
						{ "LeftArrow", KeyCode.LeftArrow },
						{ "Insert", KeyCode.Insert },
						{ "Home", KeyCode.Home },
						{ "End", KeyCode.End },
						{ "PageUp", KeyCode.PageUp },
						{ "PageDown", KeyCode.PageDown },
						{ "F1", KeyCode.F1 },
						{ "F2", KeyCode.F2 },
						{ "F3", KeyCode.F3 },
						{ "F4", KeyCode.F4 },
						{ "F5", KeyCode.F5 },
						{ "F6", KeyCode.F6 },
						{ "F7", KeyCode.F7 },
						{ "F8", KeyCode.F8 },
						{ "F9", KeyCode.F9 },
						{ "F10", KeyCode.F10 },
						{ "F11", KeyCode.F11 },
						{ "F12", KeyCode.F12 },
						{ "F13", KeyCode.F13 },
						{ "F14", KeyCode.F14 },
						{ "F15", KeyCode.F15 },
						{ "Numlock", KeyCode.Numlock },
						{ "CapsLock", KeyCode.CapsLock },
						{ "ScrollLock", KeyCode.ScrollLock },
						{ "RightShift", KeyCode.RightShift },
						{ "LeftShift", KeyCode.LeftShift },
						{ "RightControl", KeyCode.RightControl },
						{ "LeftControl", KeyCode.LeftControl },
						{ "RightAlt", KeyCode.RightAlt },
						{ "LeftAlt", KeyCode.LeftAlt },
						{ "RightApple", KeyCode.RightApple },
						{ "RightCommand", KeyCode.RightCommand },
						{ "LeftApple", KeyCode.LeftApple },
						{ "LeftCommand", KeyCode.LeftCommand },
						{ "LeftWindows", KeyCode.LeftWindows },
						{ "RightWindows", KeyCode.RightWindows },
						{ "AltGr", KeyCode.AltGr },
						{ "Help", KeyCode.Help },
						{ "Print", KeyCode.Print },
						{ "SysReq", KeyCode.SysReq },
						{ "Break", KeyCode.Break },
						{ "Menu", KeyCode.Menu },
						{ "Mouse0", KeyCode.Mouse0 },
						{ "Mouse1", KeyCode.Mouse1 },
						{ "Mouse2", KeyCode.Mouse2 },
						{ "Mouse3", KeyCode.Mouse3 },
						{ "Mouse4", KeyCode.Mouse4 },
						{ "Mouse5", KeyCode.Mouse5 },
						{ "Mouse6", KeyCode.Mouse6 },
						{ "JoystickButton0", KeyCode.JoystickButton0 },
						{ "JoystickButton1", KeyCode.JoystickButton1 },
						{ "JoystickButton2", KeyCode.JoystickButton2 },
						{ "JoystickButton3", KeyCode.JoystickButton3 },
						{ "JoystickButton4", KeyCode.JoystickButton4 },
						{ "JoystickButton5", KeyCode.JoystickButton5 },
						{ "JoystickButton6", KeyCode.JoystickButton6 },
						{ "JoystickButton7", KeyCode.JoystickButton7 },
						{ "JoystickButton8", KeyCode.JoystickButton8 },
						{ "JoystickButton9", KeyCode.JoystickButton9 },
						{ "JoystickButton10", KeyCode.JoystickButton10 },
						{ "JoystickButton11", KeyCode.JoystickButton11 },
						{ "JoystickButton12", KeyCode.JoystickButton12 },
						{ "JoystickButton13", KeyCode.JoystickButton13 },
						{ "JoystickButton14", KeyCode.JoystickButton14 },
						{ "JoystickButton15", KeyCode.JoystickButton15 },
						{ "JoystickButton16", KeyCode.JoystickButton16 },
						{ "JoystickButton17", KeyCode.JoystickButton17 },
						{ "JoystickButton18", KeyCode.JoystickButton18 },
						{ "JoystickButton19", KeyCode.JoystickButton19 },
						{ "Joystick1Button0", KeyCode.Joystick1Button0 },
						{ "Joystick1Button1", KeyCode.Joystick1Button1 },
						{ "Joystick1Button2", KeyCode.Joystick1Button2 },
						{ "Joystick1Button3", KeyCode.Joystick1Button3 },
						{ "Joystick1Button4", KeyCode.Joystick1Button4 },
						{ "Joystick1Button5", KeyCode.Joystick1Button5 },
						{ "Joystick1Button6", KeyCode.Joystick1Button6 },
						{ "Joystick1Button7", KeyCode.Joystick1Button7 },
						{ "Joystick1Button8", KeyCode.Joystick1Button8 },
						{ "Joystick1Button9", KeyCode.Joystick1Button9 },
						{ "Joystick1Button10", KeyCode.Joystick1Button10 },
						{ "Joystick1Button11", KeyCode.Joystick1Button11 },
						{ "Joystick1Button12", KeyCode.Joystick1Button12 },
						{ "Joystick1Button13", KeyCode.Joystick1Button13 },
						{ "Joystick1Button14", KeyCode.Joystick1Button14 },
						{ "Joystick1Button15", KeyCode.Joystick1Button15 },
						{ "Joystick1Button16", KeyCode.Joystick1Button16 },
						{ "Joystick1Button17", KeyCode.Joystick1Button17 },
						{ "Joystick1Button18", KeyCode.Joystick1Button18 },
						{ "Joystick1Button19", KeyCode.Joystick1Button19 },
						{ "Joystick2Button0", KeyCode.Joystick2Button0 },
						{ "Joystick2Button1", KeyCode.Joystick2Button1 },
						{ "Joystick2Button2", KeyCode.Joystick2Button2 },
						{ "Joystick2Button3", KeyCode.Joystick2Button3 },
						{ "Joystick2Button4", KeyCode.Joystick2Button4 },
						{ "Joystick2Button5", KeyCode.Joystick2Button5 },
						{ "Joystick2Button6", KeyCode.Joystick2Button6 },
						{ "Joystick2Button7", KeyCode.Joystick2Button7 },
						{ "Joystick2Button8", KeyCode.Joystick2Button8 },
						{ "Joystick2Button9", KeyCode.Joystick2Button9 },
						{ "Joystick2Button10", KeyCode.Joystick2Button10 },
						{ "Joystick2Button11", KeyCode.Joystick2Button11 },
						{ "Joystick2Button12", KeyCode.Joystick2Button12 },
						{ "Joystick2Button13", KeyCode.Joystick2Button13 },
						{ "Joystick2Button14", KeyCode.Joystick2Button14 },
						{ "Joystick2Button15", KeyCode.Joystick2Button15 },
						{ "Joystick2Button16", KeyCode.Joystick2Button16 },
						{ "Joystick2Button17", KeyCode.Joystick2Button17 },
						{ "Joystick2Button18", KeyCode.Joystick2Button18 },
						{ "Joystick2Button19", KeyCode.Joystick2Button19 },
						{ "Joystick3Button0", KeyCode.Joystick3Button0 },
						{ "Joystick3Button1", KeyCode.Joystick3Button1 },
						{ "Joystick3Button2", KeyCode.Joystick3Button2 },
						{ "Joystick3Button3", KeyCode.Joystick3Button3 },
						{ "Joystick3Button4", KeyCode.Joystick3Button4 },
						{ "Joystick3Button5", KeyCode.Joystick3Button5 },
						{ "Joystick3Button6", KeyCode.Joystick3Button6 },
						{ "Joystick3Button7", KeyCode.Joystick3Button7 },
						{ "Joystick3Button8", KeyCode.Joystick3Button8 },
						{ "Joystick3Button9", KeyCode.Joystick3Button9 },
						{ "Joystick3Button10", KeyCode.Joystick3Button10 },
						{ "Joystick3Button11", KeyCode.Joystick3Button11 },
						{ "Joystick3Button12", KeyCode.Joystick3Button12 },
						{ "Joystick3Button13", KeyCode.Joystick3Button13 },
						{ "Joystick3Button14", KeyCode.Joystick3Button14 },
						{ "Joystick3Button15", KeyCode.Joystick3Button15 },
						{ "Joystick3Button16", KeyCode.Joystick3Button16 },
						{ "Joystick3Button17", KeyCode.Joystick3Button17 },
						{ "Joystick3Button18", KeyCode.Joystick3Button18 },
						{ "Joystick3Button19", KeyCode.Joystick3Button19 },
						{ "Joystick4Button0", KeyCode.Joystick4Button0 },
						{ "Joystick4Button1", KeyCode.Joystick4Button1 },
						{ "Joystick4Button2", KeyCode.Joystick4Button2 },
						{ "Joystick4Button3", KeyCode.Joystick4Button3 },
						{ "Joystick4Button4", KeyCode.Joystick4Button4 },
						{ "Joystick4Button5", KeyCode.Joystick4Button5 },
						{ "Joystick4Button6", KeyCode.Joystick4Button6 },
						{ "Joystick4Button7", KeyCode.Joystick4Button7 },
						{ "Joystick4Button8", KeyCode.Joystick4Button8 },
						{ "Joystick4Button9", KeyCode.Joystick4Button9 },
						{ "Joystick4Button10", KeyCode.Joystick4Button10 },
						{ "Joystick4Button11", KeyCode.Joystick4Button11 },
						{ "Joystick4Button12", KeyCode.Joystick4Button12 },
						{ "Joystick4Button13", KeyCode.Joystick4Button13 },
						{ "Joystick4Button14", KeyCode.Joystick4Button14 },
						{ "Joystick4Button15", KeyCode.Joystick4Button15 },
						{ "Joystick4Button16", KeyCode.Joystick4Button16 },
						{ "Joystick4Button17", KeyCode.Joystick4Button17 },
						{ "Joystick4Button18", KeyCode.Joystick4Button18 },
						{ "Joystick4Button19", KeyCode.Joystick4Button19 }
				};


                public static readonly Dictionary<String, TouchAxis> TouchAxes = new Dictionary<String, TouchAxis> {

                        // Touch0
                        // Raw Touch Input.
                        // Equivalent to: UnityEngine.Input.GetTouch(0).position
                        // Only fires when the value has changed, or is set for the first time.
                        {"Touch0",                     TouchAxis.Touch0},
                        
                        
                        // Touch0Delta
                        // Raw Touch Input.
                        // Equivalent to: UnityEngine.Input.GetTouch(0).deltaPosition
                        // Only fires when the value has changed, or is set for the first time.
                        {"Touch0Delta",                TouchAxis.Touch0Delta},

                        
                        // Touch1
                        // Raw Touch Input.
                        // Equivalent to: UnityEngine.Input.GetTouch(1).position
                        // Only fires when the value has changed, or is set for the first time.
                        {"Touch1",                     TouchAxis.Touch1},
                        
                        
                        // Touch0Delta
                        // Raw Touch Input.
                        // Equivalent to: UnityEngine.Input.GetTouch(1).deltaPosition
                        // Only fires when the value has changed, or is set for the first time.
                        {"Touch1Delta",                TouchAxis.Touch1Delta},

                        
                        // Touch2
                        // Raw Touch Input.
                        // Equivalent to: UnityEngine.Input.GetTouch(2).position
                        // Only fires when the value has changed, or is set for the first time.
                        {"Touch2",                     TouchAxis.Touch2},
                        
                        
                        // Touch2Delta
                        // Raw Touch Input.
                        // Equivalent to: UnityEngine.Input.GetTouch(2).deltaPosition
                        // Only fires when the value has changed, or is set for the first time.
                        {"Touch2Delta",                TouchAxis.Touch2Delta},

                        
                        // Touch3
                        // Raw Touch Input.
                        // Equivalent to: UnityEngine.Input.GetTouch(2).position
                        // Only fires when the value has changed, or is set for the first time.
                        {"Touch3",                     TouchAxis.Touch3},
                        
                        
                        // Touch3Delta
                        // Raw Touch Input.
                        // Equivalent to: UnityEngine.Input.GetTouch(2).deltaPosition
                        // Only fires when the value has changed, or is set for the first time.
                        {"Touch3Delta",                TouchAxis.Touch3Delta},


                        // Touch4
                        // Raw Touch Input.
                        // Equivalent to: UnityEngine.Input.GetTouch(4).position
                        // Only fires when the value has changed, or is set for the first time.
                        {"Touch4",                     TouchAxis.Touch4},
                        
                        
                        // Touch4Delta
                        // Raw Touch Input.
                        // Equivalent to: UnityEngine.Input.GetTouch(4).deltaPosition
                        // Only fires when the value has changed, or is set for the first time.
                        {"Touch4Delta",                TouchAxis.Touch4Delta},

                        
                        // Touch5
                        // Raw Touch Input.
                        // Equivalent to: UnityEngine.Input.GetTouch(5).position
                        // Only fires when the value has changed, or is set for the first time.
                        {"Touch5",                     TouchAxis.Touch5},
                        
                        
                        // Touch5Delta
                        // Raw Touch Input.
                        // Equivalent to: UnityEngine.Input.GetTouch(4).deltaPosition
                        // Only fires when the value has changed, or is set for the first time.
                        {"Touch5Delta",                TouchAxis.Touch5Delta},
                        
                        // Touch6
                        // Raw Touch Input.
                        // Equivalent to: UnityEngine.Input.GetTouch(6).position
                        // Only fires when the value has changed, or is set for the first time.
                        {"Touch6",                     TouchAxis.Touch6},
                        
                        
                        // Touch6Delta
                        // Raw Touch Input.
                        // Equivalent to: UnityEngine.Input.GetTouch(6).deltaPosition
                        // Only fires when the value has changed, or is set for the first time.
                        {"Touch6Delta",                TouchAxis.Touch6Delta},

                        
                        // Touch7
                        // Raw Touch Input.
                        // Equivalent to: UnityEngine.Input.GetTouch(7).position
                        // Only fires when the value has changed, or is set for the first time.
                        {"Touch7",                     TouchAxis.Touch7},
                        
                        
                        // Touch7Delta
                        // Raw Touch Input.
                        // Equivalent to: UnityEngine.Input.GetTouch(7).deltaPosition
                        // Only fires when the value has changed, or is set for the first time.
                        {"Touch7Delta",                TouchAxis.Touch7Delta},
                        

                        // Touch8
                        // Raw Touch Input.
                        // Equivalent to: UnityEngine.Input.GetTouch(7).position
                        // Only fires when the value has changed, or is set for the first time.
                        {"Touch8",                     TouchAxis.Touch8},
                        
                        
                        // Touch7Delta
                        // Raw Touch Input.
                        // Equivalent to: UnityEngine.Input.GetTouch(7).deltaPosition
                        // Only fires when the value has changed, or is set for the first time.
                        {"Touch8Delta",                TouchAxis.Touch8Delta},

                        
                        // Touch9
                        // Raw Touch Input.
                        // Equivalent to: UnityEngine.Input.GetTouch(9).position
                        // Only fires when the value has changed, or is set for the first time.
                        {"Touch9",                     TouchAxis.Touch9},
                        
                        
                        // Touch9Delta
                        // Raw Touch Input.
                        // Equivalent to: UnityEngine.Input.GetTouch(9).deltaPosition
                        // Only fires when the value has changed, or is set for the first time.
                        {"Touch9Delta",                TouchAxis.Touch9Delta},
                        
				};

				#endregion

				public enum ControlType
				{
						KeyboardButton,
						MouseButton,
						Axis,
						DeltaAxis
				}

				public static InputControlBase CreateControl (String name, InputAction action)
				{
						string axis;
						if (Axes.TryGetValue (name, out axis)) {
								return new InputAxisControl (name, axis, action);
						}
						if (DeltaAxes.TryGetValue (name, out axis)) {
								return new InputDeltaAxisControl (name, axis, action);
						}
						int button;
						if (MouseButtons.TryGetValue (name, out button)) {
								return new InputMouseButtonControl (name, button, action);
						}
						KeyCode keyCode;
						if (KeyboardButtons.TryGetValue (name, out keyCode)) {
								return new InputKeyboardButtonControl (name, keyCode, action);
						}
                        MouseGesture mouseGesture;
                        if (MouseGestures.TryGetValue(name, out mouseGesture))
                        {
                            switch (mouseGesture)
                            {
                                case MouseGesture.Click:
                                    return new ClickGestureControl("Click", action);
                                case MouseGesture.DoubleClick:
                                    return new DoubleClickGestureControl("DoubleClick", action);
                                case MouseGesture.DoubleClickDrag:
                                    return new DoubleClickDragGestureControl("DoubleClickDrag", action);
                                case MouseGesture.LongClick:
                                    return new LongClickGestureControl("LongClick", action);
                                case MouseGesture.LongClickDrag:
                                    return new LongClickDragGestureControl("LongClickDrag", action);
                            }
                        }
                        TouchGesture touchGesture;
                        if (TouchGestures.TryGetValue(name, out touchGesture))
                        {
                            switch (touchGesture)
                            {
                                case TouchGesture.Touch:
                                    return new TouchGestureControl("Touch", action);
                                case TouchGesture.DoubleTouch:
                                    return new DoubleTouchGestureControl("DoubleTouch", action);
                                case TouchGesture.DoubleTouchDrag:
                                    return new DoubleTouchDragGestureControl("DoubleTouchDrag", action);
                                case TouchGesture.LongPress:
                                    return new LongPressGestureControl("LongPress", action);
                                case TouchGesture.LongPressDrag:
                                    return new LongPressDragGestureControl("LongPressDrag", action);
                                case TouchGesture.Pinch:
                                    return new PinchGestureControl("Pinch", action);
                            }
                        }
                        TouchAxis touchAxis;
                        if (TouchAxes.TryGetValue(name, out touchAxis))
                        {
                            switch (touchAxis)
                            {
                                case TouchAxis.Touch0:
                                    return new TouchAxisControl("Touch0", 0, TouchAxisControl.kOperation_Position,
                                        action);
                                case TouchAxis.Touch0Delta:
                                    return new TouchAxisControl("Touch0", 0, TouchAxisControl.kOperation_DeltaPosition,
                                        action);
                                case TouchAxis.Touch1:
                                    return new TouchAxisControl("Touch1", 1, TouchAxisControl.kOperation_Position,
                                        action);
                                case TouchAxis.Touch1Delta:
                                    return new TouchAxisControl("Touch1", 1, TouchAxisControl.kOperation_DeltaPosition,
                                        action);
                                case TouchAxis.Touch2:
                                    return new TouchAxisControl("Touch2", 2, TouchAxisControl.kOperation_Position,
                                        action);
                                case TouchAxis.Touch2Delta:
                                    return new TouchAxisControl("Touch2", 2, TouchAxisControl.kOperation_DeltaPosition,
                                        action);
                                case TouchAxis.Touch3:
                                    return new TouchAxisControl("Touch3", 3, TouchAxisControl.kOperation_Position,
                                        action);
                                case TouchAxis.Touch3Delta:
                                    return new TouchAxisControl("Touch3", 3, TouchAxisControl.kOperation_DeltaPosition,
                                        action);
                                case TouchAxis.Touch4:
                                    return new TouchAxisControl("Touch4", 4, TouchAxisControl.kOperation_Position,
                                        action);
                                case TouchAxis.Touch4Delta:
                                    return new TouchAxisControl("Touch4", 4, TouchAxisControl.kOperation_DeltaPosition,
                                        action);
                                case TouchAxis.Touch5:
                                    return new TouchAxisControl("Touch5", 5, TouchAxisControl.kOperation_Position,
                                        action);
                                case TouchAxis.Touch5Delta:
                                    return new TouchAxisControl("Touch5", 5, TouchAxisControl.kOperation_DeltaPosition,
                                        action);
                                case TouchAxis.Touch6:
                                    return new TouchAxisControl("Touch6", 6, TouchAxisControl.kOperation_Position,
                                        action);
                                case TouchAxis.Touch6Delta:
                                    return new TouchAxisControl("Touch6", 6, TouchAxisControl.kOperation_DeltaPosition,
                                        action);
                                case TouchAxis.Touch7:
                                    return new TouchAxisControl("Touch7", 7, TouchAxisControl.kOperation_Position,
                                        action);
                                case TouchAxis.Touch7Delta:
                                    return new TouchAxisControl("Touch7", 7, TouchAxisControl.kOperation_DeltaPosition,
                                        action);
                                case TouchAxis.Touch8:
                                    return new TouchAxisControl("Touch8", 8, TouchAxisControl.kOperation_Position,
                                        action);
                                case TouchAxis.Touch8Delta:
                                    return new TouchAxisControl("Touch8", 8, TouchAxisControl.kOperation_DeltaPosition,
                                        action);
                                case TouchAxis.Touch9:
                                    return new TouchAxisControl("Touch9", 9, TouchAxisControl.kOperation_Position,
                                        action);
                                case TouchAxis.Touch9Delta:
                                    return new TouchAxisControl("Touch9", 9, TouchAxisControl.kOperation_DeltaPosition,
                                        action);
                            }
                        }

                    

						return null;
				}

				protected InputControlBase (String name, InputAction action)
				{
						Name = name;
						Action = action;
				}

				public abstract void Capture ();

				public string Name { get; private set; }

				public string ActionName { get; set; }

                public int Priority { get; set; }

				public InputAction Action { get; private set; }
		}
}