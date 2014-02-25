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

namespace Hydrogen.Core
{
		/// <summary>
		/// A class represention of all information needed for an AudioClip to be played via the AudioStack.
		/// </summary>
		[System.Serializable]
		public sealed class AudioStackItem
		{
				/// <summary>
				/// The associated AudioClip
				/// </summary>
				public AudioClip Clip;
				/// <summary>
				/// Key to be used by the AudioStack
				/// </summary>
				/// <remarks>
				/// Should be unique to the AudioClip's file name.
				/// </remarks>
				public string Key;
				/// <summary>
				/// /// Fade In Speed, used in a Lerp.
				/// </summary>
				public float FadeInSpeed = 4.5f;
				/// <summary>
				/// Fade Out Speed, used in a Lerp.
				/// </summary>
				public float FadeOutSpeed = 5.0f;
				/// <summary>
				/// Should the AudioSource fade between volume levels?
				/// </summary>
				public bool Fade;
				/// <summary>
				/// Should the AudioSource loop the clip?
				/// </summary>
				public bool Loop;
				/// <summary>
				/// The max volume for the item.
				/// </summary>
				public float MaxVolume = 1f;
				/// <summary>
				/// Do not remove the AudioItem when finished playing, useful for menu clicks etc
				/// </summary>
				public bool Persistant;
				/// <summary>
				/// Should the Audio Clip be played automatically upon load?
				/// </summary>
				public bool PlayOnLoad = true;
				/// <summary>
				/// The priority that this Audio Stack Item takes over all other sounds playing through the stack.
				/// </summary>
				public int Priority = 128;
				/// <summary>
				/// Should the AudioPoolItem be destroyed, freeing it's AudioSource when it's volume reaches 0 after fading
				/// </summary>
				public bool RemoveAfterFadeOut = true;
				/// <summary>
				/// The AudioSource associated to this AudioStackItem.
				/// </summary>
				/// <remarks>
				/// Shouldn't really be playing with this.
				/// </remarks>
				[System.NonSerialized]
				internal AudioSource Source;
				/// <summary>
				/// Reference to the parent stack.
				/// </summary>
				[System.NonSerialized]
				internal AudioStack Stack;
				/// <summary>
				/// The volume to use when the sound is first played.
				/// </summary>
				public float StartVolume = 1f;
				/// <summary>
				/// The volume which the AudioSource should gravitate towards.
				/// </summary>
				public float TargetVolume = 1f;

				/// <summary>
				/// Initializes a new instance of the <see cref="Hydrogen.Core.AudioStackItem"/> class.
				/// </summary>
				public AudioStackItem ()
				{
				}

				/// <summary>
				/// Initializes a new instance of the <see cref="Hydrogen.Core.AudioStackItem"/> class.
				/// </summary>
				/// <param name="clip">An AudioClip</param>
				public AudioStackItem (AudioClip clip)
				{
						Clip = clip;
						Key = clip.name;
				}

				/// <summary>
				/// Initializes a new instance of the <see cref="Hydrogen.Core.AudioStackItem"/> class.
				/// </summary>
				/// <param name="clip">An AudioClip.</param>
				/// <param name="loop">Should the AudioSource loop?</param>
				public AudioStackItem (AudioClip clip, bool loop)
				{
						Clip = clip;
						Key = clip.name;
						Loop = loop;
				}

				/// <summary>
				/// Initializes a new instance of the <see cref="Hydrogen.Core.AudioStackItem"/> class.
				/// </summary>
				/// <param name="clip">An AudioClip.</param>
				/// <param name="key">Designate Reference Key.</param>
				public AudioStackItem (AudioClip clip, string key)
				{
						Clip = clip;
						Key = key;
				}

				/// <summary>
				/// Initializes a new instance of the <see cref="Hydrogen.Core.AudioStackItem"/> class.
				/// </summary>
				/// <param name="clip">An AudioClip.</param>
				/// <param name="key">Designate Reference Key.</param>
				/// <param name="loop">Should the AudioSource loop?</param>
				public AudioStackItem (AudioClip clip, string key, bool loop)
				{
						Clip = clip;
						Key = key;
						Loop = loop;
				}

				/// <summary>
				/// Instruct the AudioSource to Pause playback.
				/// </summary>
				public void Pause ()
				{
						Source.Pause ();
				}

				/// <summary>
				/// Instruct the AudioSource to play the currently loaded clip from its currently set position.
				/// </summary>
				public void Play ()
				{
						if (Source.enabled)
								Source.Play ();
				}

				/// <summary>
				/// Process / Update our AudioStackItem
				/// </summary>
				public void Process ()
				{
						if (Source == null || Clip == null)
								return;

						if (TargetVolume > MaxVolume)
								TargetVolume = MaxVolume;

						if (Source.volume != TargetVolume) {
								if (Fade) {
										if (Source.volume > TargetVolume) {
												Source.volume = 
														Mathf.Lerp (Source.volume, TargetVolume, FadeOutSpeed * Time.deltaTime);
										} else {
												Source.volume = 
														Mathf.Lerp (Source.volume, TargetVolume, FadeInSpeed * Time.deltaTime);
										}
								} else {
										Source.volume = TargetVolume;
								}
						}

						// Automatically remove finished processes, but only if they are not marked persistant.
						// Not checking for Loop as if someone sets it's TargetVolume to 0 its meant to go away.
						if (!Persistant && ((Fade && RemoveAfterFadeOut && Source.volume < 0.0001f) || Source.time == Clip.length)) {

								Stack.Remove (this);
						}
				}

				/// <summary>
				/// Instruct the AudioSource to Restart playback, resetting the position to the start of the clip, and playing it.
				/// </summary>
				public void Restart ()
				{
						Source.Stop ();
						Source.Play ();
				}

				/// <summary>
				/// Instruct the AudioSource to Stop playback, resetting the position in the clip.
				/// </summary>
				public void Stop ()
				{
						Source.Stop ();
				}
		}
}