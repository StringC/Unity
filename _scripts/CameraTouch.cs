// 脚本名称：CameraTouch.cs
// 名称解释：控制摄像机
// 功能描述
// 1：缩放
// 2：旋转

using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class CameraTouch : MonoBehaviour {

	public Transform cameraTarget;          //目标物体
	//public Vector3 targetOffset;
	public float averageDistance = 50.0f;   //摄像机围绕对象旋转的半径
	public float xSpeed = 200.0f;           //摄像机左右移动的角度 
	public float ySpeed = 100.0f;           //摄像机上下移动的角度
	public int zoomSpeed = 40;              //摄像机变焦（缩放）速度
	public int yMinLimit = -80;             //摄像机能移动到的最低位置
	public int yMaxLimit = 80;              //摄像机能移动到的最高位置
	public float zoomDampening = 5.0f;      //缩放变化速度
	public float rotateOnOff = 1;           //判断是否旋转所用的值
	public float maxDistance = 20;          //摄像机距离对象的最大距离
	public float minDistance = 0.6f;        //摄像机距离对象的最小距离

	// Use this for initialization

	private float desiredDistance;          //现在的距离 
	private float currentDistance;          //期望与目标之间的目标距离
	private Vector3 position;               //记录角度值
	private Quaternion rotation;            //记录位置值
	private Quaternion currentRotation;     //现在的角度 
	private Quaternion desiredRotation;     //期望与目标之间的目标角度
	//private float idleTimer = 0.0f;         //状态时间
	//private float idleSmooth = 0.0f;        //平稳时间

	private float xDeg = 0.0f;              //摄像机X轴的正方向与世界坐标X轴的正方向之间的角度
	private float yDeg = 0.0f;              //摄像机y轴的正方向与世界坐标y轴的正方向之间的角度

	public Text testText;

	void Start () {
	     Input.multiTouchEnabled=true;       //此属性表明此系统是否支持多点触控
	     Init();
	 }

	void OnEnable() { Init(); }    //先与Start执行，在一个周期中可以反复的发生(Awake → OnEnable → Start)

	// Update is called once per frame
	void Update () {

	 }
	/*
    public void OneTouch(){
        if(Input.touchCount <= 0) {        return;        }
        if(Input.touchCount == 1)//一个手指触摸屏幕
        {if(Input.touches[0].phase == TouchPhase.Began)//开始触屏
            {
                Vector2 m_screenpos=Input.touches[0].position;//自定义的二维坐标向量 记录初始触屏位置
            }else if(Input.touches[0].phase == TouchPhase.Moved)//手指移动
            {   
                //使物体旋转
            //    this.transform.Rotate(new Vector3(-Input.touches[0].deltaPosition.y*0.5f,Input.touches[0].deltaPosition.x*0.5f,0),Space.World);
            }
        }
    }
    */
	public void Init()
	{
	 if (!cameraTarget)
	  {
	      GameObject go = new GameObject("Cam Target");
	      go.transform.position = transform.position + (transform.forward * averageDistance);
	      cameraTarget = go.transform;
	  }
	 //计算出相机和目标点的距离   (半径距离)
	 averageDistance = Mathf.Sqrt(
	          (transform.position.x-cameraTarget.position.x)*(transform.position.x-cameraTarget.position.x) +
	          (transform.position.y-cameraTarget.position.y)*(transform.position.y-cameraTarget.position.y) +
	          (transform.position.z-cameraTarget.position.z)*(transform.position.z-cameraTarget.position.z) 
	      );

	 currentDistance = averageDistance;         
	 desiredDistance = averageDistance;

	 position = transform.position;
	 rotation = transform.rotation;
	 currentRotation = transform.rotation;
	 desiredRotation = transform.rotation;

	 xDeg = Vector3.Angle(Vector3.right, transform.right );
	 yDeg = Vector3.Angle(Vector3.up, transform.up );
	 //position = cameraTarget.position - (rotation * Vector3.forward * currentDistance + targetOffset);
	 //Debug.Log(targetOffset);
	 position = cameraTarget.position - (rotation * Vector3.forward* currentDistance );
}

	 void LateUpdate()
	 {  
	      if(Input.touchCount <= 0)//没有手指触摸屏幕
	       {
	           return;
	       }
		//if (Input.touches[0].position.y > Screen.height*0.15f)
		//{
	      //testText.text ="触摸屏幕Y值："+ Input.touches[0].position.y;
	      //旋转
			if(Input.touchCount == 1)      //一个手指触摸屏幕
			{    

	           if (Input.touches[0].phase == TouchPhase.Began)//开始触屏      phase(Touch)(记录触摸的阶段状态)
	            {
	                //Vector2 m_screenpos = Input.touches[0].position;//自定义的二维坐标向量 记录初始触屏位置
	            }
	           else if(Input.touches[0].phase == TouchPhase.Moved)//手指移动
	            {   

	                xDeg += Input.touches[0].deltaPosition.x * xSpeed * 0.02f;       //deltaPosition(Touch)(距离上次改变的距离增量);
	                yDeg -= Input.touches[0].deltaPosition.y * ySpeed * 0.02f;       //一帧y轴移动的角度
	                yDeg = ClampAngle(yDeg, yMinLimit, yMaxLimit);                   //记录y轴移动的角度

	                desiredRotation = Quaternion.Euler(yDeg, xDeg, 0);               //记录目标角度
	                currentRotation = transform.rotation;                            //记录初始角度
	                rotation = Quaternion.Lerp(currentRotation, desiredRotation, 0.02f  * zoomDampening);    //运用(Quaternion.Lerp)插值平滑变化从初始角度到目标角度的值
	                transform.rotation = rotation;                                    //将变化后的角度赋予摄像机
	                //idleTimer=0;
	                //idleSmooth=0;
	            }    
	       }

	      //（缩放）当有多个手指触屏时 
	      else
          if(Input.touchCount > 1 )//当有多个手指触屏 
		  {

		     //记录两个手指的位置         
		     Vector2 finger1= new Vector2();
		     Vector2 finger2= new Vector2();
		     //记录两个手指的移动距离后的坐标
		     Vector2 mov1=new Vector2();
		     Vector2 mov2=new Vector2();
		     //记录两个手指的移动距离
		     Vector2 movPos1 = new Vector2();
		     Vector2 movPos2 = new Vector2();

		     for (int i=0;i<2;i++)                               //用循环来实现记录position
			     { 
			         Touch touch = Input.touches[i];                 //[i]记录第0个、第1个触屏点的状态
			         if(touch.phase == TouchPhase.Ended)             //如果手指触屏之后离开就break
			              break;
			         if(touch.phase == TouchPhase.Moved)             // 当手指移动时
			          { 
			              float mov =0;                               // 用来记录移动增量

			              float movDisStart = 0.0f;                   //记录两指刚触摸时的两个坐标点的距离
			              float movDisEnd= 0.0f;                      //记录两指触摸完后的两个坐标点的距离

			              float movDisAA = 0.0f;                      //记录finger1的触屏距离
			              float movDisBB = 0.0f;                      //记录finger2的触屏距离
			              if (i == 0) {
			                   finger1=touch.position;                 //触摸到屏幕时的位置（坐标点）

			                   mov1=finger1 + touch.deltaPosition;     //finger1触屏（手指移动）完成后的坐标    deltaPosition(Touch)(距离上次改变的距离增量);
			                   movPos1 =  touch.deltaPosition;         //finger1触屏增量(坐标点)
			                   movDisAA = Mathf.Sqrt((movPos1.x*movPos1.x)+(movPos1.y*movPos1.y));           //计算finger1的触屏距离（移动后两个点的距离）
			                   //movDisStart = Mathf.Sqrt((mov1.x*mov1.x)+(mov1.y*mov1.y));

			               }
			              else
			               {
			                   finger2=touch.position;                      //触摸到屏幕时的位置（坐标点）
			                   mov2=finger2 + touch.deltaPosition;          //finger2触屏（手指移动）完成后的坐标   deltaPosition(Touch)(距离上次改变的距离增量);
			                   movPos2 =  touch.deltaPosition;              //finger2触屏增量（坐标点）
			                   movDisBB = Mathf.Sqrt((movPos2.x*movPos2.x)+(movPos2.y*movPos2.y));           //    计算finger2的触屏距离
			                   if(movDisAA >=movDisBB)                     //判断两指的触屏距离，谁大谁小，取最大值（取大值作为缩放的标准）
			                    {
			                        mov = movDisAA;
			                   }else{
			                        mov = movDisBB;
			                    }

			                   movDisStart =  Mathf.Sqrt(((finger2.x - finger1.x)*(finger2.x - finger1.x))+((finger2.y - finger1.y)*(finger2.y - finger1.y)));           //计算两指刚触摸时的两个坐标点的距离
			                   movDisEnd= Mathf.Sqrt(((mov2.x - mov1.x)*(mov2.x - mov1.x))+((mov2.y - mov1.y)*(mov2.y - mov1.y)));                                       //计算两指刚触摸完成后的两个坐标点的距离
			                   if (movDisStart >=movDisEnd){                //如果触摸完成后的距离大于刚触摸时的距离，说明是两指的动作是外拉，反之则是向里捏
			                        desiredDistance += mov *0.02f * zoomSpeed * Mathf.Abs(desiredDistance);        //外拉的话，相机距离目标物体要越来越远
			                   }else{
			                        desiredDistance -= mov *0.2f  * zoomSpeed * Mathf.Abs(desiredDistance);        //向里捏的话，相机距离目标物体要越来越近
			                    }

			                   desiredDistance = Mathf.Clamp(desiredDistance, minDistance, maxDistance);          //（限定缩放范围）限定desiredDistance的最大值和最小值，超过最大值则取最大值，小于最小值就取最小值，在这个范围内就返回本身的值
			                   currentDistance = Mathf.Lerp(currentDistance, desiredDistance, 0.02f  * zoomDampening);    //平滑改变角度     

			               }
			          }
			     }
		    }

	        /*  //这段代码应该是自动旋转的代码
       else{

           idleTimer+=0.02f ;
           if(idleTimer > rotateOnOff && rotateOnOff > 0){
               idleSmooth+=(0.02f +idleSmooth)*0.005f;
               idleSmooth = Mathf.Clamp(idleSmooth, 0, 1);
               xDeg += xSpeed * 0.001f * idleSmooth;
           }

           yDeg = ClampAngle(yDeg, yMinLimit, yMaxLimit);
           desiredRotation = Quaternion.Euler(yDeg, xDeg, 0);
           currentRotation = transform.rotation;
           rotation = Quaternion.Lerp(currentRotation, desiredRotation, 0.02f  * zoomDampening*2);
           transform.rotation = rotation;

       }*/

	        //position = cameraTarget.position - (rotation * Vector3.forward * currentDistance + targetOffset);
	        position = cameraTarget.position - (rotation * Vector3.forward * currentDistance );
	        transform.position = position;
		//}
	}

	 private static float ClampAngle(float angle, float min, float max)               //取限制角度范围内的值（小于最小值返回最小值，大于最大值返回最大值，不大不小返回当前值）
	 {
	      if (angle < -360)
	           angle += 360;
	      if (angle > 360)
	           angle -= 360;
	      return Mathf.Clamp(angle, min, max);                                         //取限制角度范围内的值(Mathf.Climp)（小于最小值返回最小值，大于最大值返回最大值，不大不小返回当前值）
	  }
} 
