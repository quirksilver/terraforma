using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class MusicTrack : MonoBehaviour
{
	private string name;
	private int simultaneousParts;

	//private List<MusicPart> currentParts;

	private List<MusicPart> currentParts = new List<MusicPart>();
	private Queue<MusicPart> partQueue = new Queue<MusicPart>();

	private Queue<MusicPart> oldestParts = new Queue<MusicPart>();

	private List<AudioSource> sourcesA  = new List<AudioSource>();
	private List<AudioSource> sourcesB = new List<AudioSource>();

	Dictionary<string, MusicPart> partsByTrigger = new Dictionary<string, MusicPart>();
	Dictionary<AudioClip, MusicPart> partsByClip = new Dictionary<AudioClip, MusicPart>();

	private int sourceIndex = 0;
	private int partIndex = 0;

	private double lastCheckedTime = 0.0;

	private bool flaggedForStop = false;

	private bool muted = false;

	//private bool init = false;

	public void Setup(List<MusicPart> _parts, string _name, int _simultaneousParts)
	{
		for (int i = 0 ; i < _parts.Count; i++)
		{
			partsByTrigger.Add(_parts[i].trigger, _parts[i]);
			//partsByClip.Add(_parts[i].clip, _parts[i]);
		}

		name = _name;
		simultaneousParts = _simultaneousParts;

		for (int i = 0; i < _simultaneousParts*2; i++)
		{
			GameObject sourceObj = new GameObject("Track" + name + "Source" + i);

			AudioSource source = sourceObj.AddComponent<AudioSource>();

			source.playOnAwake = false;

			if (i%2 == 0)
			{
				sourcesA.Add(source);
			}
			else
			{
				sourcesB.Add(source);
			}

			sourceObj.transform.parent = transform;
			sourceObj.transform.localPosition = Vector3.zero;
		}
	}

		// Use this for initialization
	void Start ()
	{

	}
	
	// Update is called once per frame
	void Update ()
	{

		if (partQueue.Count == 0 || flaggedForStop) return;

		double currentTime = AudioSettings.dspTime;

		MusicPart oldPart = null;

		if (oldestParts.Count > 0 && currentParts.Count >= simultaneousParts) 
		{
			oldPart = oldestParts.Peek();
		}

		double checkTime;

		if (oldPart != null) 
		{
			checkTime = MusicPlayer.instance.GetNextEntry(oldPart);
		}
		else
		{
			checkTime = MusicPlayer.instance.nextFourBarEntry;
		}

		AudioSource oldSource = null;
		AudioSource newSource = null;

		//if we haven't done a check in this phrase
		if (Math.Floor(currentTime) < Math.Floor(checkTime) && Math.Floor(currentTime) > Math.Floor(lastCheckedTime))
		{

			if (oldPart != null)
			{
				Debug.Log("DEQUEUING OLDEST PART " + oldPart.name);

				oldestParts.Dequeue();
			}



			Debug.Log("Checking times in track " + name);

			//Debug.Log("bars in current part" + currentParts[partIndex].barsLength);

			/*Debug.Log(lastCheckedTime);
			Debug.Log(checkTime);*/

			if (oldPart == null)
			{
				newSource = sourcesA[sourceIndex];
				oldSource = null;


			}
			else
			{
				for (int i = 0; i < sourcesA.Count; i++)
				{
					if (sourcesA[i].clip == oldPart.clip)
					{
						oldSource = sourcesA[i];
						newSource = sourcesB[i];
					}
					else if (sourcesB[i].clip == oldPart.clip)
					{
						oldSource = sourcesB[i];
						newSource = sourcesA[i];
					}
				}

				if (oldSource == null || newSource == null)
				{
					oldSource = sourcesA[sourceIndex];
					newSource = sourcesB[sourceIndex];
				}
			}
			/*else if (currentParts.Count >= simultaneousParts)
			{

				if (sourcesA[sourceIndex].clip == oldPart.clip)
				{
					oldSource = sourcesA[sourceIndex];
					newSource = sourcesB[sourceIndex];
				}

				else
				{
					oldSource = sourcesB[sourceIndex];
					newSource = sourcesA[sourceIndex];
				}

				/*if (sourcesA[sourceIndex].clip == currentParts[partIndex].clip)
				{
					oldSource = sourcesA[sourceIndex];
					newSource = sourcesB[sourceIndex];
				}
				else //if (sourcesB[sourceIndex].clip == currentParts[partIndex].clip)
				{
					oldSource = sourcesB[sourceIndex];
					newSource = sourcesA[sourceIndex];
				}
			}
			else
			{
				oldSource = sourcesA[sourceIndex];
				newSource = sourcesB[sourceIndex];
			}*/


			MusicPart newPart = partQueue.Dequeue();

			newSource.clip = newPart.clip;
			newSource.loop = newPart.isPersistent;// && currentParts.Count < simultaneousParts);
			newSource.PlayScheduled(checkTime);

			newPart.lastEntry = checkTime;

			newPart.isPlaying = true;

			if (currentParts.IndexOf(newPart) == -1) currentParts.Add(newPart);

			oldestParts.Enqueue(newPart);

			Debug.Log("Scheduling part " + newPart.name + "on track " + newSource.gameObject.name + "at time " + checkTime);


			//Debug.Log("Current audio time is " + AudioSettings.dspTime);

			//Debug.Log("source is playing " + newSource.isPlaying);



			if (oldPart != null)
			{
				Debug.Log("TAKING OVER FROM OLD PART " + oldPart.name);

				partQueue.Enqueue(oldPart);

				oldSource.SetScheduledEndTime(checkTime);
				oldPart.isPlaying = false;
				oldSource.loop = false;

			}
			else
			{
				Debug.Log("OLD PART IS NULL");
			}

			sourceIndex = (sourceIndex + 1) % sourcesA.Count;
			partIndex = (partIndex + 1) % currentParts.Count;

			lastCheckedTime = checkTime;
		}

		//if (currentTime + MusicPlayer.instance.bpm/60.0f * MusicPlayer.instance.barLength > sources[nextAvailableSourceIndex]
	}

	public IEnumerator FadeDown()
	{
		float volume =  1.0f;
		while  (volume > 0.0f)
		{
			volume -= 1.0f/(float)MusicPlayer.instance.fourBars * Time.deltaTime;

			SetVolume(volume);

			yield return 1;

		}

		SetVolume(0.0f);
		StopAll();
		SetVolume(1.0f);

		flaggedForStop = false;
	}

	public void SetVolume(float volume)
	{
		if (!flaggedForStop) return;

		for (int i = 0; i < sourcesA.Count; i++)
		{
			sourcesA[i].volume = volume;
			sourcesB[i].volume = volume;
				
		}
	}

	public void StopAll()
	{
		if (!flaggedForStop) return;

		for (int i = 0; i < sourcesA.Count; i++)
		{
				sourcesA[i].Stop();
				sourcesB[i].Stop();
		}
	}

	public void Reset()
	{
		partQueue.Clear();
		currentParts.Clear();
		oldestParts.Clear();

		/*for (int i = 0; i < triggers.Length; i++)
		{
			partsByTrigger.ContainsKey(triggers[i])
			{
				currentParts.Remove(partsByTrigger[triggers[i]]);
			}
		}*/

	}

	public void Clear(string nameCheck)
	{
		Debug.Log(name.IndexOf(nameCheck), this);
		muted = (name.IndexOf(nameCheck) == -1);

		Debug.Log(muted, this);

		if (!muted) return;
		//if (name.IndexOf(nameCheck) == -1) muted = true;
		
		partQueue.Clear();
		oldestParts.Clear();
		currentParts.Clear();
		partIndex = 0;
		sourceIndex = 0;

		flaggedForStop = false;

		StartCoroutine("FadeDown");

		for (int i = 0; i < sourcesA.Count; i++)
		{
			if (sourcesA[i].isPlaying || sourcesB[i].isPlaying)
			{
				sourcesA[i].loop = false;
				sourcesB[i].loop = false;

				flaggedForStop = true;
			}
				/*
			if (sourcesA[i].isPlaying)
			{
				partsByClip[sourcesA[i].clip].flaggedForClear = true;
			}

			if (sourcesB[i].isPlaying)
			{
				partsByClip[sourcesB[i].clip].flaggedForClear = true;
			}*/
		}
	}

	public void FadeUp()
	{

	}

	public void HandleEventString(string e)
	{
		//Debug.Log("Handle event " + e + " part is muted " + muted , this);

		if (muted) return;

		if (partsByTrigger.ContainsKey(e))
		{
			if (!MusicPlayer.instance.ready) MusicPlayer.instance.ready = true;



			AddPart(partsByTrigger[e]);
		}
	}

	private void AddPart(MusicPart part)
	{

		Debug.Log("Add part " + part.name, this);
		//currentParts.Add(part);

		if (currentParts.IndexOf(part) != -1) return;

		Debug.Log(part, this);

		partQueue.Enqueue(part);
	}
}

