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

    public enum MouseGesture
    {
        None,

        // 1: Press, Lift
        Click,

        // 1: Press, Lift, Press, Lift
        DoubleClick,

        // 1: Press, Lift, Press, Drag
        DoubleClickDrag,

        // 1: Press, Wait, Lift
        LongClick,

        // 1: Press, Wait, Drag
        LongClickDrag,

    }

    sealed class ClickGestureControl : PressGestureControlBase
    {
        public ClickGestureControl(string name, InputAction action)
            : base(name, action)
        {
        }

        protected override void Supply()
        {
            _touchCount = UnityEngine.Input.GetMouseButton(0) ? 1 : 0;

            if (_touchCount == 1)
            {
                _touchPosition = UnityEngine.Input.mousePosition;
            }
        }
    }

    sealed class DoubleClickGestureControl : DoublePressGestureControlBase
    {
        public DoubleClickGestureControl(string name, InputAction action)
            : base(name, action)
        {
        }

        protected override void Supply()
        {
            _touchCount = UnityEngine.Input.GetMouseButton(0) ? 1 : 0;

            if (_touchCount == 1)
            {
                _touchPosition = UnityEngine.Input.mousePosition;
            }
        }
    }

    sealed class DoubleClickDragGestureControl : DoublePressDragGestureControlBase
    {
        public DoubleClickDragGestureControl(string name, InputAction action)
            : base(name, action)
        {
        }

        protected override void Supply()
        {
            _touchCount = UnityEngine.Input.GetMouseButton(0) ? 1 : 0;

            if (_touchCount == 1)
            {
                _touchPosition = UnityEngine.Input.mousePosition;
            }
        }
    }

    sealed class LongClickGestureControl : LongPressGestureControlBase
    {
        public LongClickGestureControl(string name, InputAction action)
            : base(name, action)
        {
        }

        protected override void Supply()
        {
            _touchCount = UnityEngine.Input.GetMouseButton(0) ? 1 : 0;

            if (_touchCount == 1)
            {
                _touchPosition = UnityEngine.Input.mousePosition;
            }
        }
    }

    sealed class LongClickDragGestureControl : LongPressDragGestureControlBase
    {
        public LongClickDragGestureControl(string name, InputAction action)
            : base(name, action)
        {
        }

        protected override void Supply()
        {
            _touchCount = UnityEngine.Input.GetMouseButton(0) ? 1 : 0;

            if (_touchCount == 1)
            {
                _touchPosition = UnityEngine.Input.mousePosition;
            }
        }
    }



}