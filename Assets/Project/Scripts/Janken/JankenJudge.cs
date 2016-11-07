using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class JankenJudge : SingletonMonoBehaviour<JankenJudge> {
	bool isJankenState;
	[SerializeField]
	GameObject[] parts;
	[SerializeField]
	Image[] introImages;
	[SerializeField]
	Image judgeImage1;
	[SerializeField]
	Image judgeImage2;
	[SerializeField]
	Sprite[] introTextures;
	[SerializeField]
	Sprite[] winTexture;
	[SerializeField]
	GameObject planeObj;
	[SerializeField]
	JankenNetwork jankenNet;

	GameObject j_hand1;
	GameObject j_hand2;

	bool isHost;

	// Use this for initialization
	void Start () {
		jankenNet = GetComponent<JankenNetwork> ();
		if (FileUtils.GetPlayerType () == PlayerType.HOST) {
			isHost = true;
		}
		Show (false);
	}
	
	// Update is called once per frame
	void Update () {
		if (!isHost) {
			Show (jankenNet.fullShow);
			ShowJudge (jankenNet.judgeAllShow);
			ShowIntro (jankenNet.introShow);
			transform.position = jankenNet.bodyPosition;
			judgeImage1.transform.position = jankenNet.judgeImage1Pos;
			judgeImage2.transform.position = jankenNet.judgeImage2Pos;
			SetIntroImage (introTextures[jankenNet.introIndex]);
			if (jankenNet.isAiko) {
				Aiko ();
			} else if (jankenNet.judge1Win) {
				WinHand1 ();
			} else {
				WinHand2 ();
			}
			Debug.Log ("aiko:"+jankenNet.isAiko + "j1:"+jankenNet.judge1Win);
		}
	}

	public void StartJanken(GameObject hand1, GameObject hand2, Vector3 pos){
		if (FileUtils.GetPlayerType () != PlayerType.HOST) {
			return;
		}
		if (isJankenState) {
			return;
		}
		isJankenState = true;

		j_hand1 = hand1;
		j_hand2 = hand2;

		transform.position = pos;

		transform.LookAt (hand2.transform.position);

		Vector3 pos1 = judgeImage1.transform.position;
		Vector3 pos2 = judgeImage2.transform.position;

		if (Vector3.Distance (pos1, hand1.transform.position) < Vector3.Distance (pos2, hand1.transform.position)) {

		} else {
			judgeImage1.transform.position = pos2;
			judgeImage2.transform.position = pos1;
		}


		//transform.rotation = Quaternion.Euler (new Vector3 (0,transform.rotation.eulerAngles.y, 0));

		//Quaternion rot = Quaternion.LookRotation (transform.position, hand1.transform.position);

//		rot = Quaternion.Euler (new Vector3 (0, rot.eulerAngles.y, 0));
//		transform.rotation = rot;
		Show (true);
		ShowJudge (false);
		ShowIntro (true);
		StartCoroutine ("JankenRoutine");

		if (isHost) {
			jankenNet.bodyPosition = transform.position;
			jankenNet.judgeImage1Pos = pos1;
			jankenNet.judgeImage2Pos = pos2;
		}
	}

	void Show(bool isshow){
		foreach (GameObject g in parts) {
			g.SetActive (isshow);
		}
		if (isHost) {
			jankenNet.fullShow = isshow;
		}
	}
	void ShowJudge(bool isshow){
		judgeImage1.gameObject.SetActive (isshow);
		judgeImage2.gameObject.SetActive (isshow);
		if (isHost) {
			jankenNet.judgeAllShow = isshow;
		}
	}
	void ShowIntro(bool isshow){
		foreach (Image i in introImages) {
			i.gameObject.SetActive (isshow);
			planeObj.SetActive (isshow);
		}
		if (isHost) {
			jankenNet.introShow = isshow;
		}
	}

	IEnumerator JankenRoutine(){
		SetIntroImage (introTextures [0]);
		SetIntroIndexIfHost(0);
		yield return new WaitForSeconds (1.0f);
		SetIntroImage (introTextures [1]);
		SetIntroIndexIfHost(1);
		yield return new WaitForSeconds (1.0f);
		SetIntroImage (introTextures [2]);
		SetIntroIndexIfHost(2);
		yield return new WaitForSeconds (1.0f);
		SetIntroImage (introTextures [3]);
		SetIntroIndexIfHost(3);
		j_hand1.GetComponent<JankenHand> ().StartJudge ();
		j_hand2.GetComponent<JankenHand> ().StartJudge ();
		yield return new WaitForSeconds (0.7f);
		j_hand1.GetComponent<JankenHand> ().EndJudge ();
		j_hand2.GetComponent<JankenHand> ().EndJudge ();
		Judge ();
		ShowJudge (true);
		ShowIntro (false);
		yield return new WaitForSeconds (2.0f);
		Show (false);
		isJankenState = false;
	}
	void Judge(){
		int hand1Cmd = j_hand1.GetComponent<JankenHand> ().JankenResult ();
		int hand2Cmd = j_hand2.GetComponent<JankenHand> ().JankenResult ();
		if (hand1Cmd == hand2Cmd) {
			Aiko ();
		} else if (hand1Cmd == 0) {
			if (hand2Cmd == 1) {
				WinHand1 ();
			} else {
				WinHand2 ();
			}
		} else if (hand1Cmd == 1) {
			if (hand2Cmd == 0) {
				WinHand2 ();
			} else {
				WinHand1 ();
			}
		} else if (hand1Cmd == 2) {
			if (hand2Cmd == 0) {
				WinHand1 ();
			} else {
				WinHand2 ();
			}
		}
	}
	void Aiko(){
		judgeImage1.sprite = judgeImage2.sprite = winTexture [2];
		jankenNet.judgeStr = "Aiko";
		if (isHost) {
			
			jankenNet.isAiko = true;
		}
	}
	void WinHand1(){
		SetJudgeWin (judgeImage1, true);
		SetJudgeWin (judgeImage2, false);
		jankenNet.judgeStr = "WinHand1";
		if (isHost) {
			jankenNet.isAiko = false;
			jankenNet.judge1Win = true;
		}
	}
	void WinHand2(){
		SetJudgeWin (judgeImage1, false);
		SetJudgeWin (judgeImage2, true);
		jankenNet.judgeStr = "WinHand2";
		if (isHost) {
			jankenNet.isAiko = false;
			jankenNet.judge1Win = false;
		}
	}
	void SetJudgeWin(Image i,bool win){
		if (win) {
			i.sprite = winTexture [0];
		}else{
			i.sprite = winTexture [1];
		}
	}
	void SetIntroImage(Sprite sp){
		foreach (Image i in introImages) {
			i.sprite = sp;
		}
	}
	void SetIntroIndexIfHost(int i){
		if (isHost) {
			jankenNet.introIndex = i;
		}
	}
}
