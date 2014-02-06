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

using System.Runtime.Remoting.Messaging;
using System.Threading;
using UnityEngine;

namespace Hydrogen.Peripherals
{

    public enum TouchGesture
    {
        None,

        // 1: Press, Lift
        Touch,
        
        // 1: Press, Lift, Press, Lift
        DoubleTouch,
        
        // 1: Press, Lift, Press, Drag
        DoubleTouchDrag,
        
        // 1: Press, Wait, Lift
        LongPress,
        
        // 1: Press, Wait, Drag
        LongPressDrag,

        // 2: Press, Wait, Drag
        Pinch,
    }

    sealed class TouchGestureControl : PressGestureControlBase
    {
        public TouchGestureControl(string name, InputAction action)
            : base(name, action)
        {
        }

        protected override void Supply()
        {
             _touchCount = UnityEngine.Input.touchCount;

            if (_touchCount == 1)
            {
                _touchPosition = UnityEngine.Input.GetTouch(0).position;
            }
        }
    }

    sealed class DoubleTouchGestureControl : DoublePressGestureControlBase
    {
        public DoubleTouchGestureControl(string name, InputAction action)
            : base(name, action)
        {
        }

        protected override void Supply()
        {
            _touchCount = UnityEngine.Input.touchCount;

            if (_touchCount == 1)
            {
                _touchPosition = UnityEngine.Input.GetTouch(0).position;
            }
        }
    }

    sealed class DoubleTouchDragGestureControl : DoublePressDragGestureControlBase
    {
        public DoubleTouchDragGestureControl(string name, InputAction action)
            : base(name, action)
        {
        }

        protected override void Supply()
        {
            _touchCount = UnityEngine.Input.touchCount;

            if (_touchCount == 1)
            {
                _touchPosition = UnityEngine.Input.GetTouch(0).position;
            }
        }
    }

    sealed class LongPressGestureControl : LongPressGestureControlBase
    {
        public LongPressGestureControl(string name, InputAction action)
            : base(name, action)
        {
        }

        protected override void Supply()
        {
            _touchCount = UnityEngine.Input.touchCount;

            if (_touchCount == 1)
            {
                _touchPosition = UnityEngine.Input.GetTouch(0).position;
            }
        }
    }

    sealed class LongPressDragGestureControl : LongPressDragGestureControlBase
    {
        public LongPressDragGestureControl(string name, InputAction action)
            : base(name, action)
        {
        }

        protected override void Supply()
        {
            _touchCount = UnityEngine.Input.touchCount;

            if (_touchCount == 1)
            {
                _touchPosition = UnityEngine.Input.GetTouch(0).position;
            }
        }
    }


    sealed class PinchGestureControl : PinchGestureControlBase
    {
        public PinchGestureControl(string name, InputAction action)
            : base(name, action)
        {
        }

        protected override void Supply()
        {
            _touchCount = UnityEngine.Input.touchCount;

            if (_touchCount == 2)
            {
                _touchPositionA = UnityEngine.Input.GetTouch(0).position;
                _touchPositionB = UnityEngine.Input.GetTouch(1).position;
            }
        }
    }


}