using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Text.RegularExpressions;

public class Register : MonoBehaviour 
{
	// Secret key to communicate with server
	private string secretKey = "farmersAreCool";
	// URLs for PHP for accounts
	private string registerURL = "https://db-farmer.000webhostapp.com/create_account.php?";
	private string loginURL = "https://db-farmer.000webhostapp.com/login.php?";
	// Buttons for registering/login
	public Button registerButton;
	public Button loginButton;
	// Text Inputs
	public InputField user;
	public InputField pass;
	// Popup on button click
	public GameObject panel;
	public Text panelText;
	public GameObject panelButton;
	// Bird trigger
	public Button bird;
	// Used for debugging purposes
	public string t;
	void Start()
	{
		registerButton.onClick.AddListener(registration);
		loginButton.onClick.AddListener (login);
		panelButton.GetComponent<Button>().onClick.AddListener (panelExit);
		bird.onClick.AddListener (birdClick);
		PlayerPrefs.SetInt ("Id", 0);
	}
	void Update()
	{
	}
	void birdClick()
	{
		panel.SetActive (true);
		panelText.text = "In India 65% of the workforce is concentrated in agriculture. " +
			"This brings in almost 14% of the GDP. However, farmers in the country remain" +
			"dissatisfied. This is caused by various reason like fragmentation of land, lack of " +
			"feasible loans and absence of infrastructure.\n\nTo learn more:\nhttps://qrius.com/the-bitter-plight-of-indias-small-farmers/";
		panelButton.SetActive (true);
	}
	// Code for handling Md5 hash creation
	public string Md5Sum(string strToEncrypt)
	{
		System.Text.UTF8Encoding ue = new System.Text.UTF8Encoding();
		byte[] bytes = ue.GetBytes(strToEncrypt);

		// encrypt bytes
		System.Security.Cryptography.MD5CryptoServiceProvider md5 = new System.Security.Cryptography.MD5CryptoServiceProvider();
		byte[] hashBytes = md5.ComputeHash(bytes);

		// Convert the encrypted bytes back to a string (base 16)
		string hashString = "";

		for (int i = 0; i < hashBytes.Length; i++)
		{
			hashString += System.Convert.ToString(hashBytes[i], 16).PadLeft(2, '0');
		}

		return hashString.PadLeft(32, '0');
	}
	// Called when popup button is clicked
	void panelExit()
	{
		// Reset popup
		panelText.text = "Loading...";
		panel.SetActive (false);
		panelButton.SetActive (false);
		// If successfully registered/logged in move to game
		if (PlayerPrefs.GetInt ("Id") != 0)
		{
			SceneManager.LoadScene (1);
		}
	}
	// Called when register button clicked
	void registration()
	{
		if (user.text == "" || pass.text == "") 
		{
			panel.SetActive (true);
			panelText.text = "Enter username and password";
			panelButton.SetActive (true);
			return;
		}
		StartCoroutine (accountInput (registerURL));
	}
	// Called when login button clicked
	void login()
	{
		if (user.text == "" || pass.text == "") 
		{
			panel.SetActive (true);
			panelText.text = "Enter username and password";
			panelButton.SetActive (true);
			return;
		}
		StartCoroutine (accountInput (loginURL));
	}
	IEnumerator accountInput(string URL)
	{
		// Open up popup
		panel.SetActive(true);
		// Create verification hash
		string hash = Md5Sum(user.text + pass.text + secretKey);
		// URL to send data to
		string post_url = URL + "User=" + WWW.EscapeURL(user.text) + "&hash=" + hash;
		// Send password by POST
		WWWForm form = new WWWForm();
		form.AddField ("Pass", WWW.EscapeURL (pass.text));
		WWW hs_post = new WWW(post_url, form);
		yield return hs_post;
		// Download complete, update popup
		panelText.text = Regex.Replace(hs_post.text, "#.*", "");
		if (Regex.Replace (hs_post.text, ".*#", "") != "" && Regex.Replace (hs_post.text, ".*#", "") != hs_post.text)
			PlayerPrefs.SetInt ("Id", int.Parse (Regex.Replace (hs_post.text, ".*#", "")));
		t = hs_post.text;
		panelButton.SetActive (true);
	}
}
