using System;
using UnityEngine;
using System.Collections;
using System.Runtime.Remoting.Messaging;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelSelectUIControl : MonoBehaviour {

	// Use this for initialization
    public enum levelSprite
    {
       Zero,
       One,
       Two,
    }

    [Header("UI元素")] 
    public GameObject rightBtn;
    public GameObject leftBtn;
    public GameObject classOne;
    public GameObject classTwo;
    public GameObject classThree;
    public Animator animator;
    public Sprite intoSceneSprite;

    [Header("关卡图片数组")] 
    public Sprite[] obj = new Sprite[] { };
   
    
    
    private int count=0;
    private int enum_int ;
    private levelSprite initial=levelSprite.Zero;
    private int classoneIndex=0;
    private int classtwoIndex = 1;
    private int classthreeIndex = 2;
    private bool IsClick = true;
    public AnimatorControl ani;
    public PublicUiControl publicUI;
   // private GameObject temp;


    void Start ()
    {
         MapType();
    //    animator = GetComponent<Animator>();
    }
	
	// Update is called once per frame
	void Update () {
//	Debug.Log(index);
       // Debug.Log(animator.name);
	}

    private void RightButtonSwhitchOn()
    {
        rightBtn.SetActive(true);
    }
    private void RightButtonSwhitchOff()
    {
        rightBtn.SetActive(false);
    }
    private void LeftButtonSwhitchOn()
    {
        leftBtn.SetActive(true);
    }
    private void LeftButtonSwhitchOff()
    {
        leftBtn.SetActive(false);
    }

    public void LeftButtonControl()
    {
        enum_int = (int) initial;
        if (enum_int!=0)
        {
            count -= 1;
        }
        IntToEnum();
        MapType();
        SpriteMoveRight();
    }

    public void RightButtonControl()
    {
        enum_int = (int) initial;
        if (enum_int!=3)
        {
            count += 1;
        }
        IntToEnum();
        MapType();
        SpriteMoveLeft();
    }

    private void IntToEnum()
    {
        switch (count)
        {
            case 0 :
                initial = levelSprite.Zero;
                break;
            case 1:
                initial=levelSprite.One;
                break;
            case 2:
                initial = levelSprite.Two;
                break;
            default:
                break;

        }
    }

    private void MapType()
    {
        switch (initial)
        {
                case levelSprite.Zero:
                LeftButtonSwhitchOff();
                RightButtonSwhitchOn();
                break;
               case levelSprite.One:
                LeftButtonSwhitchOn();
                RightButtonSwhitchOn();
                break;
               case levelSprite.Two:
                LeftButtonSwhitchOn();
                RightButtonSwhitchOff();
                break;
            default:
                break;
                
        }
    }

    private void SpriteMoveLeft()
    {
        classoneIndex++;
        classtwoIndex++;
        classthreeIndex++;
        classOne.GetComponent<Image>().sprite = obj[classoneIndex];
        classTwo.GetComponent<Image>().sprite = obj[classtwoIndex];
        classThree.GetComponent<Image>().sprite = obj[classthreeIndex];
        ClassOneBtnNoarmal();
        IsClick = true;
    }

    private void SpriteMoveRight()
    {
        classoneIndex--;
        classtwoIndex--;
        classthreeIndex--;
        classOne.GetComponent<Image>().sprite = obj[classoneIndex];
        classTwo.GetComponent<Image>().sprite = obj[classtwoIndex];
        classThree.GetComponent<Image>().sprite = obj[classthreeIndex];

    }

    public void IntoClassOneScene()
    {
        if (IsClick)
        {
            if (animator)
            {
                ani.SetNextTrue();     
                ani.SetNormalFalse();
                Debug.Log("第一次点击");
            }
            IsClick = false;
        }
        else
        {

            ani.SetClickTrue();
            classOne.GetComponent<Image>().sprite = intoSceneSprite;
            Invoke("LoadScene", 0.2f);
        }
    }

    private void ClassOneBtnNoarmal()
    {
        ani.SetNormalTrue();
        Debug.Log("回复默认状态");
    }



    public void SetSwitchOn()
    {
        publicUI.SetPanelSwitchOn();
    }



    public void LoadScene()
    {
        Debug.Log("进入场景");
        SceneManager.LoadScene("LoadingScene");

    }


}//YZ
