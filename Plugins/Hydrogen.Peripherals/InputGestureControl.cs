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

    public enum Gesture
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

    abstract class GestureControlBase : InputControlBase
    {
        // How long is considered a wait
        protected const float kWaitTimeSeconds = 0.5f;

        // How long is the maximum length of a double touch
        protected const float kDoubleTouchSeconds = kWaitTimeSeconds * 0.5f;

        // How many pixels of travel is considered a drag
        protected const float kMinDragDistancePixels = 20.0f;
        protected const float kMinDragDistancePixelsSquared = kMinDragDistancePixels * kMinDragDistancePixels;

        protected GestureControlBase(string name, InputAction action) : base(name, action)
        {
        }
    }

    sealed class TouchGestureControl : GestureControlBase
    {
        private int _mode;

        private const int kMode_WaitingForConditions = 0;
        private const int kMode_WaitingForTimeout = 1;
        private const int kMode_Released = 2;
        private const int kMode_Cancelling = 3;


        private Vector2 _pressPoint, _releasePoint, _currentPoint;
        private float _pressTime, _releaseTime, _currentTime;
        private int _touchCount;
        private Touch _touch;

        public TouchGestureControl(string name, InputAction action)
            : base(name, action)
        {
        }

        public override void Capture()
        {
            _touchCount = UnityEngine.Input.touchCount;

            if (_touchCount == 1)
            {
                _touch = UnityEngine.Input.GetTouch(0);
            }

            do
            {
            } while (Handle());
        }

        bool Handle()
        {

            switch (_mode)
            {
                case kMode_WaitingForConditions:
                {
                    if (_touchCount == 1)
                    {
                        _pressPoint = _touch.position;
                        _currentPoint = _pressPoint;
                        _pressTime = Time.time;
                        _mode = kMode_WaitingForTimeout;
                        //Debug.Log("T.A");
                    }
                }
                return false;
                case kMode_WaitingForTimeout:
                {
                    
                    // Unexpected: Reelease time is longer than needed to be.
                    _currentTime = Time.time - _pressTime;
                    if (_currentTime > kWaitTimeSeconds)
                    {
                        _mode = kMode_Cancelling;
                        //Debug.Log("T.B");
                        return true;
                    }

                    // Expected: Released during an allowed time frame, and minimum movement.
                    if (_touchCount != 1)
                    {
                        // Unexpected: Release time is shorter than expected.
                        if (_currentTime < Time.fixedDeltaTime)
                        {
                            _mode = kMode_Cancelling;
                            //Debug.Log("T.C");
                            return true;
                        }

                        _releasePoint = _currentPoint;
                        _releaseTime = _currentTime;
                        _mode = kMode_Released;
                        //Debug.Log("T.D");
                        return true;
                    }

                    // Unexpected: Touch is moving longer than it needs to be.
                    _currentPoint = _touch.position;
                    float currentMovementSquared = (_pressPoint - _currentPoint).sqrMagnitude;
                    if (currentMovementSquared > kMinDragDistancePixelsSquared)
                    {
                        _mode = kMode_Cancelling;
                        //Debug.Log("T.E");
                        return true;
                    }

                }
                return false;
                case kMode_Released:
                {
                    Action(InputEvent.ValueSet, _releasePoint, _releaseTime);
                    _mode = kMode_WaitingForConditions;
                    //Debug.Log("T.F");
                }
                return false;
                case kMode_Cancelling:
                {
                    if (_touchCount == 0)
                    {
                        _mode = kMode_WaitingForConditions;
                    }
                }
                return false;
            }
            return false;
        }
    }

    sealed class DoubleTouchGestureControl : GestureControlBase
    {
        int _counter;

        public DoubleTouchGestureControl(string name, InputAction action)
            : base(name, action)
        {
        }

        public override void Capture()
        {
        }
    }

    sealed class DoubleTouchDragGestureControl : GestureControlBase
    {

        public DoubleTouchDragGestureControl(string name, InputAction action)
            : base(name, action)
        {
        }

        public override void Capture()
        {
        }
    }

    sealed class LongPressGestureControl : GestureControlBase
    {
         private int _mode;

        private const int kMode_WaitingForConditions = 0;
        private const int kMode_WaitingForThreshold = 1;
        private const int kMode_Released = 2;
        private const int kMode_Cancelling = 3;


        private Vector2 _pressPoint, _releasePoint, _currentPoint;
        private float _pressTime, _releaseTime, _currentTime;
        private int _touchCount;
        private Touch _touch;

        public LongPressGestureControl(string name, InputAction action)
            : base(name, action)
        {
        }

        public override void Capture()
        {
            _touchCount = UnityEngine.Input.touchCount;

            if (_touchCount == 1)
            {
                _touch = UnityEngine.Input.GetTouch(0);
            }

            do
            {
            } while (Handle());
        }

        bool Handle()
        {

            switch (_mode)
            {
                case kMode_WaitingForConditions:
                {
                    if (_touchCount == 1)
                    {
                        _pressPoint = _touch.position;
                        _currentPoint = _pressPoint;
                        _pressTime = Time.time;
                        _mode = kMode_WaitingForThreshold;
                        //Debug.Log("LP.A");
                    }
                }
                return false;
                case kMode_WaitingForThreshold:
                {

                    _currentTime = Time.time - _pressTime;

                    // Expected: Released during an allowed time frame, and minimum movement.
                    if (_touchCount != 1)
                    {
                        // Unexpected: Release time is shorter than expected.
                        if (_currentTime < kWaitTimeSeconds)
                        {
                            _mode = kMode_Cancelling;
                            //Debug.Log("LP.B");
                            return true;
                        }

                        _releasePoint = _currentPoint;
                        _releaseTime = _currentTime;
                        _mode = kMode_Released;
                        //Debug.Log("LP.C");
                        return true;
                    }

                    // Unexpected: Touch is moving longer than it needs to be.
                    _currentPoint = _touch.position;
                    float currentMovementSquared = (_pressPoint - _currentPoint).sqrMagnitude;
                    if (currentMovementSquared > kMinDragDistancePixelsSquared)
                    {
                        _mode = kMode_Cancelling;
                        //Debug.Log("LP.D");
                        return true;
                    }

                }
                return false;
                case kMode_Released:
                {
                    Action(InputEvent.ValueSet, _releasePoint, _releaseTime);
                    _mode = kMode_WaitingForConditions;
                    //Debug.Log("LP.E");
                }
                return false;
                case kMode_Cancelling:
                {
                    if (_touchCount == 0)
                    {
                        _mode = kMode_WaitingForConditions;
                    }
                }
                return false;
            }
            return false;
        }
    }

    sealed class LongPressDragGestureControl : GestureControlBase
    {
        private int _mode;

        private const int kMode_WaitingForConditions = 0;
        private const int kMode_WaitingForThreshold = 1;
        private const int kMode_Dragging = 2;
        private const int kMode_Cancelling = 3;


        private Vector2 _pressPoint, _releasePoint, _lastPoint, _currentPoint, _dragOffset, _dragDelta;
        private float _pressTime, _releaseTime, _currentTime;
        private int _touchCount;
        private Touch _touch;

        public LongPressDragGestureControl(string name, InputAction action)
            : base(name, action)
        {
        }

        public override void Capture()
        {
            _touchCount = UnityEngine.Input.touchCount;

            if (_touchCount == 1)
            {
                _touch = UnityEngine.Input.GetTouch(0);
            }

            do
            {
            } while (Handle());
        }

        bool Handle()
        {

            switch (_mode)
            {
                case kMode_WaitingForConditions:
                {
                    if (_touchCount == 1)
                    {
                        _pressPoint = _touch.position;
                        _currentPoint = _pressPoint;
                        _pressTime = Time.time;
                        _mode = kMode_WaitingForThreshold;
                        //Debug.Log("LP.Drag.A");
                    }
                }
                return false;
                case kMode_WaitingForThreshold:
                {

                    _currentTime = Time.time - _pressTime;

                    // Expected: Released during an allowed time frame, and minimum movement.
                    if (_touchCount != 1)
                    {
                        _mode = kMode_Cancelling;
                        //Debug.Log("LP.Drag.B");
                        return true;
                    }

                    // Unexpected: Touch is moving longer than it needs to be.
                    _currentPoint = _touch.position;
                    float currentMovementSquared = (_pressPoint - _currentPoint).sqrMagnitude;
                    if (currentMovementSquared > kMinDragDistancePixelsSquared)
                    {
                        _mode = kMode_Dragging;
                        _lastPoint = _currentPoint = _touch.position;
                        //Debug.Log("LP.Drag.C");
                        Action(InputEvent.Pressed, _currentPoint, _currentTime);
                        return true;
                    }

                }
                return false;
                case kMode_Dragging:
                {
                    if (_touchCount != 1)
                    {
                        _mode = kMode_Cancelling;
                        //Debug.Log("LP.Drag.D");
                        Action(InputEvent.Released, _currentPoint, _currentTime);
                        return true;
                    }
                    _currentTime = Time.time - _pressTime;

                    _lastPoint = _currentPoint;
                    _currentPoint = _touch.position;
                    _dragDelta = (_currentPoint - _lastPoint);
                    _dragOffset = _currentPoint - _pressPoint;

                    Action(InputEvent.ValueMoved, _dragDelta, _currentTime);
                    Action(InputEvent.ValueSet, _dragOffset, _currentTime);
                    //Debug.Log("LP.Drag.E");
                }
                return false;
                case kMode_Cancelling:
                {
                    if (_touchCount == 0)
                    {
                        _mode = kMode_WaitingForConditions;
                    }
                }
                return false;
            }
            return false;
        }
    }


    sealed class PinchGestureControl : GestureControlBase
    {
        public PinchGestureControl(string name, InputAction action)
            : base(name, action)
        {
        }

        public override void Capture()
        {
        }
    }


}