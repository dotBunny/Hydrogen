#region Copyright Notice & License Information
// 
// Input.cs
//  
// Author:
//	 Matthew Davey <matthew.davey@dotbunny.com>
//   Robin Southern <betajaen@ihoed.com>
//
// Copyright (C) 2013 dotBunny Inc. (http://www.dotbunny.com)
//
// Permission is hereby granted, free of charge, to any person obtaining a copy of
// this software and associated documentation files (the "Software"), to deal in
// the Software without restriction, including without limitation the rights to
// use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of
// the Software, and to permit persons to whom the Software is furnished to do so,
// subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS
// FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR
// COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER
// IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN
// CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
#endregion

using System;
using System.Collections.Generic;
using Hydrogen.Core;
using UnityEngine;

namespace Hydrogen.Peripherals
{
	public delegate void InputAction(InputEvent evt, float value, float time);

	public enum InputEvent
	{
		Pressed,
		Down,
		Released,
		ValueMoved,
		ValueSet
	}

	public class Input : UnityEngine.MonoBehaviour
	{

		private Dictionary<String, InputAction> _actions;
		private List<InputControlBase> _controls;

		void Awake()
		{
			if (_controls == null) _controls = new List<InputControlBase>();
			if (_actions == null) _actions = new Dictionary<String, InputAction>();
		}

		void Update()
		{
			foreach (InputControlBase control in _controls)
			{
				control.Capture();
			}
		}

		public bool AddAction(String name, InputAction action)
		{
			if (_actions.ContainsKey(name) == false)
			{
				_actions.Add(name, action);
				return true;
			}
			return false;
		}

		public bool RemoveAction(String name, bool alsoRemoveControls)
		{
			InputAction action = null;
			if (_actions.TryGetValue(name, out action))
			{
				if (alsoRemoveControls)
				{
					bool hasMore = true;
					while (hasMore)
					{
						hasMore = false;
						foreach (InputControlBase control in _controls)
						{
							if (control.Action == action)
							{
								_controls.Remove(control);
								hasMore = true;
								break;
							}
						}
					}
				}
				_actions.Remove(name);
				return true;
			}
			return false;
		}
		//TODO: make add control update as well
		public bool AddControl(String controlName, String actionName)
		{
			InputAction action = null;
			
			if (_actions.TryGetValue(actionName, out action) == false)
			{
				return false;
			}
			
			InputControlBase control = InputControlBase.CreateControl(controlName, action);
			
			if (control != null)
			{
				_controls.Add(control);
				return true;
			}
			else
			{
				return false;
			}
		}

		public void RemoveControl(String controlName)
		{
			bool hasMore = true;
			while (hasMore)
			{
				hasMore = false;
				foreach (InputControlBase control in _controls)
				{
					if (control.Name == controlName)
					{
						_controls.Remove(control);
						hasMore = true;
						break;
					}
				}
			}
		}
		//TODO make serialization
	}
}