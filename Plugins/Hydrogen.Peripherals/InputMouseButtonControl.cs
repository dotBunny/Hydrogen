#region Copyright Notice & License Information
//
// InputMouseButtonControl.cs
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

using UnityEngine;

namespace Hydrogen.Peripherals
{
		sealed class InputMouseButtonControl : InputControlBase
		{
				readonly int _button;
				bool _last, _now;
				float _timeBegan;

				public InputMouseButtonControl (string name, int button, InputAction action)
			: base (name, action)
				{
						_button = button;
				}

				public override void Capture ()
				{
						_last = _now;
						_now = UnityEngine.Input.GetMouseButton (_button);
			
						if (_now && !_last) {
								_timeBegan = Time.time;
								Action (InputEvent.Pressed, new Vector2(1.0f, 1.0f), 0.0f);
						} else if (!_now && _last) {
                            Action(InputEvent.Released, new Vector2(0.0f, 0.0f), Time.time - _timeBegan);
						} else if (_now && _last) {
								Action (InputEvent.Down, new Vector2(1.0f, 1.0f), Time.time - _timeBegan);
						}
				}
		}
}