
using System;
using UnityEngine;

/// <summary>
/// Abstract SoundObject class, containing a method for instantiating prefabs and parenting them
/// to gameObjects
///  SoundEffect, SoundPair and SoundSet all derive from this
/// </summary>
	public abstract class SoundObject
	{	
		protected GameObject prefab;
		protected string name;
	
		public GameObject Prefab
		{
			get {return prefab;}
		}
	
		public string Name 
		{
			get {return name;}
			set {name = value;}
		}
	
		public GameObject instantiate(GameObject Parent)
		{ //note to self: this method needs to be overloaded to accomodate instantiating sound objects
		//which are not parented to the calling gameObject 
		//(eg, a death noise wherein the parent object gets destroyed immediately after calling the SoundSystem,
		//which would destroy the instantiated prefab also
		
			GameObject instantiatedPrefab = UnityEngine.MonoBehaviour.Instantiate(prefab, Parent.transform.position, Quaternion.identity) as GameObject;
			instantiatedPrefab.transform.parent = Parent.transform;
			instantiatedPrefab.name = name;
		
			return instantiatedPrefab;
		}
	}