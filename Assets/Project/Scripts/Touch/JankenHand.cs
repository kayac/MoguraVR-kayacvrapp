using UnityEngine;
using System.Collections;
using OVRTouchSample;

[RequireComponent(typeof(TargetFollower))]

public class JankenHand : MonoBehaviour {
	[SerializeField]
	private OVRInput.Controller m_handedness;
	[SerializeField]
	private int maxSameCount;
	private NetworkHand m_networkhand;

	private float m_flex = 0.0f;
	private float m_flex2 = 0.0f;
	private float m_point = 0.0f;
	private float m_thumbsUp = 0.0f;
	private bool m_btn1;
	private bool m_btn2;
	[SerializeField]
	private TrackedController m_trackedController = null;
	private Animator m_animator;
	private int jankenIdx;
	private int preJankenIdx;
	private int sameIdxCount;
	private TargetFollower m_targetFollower;

	private int count_gu,count_tyoki,count_pa;
	private bool onJudge;

	public bool isAuthority;

	public const float INPUT_RATE_CHANGE = 20.0f;

	public OVRInput.Controller Handedness { get { return m_handedness; } }

	// Use this for initialization
	void Start () {
		m_trackedController = TrackedController.GetController(m_handedness);
		m_animator = GetComponent<Animator> ();
		m_targetFollower = GetComponent<TargetFollower> ();
		m_networkhand = GetComponent<NetworkHand> ();

	}
	public void SetupAutority(){
		isAuthority = true;
		m_targetFollower = GetComponent<TargetFollower> ();
		m_trackedController = TrackedController.GetController(m_handedness);
		if (m_handedness == OVRInput.Controller.LTouch) {
			m_targetFollower.target = PlayerObjectAccess.Instance.leftHandTarget;
		} else if (m_handedness == OVRInput.Controller.RTouch) {
			m_targetFollower.target = PlayerObjectAccess.Instance.rightHandTarget;
		}

	}
	// Update is called once per frame
	void Update () {
		if (isAuthority) {
			m_flex = m_trackedController.GripTrigger;
			m_flex2 = m_trackedController.Trigger;
			m_point = InputValueRateChange (m_trackedController.IsPoint, m_point);
			m_thumbsUp = InputValueRateChange (m_trackedController.IsThumbsUp, m_thumbsUp);
			m_btn1 = m_trackedController.Button1;
			m_btn2 = m_trackedController.Button2;

			if (m_flex == 0 && m_flex2 == 0 && !m_btn1 && !m_btn2) {
				//paa
				jankenIdx = 2;
			} else if (m_flex2 == 0) {
				//tyoki
				if (m_btn1 || m_btn2) {
					jankenIdx = 1;
				}
			} else {
				//gu
				jankenIdx = 0;
			}
				
			if (jankenIdx == preJankenIdx) {
				sameIdxCount++;
			} else {
				sameIdxCount = 0;
			}

			if (sameIdxCount > maxSameCount) {
				m_animator.SetInteger ("jindex", jankenIdx);
			}
			m_networkhand.CmdChangeJankenIdx(jankenIdx);

			if (onJudge) {
				switch (jankenIdx) {
				case 0:
					count_gu++;
					break;
				case 1:
					count_tyoki++;
					break;
				case 2:
					count_pa++;
					break;
				}
			}

			preJankenIdx = jankenIdx;

		
		} else {
			if (onJudge) {
				switch (m_networkhand.jankenIndex) {
				case 0:
					count_gu++;
					break;
				case 1:
					count_tyoki++;
					break;
				case 2:
					count_pa++;
					break;
				}
			}
		}
	}
	void OnTriggerEnter(Collider other){
		if (other.tag == "hand") {
			if (FileUtils.GetPlayerType () == PlayerType.HOST) {
				Vector3 pos = transform.position + 0.5f * (other.transform.position - transform.position);
				JankenJudge.Instance.StartJanken (gameObject, other.gameObject, pos);
			}
		}
	}


	private float InputValueRateChange(bool isDown, float value)
	{
		float rateDelta = Time.deltaTime * INPUT_RATE_CHANGE;
		float sign = isDown ? 1.0f : -1.0f;
		return Mathf.Clamp01(value + rateDelta * sign);
	}

	public void StartJudge(){
		count_gu = 0;
		count_pa = 0;
		count_tyoki = 0;
		onJudge = true;
	}

	public void EndJudge(){
		onJudge = false;
	}

	public int JankenResult(){
		int _max = Mathf.Max (count_gu, count_tyoki, count_pa);
		if (_max == count_gu) {
			return 0;
		} else if (_max == count_tyoki) {
			return 1;
		} else {
			return 2;
		}
	}
}
