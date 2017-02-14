//脚本名称:ExampleUIControl.cs
//名称解释:AR交互例子脚本，需要自己修改
//功能描述：
//1：显示检测到卡片内容的模型



//假如是卡片识别的话， 一定要在DefaultTrackableEventHandler.cs的  OnTrackingFound() 函数中 最后一句加上 OnTrack.findCardName = mTrackableBehaviour.TrackableName;
// 一定要在DefaultTrackableEventHandler.cs的  OnTrackingFound() 函数中 最后一句加上CardRecognitionArea.isRuned = false;
//在  OnTrackingLost() 函数中 最后一句加上 OnTrack.lostCardName = mTrackableBehaviour.TrackableName;

using UnityEngine;
using System.Collections;
using System.Diagnostics;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Vuforia;
using Debug = UnityEngine.Debug;

public class SheepCardUIControl : MonoBehaviour
{

    //start定义模型的prefab （这个函数是需要自己修改的）-------------------------------------------------------------------------------------------------------------
    [Header("PREFAB")]
    public GameObject sheep_Prefab;
    private GameObject sheepIdle;
    private GameObject bearIdle;
    public GameObject arCamera;
    private GameObject game;
    private GameObject normalTarget;
    private GameObject idle;
    private GameObject gameClone;
    private GameObject idleClone;
    private bool IsStop = true;




    //end

    [Header("UI")]
    public GameObject scannerBoxPanel;

    public Toggle bearToggle;
    public Toggle sheepToggle;
    public Toggle allSceneToggle;
    private bool isAutoRunDetectCard = true;  //设置为false之后，就再也不会为true了，只运行一遍;除非点了 nextCard!
    private OnTrack ont;
    private OffTrack oft;
    private OnOffTrack oot;
    private bool IsfristIdle = true;




    void Awake()
    {
        CardRecognitionArea.OnImportPrefab += ImportPrefab;
        CardRecognitionArea.OnCloseScannerUI += CloseScannerBoxPanel;
        OnOffTrack.OnHideLostCardPrefab += HideLostCardPrefab;

        ont = gameObject.GetComponent<OnTrack>();
        oot = gameObject.GetComponent<OnOffTrack>();
        oft = gameObject.GetComponent<OffTrack>();
    }
    // Use this for initialization
    void Start()
    {
        VuforiaBehaviour.Instance.RegisterVuforiaStartedCallback(OnVuforiaStarted);
        VuforiaBehaviour.Instance.RegisterOnPauseCallback(OnPaused);
    //    NextCard();
    }


    private void OnVuforiaStarted()
    {
        CameraDevice.Instance.SetFocusMode(CameraDevice.FocusMode.FOCUS_MODE_CONTINUOUSAUTO);
    }

