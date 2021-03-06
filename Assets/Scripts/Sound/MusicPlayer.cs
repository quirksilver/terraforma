using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System.IO;

public class MusicPlayer : MonoSingleton<MusicPlayer>
{
	//public String XMLFilePath = "Sound/Music";

	private float bpm = 60.0f;
	[HideInInspector]
	public double nextFourBarEntry { private set; get; }
	[HideInInspector]
	public double nextEightBarEntry { private set; get; }
	[HideInInspector]
	public double nextTwelveBarEntry { private set; get; }

	public double fourBars;
	private double eightBars;
	private double twelveBars;
	private double oneBar;

	private int beatsInBar = 4;

	public bool playing = false;
	public bool ready = false;
		
	Dictionary<string, MusicTrack> tracks = new Dictionary<string, MusicTrack>();

	public const string GO_TO_MAP = "gotoMap";
	public const string  GO_TO_LEVEL = "gotoLevel";

	private SFXTrack sfxTrack;

	public double nextBeat;

	public TextAsset xmlFile;


	// Use this for initialization
	void Awake ()
	{
		xmlFile = Resources.Load<TextAsset>("Music");

		loadMusic(xmlFile);


		oneBar = beatsInBar * bpm/60.0f;
		fourBars = beatsInBar * bpm/60.0f * 4.0f;
		eightBars = beatsInBar * bpm/60.0f * 8.0f;
		twelveBars = beatsInBar * bpm/60.0f * 12.0f;
	}

	public void Setup()
	{
		nextFourBarEntry = AudioSettings.dspTime + 2.0f;
		nextEightBarEntry = AudioSettings.dspTime + 2.0f;
		nextTwelveBarEntry = AudioSettings.dspTime + 2.0f;
		nextBeat = AudioSettings.dspTime + 2.0f;

		//Debug.Log(xmlFile);

		ready = true;
		playing = true;

		Debug.Log("SETUP MUSIC PLAYER");
	}

	// Update is called once per frame
	void Update ()
	{

		if (!playing) return;

		double currentTime = AudioSettings.dspTime;

		if (currentTime > nextBeat)
		{
			nextBeat += 2.0f;
			//Debug.Log("BEAT");
		}

		if (currentTime > nextFourBarEntry)
		{
			nextFourBarEntry += fourBars;
		}

		if (currentTime > nextEightBarEntry)
		{
			nextEightBarEntry += eightBars;
		}

		if (currentTime > nextTwelveBarEntry)
		{
			nextTwelveBarEntry += twelveBars;
		}

	}


	public void ReceiveEvent(string e)
	{
		switch (e)
		{
		case GO_TO_MAP:
			ChangeToMapMusic();
			break;

		case GO_TO_LEVEL:
			LoadMusicForLevel(Map.instance.GetLevel());
			break;

		/*case "RESET":
			Reset(Map.instance.GetLevel());
			break;*/
	
		default:

			sfxTrack.HandleEventString(e);

			foreach (KeyValuePair<string, MusicTrack> track in tracks)
			{
				track.Value.HandleEventString(e);
			}
			break;
		}
	}

	/*public void Reset(Level level)
	{
		foreach (KeyValuePair<string, MusicTrack> track in tracks)
		{
			track.Value.Reset();
		}
		
		for (int i = 0; i < level.buildings.Count; i++)
		{
			ReceiveEvent(level.buildings[i].eventName);
		}
	}*/

	public void LoadMusicForLevel(Level level)
	{
		foreach (KeyValuePair<string, MusicTrack> track in tracks)
		{
			track.Value.Clear("Level");
			track.Value.HandleEventString(GO_TO_LEVEL);
		}

		for (int i = 0; i < level.buildings.Count; i++)
		{
			ReceiveEvent(level.buildings[i].eventName);
		}
	}

