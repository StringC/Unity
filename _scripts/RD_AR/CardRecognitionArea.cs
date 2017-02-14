//脚本名称：CardRecognitionArea.cs
//名称解释：卡片识别区域
//功能描述:
//1:场景中有多个识别卡，每一个识别卡下有一个plane子物体，这个脚本需要挂在这个plane物体上，plane大小和识别图大小保持一致
//2：要注意的事项，当前屏分辨率和制作时候的分辨率会有所不同
//3：要注意的事项，plane物体的scale值和父物体的scale值要记得相乘;

using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;

public class CardRecognitionArea : MonoBehaviour {
	


	//申请一个CanvasScaler组件
	private CanvasScaler canS;
	//记录当前屏幕分辨率的 宽/高的比率值;
	private float x_ScreenProportion;
	//记录plane物体的【整体】缩放值为1.0f 
	//plane本身与父物体的缩放比例，假如inspector面版上 plane显示 的缩放为0.1f,而父物体的的绽放值为50，现在定义的这个变量值为5.0f
	private float objScale = 0.75f;

	//记录UI 检测范围的四个点坐标
	private Vector2 topLeftUI;  
	private Vector2 LeftBottomUI;
	private Vector2 topRightUI;
	private Vector2 RightBottomUI;

	//记录 plane 的宽和高的1/2;
	private Vector2 planeWH;

	//记录 plane 的世界坐标 （这个面片的父物体是识别卡片）0
	private Vector3 topLeftPlaneWD;
	private Vector3 LeftBottomPlaneWD;
	private Vector3 topRightPlaneWD;
	private Vector3 RightBottomPlaneWD;

	//记录 plane 转换成平面坐标的四个点的坐标
	private Vector2 topLeftPlaneSC;  
	private Vector2 LeftBottomPlaneSC;  
	private Vector2 topRightPlaneSC;  
	private Vector2 RightBottomPlaneSC;  

	// 记录 plane 状态
	// 只有同时满足这两个条件，才是最正确的判断
	public static bool isInAreaNormal;  //正着扫的时候，满足那八个点之间的判断，为真
	public static bool isInAreaInvert;  //倒着扫的时候，满足那八个点之间的判断，为真

	//
	public static bool isFirstReco = true;  //判断是否是第一次识别（也可能是点击了下一个按扭）（只有第一次识别的时候会有扫描框，识别到了之后，就不需要扫描框了）
	public static bool isRecSuccess = false;  //记录是否已经识别成功

	//存储三个材质
	private Material greenMat ;
	private Material redMat;
	private Material transparentMat ;
	public static bool isRuned = false;  // 协同程序是否运行过  true： 运行过   false： 没有运行过


	//关闭扫描UI
	public static event Action OnCloseScannerUI;
	//检测到卡片，导入模型
	public static event Action OnImportPrefab;

	//记录plane物体的比例 （正方形，比例为1.0f） 长方形时，取值为0-1之间
	private float cardScale = 1.0f;

	void Awake(){
		greenMat = (Material) Resources.Load("materials/Green");
		redMat =(Material) Resources.Load("materials/Red");
		transparentMat = (Material) Resources.Load("materials/Transparent");

	}

	// Use this for initialization
	void Start () {
		
		Debug.Log (greenMat);
		canS = GameObject.Find ("Canvas").GetComponent<CanvasScaler> ();
		x_ScreenProportion = Screen.width / canS.referenceResolution.x;
		gameObject.GetComponent<Renderer> ().material = transparentMat;

	

	}
	
	// Update is called once per frame
	void Update () {
		
	

		if (isFirstReco == true || isRecSuccess == false)
		{
			RecognitionArea ();
		}

		//Debug.Log ("isFirstReco:"+isFirstReco.ToString());
		//Debug.Log ("isRecSuccess:"+isRecSuccess.ToString());
        //Debug.Log("isInAreaNormal++++++++:" + isInAreaNormal);
        //Debug.Log("isInAreaInvert++++++++:" + isInAreaInvert);
	}

