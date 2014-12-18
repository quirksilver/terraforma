
using System;
using UnityEngine;
/// <summary>
/// Class for paired sound objects. Used to play sounds that have an initialisation followed by an indefinite loop
/// (eg a car starting and then the motor idling until it is stopped)
/// Contains one gameObject w/ Audiosource for each clip to allow the sounds to overlap, as Unity does not handle seamless
/// transitions between clips particularly well. Both objects are parented to the prefab gameObject
/// </summary>

	public class SoundPair:SoundObject
	{
		private AudioClip initClip;
		private AudioClip loopingClip;
		
		public SoundPair (AudioClip _initClip, AudioClip _loopingClip, string _name, float _killTime)
		{
			initClip = _initClip;
			loopingClip = _loopingClip;
		
			name = _name;
			
			prefab = new GameObject(name);
			GameObject initObject= new GameObject("init");
			GameObject loopObject = new GameObject("loop");
			
			initObject.transform.parent = prefab.transform;
			loopObject.transform.parent = prefab.transform;
			
			SoundPairScript script = prefab.AddComponent<SoundPairScript>();
			script.InitClip = initClip;
			script.LoopingClip = loopingClip;
			
			script.initObj = initObject;
			script.loopObj = loopObject;
			
			script.initSource = initObject.AddComponent<AudioSource>();
			script.loopSource = loopObject.AddComponent<AudioSource>();
			
			script.killTime = _killTime;
			
			script.getClips();
		
			prefab.active = false;
			initObject.active = false;
			loopObject.active = false;
		}
	}