	public void ChangeToMapMusic()
	{
		Debug.Log("MUSIC PLAYER SWITCH TO MAP");

		foreach (KeyValuePair<string, MusicTrack> track in tracks)
		{
			track.Value.Clear("World");
			track.Value.HandleEventString(GO_TO_MAP);
		}
	}

	/*public double GetNextEntry(int barLength)
	{


		switch (barLength)
		{
		case 8:
			//Debug.Log("returning 8 bar entry");
			return nextEightBarEntry;
			break;

		case 12:
			//Debug.Log("returning 12 bar entry");
			return nextTwelveBarEntry;
			break;

		case 4:
		default:
			//Debug.Log("returning 4 bar entry");
			return nextFourBarEntry;
			break;
		}
	}*/

	public double GetNextEntry(MusicPart lastPart)
	{

		if (lastPart.lastEntry == -1.0D) return nextFourBarEntry;

		double nextEntry = lastPart.lastEntry + lastPart.barsLength * oneBar;

		while (nextEntry <= AudioSettings.dspTime)
		{
			nextEntry += (lastPart.barsLength*oneBar);
		}

		return nextEntry;

		//double entry = nextFourBarEntry;
		//double timeToAdd = fourBars;

		/*switch (newPartLength)
		{
		case 8:
			//Debug.Log("returning 8 bar entry");
			entry = nextEightBarEntry;
			timeToAdd = eightBars;
			break;
			
		case 12:
			//Debug.Log("returning 12 bar entry");
			entry = nextTwelveBarEntry;
			timeToAdd = twelveBars;
			break;
		}*/

	/*	if (lastPartLength > newPartLength)
		{
			entry += timeToAdd * (lastPartLength - newPartLength)/beatsInBar;
		}

		return entry;*/
	}
		
	#region XML PARSING/SOUND SETUP METHODS
	//This code is adapted from Michael Barnes' XML Level Parser for the in-development game Migration
	
	//XML template:	
	
	/*<SOUNDSCAPE name="name">

    <SOUNDEFFECT name ="name" clip = "clipname.mp3">
	</SOUNDEFFECT>

	<SOUNDSET name = "name" numClips = "0">
		<CLIP name = "name" clip = "clipname.mp3">
		…
	</SOUNDSET>

	<SOUNDPAIR name = "name">
		<INITCLIP name = "name" clip = "clipname.mp3">
		<LOOPCLIP name = "name" clip = "clipname.mp3">
	</SOUNDPAIR>

	…

</SOUNDSCAPE>*/
	
	
	/// <summary>
	/// Opens the SoundSystem_names file and attempts to read the Soundscape definition
	/// </summary>
	private static void loadMusic( TextAsset xmlFile )
	{
		Debug.Log(xmlFile);

		XmlTextReader reader = new XmlTextReader ( new StringReader(xmlFile.text) );
		reader.WhitespaceHandling = WhitespaceHandling.None; // ignore whitespace
		
		try
		{
			// Create a soundScape from the definition.
			readMusic ( reader );
			reader.Close ();
		}
		catch ( Exception e )
		{
			reader.Close ();
			throw new Exception ( String.Format ( "Error reading music definition: {0}", e.Message ), e );
		}
	}
	
