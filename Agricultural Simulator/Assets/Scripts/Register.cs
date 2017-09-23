using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Register : MonoBehaviour 
{
	private string secretKey = "farmersAreCool"; // Edit this value and make sure it's the same as the one stored on the server
	public bool registering = false;
	public string RegisterURL = "https://db-farmer.000webhostapp.com/create_account.php?"; //be sure to add a ? to your url
	public Button yourButton;
	public InputField user;
	public InputField pass;
	public GameObject panel;
	public Text panel_text;
	public GameObject main;
	void Start()
	{
		Button btn = yourButton;
		btn.onClick.AddListener(Post);
	}
	void Update()
	{
		if (registering) {
			StartCoroutine (PostScores ());
			//main.SetActive (false);
			registering = false;
		}
	}
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
	void Post()
	{
		registering = true;
	}
	// remember to use StartCoroutine when calling this function!
	IEnumerator PostScores()
	{
		panel.SetActive(true);
		//This connects to a server side php script that will add the name and score to a MySQL DB.
		// Supply it with a string representing the players name and the players score.
		string hash = Md5Sum(user.text + pass.text + secretKey);

		string post_url = RegisterURL + "User=" + WWW.EscapeURL(user.text) + "&hash=" + hash;

		// Post the URL to the site and create a download object to get the result.
		WWWForm form = new WWWForm();
		form.AddField ("Pass", WWW.EscapeURL (pass.text));
		WWW hs_post = new WWW(post_url, form);
		yield return hs_post; // Wait until the download is done
		panel.SetActive(true);
		panel_text.text = hs_post.text;
	}
}
