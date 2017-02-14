#pragma warning disable 0168 // variable declared but not used.
#pragma warning disable 0219 // variable assigned but not used.

using UnityEngine;
using System;
using System.IO;
using System.Collections;
using System.Runtime.InteropServices;

public class ScreenshotManager : MonoBehaviour
{

    public static event Action ScreenshotFinishedSaving;
    public static event Action ImageFinishedSaving;

    public static string androidPath;

#if UNITY_IPHONE
	
	[DllImport("__Internal")]
    private static extern bool saveToGallery( string path );
	
#endif

    public static IEnumerator Save(string fileName, string albumName = "MyScreenshots", bool callback = false, bool IsHideUI = false)
    {
        ///DM  +++
        Canvas allCanvas = GameObject.Find("Canvas").GetComponent<Canvas>();
        if (IsHideUI)
        {
            allCanvas.enabled = false;
        }

        bool photoSaved = false;

        //string date = System.DateTime.Now.ToString("dd-MM-yy");
        string date = System.DateTime.Now.ToString("yyyyMMdd_HHmmss");
        //ScreenshotManager.ScreenShotNumber++;
        //string screenshotFilename = fileName + "_" + ScreenshotManager.ScreenShotNumber + "_" + date + ".png";
        string screenshotFilename = fileName + "_" + date + ".png";
        Debug.Log("Save screenshot " + screenshotFilename);

#if UNITY_IPHONE
		
			if(Application.platform == RuntimePlatform.IPhonePlayer) 
			{
				Debug.Log("iOS platform detected");
				
				string iosPath = Application.persistentDataPath + "/"  + albumName + "/"+ screenshotFilename;
		
				Application.CaptureScreenshot(screenshotFilename);
				
				while(!photoSaved) 
				{
					photoSaved = saveToGallery( iosPath );
					
					yield return new WaitForSeconds(.5f);
				}
			
				iPhone.SetNoBackupFlag( iosPath );
			
			} else {
			
				Application.CaptureScreenshot(screenshotFilename);
			
			}
			
#elif UNITY_ANDROID

        if (Application.platform == RuntimePlatform.Android)   //(App;ication.platform = ������Ϸ���е�ƽ̨��ֻ����) ( RuntimePlatform.Android ����ƽ̨����׿�� (RuntimePlatform:����ƽ̨))
        {
            Debug.Log("Android platform detected");

            androidPath = "/../../../../DCIM/" + albumName + "/" + screenshotFilename;
            string path = Application.persistentDataPath + androidPath;                    //(Application.persistentDataPath ����һ���־�����Ŀ¼��·����ֻ����)
            string pathonly = Path.GetDirectoryName(path);                                 //��ȡĿ¼����\����ָ��·���ַ��������Ŀ¼���֡�
            //(Path.GetDirectoryName(path))�˷�������·�����ַ��������������ַ����׸������һ��DirectorySeparatorChar��AltDirectorySeparatorChar�ַ�֮�䡣�׸��ָ��ַ��������ڣ��������һ���ָ����������ڷ��ص��ַ�����
            Directory.CreateDirectory(pathonly);
            Application.CaptureScreenshot(androidPath);                                    //Application.CaptureScreenshot ����\��׽��Ļ��Ϊһ��PNG�ļ�������·��filename������˵���ǽ���
            //(Application.CaptureScreenshot(androidPath))����ļ��Ѿ����ڣ����������ǡ������web����������Dashboard������ʹ�øú��������������κ����顣
            AndroidJavaClass obj = new AndroidJavaClass("com.ryanwebb.androidscreenshot.MainActivity");
            //AndroidJavaClass.AndroidJavaClass ���찲׿Java��\������className����һ��AndroidJavaClass��
            //��ʵ������ζ�Ų���������ͣ�������һ���ض����͵�java.lang.Class����

            while (!photoSaved)
            {
                photoSaved = obj.CallStatic<bool>("scanMedia", path);
                //AndroidJavaClass.CallStatic<��������>����һ�����ϣ�����һ����̬Java������\����һ����̬Java��������һ��non-void�������ͣ�ʹ��ͨ�õİ汾��
                //AndroidJavaClass.CallStatic:����һ����̬Java����������һ����̬��������������Ϊvoid��ʹ����ͨ�汾��

                yield return new WaitForSeconds(.5f);
                // DM +++++	
                allCanvas.enabled = true;
            }

        }
        else
        {

            Application.CaptureScreenshot(screenshotFilename);

        }
#else
			
			while(!photoSaved) 
			{
				yield return new WaitForSeconds(.5f);
		
				Debug.Log("Screenshots only available in iOS/Android mode!");
			
				photoSaved = true;
			}
		
#endif

        if (callback)
            ScreenshotFinishedSaving();
    }


    public static IEnumerator SaveExisting(string filePath, bool callback = false)
    {
        bool photoSaved = false;

        Debug.Log("Save existing file to gallery " + filePath);

#if UNITY_IPHONE
		
			if(Application.platform == RuntimePlatform.IPhonePlayer) 
			{
				Debug.Log("iOS platform detected");
				
				while(!photoSaved) 
				{
					photoSaved = saveToGallery( filePath );
					
					yield return new WaitForSeconds(.5f);
				}
			
				iPhone.SetNoBackupFlag( filePath );
			}
			
#elif UNITY_ANDROID

        if (Application.platform == RuntimePlatform.Android)
        {
            Debug.Log("Android platform detected");

            AndroidJavaClass obj = new AndroidJavaClass("com.ryanwebb.androidscreenshot.MainActivity");

            while (!photoSaved)
            {
                photoSaved = obj.CallStatic<bool>("scanMedia", filePath);

                yield return new WaitForSeconds(.5f);
            }

        }

#else
			
			while(!photoSaved) 
			{
				yield return new WaitForSeconds(.5f);
		
				Debug.Log("Save existing file only available in iOS/Android mode!");

				photoSaved = true;
			}
		
#endif

        if (callback)
            ImageFinishedSaving();
    }


    public static int ScreenShotNumber
    {
        set { PlayerPrefs.SetInt("screenShotNumber", value); }

        get { return PlayerPrefs.GetInt("screenShotNumber"); }
    }
}