	/// <summary>
	/// Reads the XML Soundscape definition and creates the SoundObjects defined within.
	/// SoundObjects can be defined in any order within the SoundScape
	/// </summary>
	private static void readMusic( XmlReader reader )
	{
		reader.Read ();
		
		// UnityEngine.Debug.Log ( "Reading begins..." );
		
		
		// <SOUNDSCAPE name = "name">
		if ( !reader.IsStartElement ( "MUSIC" ) ) throw new Exception ( "Expected <MUSIC> Element" );
		
		//string soundscapeName = ReadStringAttribute ( reader, "name", "No Name" );
		
		reader.Read ();
		
		//UnityEngine.Debug.Log ( String.Format ( "Read SOUNDSCAPE {0}", soundscapeName  ) );
		
		/*
    	  	<SOUNDEFFECT name ="name" clip = "clipname.mp3">
		</SOUNDEFFECT>
      */
		while (true)
		{

			if ( reader.IsStartElement( "BPM" ) )
			{
				instance.bpm = reader.ReadElementContentAsFloat();
			}

			else if (reader.IsStartElement( "BAR" ) )
			{
				instance.beatsInBar = reader.ReadElementContentAsInt();
			}

			else if ( reader.IsStartElement ( "TRACK" ) )
			{
				readTrack ( reader );
			}

			else if (reader.IsStartElement ( "SFX" ) )
			{
				readSFX ( reader ) ;
			}

			
			/*<SOUNDPAIR name = "name">
				<INITCLIP name = "name" clip = "clipname.mp3">
				<LOOPCLIP name = "name" clip = "clipname.mp3">
			</SOUNDPAIR>*/
			/*
			else if (reader.IsStartElement( "SOUNDPAIR"))
			{
				readSoundPair( reader );
			}
			
			/*<SOUNDSET name = "name" numClips = "number">
			<CLIP name = "name" clip = "clipname.mp3">
			…
			</SOUNDSET>
			*/

			/*else if (reader.IsStartElement( "SOUNDSET" ))
			{
				readSoundSet( reader );
			}*/
			
			else
			{
				reader.ReadEndElement ();//</SOUNDSCAPE> 
				break;
			}
		}
	}
	
	/// <summary>
	/// Creates a SoundObject of type SoundSet from the XML definition and adds it to the soundObjects reference hashtable
	/// </summary>
	private static void readTrack(XmlReader reader)
	{		
		if ( !reader.IsStartElement ( "TRACK" ) ) throw new Exception ( "<TRACK> element expected.");

		string name = ReadStringAttribute(reader, "name", "no name");

		if (name == "no name")
		{
			throw new Exception("MUSICPLAYER ERROR at readTrack(): track has not been assigned a name");
		}

		GameObject trackObject = new GameObject(name);

		trackObject.transform.parent = instance.transform;

		trackObject.transform.localPosition = Vector3.zero;

		MusicTrack track = trackObject.AddComponent<MusicTrack>();



		int simultaneousParts = ReadIntAttribute(reader, "simultaneousParts", 1);
		
		bool isEmpty = reader.IsEmptyElement;
		
		List<MusicPart> parts = new List<MusicPart>();

		reader.ReadStartElement();
		
		if (!isEmpty)
		{
			readAllParts(reader, parts);
			reader.ReadEndElement(); //</SOUNDSET>
		}

		track.Setup(parts, name, simultaneousParts);

		instance.tracks.Add(name, track);
	}
	
	/// <summary>
	/// Reads multiple clips in a SoundSet or SoundPair XML element
	/// </summary>
	private static void readAllParts( XmlReader reader, List<MusicPart> parts)
	{
		while ( reader.IsStartElement ( "PART" ) ) 
		{
			parts.Add(readPart(reader));	
			reader.Read ();  // next element...
		}
	}
	/// Loads an AudioClip from Resources based on the XML definition, and adds it to the soundClips reference hashtable
	/// </summary>
	private static MusicPart readPart( XmlReader reader )
	{
		
		if ( !reader.IsStartElement ( "PART" ) ) throw new Exception ( "<PART> element expected.");

		string name = ReadStringAttribute(reader, "name", "no name");
		string clip = ReadStringAttribute(reader, "clip", "no clip name");
		string trigger = ReadStringAttribute(reader, "trigger", "no trigger");
		bool isPersistent = ReadBoolAttribute(reader, "isPersistent", true);
		int barsLength = ReadIntAttribute(reader, "barsLength", 4);


		MusicPart part = new MusicPart(clip, name, trigger, barsLength, isPersistent);

		return part;


		///I cannot be arsed with error handling today
		/*if (name == "")
		{
			throw new Exception("SOUNDSYSTEM ERROR at readPart(): soundEffect has not been assigned a name");
		}*/
	}

