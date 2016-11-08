using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class SyncTrasnformLerp : NetworkBehaviour {

	[SyncVar]
	private Vector3 syncPos;

	[SyncVar]
	private Quaternion syncRot = Quaternion.identity;

	float lerpRate = 15;

	private Vector3 lastPos;
	private float threshold;


	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
		if (!hasAuthority) {
			transform.position = Vector3.Lerp (transform.position, syncPos, Time.deltaTime * lerpRate);
			transform.rotation = Quaternion.Lerp (transform.rotation, syncRot, Time.deltaTime * lerpRate);
		}
	}

	void FixedUpdate(){
		if (hasAuthority) {
			if (isServer) {
				RpcState(transform.position, transform.rotation);
			} else {
				CmdState (transform.position, transform.rotation);
			}

		}
	}

	[Command(channel = Channels.DefaultUnreliable)]
	void CmdState(Vector3 p, Quaternion r)
	{
		RpcState(p, r);
	}

	[ClientRpc(channel = Channels.DefaultUnreliable)]
	void RpcState(Vector3 p, Quaternion r)
	{
		if (!hasAuthority)
		{
			syncPos = p;
			syncRot = r;
		}
	}

}
