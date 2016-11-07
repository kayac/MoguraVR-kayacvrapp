using UnityEngine;
using System.Collections;

public enum PlayerType
{
	HOST,
	CLIENT,
	OBSERVER
}

public class FileUtils {
	public static string ip;

	public static PlayerType GetPlayerType()
	{
		if (ip != "") {
			return PlayerType.CLIENT;
		} else {
			return PlayerType.HOST;
		}
	}

	public static int GetPort()
	{
		return 7777;
	}
}
