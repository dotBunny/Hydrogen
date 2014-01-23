#region Copyright Notice & License Information
//
// Material.cs
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

namespace Hydrogen
{
		/// <summary>
		/// Additional static functions and constants used to extend existing Material support inside of Unity.
		/// </summary>
		public static class Material
		{
				public static int GetDataHashCode (this UnityEngine.Material material)
				{
						string longName = material.name.Replace (" (Instance)", "") +
						                  material.color.r +
						                  material.color.g +
						                  material.color.b +
						                  material.color.a; 

						if (material.mainTexture != null) {
								longName += material.mainTexture.name +
								material.mainTextureOffset +
								material.mainTextureScale;
						}

						if (material.shader != null) {
								longName += material.shader.name +
								material.shader.renderQueue;
						}
								
						// Send it back!
						return longName.GetHashCode ();
				}
		}
}