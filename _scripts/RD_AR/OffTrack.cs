//脚本名称:DetachCard.cs
//名称解释:脱离卡
//功能描述： 脱离卡之后要做的事情
//

using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class OffTrack : MonoBehaviour {
	public GameObject defaultTarget;
	public GameObject arCamera;


	//private Vector3 cameraPostion = new Vector3(9.0f,9.0f,-9.5f);
	public Vector3 prefabPostion;  //将模型坐标调至目标点正中心
    public Sprite OffSprite;
    public GameObject headPanel;
    public GameObject trackBtn;

	// Use this for initialization
	void Start () {
		//注册委托
		OnOffTrack.OnNoCard += DetachCard ;
	}
	
	// Update is called once per frame
	void Update () {
		
	}
	void OnDestroy(){
		//注销委托
		OnOffTrack.OnNoCard -= DetachCard;
	}

	//取消跟踪后，需要做的事情
	public void DetachCard(){
		if (OnTrack.tempPrefab_GB == null) {
			return;
		}
        trackBtn.GetComponent<Image>().sprite = OffSprite;
        arCamera.GetComponent<bl_CameraOrbit>().enabled = true;
		OnTrack.tempPrefab_GB.transform.parent = defaultTarget.transform;
		OnTrack.tempPrefab_GB.transform.localPosition = prefabPostion;
		ShowModel ();
        headPanel.SetActive(true);
        Debug.Log("——————————————————————脱卡");

	}

	//显示模型
	private void ShowModel(){
		//网格为 SkinnedMeshRenderer 的模型
		foreach(SkinnedMeshRenderer temp in (OnTrack.tempPrefab_GB.GetComponentsInChildren<SkinnedMeshRenderer>()))
		{
			temp.enabled = true;

		}
		 
		//网格为 MeshRenderer 的模型
		foreach(MeshRenderer temp in (OnTrack.tempPrefab_GB.GetComponentsInChildren<MeshRenderer>()))
		{
			temp.enabled = true;

		}

	    foreach (Renderer temp in (OnTrack.tempPrefab_GB.GetComponentsInChildren<Renderer>()))
	    {
	        temp.enabled = true;
	    }
	
	}
}
