#region Copyright Notice & License Information
//
// HydrogenStartup.cs
//
// Author:
//       Matthew Davey <matthew.davey@dotbunny.com>
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

using System;
using UnityEngine;
using UnityEditor;

[InitializeOnLoad]
public class HydrogenStartup 
{
	static HydrogenStartup()
	{	
		string lastChecked = EditorPrefs.GetString("HydrogenFramework_LastChecked");

		// Have we ever checked for an update?
		if ( string.IsNullOrEmpty(lastChecked) ) 
		{
			CheckForUpdate.CheckUpdate();
		} 
		else 
		{
			// Parse out times to determine duration
			DateTime lastTime = DateTime.Parse(lastChecked);
			TimeSpan span = DateTime.Now - lastTime;

			// Every 48 hours
			if ( span.TotalHours > 48 ) 
			{
				CheckForUpdate.CheckUpdate();
			}
		}
	}
}