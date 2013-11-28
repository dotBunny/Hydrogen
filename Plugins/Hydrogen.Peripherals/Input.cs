#region Copyright Notice & License Information
// 
// Input.cs
//  
// Author:
//	 Matthew Davey <matthew.davey@dotbunny.com>
//   Robin Southern <betajaen@ihoed.com>
//
// Copyright (c) 2013 dotBunny Inc. (http://www.dotbunny.com)
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
using System.Collections;
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

		public bool AddControl(String controlName, String actionName)
		{
			InputAction action = null;
			
			if (_actions.TryGetValue(actionName, out action) == false)
			{
				return false;
			}


			InputControlBase control = InputControlBase.CreateControl(controlName, action);
			control.ActionName = actionName;
			
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

		/// <summary>
		/// Clears all controls.
		/// </summary>
		public void ClearControls()
		{
			_controls.Clear();
		}

		/// <summary>
		/// Gets the controls.
		/// </summary>
		/// <returns>The controls as a ist<KeyValuePair<string, string>></returns>
		public List<KeyValuePair<string, string>> GetControls()
		{
			List<KeyValuePair<string, string>> controlList = new List<KeyValuePair<string, string>>();

			for ( int x = 0; x < _controls.Count; x++ )
			{
				controlList.Add( new KeyValuePair<string, string>(_controls[x].ActionName, _controls[x].Name ));
			}
		
			return controlList;
		}

		/// <summary>
		/// Set the controls from a List<KeyValuePair<string, string>>
		/// </summary>
		/// <remarks>
		/// This is useful for loading saved settings, specifically using Hydrogen.Serialization.INI 
		/// </remarks>
		/// <returns><c>true</c>, if controls were set, <c>false</c> otherwise.</returns>
		/// <param name="controlScheme">Control Scheme.</param>
		public bool SetControls(List<KeyValuePair<string, string>> controlScheme)
		{
			bool check = true;

			for(int x = 0; x < controlScheme.Count; x++ )
			{
				// Reverse because we're assuming this came from a serialized version of the config
				if ( !AddControl(controlScheme[x].Value, controlScheme[x].Key) ) check = false;
			}
		
			return check;
		}
	}
}