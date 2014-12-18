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
	private List<AudioSource> sourcesA  = new List<AudioSource>();
	private List<AudioSource> sourcesB = new List<AudioSource>();

	Dictionary<string, MusicPart> parts = new Dictionary<string, MusicPart>();

	private int sourceIndex = 0;
	private int partIndex = 0;

	private double lastCheckedTime = 0.0;

	//private bool init = false;

	public void Setup(List<MusicPart> _parts, string _name, int _simultaneousParts)
	{
		for (int i = 0 ; i < _parts.Count; i++)
		{
			parts.Add(_parts[i].trigger, _parts[i]);
		}

		name = _name;
		simultaneousParts = _simultaneousParts;

		for (int i = 0; i < _simultaneousParts*2; i++)
		{
			GameObject sourceObj = new GameObject("Track" + name + "Source" + i);

			AudioSource source = sourceObj.AddComponent<AudioSource>();

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

		if (partQueue.Count == 0) return;

		double currentTime = AudioSettings.dspTime;

		double checkTime = (currentParts.Count > 0) ? MusicPlayer.instance.GetNextEntry(partQueue.Peek().barsLength, currentParts[partIndex].barsLength) : MusicPlayer.instance.nextFourBarEntry;

		AudioSource oldSource;
		AudioSource newSource;

		//if we haven't done a check in this phrase
		if (Math.Floor(currentTime) < Math.Floor(checkTime) && Math.Floor(currentTime) > Math.Floor(lastCheckedTime))
		{

			Debug.Log("Checking times in track " + name);

			//Debug.Log("bars in current part" + currentParts[partIndex].barsLength);

			/*Debug.Log(lastCheckedTime);
			Debug.Log(checkTime);*/


			if (sourcesA[sourceIndex].clip == currentParts[partIndex].clip)
			{
				oldSource = sourcesA[sourceIndex];
				newSource = sourcesB[sourceIndex];
			}
			else if (sourcesB[sourceIndex].clip == currentParts[partIndex].clip)
			{
				oldSource = sourcesB[sourceIndex];
				newSource = sourcesA[sourceIndex];
			}
			else //if (currentParts.Count == 1)
			{
				newSource = sourcesA[sourceIndex];
				oldSource = null;
			}
			/*else 
			{
				return;
			}*/

			MusicPart newPart = partQueue.Dequeue();

			newSource.clip = newPart.clip;
			newSource.loop = newPart.isPersistent;
			newSource.PlayScheduled(checkTime);

			newPart.isPlaying = true;

			Debug.Log("Scheduling part " + newPart.name + "on track " + newSource.gameObject.name);


			if (oldSource)
			{
				if (currentParts[partIndex] != newPart)
					partQueue.Enqueue(currentParts[partIndex]);

				oldSource.SetScheduledEndTime(checkTime);
				currentParts[partIndex].isPlaying = false;

				sourceIndex = (sourceIndex + 1) % sourcesA.Count;
				partIndex = (partIndex + 1) % currentParts.Count;
			}

			lastCheckedTime = checkTime;
		}

		//if (currentTime + MusicPlayer.instance.bpm/60.0f * MusicPlayer.instance.barLength > sources[nextAvailableSourceIndex]
	}


	public void HandleEventString(string e)
	{

		if (parts.ContainsKey(e))
		{
			if (!MusicPlayer.instance.ready) MusicPlayer.instance.ready = true;

			AddPart(parts[e]);
		}
	}

	private void AddPart(MusicPart part)
	{


		currentParts.Add(part);
		partQueue.Enqueue(part);
	}
}

