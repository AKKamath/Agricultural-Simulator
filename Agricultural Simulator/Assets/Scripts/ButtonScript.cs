using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

public class ButtonScript : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerClickHandler 
{
	private MainGame trigger;
	public int value;
	public Color highlight;
	public Color click;
	private Color defColor;
	public Texture2D cursor;
	// Use this for initialization
	void Start () 
	{
		trigger = GameObject.Find ("Trigger").GetComponent<MainGame> ();
		defColor = GetComponent<Image> ().color;
	}
	public void OnPointerEnter(PointerEventData eventData)
	{
		GetComponent<Image> ().color = highlight;
	}
	public void OnPointerExit (PointerEventData eventData)
	{
		GetComponent<Image> ().color = defColor;
	}
	public void OnPointerDown(PointerEventData eventData)
	{
		GetComponent<Image> ().color = click;
	}
	public void OnPointerClick(PointerEventData eventData)
	{
		if (trigger.opt != value)
		{
			trigger.opt = value;
			Cursor.SetCursor (cursor, new Vector2(cursor.height / 2, cursor.width / 2), CursorMode.Auto);
		} else
		{
			Cursor.SetCursor (null, Vector2.zero, CursorMode.Auto);
			trigger.opt = 0;
		}
	}
	// Update is called once per frame
	void Update () {
		
	}
}
