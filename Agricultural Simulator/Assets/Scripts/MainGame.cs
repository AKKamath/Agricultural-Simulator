using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Text.RegularExpressions;

public class MainGame : MonoBehaviour {
	// Text for money
	public Text money;
	public int currMoney;
	// Popup screen
	public GameObject popup;
	public GameObject popupButton;
	public Text popupText;
	// Current selected option
	public int opt;
	// Debug
	public Transform[] tileTypes;
	public int id;
	public bool isUIOverride;
	// New game?
	public bool newPlay;
	// Camera, to adjust size
	public Camera cam;
	// Default URL
	private string pageURL = "https://db-farmer.000webhostapp.com/";
	// Store ingame tiles
	private List<Transform> gobjs = new List<Transform>();
	// Function that executes on initial creation
	void Start () {
		opt = 0;
		// Show loading screen
		popup.SetActive (true);
		id = PlayerPrefs.GetInt ("Id");
		if (PlayerPrefs.GetInt ("Id") == 0)
			SceneManager.LoadScene (0);
		// Start import
		newPlay = getTiles();
		if (!newPlay) {
			getTasks ();
			getCurrency ();
		}
		else
			currMoney = 10000;
		popupButton.SetActive (true);
	}
	// Fetch tiles from database, and create in game environment
	// Returns whether user is a new player
	bool getTiles()	{
		// Create valid tile request
		string URL = pageURL + "get_tiles.php?Id=" + PlayerPrefs.GetInt ("Id");
		WWW msg = new WWW (URL);
		// Block until complete
		while (msg.isDone == false);
		// Invalid user ID obtained
		if (msg.text == "-1" || msg.text == "")
			SceneManager.LoadScene (0);
		// Container for tiles
		string[] Tiles = Regex.Split (msg.text, "\n");
		int max = int.MinValue, min = int.MaxValue;

		bool newPlay = false;
		List<string []> singleTile = new List<string []> ();
		// Fetch details of each tile and create
		for (int i = 0; i < Tiles.Length - 1; ++i)	{
			// Format: UserId, TileId, X, Y, Value, Type, Parent
			singleTile.Add(Regex.Split (Tiles [i], " "));
			// Id = -1, implies player data does not exist
			if (i == 0 && singleTile [i][0] == "-1")
					newPlay = true;
			int X = int.Parse (singleTile [i][2]);
			int Y = int.Parse (singleTile [i][3]);
			max = Mathf.Max (X, Y, max);
			min = Mathf.Min (X, Y, min);
			// Create tile and set parameters
			gobjs.Add(Instantiate (tileTypes[int.Parse(singleTile [i][5])], new Vector3 (X, Y, 0), Quaternion.identity));
			gobjs[i].GetComponent<Tile> ().tileId = int.Parse (singleTile [i][1]);
		}
		// Attach child to parents
		for (int i = 0; i < gobjs.Count; ++i)
		{
			// Search for parent with matching tile id
			for (int j = 0; j < gobjs.Count; ++j) {
				// Tile does not have a parent
				if (singleTile [i].Length < 7 || singleTile [i] [6] == null || singleTile [i] [6] == "")
					break;
				if (i == j)
					continue;
				if (singleTile [i] [6] == gobjs [j].GetComponent<Tile>().tileId.ToString()) {
					gobjs [i].SetParent (gobjs [j]);
					Soil s = gobjs [j].GetComponent<Soil> ();
					if (s != null)
						s.childPlant = gobjs [i].GetComponent<GameObject> ();
					gobjs[i].localPosition = new Vector3 (0, 0, -2);
					break;
				}
			}
		}
		for(int i = 0; i < gobjs.Count; ++i)
			gobjs[i].GetComponent<Tile> ().value = int.Parse (singleTile [i][4]);
		// Set camera field of view to show all tiles
		cam.orthographicSize = Mathf.Max (max, -1 * min) + 3;
		if (newPlay)
			popupText.text = "Welcome to agricultural simulator. Plow and water the soil, then plant a seed to continue.";
		else
			popupText.text = "Welcome back!";
		return newPlay;
	}
	// Fetch the ongoing tasks a user has
	void getTasks()	{
		// Create valid tile request
		string URL = pageURL + "get_tasks.php?Id=" + PlayerPrefs.GetInt ("Id");
		WWW msg = new WWW (URL);
		// Block until complete
		while (msg.isDone == false);
		// Container for tasks
		string[] taskList = Regex.Split (msg.text, "\n");
		// Fetch details of each task
		for (int i = 0; i < taskList.Length - 1; ++i) {
			
			// Format: Type, TileId, Completion_time, Fail_time, Update_time
			string[] singleTask = Regex.Split (taskList [i], "\t");
			if (singleTask [1] == "")
				continue;
			for (int j = 0; j < gobjs.Count; ++j) {
				if (gobjs [j].GetComponent<Tile> ().tileId != int.Parse (singleTask [1]))
					continue;
				switch (int.Parse (singleTask [0]))	{
					case 1:
						gobjs [j].GetComponent<Soil> ().dryTime = DateTime.Parse (singleTask [4]);
						break;
					case 2:	{
							Plant p = gobjs [j].GetComponent<Plant>();
							p.updateTime = DateTime.Parse (singleTask [4]);
							p.failTime = DateTime.Parse (singleTask [3]);
							p.harvestTime = DateTime.Parse (singleTask [2]);
						}
						break;
				}
				break;
			}
		}
	}
	// Function to fetch user's currency from DB
	void getCurrency() {
		string URL = pageURL + "get_currency.php?" + "Id=" + PlayerPrefs.GetInt ("Id");
		WWW msg = new WWW (URL);
		// Pause until download complete
		while (msg.isDone == false);
		if (msg.text == "-1" || msg.text == "")
			SceneManager.LoadScene (0);
		print (msg.text);
		currMoney = int.Parse(msg.text);
	}
	public IEnumerator CreateNewGame() {
		popupText.text = "Your father has died. His land has been divided among you and your 6 brothers. " +
			"Your eldest brother inherited the rest of his estate along with his wealth. Due to this, you have gone and obtained a loan" +
			" from a local moneylender.";
		popup.SetActive (true);
		popupButton.SetActive (true);
		while (popup.activeSelf)
			yield return null;
		string URL = pageURL + "create_game.php?" + "UserId=" + PlayerPrefs.GetInt ("Id");
		WWW msg = new WWW (URL);
		// Pause until download complete
		yield return msg;
		SceneManager.LoadScene (1);
	}
	// Update is called once per frame
	void Update () 	{
		money.text = currMoney.ToString();
		isUIOverride = popup.activeSelf;
	}
}