	private static void readSFX(XmlReader reader)
	{		
		if ( !reader.IsStartElement ( "SFX" ) ) throw new Exception ( "<SFX> element expected.");

		GameObject trackObject = new GameObject("SFXTRACK");
		
		trackObject.transform.parent = instance.transform;
		
		trackObject.transform.localPosition = Vector3.zero;
		
		SFXTrack track = trackObject.AddComponent<SFXTrack>();

		bool isEmpty = reader.IsEmptyElement;
		
		List<SFX> effects = new List<SFX>();
		
		reader.ReadStartElement();
		
		if (!isEmpty)
		{
			readAllSFX(reader, effects);
			reader.ReadEndElement(); //</SOUNDSET>
		}
		
		track.Setup(effects);

		instance.sfxTrack = track;

		//instance.tracks.Add(name, track);
	}
	
	/// <summary>
	/// Reads multiple clips in a SoundSet or SoundPair XML element
	/// </summary>
	private static void readAllSFX( XmlReader reader, List<SFX> effects)
	{
		while ( reader.IsStartElement ( "EFFECT" ) ) 
		{
			effects.Add(readEffect(reader));	
			reader.Read ();  // next element...
		}
	}

	private static SFX readEffect( XmlReader reader )
	{
		
		if ( !reader.IsStartElement ( "EFFECT" ) ) throw new Exception ( "<EFFECT> element expected.");
		
		string name = ReadStringAttribute(reader, "name", "no name");
		string clip = ReadStringAttribute(reader, "clip", "no clip name");
		string trigger = ReadStringAttribute(reader, "trigger", "no trigger");
		bool fadeInOut = ReadBoolAttribute(reader, "fadeInOut", true);
		int minLoops = ReadIntAttribute(reader, "minLoops", 0);
		int maxLoops = ReadIntAttribute(reader, "maxLoops", 0);
		bool onBeat = ReadBoolAttribute(reader, "onBeat", false);
		

		SFX effect = new SFX(clip, name, trigger, fadeInOut, minLoops, maxLoops, onBeat);

		//MusicPart part = new MusicPart(clip, name, trigger, barsLength, isPersistent);
		
		return effect;
		
		
		///I cannot be arsed with error handling today
		/*if (name == "")
		{
			throw new Exception("SOUNDSYSTEM ERROR at readPart(): soundEffect has not been assigned a name");
		}*/
	}
	
	//The following comments and methods are directly from Michael's code. If it ain't broke, don't fix it.
	
	/// <summary>
	/// Looks for an attribute of the specified name, and returns its value.  If no such attribute exists,
	/// the specified default value is returned.
	/// </summary>
	protected static string ReadStringAttribute ( XmlReader reader, string name, string defaultValue )
	{
		// attributeName="string-value"
		string attributeText = reader.GetAttribute ( name );
		if ( attributeText == null ) attributeText = defaultValue;
		return attributeText;
	}
	
	/// <summary>
	/// Looks for an attribute of the specified name, and returns its value.  If no such attribute exists,
	/// the specified default value is returned.
	/// </summary>
	protected static int ReadIntAttribute ( XmlReader reader, string name, int defaultValue )
	{
		// attributeName="int-value"
		int attributeValue;
		string attributeText = reader.GetAttribute ( name );
		if ( attributeText == null
		    || int.TryParse ( attributeText, out attributeValue ) == false )
			attributeValue = defaultValue;
		return attributeValue;
	}

	protected static bool ReadBoolAttribute ( XmlReader reader, string name, bool defaultValue )
	{
		// attributeName="int-value"
		bool attributeValue;
		string attributeText = reader.GetAttribute ( name );
		if ( attributeText == null
		    || bool.TryParse ( attributeText, out attributeValue ) == false )
			attributeValue = defaultValue;
		return attributeValue;
	}
	#endregion
}

