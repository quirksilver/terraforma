using UnityEngine;
using System;
using System.Collections;
using System.IO;

/// <summary>
/// Interface for all key sound operations. Implemented by ProxyObject and all sound scripts
/// Enables the sound system to call generic sound play and stop functions without casting specifically by
/// type of sound object (eg set, effect) 
/// </summary>
	public interface ISound
	{
		void play();
		void play(float volume);
	
		void start();
		void start(float volume);
	
		void stop();
	
		//play at interval is currently only implemented in SoundSet
		void playAtInterval(float minInterval, float maxInterval);
		void playAtInterval(float minInterval, float maxInterval, float volume);
		void playAtInterval(float interval);
	
		//fadein and fadeout are not currently implemented in a useable fashion anywhere in the sound system
		void fadeIn();
		void fadeOut();
	
	}