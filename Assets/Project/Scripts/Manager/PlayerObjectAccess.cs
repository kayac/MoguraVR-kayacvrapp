using UnityEngine;
using System.Collections;

public class PlayerObjectAccess : SingletonMonoBehaviour<PlayerObjectAccess> {
	public Transform leftHandTarget;
	public Transform rightHandTarget;
	public Transform eyeTarget;
	public GameObject cameraContainer;
	public int playerControllerId;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
