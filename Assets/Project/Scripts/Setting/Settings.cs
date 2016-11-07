using UnityEngine;
using System.Collections;

public class Settings : MonoBehaviour {

	// Use this for initialization
	void Awake () {
		LoadIpFile ();
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void LoadIpFile(){
		string contents = Resources.Load<TextAsset>("ip").text;
		FileUtils.ip = contents;
	}
}
