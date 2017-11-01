using System.Collections;
using System.Collections.Generic;
using UnityEngine;
// Abstract Tile Class
public class Tile : MonoBehaviour {
	// Current tile type
	public int tileId;
	public int value;
	// Used for detecting changes
	protected int prevVal;

	void Start (){}
	void Update (){}
}
