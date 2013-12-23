﻿#region Copyright Notice & License Information
//
// AudioStack.cs
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
using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

namespace Hydrogen.Core
{
		/// <summary>
		/// A stack system for 2D audio sources.
		/// </summary>
		/// <remarks>This should NEVER be used for 3D audio sources.</remarks>
		public class AudioStack : MonoBehaviour
		{
				/// <summary>
				/// Minimum number of sources to have in total (Stack + Playing).
				/// </summary>
				public int MinimumSources = 10;
				/// <summary>
				/// Maximum number of sources to have in total (Stack + Playing).
				/// </summary>
				public int MaximumSources = 20;
				/// <summary>
				/// Should the stack look at the priority of the existing Audio Stack Items playing, and if the stack is
				/// full stop the lowest and use it's newly available source to play the new item.
				/// </summary>
				/// <remarks>This will ignore any playing sources that are set to loop.</remarks>
				public bool UsePriorities = false;
				/// <summary>
				/// A Stack of Audio Sources
				/// </summary>
				Stack _audioSources;
				/// <summary>
				/// All currently loaded sounds are stored in here, with references to what Audio Source they are using.
				/// </summary>
				Dictionary<string, AudioStackItem> _loadedItems;
				/// <summary>
				/// The GameObject used to hold all of the AudioSources that are created.
				/// </summary>
				GameObject _poolObject;

				/// <summary>
				/// The number of currently used AudioSources.
				/// </summary>
				/// <value>Used AudioSource Count.</value>
				public int SourcesCount {
						get { return _loadedItems.Count; }
				}

				public Dictionary<string, AudioStackItem> LoadedItems {
						get { return _loadedItems; }
				}

				public bool IsLoaded (AudioClip clip)
				{
						return clip != null && _loadedItems.ContainsKey (clip.name);

				}

				/// <summary>
				/// Is the AudioStackItem currently loaded?
				/// </summary>
				/// <returns><c>true</c> if the AudioStackItem is loaded; otherwise, <c>false</c>.</returns>
				/// <param name="item">Target AudioStackItem.</param>
				public bool IsLoaded (AudioStackItem item)
				{
						return item != null && IsLoaded (item.Key);
				}

				public bool IsLoaded (string key)
				{
						return key != null && _loadedItems.ContainsKey (key);
				}

				public bool IsPlaying (AudioClip clip)
				{	
						return IsLoaded (clip.name) && _loadedItems [clip.name].Source.isPlaying;
				}

				public bool IsPlaying (AudioStackItem item)
				{
						return item != null && item.Source != null && item.Source.isPlaying;
				}

				public bool IsPlaying (string key)
				{
						return key != null && IsLoaded (key) && _loadedItems [key].Source.isPlaying;
				}

				public string Add (AudioClip clip)
				{
						return Add (new AudioStackItem (clip), false);
				}

				public string Add (AudioClip clip, bool forceDuplicate)
				{
						return Add (new AudioStackItem (clip), forceDuplicate);
				}

				public string Add (AudioStackItem item)
				{
						return Add (item, false);
				}

