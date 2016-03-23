using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Xml;

public class SoundSystem:MonoBehaviour {
	
	//path to the XML definition of the SoundScape - 
	//varies depending on project and should be set in the Inspector view
	public String XMLFilePath = "Assets/Standard Assets/SoundSystem/Resources/SoundSystemXML.txt";
	
	#region Hashtable Collections
	private static Hashtable soundClips;
	private static Hashtable proxies;
	private static Hashtable soundObjects;
	#endregion
	
	#region DUMMY REFERENCE OBJECTS
	//these objects ensure that Unity compiles all required scripts when a build is created
	private ProxyObject proxyRef;
	private SoundEffect sfxRef;
	private SoundPair sPairRef;
	private SoundSet setRef;
#endregion
	
	private static float killTime = 10.0f;

	// Use this for initialization
	void Awake () {
						
		soundClips = new Hashtable();
		proxies = new Hashtable();
		soundObjects = new Hashtable();

		loadSounds(XMLFilePath);
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
	private static void loadSounds( string fileName )
	{
		XmlTextReader reader = new XmlTextReader ( fileName );
      reader.WhitespaceHandling = WhitespaceHandling.None; // ignore whitespace
		
		 try
      {
        // Create a soundScape from the definition.
       	readSounds ( reader );
        reader.Close ();
      }
      catch ( Exception e )
      {
        reader.Close ();
        throw new Exception ( String.Format ( "Error reading sounds definition: {0}", e.Message ), e );
      }
	}
	
	/// <summary>
	/// Reads the XML Soundscape definition and creates the SoundObjects defined within.
	/// SoundObjects can be defined in any order within the SoundScape
	/// </summary>
	private static void readSounds( XmlReader reader )
	{
		reader.Read ();

     // UnityEngine.Debug.Log ( "Reading begins..." );

	
		// <SOUNDSCAPE name = "name">
	if ( !reader.IsStartElement ( "SOUNDSCAPE" ) ) throw new Exception ( "Expected <SOUNDSCAPE> Element" );

		//string soundscapeName = ReadStringAttribute ( reader, "name", "No Name" );
      
		reader.Read ();
		
      //UnityEngine.Debug.Log ( String.Format ( "Read SOUNDSCAPE {0}", soundscapeName  ) );

      /*
    	  	<SOUNDEFFECT name ="name" clip = "clipname.mp3">
		</SOUNDEFFECT>
      */
      while (true)
		{
		if ( reader.IsStartElement ( "SOUNDEFFECT" ) )
	      {
	        readSoundEffect ( reader );
	      }
		
			/*<SOUNDPAIR name = "name">
				<INITCLIP name = "name" clip = "clipname.mp3">
				<LOOPCLIP name = "name" clip = "clipname.mp3">
			</SOUNDPAIR>*/
			
		else if (reader.IsStartElement( "SOUNDPAIR"))
			{
				readSoundPair( reader );
			}
						
			/*<SOUNDSET name = "name" numClips = "number">
			<CLIP name = "name" clip = "clipname.mp3">
			…
			</SOUNDSET>
			*/
			
		else if (reader.IsStartElement( "SOUNDSET" ))
			{
				readSoundSet( reader );
			}
						
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
	private static void readSoundSet(XmlReader reader)
	{		
		if ( !reader.IsStartElement ( "SOUNDSET" ) ) throw new Exception ( "<SOUNDSET> element expected.");
		
		string name = ReadStringAttribute(reader, "name", "no name");
		int numClips = ReadIntAttribute(reader, "numClips", 0);
		AudioClip [] clips;      
		
		bool isEmpty = reader.IsEmptyElement;
		                             
		if (name == "no name")
			{
				throw new Exception("SOUNDSYSTEM ERROR at readSoundEffect(): soundEffect has not been assigned a name");
			}
			
		if (numClips == 0)
		{ 
			throw new Exception ("SOUNDSYSTEM ERROR at readSoundSet(): Soundset " + name + " has not been assigned a number of clips.");
		}
		else
		{
				clips = new AudioClip [numClips];
		}
		
		reader.ReadStartElement();
	
	if (!isEmpty)
		{
			readAllClips(reader, clips);
			reader.ReadEndElement(); //</SOUNDSET>
		}
		
		soundObjects.Add(name, new SoundSet(clips, name, killTime));
	}
	
	/// <summary>
	/// Reads multiple clips in a SoundSet or SoundPair XML element
	/// </summary>
	private static void readAllClips( XmlReader reader, AudioClip [] clips )
	{		
		int i = 0;
		
		while ( reader.IsStartElement ( "CLIP" ) ) 
      {
        clips[i] = readClip(reader);
		i++;		
        reader.Read ();  // next element...
      }
	}
	
	/// <summary>
	/// Creats a SoundObject of type SoundPair from an XML definition and adds it to the soundObjects reference hashtable
	/// </summary>
	private static void readSoundPair( XmlReader reader )
	{
		if ( !reader.IsStartElement ( "SOUNDPAIR" ) ) throw new Exception ( "<SOUNDPAIR> element expected.");
				
		string name = ReadStringAttribute(reader, "name", "no name");
		AudioClip [] clips;      
		
		bool isEmpty = reader.IsEmptyElement;
		                             
		if (name == "no name")
			{
				throw new Exception("SOUNDSYSTEM ERROR at readSoundEffect(): soundEffect has not been assigned a name");
			}
			
		clips = new AudioClip [2];
		
		reader.ReadStartElement();
	
	if (!isEmpty)
		{
			readAllClips(reader, clips);
			reader.ReadEndElement(); //</SOUNDPAIR>

		}
				
		soundObjects.Add(name, new SoundPair(clips[0], clips[1], name, killTime));
	}
	
	/// <summary>
	/// Creates a SoundObject of type SoundEffect from the XML definition and adds it to the soundObjects reference hashtable
	/// </summary>
	private static void readSoundEffect( XmlReader reader )
	{

		if ( !reader.IsStartElement ( "SOUNDEFFECT" ) ) throw new Exception ( "<SOUNDEFFECT> element expected.");
		
		
		string name = ReadStringAttribute(reader, "name", "no name");
		string clip = ReadStringAttribute(reader, "clip", "no clip name");
	
			if (name == "no name")
			{
				throw new Exception("SOUNDSYSTEM ERROR at readSoundEffect(): soundEffect has not been assigned a name");
			}
			
		AudioClip loadedClip = Resources.Load(clip) as AudioClip;
			
		if (loadedClip == null)
		{
			throw new Exception("SOUNDSYSTEM ERROR at readSoundEffect(): Could not load resource " + clip + ". Check that file names in the XML file are correct");
		}
			
		else
		{
			if (!soundClips.ContainsKey(name))
			{
				soundClips.Add(name, loadedClip);
			}
			soundObjects.Add(name, new SoundEffect(loadedClip, name, killTime));
		}
		
			reader.Read();
	}
	
	/// <summary>
	/// Loads an AudioClip from Resources based on the XML definition, and adds it to the soundClips reference hashtable
	/// </summary>
	private static AudioClip readClip( XmlReader reader )
	{
		
		if ( !reader.IsStartElement ( "CLIP" ) ) throw new Exception ( "<CLIP> element expected.");
		
		string name = ReadStringAttribute(reader, "name", "no name");
		string clip = ReadStringAttribute(reader, "clip", "no clip name");
	
			if (name == "no name")
			{
				throw new Exception("SOUNDSYSTEM ERROR at readClip(): soundEffect has not been assigned a name");
			}
			
		AudioClip loadedClip = Resources.Load(clip) as AudioClip;
			
			if (loadedClip == null)
			{
				throw new Exception("SOUNDSYSTEM ERROR at readClip(): Could not load resource " + clip + ". Check that file names in the XML file are correct");
			}
				
			else if (!soundClips.ContainsKey(name))
			{
				soundClips.Add(name, loadedClip);
			}
		
		return loadedClip;
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
#endregion

				
#region PUBLIC SOUNDSYSTEM METHODS
	
	//these are the methods to be called by the developer to play, start and stop sounds at runtime
	
	/// <summary>
	/// Sets the global killTime for all active SoundObjects.
	///  After a SoundObject has finished playing, it will kill itself after this time has elapsed
	/// unless it is called again
	/// </summary>
	public static void setKillTime(float newKillTime)
	{
		killTime = newKillTime;
	}
		
	/// <summary>
	/// Plays a single shot audio clip using Unity's built-in PlayClipAtPoint() functionality
	/// Does not use a SoundSystem SoundObject.
	/// Can be called on any individual clip that has been loaded, even if it was part of a SoundPair or SoundSet
	/// </summary>
	public static void playOnce(string Name, GameObject Caller)
	{		
		AudioClip clipToPlay = soundClips[Name] as AudioClip;
        if ( clipToPlay == null ) Debug.Log ( System.String.Format ( "SOUNDSYSTEM ERROR at playOnce(): Cannot find sound clip named '{0}'", Name ) );
        else AudioSource.PlayClipAtPoint(clipToPlay, Caller.transform.position);
	}
	
	/// <summary>
	/// Plays a single sound by name at the position of a gameObject Caller 
	/// Uses a SoundSystem SoundObject through a ProxyObject reference
	/// If the specified SoundObject does not exist, it will be instantiated
	/// </summary>
	public static void play(string Name, GameObject Caller)
	{
	
		ProxyObject tempProxy = instantiateSoundAndGetProxy(Name, Caller);

		tempProxy.play();

	}
	
	public static void play(string Name, GameObject Caller, float volume)
	{
	
		ProxyObject tempProxy = instantiateSoundAndGetProxy(Name, Caller);

		tempProxy.play(volume);

	}

	
	/// <summary>
	/// Starts a sound looping by name at the position of a gameObject Caller 
	/// Uses a SoundSystem SoundObject through a ProxyObject reference
	/// If the specified SoundObject does not exist, it will be instantiated
	/// </summary>
	public static void startLooping(string Name, GameObject Caller)
	{
		
		ProxyObject tempProxy = instantiateSoundAndGetProxy(Name, Caller);
	
		tempProxy.start();
	}
	
	public static void startLooping(string Name, GameObject Caller, float volume)
	{
		
		ProxyObject tempProxy = instantiateSoundAndGetProxy(Name, Caller);
	
		tempProxy.start(volume);
	}

	/// <summary>
	/// Stops a sound looping by name at the position of a gameObject Caller 
	/// Uses a SoundSystem SoundObject through a ProxyObject reference
	/// If the specified SoundObject does not exist, it will be instantiated
	/// </summary>
	public static void stopLooping(string Name, GameObject Caller)
	{
		ProxyObject tempProxy = instantiateSoundAndGetProxy(Name, Caller);
	
		tempProxy.stop();
	}
	
	/// <summary>
	/// Starts a sound playing at a set interval by name at the position of a gameObject Caller 
	/// Uses a SoundSystem SoundObject through a ProxyObject reference
	/// Currently only implemented for SoundSets
	/// If the specified SoundObject does not exist, it will be instantiated
	/// </summary>
	public static void playAtInterval(string Name, GameObject Caller, float interval)
	{
		ProxyObject tempProxy = instantiateSoundAndGetProxy(Name, Caller);
	
		tempProxy.playAtInterval(interval);
	}
	
		public static void playAtInterval(string Name, GameObject Caller, float minInterval, float maxInterval)
	{
		ProxyObject tempProxy = instantiateSoundAndGetProxy(Name, Caller);
	
		tempProxy.playAtInterval(minInterval, maxInterval);
	}
	
	public static void playAtInterval(string Name, GameObject Caller, float minInterval, float maxInterval, float volume)
	{
		ProxyObject tempProxy = instantiateSoundAndGetProxy(Name, Caller);
	
		tempProxy.playAtInterval(minInterval, maxInterval, volume);
	}
	

	/// <summary>
	/// Removes a proxy object from the proxies reference hashtable.
	/// Public to allow it to be called from a SoundObject script after killSelf() has been invoked on the object
	/// This method is not intended to be called from other scripts in a Unity scene
	/// </summary>
	public static void killProxy(string Name, GameObject Parent)
	{
		string Key = Parent.GetInstanceID() + Name;
		
//		print("removing proxy " + Key + " from proxies");
			
		proxies.Remove(Key);
	}
	
#endregion
	
#region PRIVATE SOUNDSYSTEM OBJECT INSTANTIATION METHODS	
	/// <summary>
	/// Checks to see if the requested sound effect already has a corresponding Soundobject in the scene
	/// using a proxyobject reference
	/// If there is no corresponding object, it will be instantiated
	/// </summary>
	private static ProxyObject instantiateSoundAndGetProxy(string Name, GameObject Caller)
	{		
			string Key = Caller.GetInstanceID() + Name;
		//proxyobject key is the unique instance id of the calling gameobject plus the requested SoundObject name
		
			ProxyObject tempProxy;
					
			if (proxies.ContainsKey(Key))
			{
				tempProxy = proxies[Key] as ProxyObject;
			}
		
				else
			{
				instantiateSoundObject(Name, Caller, Key);	
			
				tempProxy = proxies[Key] as ProxyObject;	
			}	
		
			return tempProxy;
	}

	/// <summary>
	/// Instantiates a SoundObject by name at the position of a gameObject Caller
	/// and creates a matching ProxyObject that has a reference to it.
	/// </summary>
	private static GameObject instantiateSoundObject(string Name, GameObject Caller, string Key)
	{
		SoundObject tempSoundObject = soundObjects[Name] as SoundObject;

			if (tempSoundObject == null)
			{
				throw new Exception ( "SOUNDSYSTEM ERROR at instantiateSoundObject(): No sound with name '" + Name + "' exists");
			}
		
			GameObject temp = tempSoundObject.instantiate(Caller);
						
			proxies.Add(Key, new ProxyObject(temp));
			return temp;	
	}
#endregion
}