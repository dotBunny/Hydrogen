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