				public string Add (AudioStackItem item, bool createDuplicate)
				{
						if (IsLoaded (item.Key) && !createDuplicate) {

								// Update any settings we need too
								_loadedItems [item.Key].Fade = item.Fade;
								_loadedItems [item.Key].Loop = item.Loop;
								_loadedItems [item.Key].PlayOnLoad = item.PlayOnLoad;

								_loadedItems [item.Key].FadeInSpeed = item.FadeInSpeed;
								_loadedItems [item.Key].FadeOutSpeed = item.FadeOutSpeed;
								_loadedItems [item.Key].Persistant = item.Persistant;
								_loadedItems [item.Key].TargetVolume = item.TargetVolume;
								_loadedItems [item.Key].StartVolume = item.StartVolume;
								_loadedItems [item.Key].RemoveAfterFadeOut = item.RemoveAfterFadeOut;
								_loadedItems [item.Key].Priority = item.Priority;

								if (_loadedItems [item.Key].Source != null) {
										_loadedItems [item.Key].Source.priority = item.Priority;
								}

						} else {

								// If we do not have enough sources, we should make one, if we can.
								if (_audioSources.Count <= 0 && _loadedItems.Count < MaximumSources) {
										var source = _poolObject.AddComponent<AudioSource> ();

										// Make sure we dont have any fun little hickups
										source.playOnAwake = false;

										_audioSources.Push (source);
								}

								// Let's rock this playing of one
								if (_audioSources.Count > 0) {

										// Create a slightly different key if need be
										// You should never be starting the exact same sound twice in the same frame, 
										// thus we can just use the this simple addition
										if (createDuplicate && IsLoaded (item.Key)) {
												item.Key = item.Key + Time.time + item.GetHashCode ();
										}

										// Use this stack for callback
										// TODO: Don't like this
										item.Stack = this;

										item.Source = _audioSources.Pop () as AudioSource;

// Update Our Source
								
										item.Source.clip = item.Clip;
										item.Source.volume = item.StartVolume;
										item.Source.loop = item.Loop;
										item.Source.priority = item.Priority;

										// Auto Play Stuff
										if (item.PlayOnLoad) {
												item.Source.Play ();
										}

										_loadedItems.Add (item.Key, item);

								} else if (_audioSources.Count <= 0 && UsePriorities) {

										// Find our sucker to replace.
										//TODO: Maybe we can search for how long a track has already played
										AudioStackItem replaceItem = item;
										foreach (string s in _loadedItems.Keys.ToList()) {
												if (_loadedItems [s].Priority < replaceItem.Priority &&
												    !_loadedItems [s].Loop) {
														replaceItem = _loadedItems [s];
												}
										}

										// Did any thing qualify?
										if (replaceItem != item) {
												Remove (replaceItem);
												Add (item);
										} else {
												Debug.Log ("[H] No available Audio Sources from the AudioStack to use, even when prioritized.");
										}
								} else {
										Debug.Log ("[H] No available Audio Sources from the AudioStack to use.");
								}
						}

						return item.Key;
				}

				public void Remove (AudioClip clip)
				{
						if (IsLoaded (clip)) {
								Remove (_loadedItems [clip.name]);
						}
				}

				public void Remove (AudioStackItem item)
				{
						if (!IsLoaded (item))
								return;

						// Make sure the item is stopped
						item.Source.Stop ();

						// Remove the clip association
						item.Source.clip = null;

						// Add the source back to the stack
						_audioSources.Push (item.Source);

						// Remove our item definition
						_loadedItems.Remove (item.Key);

						// Free Item
						item = null;
				}

				/// <summary>
				/// Unity's Awake Event
				/// </summary>
				protected virtual void Awake ()
				{
						// Create our stack
						_audioSources = new Stack (MinimumSources);
						// Create our holder GameObject
						_poolObject = new GameObject ();
						// Create our lookup
						_loadedItems = new Dictionary<string, AudioStackItem> ();

						// Assign our new friend as our child
						_poolObject.transform.parent = gameObject.transform;
						_poolObject.name = "Audio Pool";

						// Create the AudioSources and associate them to their respective arrays.
						for (int x = 0; x < MinimumSources; x++) {

								var source = _poolObject.AddComponent<AudioSource> ();

								// Make sure we dont have any fun little hickups
								source.playOnAwake = false;

								_audioSources.Push (source);
						}
				}

				/// <summary>
				/// Unity's Update Event
				/// </summary>
				/// <remarks>>
				/// This neeeds to be executed to handle processing the Audio Sources correctly.
				/// </remarks>
				protected virtual void Update ()
				{
						foreach (string s in _loadedItems.Keys.ToList()) {
								_loadedItems [s].Process ();
						}
				}
		}
}