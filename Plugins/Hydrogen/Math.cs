#region Copyright Notice & License Information
// 
// Math.cs
//  
// Author:
//   Matthew Davey <matthew.davey@dotbunny.com>
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

namespace Hydrogen
{
    /// <summary>
    /// Additional static functions, constants and classes used to extend existing Math support inside of Unity.
    /// </summary>
    public static class Math
    {
        
		public static float ClampAngle (float angle, float minimumAngle, float maximumAngle)
		{
			// Clamp that angle up
			return UnityEngine.Mathf.Clamp (NeutralizeAngle(angle), minimumAngle, maximumAngle);
		}
		public static float NeutralizeAngle(float angle)
		{
			// Neutralize really fucked up angles
			return angle % 360f;
		}
		
		public static float UnsignedAngle(float angle)
		{
			if ( angle < 0f )
			{
				if ( angle < -360f )
				{
					return (angle - (angle % 360f) * -1f) + (360f + (angle % 360f));
				}
				else
				{
					return 360f + angle;
				}
			}
			return angle;
		}
		
		public static float SignedAngle(float angle)
		{
			angle = NeutralizeAngle(angle);
			
			if ( angle > 180f ) 
			{
				return ((angle % 180f) * -1f);
			}
			else if ( angle < -180 )
			{
				return ((angle % 180) * -1f);
			}
			return angle;
		}
    }
}
