#region Copyright Notice & License Information
//
// NavMesh.cs
//
// Author:
//       Matthew Davey <matthew.davey@dotbunny.com>
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

using UnityEngine;

namespace Hydrogen
{
		/// <summary>
		/// Additional static functions used to extend existing NavMesh support inside of Unity.
		/// </summary>
		public static class NavMesh
		{
				/// <summary>
				/// Determine if NavMeshAgent is at its destination.
				/// </summary>
				/// <returns><c>true</c>, if at destination was agented, <c>false</c> otherwise.</returns>
				/// <param name="agent">The target NavMeshAgent.</param>
				public static bool AgentAtDestination (this NavMeshAgent agent)
				{
						float dist = agent.remainingDistance; 
						if (!float.IsPositiveInfinity (dist) && agent.pathStatus == NavMeshPathStatus.PathComplete &&
						    agent.remainingDistance == 0f) {
								return true;
						}
						return false;
				}

				/// <summary>
				/// Find a random position on the NavMesh.
				/// </summary>
				/// <returns>The random position.</returns>
				/// <param name="basePosition">A base position to use as an offset from.</param>
				/// <param name="distance">The maximum allowed distance from the basePosition that the random position can be.</param>
				/// <param name="allowedMask">Any limitations on what NavMeshLayer the position can be on.</param>
				public static Vector3 RandomPosition (Vector3 basePosition, float distance, int allowedMask = 1)
				{
						NavMeshHit _hit;

						return UnityEngine.NavMesh.SamplePosition ((Random.insideUnitSphere * distance) + basePosition, 
								out _hit, distance, allowedMask) ? _hit.position : basePosition;
				}
		}
}