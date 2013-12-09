#region Copyright Notice & License Information
//
// Layers.cs
//
// Author:
//       Bob Berkebile
//       Matthew Davey <matthew.davey@dotbunny.com>
//
// Copyright (c) 2012 PixelPlacement (http://pixelplacement.com/)
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

using UnityEngine;

namespace Hydrogen
{
		/// <summary>
		/// Additional static functions used to extend existing Layers support inside of Unity.
		/// </summary>
		public static class Layers
		{
				/// <summary>
				/// Does the layerMask contain the specified layer?
				/// </summary>
				/// <returns><c>true</c>, if the layer is contained, <c>false</c> otherwise.</returns>
				/// <param name="layerMask">The LayerMask to poll</param>
				/// <param name="targetLayer">The layer to check.</param>
				public static bool ContainsLayer (this LayerMask layerMask, int targetLayer)
				{
						return (layerMask.value & 1 << targetLayer) != 0;
				}

				/// <summary>
				/// Create a layer mask of everything but these layers.
				/// </summary>
				/// <returns>A layer mask of everything but the passed layers.</returns>
				/// <param name="layers">Array of Layer IDs.</param>
				public static int EverythingBut (params int[] layers)
				{
						return ~OnlyIncluding (layers);
				}

				/// <summary>
				/// Checks if a GameObject is in a LayerMask.
				/// </summary>
				/// <param name="obj">GameObject to test</param>
				/// <param name="layerMask">LayerMask with all the layers to test against</param>
				/// <returns>True if in any of the layers in the LayerMask</returns>
				public static bool IsInLayerMask (this GameObject obj, LayerMask layerMask)
				{
						// Convert the object's layer to a bitfield for comparison
						int objLayerMask = (1 << obj.layer);
						return (layerMask.value & objLayerMask) > 0;
				}

				/// <summary>
				/// Create a layer mask only including these layers.
				/// </summary>
				/// <returns>A layer mask including only the passed layers.</returns>
				/// <param name="layers">Array of Layer IDs</param>
				public static int OnlyIncluding (params int[] layers)
				{
						int mask = 0;
						foreach (int item in layers) {
								mask |= 1 << item;
						}
						return mask;
				}
		}
}
