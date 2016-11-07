using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class NetworkHand : NetworkBehaviour {
	[SyncVar]
	public int jankenIndex;
	public override void OnStartAuthority()
	{
		base.OnStartAuthority ();
		GetComponent<JankenHand> ().SetupAutority ();
	}

	[Command]
	public void CmdChangeJankenIdx(int i){
		jankenIndex = i;
	}
}
