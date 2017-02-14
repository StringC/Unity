using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoadControl : MonoBehaviour {

	// Use this for initialization
    private AsyncOperation async;
    public Slider slider;
	void Start ()
	{
	    StartCoroutine(LoadScene());

	}
	
	// Update is called once per frame
	void Update () {

	}

    IEnumerator LoadScene()
    {
        async = SceneManager.LoadSceneAsync("Sheep");
        int startProgress = 0;
        int stopProgress = 0;
        async.allowSceneActivation = false;
        while (async.progress<0.9f)
        {
            stopProgress = (int) (async.progress*100);
            while (startProgress<stopProgress)
            {
                ++startProgress;
                SliderValue(startProgress);
            }
            yield return new WaitForEndOfFrame();
        }
        stopProgress = 100;
        while (startProgress < stopProgress)
        {
            ++startProgress;
            SliderValue(startProgress);
            yield return new WaitForEndOfFrame();
        }
      //  yield return new WaitForSeconds(5f);
        async.allowSceneActivation = true;
    }


    private void SliderValue(int _value)
    {
        slider.value = _value/100;
    }
}//YZ