    private void OnPaused(bool paused)
    {
        if (!paused)
        {
            CameraDevice.Instance.SetFocusMode(CameraDevice.FocusMode.FOCUS_MODE_CONTINUOUSAUTO);
        }
    }
    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.Home))
        {
            Application.Quit();
        }
   

    }

    void OnDestroy()
    {
        CardRecognitionArea.OnImportPrefab -= ImportPrefab;
        CardRecognitionArea.OnCloseScannerUI -= CloseScannerBoxPanel;
        OnOffTrack.OnHideLostCardPrefab -= HideLostCardPrefab;

    }


    //得到了相对应的prefab，然后实例化相对应的模型
    private void ImportPrefab()
    {
        if (GetPrefab() == null)
        {
            return;
        }

        if (OnTrack.tempPrefab_GB == null)
        {
            OnTrack.tempPrefab_GB = Instantiate(GetPrefab());
          // oot._CardState = CardState.NoCard;
        }

        //此判断是用来自动运行卡片识别到后的该做什么事，因为OnOffTrack.OnHaveCard 委托只有在点击完 OnOffTrack 按扭后才会运行;
        //所以如果没有这个判断的话，识别到卡片了，只是上面的实例化进场景了，但是没有设置父物体一系列之后的操作了;
        //下面这个函数只运行一遍;
        if (isAutoRunDetectCard)
        {
            ont.DetectCard();
            oot.OnOffTrackCard();
            isAutoRunDetectCard = false;
            ShowIdle();
          

        }
        //	Debug.Log ("---------------------------------");
    }
    //通过检测到的卡片名称，得到相对应的prefab;（这个函数是需要自己修改的）-------------------------------------------------------------------------------------------------------------
    private GameObject GetPrefab()
    {
        switch (OnTrack.findCardName)
        {
            case "sheep":
                return sheep_Prefab;

            default:
                return null;
        }
    }

    //隐藏丢失卡片的模型  （这个函数是需要自己修改的）-------------------------------------------------------------------------------------------------------------
    public void HideLostCardPrefab()
    {
        switch (OnTrack.lostCardName)
        {
            case "sheep":
                if (GetTempPrefabGBName() == "Scene(Clone)")
                {
                    HideModel();
                }
                break;
            default:
                break;

        }
    }
    //得到当前prefab的模型的名称
    private string GetTempPrefabGBName()
    {
        if (OnTrack.tempPrefab_GB != null)
        {
            return (OnTrack.tempPrefab_GB.name);
        }
        else
        {
            return "xxxxxx";
        }
    }

    //显示扫描框UI
    private void CloseScannerBoxPanel()
    {
        scannerBoxPanel.SetActive(false);
    }
    //隐藏扫描框UI
    private void OpenScannerBoxPanel()
    {
        scannerBoxPanel.SetActive(true);
    }

    //点击下一张卡片的函数
    public void NextCard()
    {
        //显示扫描UI
        OpenScannerBoxPanel();

        //如果卡片状态为无卡状态，则切换成有卡状态，毕竟需要切换成有卡状态才能继续扫描！
        if (oot._CardState == CardState.NoCard)
        {
            oot.OnOffTrackCard();
        }
        // 重新激活卡片识别脚本
        CardRecognitionArea.NextCard();
        //重新刷新自动运行卡片检测
        isAutoRunDetectCard = true;
        //删除导入进来的模型
        if (OnTrack.tempPrefab_GB != null)
        {
            Destroy(OnTrack.tempPrefab_GB);
        }
        StopAllCoroutines();


    }

    //隐藏模型的网格显示 
    private void HideModel()
    {
        //网格为 SkinnedMeshRenderer 的模型
        foreach (SkinnedMeshRenderer temp in (OnTrack.tempPrefab_GB.GetComponentsInChildren<SkinnedMeshRenderer>()))
        {
            temp.enabled = false;

        }

        //网格为 MeshRenderer 的模型
        foreach (MeshRenderer temp in (OnTrack.tempPrefab_GB.GetComponentsInChildren<MeshRenderer>()))
        {
            temp.enabled = false;

        }
        foreach (Renderer temp in (OnTrack.tempPrefab_GB.GetComponentsInChildren<Renderer>()))
        {
            temp.enabled = false;
        }

    }


    public void ShowIdle()
    {
        if (gameClone != null)
        {
            Destroy(gameClone);
        }

        if (IsStop && idleClone == null && IsfristIdle == false)
        {
            //     Debug.Log("Idle实例化············································");
            idle = Resources.Load("Idle") as GameObject;
            Vector3 prefabsPos = idle.transform.position;
            Quaternion prefabsRot = idle.transform.rotation;
            Vector3 prefabsScale = idle.transform.localScale;
            idleClone = Instantiate(idle, prefabsPos + oft.prefabPostion, prefabsRot) as GameObject;
            idleClone.transform.parent = GameObject.Find("Scene(Clone)").transform;
            idleClone.transform.localScale = prefabsScale;
            IsStop = false;
        }
        if (IsfristIdle)
        {
            idle = Resources.Load("Idle") as GameObject;
            Vector3 prefabsPos = idle.transform.position;
            Quaternion prefabsRot = idle.transform.rotation;
            Vector3 prefabsScale = idle.transform.localScale;
            idleClone = Instantiate(idle, prefabsPos+oft.prefabPostion, prefabsRot) as GameObject;
            idleClone.transform.parent = GameObject.Find("Scene(Clone)").transform;
            idleClone.transform.localScale = prefabsScale;
            IsfristIdle = false;

        }

    }

    public void ShowGame()
    {
        if (idleClone != null)
        {
            Destroy(idleClone);
        }

        if (IsStop && gameClone == null)
        {
            game = Resources.Load("Game") as GameObject;
            Vector3 prefabsPos = game.transform.position;
            Quaternion prefabsRot = game.transform.rotation;
            Vector3 prefabsScale = game.transform.localScale;
            gameClone = Instantiate(game, prefabsPos + oft.prefabPostion, prefabsRot) as GameObject;
            gameClone.transform.parent = GameObject.Find("Scene(Clone)").transform;
            gameClone.transform.localScale = prefabsScale;
            IsStop = false;
        }
    }

    private void FindPrefab()
    {
        sheepIdle = GameObject.Find("SheepIdleE");
        bearIdle = GameObject.Find("BearIdleE");
        normalTarget = GameObject.Find("DefaultTarget");
    }

    public void SheepTransForm(bool ison)
    {
        if (ison)
        {
            sheepToggle.GetComponent<RectTransform>().anchoredPosition3D = Vector3.Lerp(new Vector3(-183, -92, 0),
              new Vector3(-143, -92, 0), Time.deltaTime * 2);
            IsStop = true;
            ShowIdle();
            FindPrefab();
            arCamera.GetComponent<bl_CameraOrbit>().SetTarget(sheepIdle.transform);
            arCamera.GetComponent<bl_CameraOrbit>().DistanceClamp = new Vector2(3, 4);
            //    Invoke("NormalDistance", 1f);  
        }
        else
        {
            sheepToggle.GetComponent<RectTransform>().anchoredPosition3D = Vector3.Lerp(new Vector3(-143, -92, 0),
      new Vector3(-183, -92, 0), Time.deltaTime * 2);
        }

    }

    public void BearTransForm(bool ison)
    {

        if (ison)
        {
            bearToggle.GetComponent<RectTransform>().anchoredPosition3D = Vector3.Lerp(new Vector3(-183, -302, 0),
                new Vector3(-143, -302, 0), Time.deltaTime * 2);
            IsStop = true;
            ShowIdle();
            FindPrefab();
            arCamera.GetComponent<bl_CameraOrbit>().SetTarget(bearIdle.transform);
            arCamera.GetComponent<bl_CameraOrbit>().DistanceClamp = new Vector2(3, 4);
            //    Invoke("NormalDistance", 4f);
        }
        else
        {
            bearToggle.GetComponent<RectTransform>().anchoredPosition3D = Vector3.Lerp(new Vector3(-143, -302, 0),
                new Vector3(-183, -302, 0), Time.deltaTime * 2);
        }
    }

    public void GameTransForm(bool ison)
    {
        if (ison)
        {
            allSceneToggle.GetComponent<RectTransform>().anchoredPosition3D = Vector3.Lerp(new Vector3(-183, -504, 0),
                new Vector3(-143, -504, 0), Time.deltaTime * 2);

            IsStop = true;
            FindPrefab();
            ShowGame();
            arCamera.GetComponent<bl_CameraOrbit>().SetTarget(normalTarget.transform);
            arCamera.GetComponent<bl_CameraOrbit>().DistanceClamp = new Vector2(10, 25);
          
        }
        else
        {
            allSceneToggle.GetComponent<RectTransform>().anchoredPosition3D = Vector3.Lerp(new Vector3(-143, -504, 0),
                new Vector3(-183, -504, 0), Time.deltaTime * 2);
        }

    }


    public void NormalDistance()
    {
        arCamera.GetComponent<bl_CameraOrbit>().DistanceClamp = new Vector2(10, 15);
    }

    public void ReturnLevelSelect()
    {
        SceneManager.LoadScene("LevelSelect");
    }



}//YZ
