#region Copyright Notice & License Information
// 
// Components.cs
//  
// Author:
//   Matthew Davey <matthew.davey@dotbunny.com>
//	 Robin Southern <betajaen@ihoed.com>
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
	/// Additional static functions, constants and classes used to extend existing Component support inside of Unity.
	/// </summary>
    public static class Components
    {
		/// <summary>
		/// Get a component reference, checking if its already referenced.
		/// </summary>
		/// <returns>The desired component.</returns>
		/// <param name="targetObject">The object to look on for the component.</param>
		/// <param name="cachedReference">Possible pre-existing reference to component.</param>
		/// <typeparam name="T">Object Type.</typeparam>
		/// <example>
		/// private AudioSource _localAudioSource = null;
		/// public AudioSource LocalAudioSource
		/// {
		///		get
		///		{
		///			_localAudioSource = Hydrogen.Components.GetComponentIfNull( this, _localAudioSource );
		///			return _localAudioSource;
		///		}
		///	}
		/// </example>
		public static T GetComponentIfNull< T >( this UnityEngine.Component targetObject, T cachedReference ) where T : UnityEngine.Component
		{
			if( cachedReference == null )
		    {
				cachedReference = (T)targetObject.GetComponent( typeof( T ) );
				if( cachedReference == null )
		        {
		            UnityEngine.Debug.LogWarning( "GetComponent of type " + typeof( T ) + " failed on " + targetObject.name, targetObject );
		        }
		    }
		
			return cachedReference;
		}

		/// <summary>
		/// Get a component reference, checking if its already referenced.
		/// </summary>
		/// <returns>The desired component.</returns>
		/// <param name="targetObject">The object to look on for the component.</param>
		/// <param name="cachedReference">Possible pre-existing reference to component.</param>
		/// <typeparam name="T">Object Type.</typeparam>
		/// <example>
		/// private AudioSource _localAudioSource = null;
		/// public AudioSource LocalAudioSource
		/// {
		///		get
		///		{
		///			_localAudioSource = Hydrogen.Components.GetComponentIfNull( this, _localAudioSource );
		///			return _localAudioSource;
		///		}
		///	}
		/// </example>
		public static T GetComponentIfNull< T >( this UnityEngine.GameObject targetObject, T cachedReference ) where T : UnityEngine.Component
		{
			if( cachedReference == null )
		    {
				cachedReference = (T)targetObject.GetComponent( typeof( T ) );
				if( cachedReference == null )
		        {
					UnityEngine.Debug.LogWarning( "GetComponent of type " + typeof( T ) + " failed on " + targetObject.name, targetObject );
		        }
		    }
		
			return cachedReference;
		}

		/// <summary>
		/// Get a component reference from a gameObjects parents.
		/// </summary>
		/// <returns>The desired component.</returns>
		/// <param name="targetObject">The root object to look on for the component (backwards).</param>
		/// <param name="cachedReference">Possible pre-existing reference to component.</param>
		/// <typeparam name="T">Object Type.</typeparam>
		/// <example>
		/// private AudioSource _localAudioSource = null;
		/// public AudioSource LocalAudioSource
		/// {
		///		get
		///		{
		///			_localAudioSource = Hydrogen.Components.GetComponentInParents( this, _localAudioSource );
		///			return _localAudioSource;
		///		}
		///	}
		/// </example>
		public static T GetComponentInParents< T >( this UnityEngine.GameObject targetObject, T cachedReference) where T : UnityEngine.Component
		{
			if( cachedReference == null )
			{
				UnityEngine.Transform p = targetObject.transform.parent;

				while (p != null)
				{
					T t = (T)targetObject.GetComponent( typeof ( T ) );
					
					// Return as soon as we find the component
					if (t != null) 
					{
						cachedReference = t;
						return cachedReference;
					}
					
					// Next parent to search
					p = p.parent;
				}

				if( cachedReference == null )
				{
					UnityEngine.Debug.LogWarning( "GetComponentInParents of type " + typeof( T ) + " failed on " + targetObject.name, targetObject );
				}
			}
			
			return cachedReference;
		}
	}	
}