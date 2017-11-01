using System;
using System.Globalization;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Soil : Tile
{
	// Trigger that handles options
	private MainGame trigger;
	// Tooltip prefab
	public GameObject toolTip;
	// Current tiles renderer
	private SpriteRenderer rendered;
	// Created tooltip
	private GameObject TT;
	// UI canvas
	private Canvas canvas;
	// Used for display purposes
	public Sprite defSprite;
	public Sprite wateredImage;
	public GameObject hoed;
	private GameObject hoeOverlay;
	public GameObject seeded;
	public GameObject childPlant;
	bool ping;
	// Time to dry
	public DateTime dryTime; 
	// URL for PHP for tile updates and seed planting
	private string updateURL = "https://db-farmer.000webhostapp.com/update_tile.php?";
	private string seedURL = "https://db-farmer.000webhostapp.com/plant_seed.php?";
	private string taskURL = "https://db-farmer.000webhostapp.com/create_task.php?";
	private string updtaskURL = "https://db-farmer.000webhostapp.com/update_task.php?";
	// Use this for initialization
	void Start () 
	{
		ping = false;
		rendered = GetComponent<SpriteRenderer> ();
		trigger = GameObject.Find ("Trigger").GetComponent<MainGame> ();
		canvas = FindObjectOfType<Canvas> ();
		prevVal = 0;
	}
	void OnMouseEnter()
	{
		if (trigger.isUIOverride)
			return;
		if (trigger.opt == 0)
		{
			if (!TT)
			{
				TT = Instantiate (toolTip, transform.position, new Quaternion (0, 0, 0, 0));
				TT.GetComponent<Transform> ().SetParent (canvas.GetComponent<Transform> ());
				TT.GetComponent<Transform> ().position = Input.mousePosition;
				Text title = TT.transform.GetChild (0).GetComponent<Text> ();
				Text description = TT.transform.GetChild (1).GetComponent<Text> ();
				if ((value & 2) != 0)
				{
					description.text += "This has been watered.\n";
					description.text += "Dry Time: " + (int)dryTime.Subtract(DateTime.Now).TotalMinutes + " minutes.\n";
					title.text += "Watered ";
				}
				if ((value & 4) != 0)
				{
					description.text += "This has been hoed.\n";
					title.text += "Hoed ";
				}
				title.text += "Soil";
				description.text += "One of nature's basic resources. Soil is necessary for growth of crops.";

			}
			rendered.color -= new Color (0, 0, 0, 0.3F);
		} else if (trigger.opt == 2)
		{
			rendered.color -= new Color ((float)0x42 / (float)0xFF, 0, 0, 0F);
		} else if (trigger.opt == 8)
		{
			TT = Instantiate (toolTip, transform.position, new Quaternion (0, 0, 0, 0));
			TT.GetComponent<Transform> ().SetParent (canvas.GetComponent<Transform> ());
			TT.GetComponent<Transform> ().position = Input.mousePosition;
			Text title = TT.transform.GetChild (0).GetComponent<Text> ();
			title.color = new Color (1, 0, 0);
			Text description = TT.transform.GetChild (1).GetComponent<Text> ();
			description.color = new Color (1, 0, 0);

			if ((value & 4) == 0 || (value & 2) == 0)
			{
				title.text += "Cannot plant";
				description.text += "Requires soil that has been plowed and watered.";
			} else if (trigger.currMoney < 5)
			{
				title.text += "Cannot plant";
				description.text += "Requires at least 5 rupees.";
			} else if (childPlant != null)
			{
				title.text += "Cannot plant";
				description.text += "Plant already exists.";
			} else
			{
				title.color = new Color (0, 1, 0);
				description.color = new Color (0, 1, 0);
				title.text = "Plant";
				description.text = "Plant seed here.";
			}
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
		if (trigger.opt != 8)
			value = (value | trigger.opt);
		else if ((value & 6) == 6 && trigger.currMoney >= 5 && childPlant == null)
		{
			value = (value | trigger.opt);
		}
		ping = true;
	}
	void OnMouseExit ()
	{
		if (trigger.isUIOverride)
			return;
		if (TT != null)
			Destroy (TT);
		rendered.color = Color.white;
	}
	void Hoe()
	{
		if (hoeOverlay != null)
			return;
		hoeOverlay = Instantiate (hoed, new Vector3(0, 0, -1), transform.rotation);
		hoeOverlay.GetComponent<Transform> ().parent = transform;
		hoeOverlay.GetComponent<Transform> ().localPosition = new Vector3 (0, 0, -1);
	}
	IEnumerator Plant()
	{
		if (childPlant != null)
			yield break;
		trigger.currMoney -= 50;
		childPlant = Instantiate (seeded, transform.position, transform.rotation);
		childPlant.GetComponent<Transform> ().parent = transform;
		childPlant.transform.localPosition += new Vector3 (0, 0, -2);
		Plant p = childPlant.GetComponent<Plant> ();
		p.updateTime = dryTime;
		p.failTime = dryTime.AddMinutes (30);
		p.harvestTime = DateTime.Now.AddHours (2);
		if (trigger.newPlay)
		{
			StartCoroutine(trigger.CreateNewGame ());
			if (TT)
				Destroy (TT);
			yield break;
		}
		string URL = seedURL + "UserId=" + trigger.id + "&Parent=" + tileId + "&Type=1&Value=" + value;
		WWW www = new WWW(URL);
		yield return www;
		print (www.text);
		p.tileId = int.Parse(www.text);
		URL = taskURL + "UserId=" + trigger.id + "&start=" + DateTime.Now.ToString ("s") + "&complete=" + p.harvestTime.ToString ("s")
			+ "&update=" + p.updateTime.ToString ("s") + "&type=2&TileId=" + p.tileId + "&fail=" + p.failTime.ToString("s");
		print (URL);
		www = new WWW(URL);
		yield return www;
		if(www.text != "")
			print (www.text);
	}
	IEnumerator callURL(string URL)
	{
		WWW www = new WWW(URL);
		yield return www;
		if(www.text != "")
			print (www.text);
	}
	void UpdateValue()
	{
		if (trigger.newPlay)
			return;
		string URL = updateURL + "TileId=" + tileId + "&Value=" + value;
		StartCoroutine(callURL (URL));
	}
	void Water(bool ping)
	{
		GetComponent<Animation>().Play ("WaterAnim");
		rendered.sprite = wateredImage;
		if (dryTime == DateTime.MinValue && ping)
		{
			dryTime = DateTime.Now.AddMinutes (30);
			string URL = updtaskURL + "TileId=" + tileId + "&update=" + dryTime.ToString("s") + "&UserId=" + trigger.id;
			StartCoroutine(callURL (URL));
		}

	}
	// Update is called once per frame
	void Update () 
	{
		if (dryTime != DateTime.MinValue && DateTime.Now > dryTime)
		{
			value = (value ^ 2);
			dryTime = DateTime.MinValue;
			rendered.sprite = defSprite;
		}
		if (value != prevVal)
		{
			if ((prevVal & 2) == 0 && (value & 2) != 0)
				Water (ping);
			if ((prevVal & 4) == 0 && (value & 4) != 0)
				Hoe ();
			if ((prevVal & 8) == 0 && (value & 8) != 0 && ping)
				StartCoroutine (Plant ());
			ping = false;
			UpdateValue ();
			prevVal = value;
		}
	}
}
