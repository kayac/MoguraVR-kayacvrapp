using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class JankenNetwork : NetworkBehaviour {
	[SyncVar]
	public Vector3 bodyPosition;
	[SyncVar]
	public Vector3 judgeImage1Pos;
	[SyncVar]
	public Vector3 judgeImage2Pos;
	[SyncVar]
	public bool judgeImage1Show;
	[SyncVar]
	public bool judgeImage2Show;
	[SyncVar]
	public bool judgeAllShow;
	[SyncVar]
	public bool fullShow;
	[SyncVar]
	public bool judge1Win;
	[SyncVar]
	public bool isAiko;
	[SyncVar]
	public bool introShow;
	[SyncVar]
	public int introIndex;
	[SyncVar]
	public string judgeStr;
}
