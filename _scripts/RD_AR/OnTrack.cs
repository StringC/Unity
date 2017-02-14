//脚本名称:DetectCard.cs
//名称解释: 检测卡
//功能描述： 检测到卡之后要做的事情
//1:存储检测到卡片的名称
//2：存储检测到卡片

using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class OnTrack : MonoBehaviour {

	public static GameObject tempPrefab_GB; //这个变量存储检测到卡片对对应的模型
	public static string findCardName;  //这个变量存储检测到卡片的名称
	public static string lostCardName;  //这个变量存储检测到卡片的名称

	public GameObject arCamera;
	public Vector3 prefabPostion;   //将模型坐标调至卡上
    public Sprite OnSprite;
    public GameObject headPanel;
    public GameObject trackBtn;


	// Use this for initialization
	void Start () {
		//注册委托
		OnOffTrack.OnHaveCard += DetectCard ;
	}
	
	// Update is called once per frame
	void Update () {

	}
	void OnDestroy(){
		//注销委托
		OnOffTrack.OnHaveCard -= DetectCard ;
	}

	//开启跟踪后，需要做的事情
	public void DetectCard(){
		if (OnTrack.tempPrefab_GB == null) {
			return;
		}
        trackBtn.GetComponent<Image>().sprite = OnSprite;
        arCamera.GetComponent<bl_CameraOrbit>().enabled = false;
		OnTrack.tempPrefab_GB.transform.parent = GameObject.Find ("Plane_"+ findCardName).transform;
	    OnTrack.tempPrefab_GB.transform.localPosition = prefabPostion;
        headPanel.SetActive(false);
        Debug.Log("——————————————————————卡上");
	}


}
