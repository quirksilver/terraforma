using UnityEngine;
using System.Collections;

public class MusicPart
{

	public AudioClip clip;
	public string name;
	public string track;
	public string trigger;
	public int barsLength;
	public double lastEntry;

	public bool isPersistent = true;

	public bool isPlaying = false;

	public bool flaggedForStop = false;

	public MusicPart(string _clip, string _name, string _trigger, int _barsLength, bool _isPersistent)
	{
		clip = Resources.Load("Sound/"+ _clip) as AudioClip;

		Debug.Log(_clip);
		Debug.Log(clip);

		name = _name;
		trigger = _trigger;
		barsLength = _barsLength;
		isPersistent = _isPersistent;
		
	}
}

