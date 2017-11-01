using System;
using System.Globalization;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Plant : Tile {
	// Plant details
	public string plantName;
	public string descript;
	public int cost;
	// Trigger that handles options
	private MainGame trigger;
	// Tooltip prefab
	public GameObject toolTip;
	// Current tiles renderer
	private SpriteRenderer rendered;
	// Display purposes
	public Sprite grown;
	public Color dead;
	// Created tooltip
	private GameObject TT;
	// UI canvas
	private Canvas canvas;
	// The soil this plant is on
	private Soil soil;
	// Time when stuff happens
	public DateTime failTime;
	public DateTime harvestTime;
	public DateTime updateTime;
	// PHP for soil tile updates
	private string harvestURL = "https://db-farmer.000webhostapp.com/remove_tile.php?";
	private string updtaskURL = "https://db-farmer.000webhostapp.com/update_task.php?";
	// Use this for initialization
	void Start () {
		rendered = GetComponent<SpriteRenderer> ();
		trigger = GameObject.Find ("Trigger").GetComponent<MainGame> ();
		canvas = FindObjectOfType<Canvas> ();
		plantName = "Wheat";
		descript = "Wheat is the second most important crop in India, following rice.";
	}
	void OnMouseEnter()
	{
		if (trigger.isUIOverride)
			return;
		if (!TT)
		{
			TT = Instantiate (toolTip, transform.position, new Quaternion (0, 0, 0, 0));
			TT.GetComponent<Transform> ().SetParent (canvas.GetComponent<Transform> ());
			TT.GetComponent<Transform> ().position = Input.mousePosition;
			Text title = TT.transform.GetChild (0).GetComponent<Text> ();
			Text description = TT.transform.GetChild (1).GetComponent<Text> ();
			if (value == 3)
			{
				description.text = "This plant is dead.\n";
				title.text = "Dead ";
			} else if (value == 2)
			{
				description.text = "This plant can be harvested.\n";
				title.text = "Fully Grown ";
			} else if ((soil.value & 2) == 0)
			{
				description.text += "Death Time: " + (int)failTime.Subtract (DateTime.Now).TotalMinutes + " minutes\n";
				description.text += "This plant needs to be watered.\n";
				title.text += "Dry ";
			} else
			{
				description.text = "Grow time: " + (int)harvestTime.Subtract (DateTime.Now).TotalMinutes + " minutes\n";
				description.text += "Water time: " + (int)updateTime.Subtract (DateTime.Now).TotalMinutes + "minutes\n";

			}
			title.text += plantName;
			description.text += descript;

		}
	}
	void OnMouseOver()
	{
		if (trigger.isUIOverride)
			return;
		if (TT)
			TT.GetComponent<Transform> ().position = Input.mousePosition;
	}
	void OnMouseUpAsButton()
	{
		if (trigger.isUIOverride)
			return;
		if (trigger.opt == 2 && (soil.value & 2) == 0)
		{
			soil.value = (soil.value | 2);
			soil.dryTime = DateTime.Now.AddMinutes (30);
			string URL = updtaskURL + "TileId=" + soil.tileId + "&update=" + soil.dryTime.ToString("s") + "&UserId=" + trigger.id;
			StartCoroutine(callURL (URL));
			if (failTime != DateTime.MinValue)
			{
				updateTime = soil.dryTime;
				failTime = updateTime.AddMinutes(30);
				URL = updtaskURL + "TileId=" + tileId + "&update=" + updateTime.ToString("s") + "&UserId=" + trigger.id + "&fail=" + failTime.ToString("s");
				StartCoroutine(callURL(URL));
			}
		}
		if (value == 3 || value == 2)
		{
			if (!trigger.newPlay)
			{
				string URL = harvestURL + "UserId=" + trigger.id + "&TileId=" + tileId;
				if (value == 2)
				{
					URL += "&Harvest=1";
					trigger.currMoney += 100;
				}
				else
					URL += "&Harvest=0";
				StartCoroutine (callURL (URL));
			}
			if (TT != null)
				Destroy (TT);
			Destroy (soil.hoed);
			soil.value = 1;
			Destroy (gameObject);
		}
	}
	void OnMouseExit ()
	{
		if (TT != null)
			Destroy (TT);
	}
	IEnumerator callURL(string URL)
	{
		WWW www = new WWW(URL);
		yield return www;
		if(www.text != "")
			print (www.text);
	}
	// Update is called once per frame
	void Update () {
		if (transform.parent != null && soil == null)
		{
			soil = transform.parent.GetComponent<Soil> ();
		}
		if (value != 2 && failTime != DateTime.MinValue && DateTime.Now >= failTime)
		{
			value = 3;
			rendered.color = dead;
		}
		if (value != 3 && harvestTime != DateTime.MinValue && DateTime.Now >= harvestTime.Subtract(TimeSpan.FromMinutes(30)))
		{
			rendered.sprite = grown;
		}
		if (value != 3 && harvestTime != DateTime.MinValue && DateTime.Now >= harvestTime)
		{
			value = 2;
		}

	}
}
