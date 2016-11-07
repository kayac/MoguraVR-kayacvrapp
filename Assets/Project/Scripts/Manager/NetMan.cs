using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class NetMan : NetworkManager {
	enum State
	{
		CONNECTING,
		CONNECTED,
		FAILED,
		RELOADED
	}

	[SerializeField]
	Transform[] startTransforms;
	[SerializeField]
	Transform playerOVR;
	[SerializeField]
	GameObject disconnectMsg;

	// Use this for initialization
	int playerConnectionId;
	State state;

	void Start () {
		disconnectMsg.SetActive (false);
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetKey (KeyCode.Return)) {
			if (state == State.FAILED) {
				ReloadScene ("Sample1");
			}
		}
	}

	public override void OnServerAddPlayer(NetworkConnection conn, short playerControllerId, NetworkReader extraMessageReader)
	{
		Transform startPoint = null;
		GameObject prefab = null;
		PlayerObjectAccess.Instance.playerControllerId = playerControllerId;
		Debug.Log ("on Server Add==");
		switch (playerControllerId)
		{
		case 0:
			startPoint = startTransforms[0];
			prefab = playerPrefab;
			break;
		case 1:
			startPoint = startTransforms[1];
			prefab = playerPrefab;
			playerConnectionId = conn.connectionId;
			break;
		case 2:
			startPoint = startTransforms[2];
			prefab = playerPrefab;
			break;
		}

		GameObject player = GameObject.Instantiate(prefab, startPoint.position, startPoint.rotation) as GameObject;
		NetworkServer.AddPlayerForConnection(conn, player, playerControllerId);
		if (playerOVR != null) {
			playerOVR.transform.position = startPoint.position;
			playerOVR.transform.rotation = startPoint.rotation;
		}
	}

	public override void OnClientConnect(NetworkConnection conn)
	{
		base.OnClientConnect(conn);

		state = State.CONNECTED;

		PlayerType playerType = FileUtils.GetPlayerType();
		PlayerInfoMessage info = new PlayerInfoMessage();
		info.nickName = "a";
		Debug.Log ("on Client==="+playerType);
		switch (playerType)
		{
		case PlayerType.HOST:
			info.nickName = "b";
			ClientScene.AddPlayer (conn, 0, info);
			break;
		case PlayerType.CLIENT:
			info.nickName = "c";
			ClientScene.AddPlayer(conn, 1, info);
			break;
		}
	}

	public override void OnServerDisconnect (NetworkConnection conn)
	{
		base.OnServerDisconnect (conn);
	}

	public override void OnClientDisconnect (NetworkConnection conn)
	{
		base.OnClientDisconnect (conn);
		state = State.FAILED;

		disconnectMsg.SetActive (true);
		disconnectMsg.transform.position = playerOVR.transform.position;
		disconnectMsg.transform.rotation = Quaternion.Euler (new Vector3(0, playerOVR.transform.rotation.eulerAngles.y, 0));
	}

	void ReloadScene(string sceneName)
	{
		if (state != State.RELOADED)
		{
			state = State.RELOADED;
			UnityEngine.SceneManagement.SceneManager.LoadScene(sceneName, UnityEngine.SceneManagement.LoadSceneMode.Single);
		}
	}

}
