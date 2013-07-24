#region Copyright Notice & License Information
// 
// Components.cs
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

using System;

namespace Hydrogen
{

    /// <summary>
    /// Additional static functions, constants and classes used to extend existing Validation support inside of Unity.
    /// </summary>
    public static class Components
    {
		public static T GetComponentIfNull< T >( UnityEngine.Component that, T cachedT ) where T : UnityEngine.Component
		{
		    if( cachedT == null )
		    {
		        cachedT = (T)that.GetComponent( typeof( T ) );
		        if( cachedT == null )
		        {
		            UnityEngine.Debug.LogWarning( "GetComponent of type " + typeof( T ) + " failed on " + that.name, that );
		        }
		    }
		
		    return cachedT;
		}
		
		public static T GetComponentIfNull< T >( UnityEngine.GameObject that, T cachedT ) where T : UnityEngine.Component
		{
		    if( cachedT == null )
		    {
		        cachedT = (T)that.GetComponent( typeof( T ) );
		        if( cachedT == null )
		        {
		            UnityEngine.Debug.LogWarning( "GetComponent of type " + typeof( T ) + " failed on " + that.name, that );
		        }
		    }
		
		    return cachedT;
		}	
	}	
}