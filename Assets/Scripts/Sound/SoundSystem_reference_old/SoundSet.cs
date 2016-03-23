
using System;
using UnityEngine;

/// <summary>
/// SoundSet class for objects that contain sets of multiple related sounds
/// (eg. a variety of explosions for when a rocket hits an object 
/// or a number of short dialogue lines to spoken by a character.
/// 
/// </summary>
	public class SoundSet:SoundObject
	{
		private AudioClip [] clips;

		public SoundSet (AudioClip [] _clips, string _name, float _killTime)
		{
		
			clips = _clips;
			name = _name;
			
			prefab = new GameObject(name);
			prefab.AddComponent<AudioSource>();
			
			SoundSetScript script = prefab.AddComponent<SoundSetScript>();
			script.Clips = clips;
			script.getSource();
		
			script.killTime = _killTime;	
		
			prefab.active = false;
		}
	}