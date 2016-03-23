
using System;
using UnityEngine;
/// <summary>
/// Currently SoundSets can only select random clips from an array - there is not yet a function to 
/// specify which sound to play, or to sort or iterate through the clips in the set. All this coming in
/// the next version!
/// </summary>
	public class SoundSetScript:MonoBehaviour, ISound
	{
		public AudioSource source;
		public AudioClip [] clips;	
		
		private float minInterval;
		private float maxInterval;
	
		private float intervalTimer;
	
		private bool playingOnInterval;
	
		public float killTime = 10.0f;
		
		void Update()
		{
			intervalCheck();
		}	
	
		private void intervalCheck()
		{
			//getSource();
		
			//interval logic
			if (playingOnInterval && intervalTimer <=0)
				{
					if (source.isPlaying)
					{
						source.loop = false;
						source.clip = clips[UnityEngine.Random.Range(0,clips.Length)];
						source.Play();
						// if the timer is up, play a sound and reset the interval
						//it will do this indefinitely until stopped, or another play function is
						//called during the interval
						if (minInterval == maxInterval)
						{
							intervalTimer = minInterval;
						}
						
						else
						{
							intervalTimer = UnityEngine.Random.Range(minInterval, maxInterval);
						}
					}
				}
		
			if (!source.isPlaying)
			{
				intervalTimer -= Time.deltaTime;
			}
		}
	
#region ISOUND METHODS
		public void play()
		{
			playSound();
		}
		
		public void play(float volume)
		{
			source.volume = volume;
			
			playSound();
		}
		
	//plays a random sound
		private void playSound()
		{
			CancelInvoke("killSelf");
			
		//currently, a second play call will be ignored if the sound is already playing
		//this may get changed in future iterations depending on feedback
			if (!source.isPlaying)
			{
				source.loop = false;
				source.clip = clips[UnityEngine.Random.Range(0,clips.Length)];
				source.Play();
				Invoke("killSelf", killTime + source.clip.length);
			}
		}
	
		/// <summary>
		/// Continually plays a random sound at the specified interval, until destroyed or stopped
		/// </summary>
		public void playAtInterval(float _interval)
		{
			CancelInvoke("killSelf");

			minInterval = _interval;
			maxInterval = _interval;
			intervalTimer = _interval;
			playingOnInterval = true;
		}
		
		public void playAtInterval(float _minInterval, float _maxInterval)
		{
			CancelInvoke("killSelf");

			minInterval = _minInterval;
			maxInterval = _maxInterval;
			intervalTimer = UnityEngine.Random.Range(minInterval, maxInterval);
			playingOnInterval = true;
		}
		
		public void playAtInterval(float _minInterval, float _maxInterval,  float volume)
		{
			source.volume = volume;

			CancelInvoke("killSelf");

			minInterval = _minInterval;
			maxInterval = _maxInterval;
			intervalTimer = UnityEngine.Random.Range(minInterval, maxInterval);
			playingOnInterval = true;
		}
	
		public void start()
		{			
			CancelInvoke("killSelf");
		
			source.loop = true;
			source.clip = clips[UnityEngine.Random.Range(0,clips.Length)];
			source.Play();
			//Debug.Log("SOUNDSYSTEM WARNING in SoundSetScript: Function 'start' cannot be used with a SoundObject of type 'SoundSet'");
		}
		
		public void start(float volume)
		{
				source.volume = volume;
				CancelInvoke("killSelf");
				source.loop = true;
				source.clip = clips[UnityEngine.Random.Range(0,clips.Length)];
				source.Play();
			//Debug.Log("SOUNDSYSTEM WARNING in SoundSetScript: Function 'start' cannot be used with a SoundObject of type 'SoundSet'");
		}
		
		public void stop()
		{
			Invoke("killSelf", killTime);
			playingOnInterval = false;
			source.Stop();
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
	
		public void getSource()
		{
			if (source == null)
			{
				source = gameObject.GetComponent(typeof(AudioSource)) as AudioSource;
			}
		}
	
		public AudioClip [] Clips
		{
			set {clips = value;}
		}
	
		public void killSelf()
		{
			SoundSystem.killProxy(gameObject.name, gameObject.transform.parent.gameObject);
			Destroy(this.gameObject);
		}
	}
