using System;
using System.Collections;
using UnityEngine;

/// <summary>
/// Proxy object class. Contains  a reference to a gameObject and its sound script
/// Used by the sound system to find and play sounds without having to use UnityEngine.Transform.Find
/// or similar.
/// </summary>
	public class ProxyObject:ISound
	{
		private ISound script;
		private string soundName;
		private GameObject soundObject;
		
		public ProxyObject (GameObject _soundObject)
		{
			soundObject = _soundObject;
			soundName = soundObject.name;
		//GetComponent is only called once. After instantiation all sound script functions 
		//can be accessed through the ProxyObjecct
			script = soundObject.GetComponent(typeof(ISound)) as ISound;
		}
	
#region ISOUND METHODS
	
		public void playAtInterval(float minInterval, float maxInterval)
		{
			script.playAtInterval(minInterval, maxInterval);
		}
	
		public void playAtInterval(float interval)
		{
			script.playAtInterval(interval);
		}
		
		public void playAtInterval(float minInterval, float maxInterval, float volume)
		{
			script.playAtInterval(minInterval, maxInterval, volume);
		}

		public void play()
		{
			script.play();
		}
		
		public void play(float volume)
		{
			script.play(volume);
		}
	
	
		public void start()
		{
			script.start();
		}
		
		public void start(float volume)
		{
			script.start(volume);
		}
		
		public void stop()
		{
			script.stop();
		}
	
		public void fadeIn()
		{
			script.fadeIn();
		}
	
		public void fadeOut()
		{
			script.fadeOut();
		}
#endregion
	
		public string Name
		{
			get {return soundName;}
		}
	}
