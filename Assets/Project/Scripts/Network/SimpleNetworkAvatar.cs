using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.IO;
using System;
using System.Collections.Generic;

public class SimpleNetworkAvatar : NetworkBehaviour {
	public OvrAvatar avatar;
	OvrAvatarRemoteDriver m_RemoteDriver;
	int packetSequence = 0;

	// Use this for initialization
	void Start () {
		if (hasAuthority)
		{
			avatar.Driver = avatar.gameObject.AddComponent<OvrAvatarLocalDriver>();
			avatar.RecordPackets = true;
			avatar.PacketRecorded += OnLocalAvatarPacketRecorded;
			avatar.ShowFirstPerson = true;
			avatar.ShowThirdPerson = false;
			m_RemoteDriver = null;
		}
		else
		{
			m_RemoteDriver = avatar.gameObject.AddComponent<OvrAvatarRemoteDriver>();
			avatar.Driver = m_RemoteDriver;
			avatar.ShowFirstPerson = false;
			avatar.ShowThirdPerson = true;
		}

		avatar.enabled = true;

        Vector3 pos;
        Quaternion rot;
        if ((FileUtils.GetPlayerType () == PlayerType.HOST && hasAuthority) || (FileUtils.GetPlayerType () != PlayerType.HOST && !hasAuthority)){
            pos = PositionAccess.Instance.player1Position.position;
            rot = PositionAccess.Instance.player1Position.rotation;
        } else {
            pos = PositionAccess.Instance.player2Position.position;
            rot = PositionAccess.Instance.player2Position.rotation;
        }

        transform.position = pos;
        transform.rotation = rot;

        if (hasAuthority)
        {
            PlayerObjectAccess.Instance.cameraContainer.transform.position = pos;
            PlayerObjectAccess.Instance.cameraContainer.transform.rotation = rot;
        }
	}
	
	void OnLocalAvatarPacketRecorded(object sender, OvrAvatar.PacketEventArgs args)
	{
		using (MemoryStream outputStream = new MemoryStream())
		{
			BinaryWriter writer = new BinaryWriter(outputStream);
			writer.Write(packetSequence++);
			args.Packet.Write(outputStream);
			SendPacketData(outputStream.ToArray());
		}
	}

	void SendPacketData(byte[] data){
		if (isServer)
			RpcHandlePacketData(data);
		else
			CmdSendPacketData(data);            
	}

	[Command]
	void CmdSendPacketData(byte[] data)
	{
		RpcHandlePacketData(data);
	}

	[ClientRpc]
	void RpcHandlePacketData(byte[] data)
	{
		if (m_RemoteDriver == null)
			return;

		using (MemoryStream inputStream = new MemoryStream(data))
		{
			BinaryReader reader = new BinaryReader(inputStream);
			int sequence = reader.ReadInt32();
			OvrAvatarPacket packet = OvrAvatarPacket.Read(inputStream);
			m_RemoteDriver.QueuePacket(sequence, packet);
		}
	}
}
