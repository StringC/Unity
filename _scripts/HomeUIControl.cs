using UnityEngine;
using System.Collections;

public class HomeUIControl : MonoBehaviour {

	// Use this for initialization
    public GameObject setBtn;
    public GameObject startBtn;
    public PublicUiControl publicUI;
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public void IntoScene()
    {
        Application.LoadLevel("LoadingScene");
    }

    public void SetSwitchOn()
    {
        publicUI.SetPanelSwitchOn();
    }
}
