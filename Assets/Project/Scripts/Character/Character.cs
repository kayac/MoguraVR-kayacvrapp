using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class Character : NetworkBehaviour {
	[SerializeField] GameObject m_HeadPrefab;
	[SerializeField] GameObject m_LeftHandPrefab;
	[SerializeField] GameObject m_RightHandPrefab;
	[SerializeField] GameObject m_JankenJudgePrefab;
	[SerializeField]
	GameObject m_Head;
	[SerializeField]
	GameObject m_LeftHand;
	[SerializeField]
	GameObject m_RightHand;
	[SerializeField]
	GameObject m_JankenJudge;
	[SyncVar]
	uint m_LeftNetId;

	[SyncVar]
	uint m_RightNetId;

	[SyncVar]
	uint m_HeadNetId;

	[SyncVar]
	uint m_JankenJudgeId;

	[SyncVar]
	string m_NickName = string.Empty;

	public override void OnStartLocalPlayer()
	{
		if (m_LeftHandPrefab != null)
		{
			CmdCreateLeftHand();
		}

		if (m_RightHandPrefab != null)
		{
			CmdCreateRightHand();
		}

		if (m_HeadPrefab != null)
		{
			CmdCreateHead();
		}

		if (FileUtils.GetPlayerType () == PlayerType.HOST) {
			CmdCreateJunkenJudge ();
		}
	}

	[Command]
	void CmdCreateLeftHand()
	{
		m_LeftHand = (GameObject)Instantiate(m_LeftHandPrefab);
		NetworkServer.SpawnWithClientAuthority(m_LeftHand, connectionToClient);
		m_LeftNetId = m_LeftHand.GetComponent<NetworkIdentity>().netId.Value;
	}

	[Command]
	void CmdCreateRightHand()
	{
		m_RightHand = (GameObject)Instantiate(m_RightHandPrefab);
		NetworkServer.SpawnWithClientAuthority(m_RightHand, connectionToClient);
		m_RightNetId = m_RightHand.GetComponent<NetworkIdentity>().netId.Value;
	}

	[Command]
	void CmdCreateHead()
	{
		m_Head = (GameObject)Instantiate(m_HeadPrefab);
		NetworkServer.SpawnWithClientAuthority(m_Head, connectionToClient);
		m_HeadNetId = m_Head.GetComponent<NetworkIdentity>().netId.Value;
	}

	[Command]
	void CmdCreateJunkenJudge()
	{
		m_JankenJudge = (GameObject)Instantiate(m_JankenJudgePrefab);
		NetworkServer.SpawnWithClientAuthority(m_JankenJudge, connectionToClient);
		m_JankenJudgeId = m_JankenJudge.GetComponent<NetworkIdentity>().netId.Value;
	}
}
