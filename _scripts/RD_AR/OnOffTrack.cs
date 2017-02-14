//脚本名称:OnOffTrack.cs
//名称解释:开启或关闭场景中AR跟踪功能
//功能描述：
//给场景设置初始跟踪状态（即打开场景是默认开启跟踪的，还是要脱卡状态）



using UnityEngine;
using System.Collections;
using Vuforia;
using System;
using UnityEngine.UI;
using Image = UnityEngine.UI.Image;

public enum CardState{
	HaveCard,   //跟踪状态
	NoCard   //脱卡状态
}

public class OnOffTrack : MonoBehaviour {
	
	public  CardState _CardState;// 记录跟踪状态的变量
	private ObjectTracker tracker;  //vuforia 类下的


	//定义两个 Action 委托
	public static event Action OnNoCard;   //改变为脱卡状态的时候做什么事
	public static event Action OnHaveCard; //改变为跟踪卡片状态的时候做什么事
	public static event Action OnHideLostCardPrefab; //删除丢失卡片相对就的prefab;
	// Use this for initialization

    public GameObject HeadPanel;
	void Start () {
		MatchTrackCard ();

	}
	
	// Update is called once per frame
	void Update () {
	
	}

	//跟踪和脱卡的按扭点击
	public void OnOffTrackCard(){
		if (_CardState == CardState.HaveCard) {
			_CardState = CardState.NoCard;
			MatchTrackCard ();
			if (OnNoCard != null) {
				OnNoCard ();
			}


		}else{
			_CardState = CardState.HaveCard;
			MatchTrackCard ();
			if (OnHaveCard != null) {
				OnHaveCard ();
			}

			if (OnHideLostCardPrefab != null) {
				OnHideLostCardPrefab ();
			}
		}

		//Debug.Log (_CardState);
	}

	//根据场景卡的状态，来开启或者关闭跟踪
	private void MatchTrackCard(){
		if (VuforiaRuntimeUtilities.IsVuforiaEnabled ()) {
			tracker = TrackerManager.Instance.GetTracker<ObjectTracker> ();
			if (_CardState == CardState.HaveCard){
				tracker.Start (); //开启跟踪

			}else{
				tracker.Stop (); //取消跟踪
			}
		}
	}
}
