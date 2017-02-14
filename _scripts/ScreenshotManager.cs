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

        if (Application.platform == RuntimePlatform.Android)   //(App;ication.platform = 返回游戏运行的平台（只读）) ( RuntimePlatform.Android 运行平台（安卓） (RuntimePlatform:运行平台))
        {
            Debug.Log("Android platform detected");

            androidPath = "/../../../../DCIM/" + albumName + "/" + screenshotFilename;
            string path = Application.persistentDataPath + androidPath;                    //(Application.persistentDataPath 包含一个持久数据目录的路径（只读）)
            string pathonly = Path.GetDirectoryName(path);                                 //获取目录名字\返回指定路径字符串组件的目录名字。
            //(Path.GetDirectoryName(path))此方法返回路径的字符串包含的所有字符在首个和最后一个DirectorySeparatorChar或AltDirectorySeparatorChar字符之间。首个分割字符包含在内，但是最后一个分隔符不包含在返回的字符串。
            Directory.CreateDirectory(pathonly);
            Application.CaptureScreenshot(androidPath);                                    //Application.CaptureScreenshot 截屏\捕捉屏幕作为一个PNG文件保存在路径filename。简单来说就是截屏
            //(Application.CaptureScreenshot(androidPath))如果文件已经存在，它将被覆盖。如果在web播放器或者Dashboard窗口中使用该函数，它将不做任何事情。
            AndroidJavaClass obj = new AndroidJavaClass("com.ryanwebb.androidscreenshot.MainActivity");
            //AndroidJavaClass.AndroidJavaClass 构造安卓Java类\从类名className构造一个AndroidJavaClass。
            //这实际上意味着查找类的类型，并分配一个特定类型的java.lang.Class对象。

            while (!photoSaved)
            {
                photoSaved = obj.CallStatic<bool>("scanMedia", path);
                //AndroidJavaClass.CallStatic<变量类型>：在一个类上，调用一个静态Java方法。\调用一个静态Java方法带有一个non-void返回类型，使用通用的版本。
                //AndroidJavaClass.CallStatic:调用一个静态Java方法。调用一个静态方法，返回类型为void，使用普通版本。

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