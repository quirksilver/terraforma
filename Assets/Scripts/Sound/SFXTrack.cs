using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SFXTrack : MonoBehaviour
{

	List<AudioSource> sources = new List<AudioSource>();
	Queue<AudioSource> availableSources = new Queue<AudioSource>();

	Dictionary<string, SFX> FXByTrigger = new Dictionary<string, SFX>();

	public void Setup(List<SFX> _effects)
	{
		for (int i = 0 ; i < _effects.Count; i++)
		{
			FXByTrigger.Add(_effects[i].trigger, _effects[i]);
			//partsByClip.Add(_parts[i].clip, _parts[i]);
		}

		for (int i = 0; i < 5; i++)
		{
			GameObject sourceObj = new GameObject("Track" + name + "Source" + i);
			
			AudioSource source = sourceObj.AddComponent<AudioSource>();

			sources.Add(source);

			availableSources.Enqueue(source);
			
			sourceObj.transform.parent = transform;
			sourceObj.transform.localPosition = Vector3.zero;
		}
	}

	void Start()
	{


	}

	public void HandleEventString(string e)
	{

		if (FXByTrigger.ContainsKey(e))
		{
			Debug.Log("PLAY EFFECT for event" + e);
			PlayEffect(FXByTrigger[e]);
		}

	}


	private void PlayEffect(SFX effect)
	{
		AudioSource source;

		if (availableSources.Count == 0)
		{
			//no sources available, make a new one

			GameObject sourceObj = new GameObject("Track" + name + "Source" + sources.Count + 1);
			
			source = sourceObj.AddComponent<AudioSource>();
			
			sources.Add(source);
			
			sourceObj.transform.parent = transform;
			sourceObj.transform.localPosition = Vector3.zero;

		}
		else
		{
			source = availableSources.Dequeue();

		}

		if (effect.minLoops == 0 && effect.maxLoops == 0)
		{
			source.PlayOneShot(effect.clip);
			
			StartCoroutine(StopSourceAndAddToQueue(effect.clip.length + 0.1f, source));
		}
		else
		{
			
			source.clip = effect.clip;
			source.loop = true;
			
			float clipTime = Random.Range(effect.minLoops, effect.maxLoops) * effect.clip.length;
			
			source.SetScheduledEndTime(AudioSettings.dspTime + clipTime);
			
			StartCoroutine(StopSourceAndAddToQueue(clipTime + 0.1f, source));
				
		}
	}

	private IEnumerator StopSourceAndAddToQueue(float waitTime, AudioSource source)
	{
		yield return new WaitForSeconds(waitTime);

		source.Stop();
		source.loop = false;

		availableSources.Enqueue(source);
	}


}

