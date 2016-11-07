using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class NetworkHead : NetworkBehaviour {

	public override void OnStartAuthority()
	{
		GetComponent<TargetFollower> ().target = PlayerObjectAccess.Instance.eyeTarget;
		Vector3 pos;
		Quaternion rot;
		if (PlayerObjectAccess.Instance.playerControllerId == 0) {
			pos = PositionAccess.Instance.player1Position.position;
			rot = PositionAccess.Instance.player1Position.rotation;
		} else {
			pos = PositionAccess.Instance.player2Position.position;
			rot = PositionAccess.Instance.player2Position.rotation;
		}
		PlayerObjectAccess.Instance.cameraContainer.transform.position = pos;
		PlayerObjectAccess.Instance.cameraContainer.transform.rotation = rot;
	}
}
