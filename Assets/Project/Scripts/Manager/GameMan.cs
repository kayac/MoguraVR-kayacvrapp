using UnityEngine;
using System.Collections;

public class GameMan : MonoBehaviour
{
	public static GameMan instance { get; protected set; }

	void Awake()
	{
		instance = this;
	}

	void Start()
	{
		Resources.UnloadUnusedAssets();
		System.GC.Collect();

		PlayerType playerType = FileUtils.GetPlayerType();

		switch (playerType)
		{
		case PlayerType.HOST:
			Connect();
			break;
		case PlayerType.CLIENT:
			Connect();
			break;
		
		}
	}

	public void Connect()
	{
		StartCoroutine(CoConnect());
	}

	IEnumerator CoConnect()
	{
		
		yield return new WaitForSeconds(1.0f);

		string ip = FileUtils.ip;


		if (ip == "")
		{
			NetMan.singleton.StartHost();
			Debug.Log ("Start Host===");
		}
		else
		{
			Debug.Log ("Start Client===");
			int port = FileUtils.GetPort();

			NetMan.singleton.networkAddress = ip;
			NetMan.singleton.networkPort = port;
			NetMan.singleton.StartClient();
		}
	}
}