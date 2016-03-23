
using System;
using System.Collections;
using UnityEngine;

	/// <summary>
	/// MonoBehaviour script that is attached to the SoundEffects Unity gameObject prefab
	/// Accesses and manipulates a Unity AudioSource
	/// </summary>
	public class SoundEffectScript:MonoBehaviour, ISound
	{
		//fields must be public in order for the values to carry across when
		//MonoBehaviour.Instantiate is called on an object with the script attached
		public AudioSource source;
		public AudioClip clip;
		public float killTime = 	10.0f; 
		//if it is not being used, the object invokes a destruction sequence that triggers
		//after killTime if the object is not called again.
	
#region ISOUND METHODS
	//playAtinterval methods, to be implemented for sound effects in the next version
		public void playAtInterval(float interval)
		{
			Debug.Log("SOUNDSYSTEM WARNING in SoundEffectScript: The function 'playAtInterval' can only currently be used with a SoundObject of type 'SoundSet'.");
		}
	
		public void playAtInterval(float minInterval, float maxInterval)
		{
			Debug.Log("SOUNDSYSTEM WARNING in SoundEffectScript: The function 'playAtInterval' can only currently be used with a SoundObject of type 'SoundSet'.");
		}
	
			public void playAtInterval(float minInterval, float maxInterval, float volume)
		{
			Debug.Log("SOUNDSYSTEM WARNING in SoundEffectScript: The function 'playAtInterval' can only currently be used with a SoundObject of type 'SoundSet'.");
		}
		
		public void play()
		{ 
			playSound();
		}
		//ISound method overloads to accommodate extra parameters such as volume have only been recently
		//implemented (for use in the Islands project), and will be refined in the next version
		public void play(float volume)
		{ 
			source.volume = volume;
			
			playSound();
		}
		
		private void playSound()
		{
			CancelInvoke("killSelf");

			if (!source.isPlaying)
			{
				source.loop = false;
				source.Play();
				Invoke("killSelf", killTime+clip.length);
			}
		}
	
		public void start()
		{	
			startSound();
		}
		
		public void start(float volume)
		{	
			source.volume = volume;
			startSound();
		}
		
		private void startSound()
		{
			//source.rolloffFactor  = 0.02f;
			
			CancelInvoke("killSelf");

			if (!source.isPlaying)
			{
				source.loop = true;
				source.Play();
			}
		}
		
		public void stop()
		{		
			if (source.isPlaying)
			{
				source.Stop();
				source.loop = false;
				Invoke("killSelf", killTime+clip.length);

			}
		}
	
		public void fadeIn()
		{		
			while (source.volume <= 0)
			{
				source.volume += Time.deltaTime;	
			}
		}
	
		public void fadeOut()
		{
			while (source.volume >= 0)
			{
				source.volume -= Time.deltaTime;	
			}
		}
#endregion
	
		/// <summary>
		/// checks if the source is null and gets it if it is
		/// not currently being used, as the value for AudioSource is carried across 
		/// when instantiated
		/// </summary>
		public void getSource()
		{
			if (source == null)
			{
				source = gameObject.GetComponent(typeof(AudioSource)) as AudioSource;
			}
		}
	/// <summary>
	/// self-destruct method, invoked ten seconds after the AudioSource is stopped
	/// </summary>
		public void killSelf()
		{
			//print("killSelf invoked on " + name);
			SoundSystem.killProxy(gameObject.name, gameObject.transform.parent.gameObject);
			Destroy(this.gameObject);
		}
	}