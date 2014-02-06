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

    public enum TouchAxis
    {
        Touch0,
        Touch0Delta,
        Touch1,
        Touch1Delta,
        Touch2,
        Touch2Delta,
        Touch3,
        Touch3Delta,
        Touch4,
        Touch4Delta,
        Touch5,
        Touch5Delta,
        Touch6,
        Touch6Delta,
        Touch7,
        Touch7Delta,
        Touch8,
        Touch8Delta,
        Touch9,
        Touch9Delta
    }

    sealed class TouchAxisControl : InputControlBase
    {

        public const int kOperation_Position = 0;
        public const int kOperation_DeltaPosition = 1;


        readonly int _touchIndex;
        readonly int _operation;
        
        Vector2 _lastValue;
        bool _hasLastValue;

        public TouchAxisControl(string name, int touchIndex, int operation, InputAction action)
            : base(name, action)
        {
            _touchIndex = touchIndex;
            _operation = operation;
        }

        public override void Capture()
        {
            if (UnityEngine.Input.touchCount > _touchIndex)
            {
                UnityEngine.Touch touch = UnityEngine.Input.touches[_touchIndex];
                Vector2 value;
                bool fireEvent = false;

                if (_operation == kOperation_Position)
                {
                    value = touch.position;
                    
                }
                else
                {
                    value = touch.deltaPosition;
                }
                
                if (_hasLastValue)
                {
                    fireEvent = true;
                    _hasLastValue = true;
                }
                else
                {
                    fireEvent = (UnityEngine.Mathf.Approximately(_lastValue.x, value.y) == false) || (UnityEngine.Mathf.Approximately(_lastValue.y, value.y) == false);
                }

                _lastValue = value;

                if (fireEvent)
                {
                    Action(InputEvent.ValueMoved, value, touch.deltaTime);
                }

            }
        }
    }

}