using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PathTile : MonoBehaviour
{
	[HideInInspector] public List<PathTile> connections = new List<PathTile>();

	public Tile tile;
	public TileMap tileMap {get; set;}

	public bool flagForDestruction = false;

	public void Start()
	{
		tile = GetComponent<Tile>();
	}
}
