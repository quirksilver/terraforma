using System;
using System.Collections;
using UnityEngine;

/// <summary>
/// MonoBehaviour script that attaches to the gameObject prefab in SoundPair
/// </summary>
	public class SoundPairScript:MonoBehaviour, ISound
	{
		public AudioSource initSource;
		public AudioSource loopSource;
		public AudioClip initClip;
		public AudioClip loopingClip;
		
		public GameObject initObj;
		public GameObject loopObj;
	
		public float killTime = 10.0f;
	
		public void Update()
		{	
			//check if the initialisation clip has finished and play the looping clip
			if (initSource.time >= initClip.length-0.06) 
			//-0.06 to ensure they overlap slightly and there is no gap
			{
				loopSource.Play();
			}
		}
	
	#region ISOUND METHODS
		//playAtInterval also to be implemented for SoundPair in a future iteration 
		public void playAtInterval(float interval)
		{
			Debug.Log("SOUNDSYSTEM WARNING in SoundPairScript: The function 'playAtInterval' can currently only be used with a SoundObject of type 'SoundSet'.");
		}
	
		public void playAtInterval(float minInterval, float maxInterval)
		{
			Debug.Log("SOUNDSYSTEM WARNING in SoundPairScript: The function 'playAtInterval' can currently only be used with a SoundObject of type 'SoundSet'.");
		}
		
		public void playAtInterval(float minInterval, float maxInterval, float volume)
		{
			Debug.Log("SOUNDSYSTEM WARNING in SoundPairScript: The function 'playAtInterval' can currently only be used with a SoundObject of type 'SoundSet'.");
		}
		
		public void play()
		{
			playSound();
		}
		
		public void play(float volume)
		{
			initSource.volume = volume;
			loopSource.volume = volume;
			
			playSound();
		}

		private void playSound()
		{
			CancelInvoke("killSelf");
			if (!initSource.isPlaying && !loopSource.isPlaying)
			{
				initSource.Play();
				Invoke("killSelf", killTime + initClip.length + loopingClip.length);
			}
		}
	
		public void start()
		{
			startSound();
		}
		
		public void start(float volume)
		{
			initSource.volume = volume;
			loopSource.volume = volume;
			startSound();
		}
		
		private void startSound()
		{
			CancelInvoke("killSelf");

			if (!initSource.isPlaying && !loopSource.isPlaying)
			{
				initSource.Play();
			}
		}
		
		public void stop()
		{
		
			if (initSource.isPlaying || loopSource.isPlaying)
			{
				initSource.Stop();
				loopSource.Stop();
						Invoke("killSelf", killTime + initClip.length + loopingClip.length);

			}	
		}
	
		public void fadeIn()
		{
		
			//~ while (source.volume <= 0)
			//~ {
				//~ source.volume += Time.deltaTime;	
			//~ }
		}
	
		public void fadeOut()
		{

			//~ 
		}
	
#endregion
	/// <summary>
	/// gets the AudioSources if null, as in SoundEffectScript
	/// </summary>
		public void getSource()
		{
			if (initSource == null)
			{
				initSource = gameObject.GetComponent(typeof(AudioSource)) as AudioSource;
			}
			
			if (loopSource == null)
			{
				loopSource = gameObject.GetComponent(typeof(AudioSource)) as AudioSource;
			}
		}
		
	/// <summary>
	/// sets the clips to the audio sources and their default values
	/// </summary>
		public void getClips()
		{
			loopSource.clip = loopingClip;
			loopSource.loop = true;
			loopSource.playOnAwake = false;
			initSource.clip = initClip;
			initSource.loop = false;
			initSource.playOnAwake = false;

		}
	
		public AudioClip InitClip
		{
			set{initClip = value;}
		}	
	
		public AudioClip LoopingClip
		{
			set{loopingClip = value;}
		}	
	
	/// <summary>
	/// Self-destruct function as in SoundEFfectScript
	/// </summary>
		public void killSelf()
		{
			//calls back to the SoundSystem to remove the proxyObject before destruction - otherwise
		//the system may continue to try to access a proxy for a gameObject that no longer exists
			SoundSystem.killProxy(gameObject.name, gameObject.transform.parent.gameObject);
			Destroy(this.gameObject);
		}
	}