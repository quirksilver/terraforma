using UnityEngine;
using System.Collections;

public class SFX
{

	public AudioClip clip;
	public string name;
	public string trigger;
	public bool fadeInOut;
	public int minLoops, maxLoops;

	public SFX( string _clip, string _name, string _trigger, bool _fadeInOut, int _minLoops, int _maxLoops)
	{

		clip = Resources.Load("Sound/"+ _clip) as AudioClip;

		Debug.Log(clip);

		name = _name;
		trigger = _trigger;
		fadeInOut = _fadeInOut;
		minLoops = _minLoops;
		maxLoops = _maxLoops;

	}

}

