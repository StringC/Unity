using UnityEngine;
using System.Collections;
using System.Runtime.Remoting.Messaging;
using UnityEngine.UI;

public class PublicUiControl : MonoBehaviour
{

    // Use this for initialization
    [Header("UI元素")] 
    public Sprite musicOn;
    public Sprite musicOff;
    public Sprite soundOn;
    public Sprite soundOff;
    public Image musicBtn;
    public Image soundFxBtn;
    public GameObject setPanel;
    public GameObject closeBtn;
    

    private static bool  Ismusic=true;
    private static  bool IsSoundFx=true;


    [Header("游戏物体")]
    public AudioSource musicAudio;
    public AudioSource soundFxAudio;
	void Start () {

        SoundFxSprite();
        MusicControl();
	    SetPanelSwitchOff();
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public void DecisionMusicSwitch()
    {
        Ismusic = !Ismusic;
        MusicControl();
    }

    private void MusicControl()
    {
        if (Ismusic)
        {
            musicBtn.sprite = musicOn;
            musicAudio.Play();
        }
        else
        {
            musicBtn.sprite = musicOff;
            musicAudio.Pause();
        }
    }

    public void DecisionSoundFxSwitch()
    {
        IsSoundFx = !IsSoundFx;
        SoundFxSprite();
    }

    private void SoundFxSprite()
    {
        if (IsSoundFx)
        {
            soundFxBtn.sprite = soundOn;
        }
        else
        {
            soundFxBtn.sprite = soundOff;
        }
    }

    public void SoudFxControl()
    {
        if (IsSoundFx)
        {
            soundFxAudio.Play();
        }
        else
        {
            soundFxAudio.Pause();
        }
    }

    public void SetPanelSwitchOn()
    {
        setPanel.SetActive(true);
    }

    public void SetPanelSwitchOff()
    {
        setPanel.SetActive(false);
    }
}//YZ
