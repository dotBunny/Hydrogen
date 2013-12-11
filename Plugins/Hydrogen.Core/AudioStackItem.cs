#region Copyright Notice & License Information
//
// AudioStackItem.cs
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
using System.Collections;

namespace Hydrogen.Core
{
		[System.Serializable]
		public sealed class AudioStackItem
		{
				/// <summary>
				/// The associated AudioClip
				/// </summary>
				public AudioClip Clip;
				public string Key;
				public float StartVolume;
				public float TargetVolume = 1f;
				public float FadeInTime = 4.5f;
				public float FadeOutTime = 5.0f;
				public bool ShouldFade;
				public bool ShouldLoop;
				public bool ShouldPlay = true;
				public AudioSource Source;
				public AudioStack Stack;
				/// <summary>
				///  does not get removed, useful for menu clicks etc
				/// </summary>
				public bool Persistant;
				/// <summary>
				/// Should the AudioPoolItem be destroyed, freeing it's AudioSource when it's volume reaches 0
				/// </summary>
				public bool removeOnFinish;

				public AudioStackItem (AudioClip clip)
				{
						Clip = clip;
						Key = clip.name;
				}

				public AudioStackItem (AudioClip clip, string key)
				{
						Clip = clip;
						Key = key;
				}

				public void Process ()
				{
						if (Source.volume != TargetVolume) {
								if (ShouldFade) {
										if (Source.volume > TargetVolume) {
												Source.volume = 
												Mathf.Lerp (Source.volume, 
														TargetVolume,
														FadeOutTime * Time.deltaTime);
										} else {
												Source.volume = 
												Mathf.Lerp (Source.volume, 
														TargetVolume,
														FadeInTime * Time.deltaTime);
										}
								} else {
										Source.volume = TargetVolume;
								}
						}
						// Removing Finished Items
						if (!Persistant && ((Source.volume == 0f && removeOnFinish) || Source.time == Clip.length)) {

								Stack.Remove (this);
						}
				}
		}
}