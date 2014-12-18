
using System;
using UnityEngine;
	
	/// <summary>
	/// Single sound effect SoundObject
	/// Constructs a Unity gameObject that represents a single sound effect and is used as a prefab
	/// The prefab can then be instantiated when- and where-ever required
	/// </summary>
	public class SoundEffect: SoundObject
	{
		public AudioClip clip;
	
		public SoundEffect (AudioClip _clip, string _name,float  _killTime)
		{
			clip = _clip;
			name = _name;
				
			prefab = new GameObject(name);
			
			AudioSource source = prefab.AddComponent<AudioSource>();
			source.clip = clip;
			source.playOnAwake = false;
		
			SoundEffectScript script = prefab.AddComponent<SoundEffectScript>();
			script.clip = clip;
			script.getSource();
		
			script.killTime = _killTime;
		
			prefab.active = false; 
		}
	}