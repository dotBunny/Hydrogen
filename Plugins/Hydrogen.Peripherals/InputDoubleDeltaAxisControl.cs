#region Copyright Notice & License Information
//
// InputDeltaAxisControl.cs
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
    sealed class InputDoubleDeltaAxisControl : InputControlBase
    {
        readonly string _axis1, _axis2;

        public InputDoubleDeltaAxisControl(string name, string axisName1, string axisName2, InputAction action)
            : base(name, action)
        {
            _axis1 = axisName1;
            _axis2 = axisName2;
        }

        public override void Capture()
        {
            float value1 = UnityEngine.Input.GetAxis(_axis1);
            float value2 = UnityEngine.Input.GetAxis(_axis2);

            if (!UnityEngine.Mathf.Approximately(value1, 0.0f) || !UnityEngine.Mathf.Approximately(value2, 0.0f))
            {
                Action(InputEvent.ValueMoved, new Vector2(value1, value2), 0.0f);
            }
        }
    }
}