	// 识别区域功能
	public void RecognitionArea(){
		//计算UI在屏幕上的坐标;
		//x_ScreenProportion 是当前屏幕分辨率的 宽/高的比率值;
		//左上角  x =（屏幕宽度 - 识别UI宽度）*0.5
		//左上角  y = (屏幕高度 + 识别UI高度) *0.5
		//topLeftUI = new Vector2 ((Screen.width - 1200)*0.5f,(Screen.height+ 750) *0.5f);
		topLeftUI = new Vector2 ((Screen.width - 1200*x_ScreenProportion),(Screen.height+ 750*x_ScreenProportion)) *0.5f;
		//左下角  x =（屏幕宽度 - 识别UI宽度）*0.5
		//左下角  y = (屏幕高度 - 识别UI高度) *0.5
		//LeftBottomUI = new Vector2 ((Screen.width - 1200)*0.5f,(Screen.height- 750) *0.5f);
		LeftBottomUI = new Vector2 ((Screen.width - 1200*x_ScreenProportion),(Screen.height- 750*x_ScreenProportion)) *0.5f;
		//右上角  x =（屏幕宽度 - 识别UI宽度）*0.5
		//右上角  y = (屏幕高度 + 识别UI高度) *0.5
		//topRightUI = new Vector2 ((Screen.width + 1200)*0.5f,(Screen.height+ 750) *0.5f);
		topRightUI = new Vector2 ((Screen.width + 1200*x_ScreenProportion),(Screen.height+ 750*x_ScreenProportion)) *0.5f;
		//右下角  x =（屏幕宽度 + 识别UI宽度）*0.5
		//右下角  y = (屏幕高度 - 识别UI高度) *0.5
		//RightBottomUI = new Vector2 ((Screen.width + 1200)*0.5f,(Screen.height- 750) *0.5f);
		RightBottomUI = new Vector2 ((Screen.width + 1200*x_ScreenProportion),(Screen.height- 750*x_ScreenProportion)) *0.5f;


		// 计算plane 的宽和高的1/2;
		planeWH = new Vector2 (gameObject.GetComponent<MeshFilter>().mesh.bounds.size.x,(gameObject.GetComponent<MeshFilter>().mesh.bounds.size.z )* cardScale)*0.5f*objScale; //

		// 计算plane的世界坐标; plane有父物体,所以计算的是父物体的中心点坐标，再偏移出plane尺寸的四个点的世界坐标;
		topLeftPlaneWD = gameObject.transform.parent.position + new Vector3 (-planeWH.x,0,planeWH.y);
		LeftBottomPlaneWD  = gameObject.transform.parent.position + new Vector3 (-planeWH.x,0,-planeWH.y);
		topRightPlaneWD = gameObject.transform.parent.position + new Vector3 (planeWH.x,0,planeWH.y);
		RightBottomPlaneWD = gameObject.transform.parent.position + new Vector3 (planeWH.x,0,-planeWH.y);

		// 计算将世界坐标转换成屏幕坐标
		topLeftPlaneSC = Camera.main.WorldToScreenPoint(topLeftPlaneWD);
		LeftBottomPlaneSC = Camera.main.WorldToScreenPoint(LeftBottomPlaneWD);
		topRightPlaneSC = Camera.main.WorldToScreenPoint(topRightPlaneWD);
		RightBottomPlaneSC = Camera.main.WorldToScreenPoint(RightBottomPlaneWD);

		// 识别图正着放时 比较这8个坐标点
		if (topLeftPlaneSC.x > topLeftUI.x && topLeftPlaneSC.y < topLeftUI.y && LeftBottomPlaneSC.x > LeftBottomUI.x && LeftBottomPlaneSC.y > LeftBottomUI.y && topRightPlaneSC.x < topRightUI.x && topRightPlaneSC.y < topRightUI.y && RightBottomPlaneSC.x < RightBottomUI.x && RightBottomPlaneSC.y > RightBottomUI.y)
		{
			//Debug.Log("识别成功");
			isInAreaNormal = true;


		}else{
			//Debug.Log("识别失败");

			isInAreaNormal = false;
		
		}

		// 识别图倒着放时 比较这8个坐标点
		if(topLeftPlaneSC.x < RightBottomUI.x && topLeftPlaneSC.y > RightBottomUI.y && LeftBottomPlaneSC.x < topRightUI.x &&  LeftBottomPlaneSC.y < topRightUI.y && topRightPlaneSC.x > LeftBottomUI.x && topRightPlaneSC.y > LeftBottomUI.y && RightBottomPlaneSC.x >topLeftUI.x && RightBottomPlaneSC.y <topLeftUI.y)
		{
			isInAreaInvert = true;

		}else
		{
			isInAreaInvert = false;

		}

		if ( isInAreaNormal == true && isInAreaInvert == true && isRuned == false )  //只有当这两种情况都满足的条件下，才确保识别图是完全处于这个扫描框内
		{
			//在这个扫描框内，需要做什么事
			if (isRecSuccess == false) //识别状态是不成功时
			{
                //Debug.Log("isInAreaNormal--------:" + isInAreaNormal);
                //Debug.Log("isInAreaInvert--------:" + isInAreaInvert);
				gameObject.GetComponent<Renderer> ().material = transparentMat;


					StartCoroutine ("RecSuccess");

				isFirstReco = false;
				isRecSuccess = true;
				isInAreaInvert = false;
				isInAreaNormal = false;
			//	Debug.Log("识别成功");
			}
		//	Debug.Log("在范围内");
		}else
		{
			//不在这个扫描框内，需要做什么事
            gameObject.GetComponent<Renderer>().material = transparentMat;
				//Debug.Log("在范围外");
				isRecSuccess = false;
		}

	}

	IEnumerator  RecSuccess(){
			isRuned = true;
	//	Debug.Log ("前");
			yield return new WaitForSeconds (0.5f);
			gameObject.GetComponent<Renderer> ().material = transparentMat;

			//识别成功后，导入模型;
			if (OnImportPrefab != null) {
				OnImportPrefab ();
			}
			if (OnCloseScannerUI != null) {
				OnCloseScannerUI ();
			}
	//	Debug.Log ("后");
		
	}

	public static void NextCard(){
		isFirstReco = true;
		isRecSuccess = false;

	}
		
		
}
