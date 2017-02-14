// 脚本名称：ImageProcessing.cs
// 名称解释：图像处理
// 功能描述
// 1：截图保存，
// 2：查看已保存的图像

using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System;

public class ImageProcessing : MonoBehaviour
{

    // 储存获取到的图片  
    List<Texture2D> allTex2D = new List<Texture2D>();
    List<string> allimagename = new List<string>();
    List<string> filePaths = new List<string>();    //数组储存获取到的路径

    private int i;
    private GameObject newImageObj;
    private Button button;
    private RectTransform showGreatPicturePanelPos;
    public GameObject showGreatPicturePanel;                     //显示放大图片
    public GameObject imageContent;
    public DateTime t1, t2;
    public GameObject showImagePanel;                  //显示图集面板

    private GridLayoutGroup imageContentManager;

    void Awake()
    {
        showGreatPicturePanel.GetComponent<Image>().sprite = null;
        showGreatPicturePanelPos = showGreatPicturePanel.GetComponent<RectTransform>();
        t1 = DateTime.Now;
        t2 = DateTime.Now;
    }

    // Use this for initialization
    void Start()
    {
        imageContentManager = imageContent.GetComponent<GridLayoutGroup>();
        ScreenshotManager.ScreenshotFinishedSaving += ScreenshotSaved;
        ScreenshotManager.ImageFinishedSaving += ImageSaved;
        showGreatPicturePanelPos.anchoredPosition3D = new Vector3(-1920.0f, -0.0f, 0);
        showGreatPicturePanel.SetActive(true);


    }

    // Update is called once per frame
    void Update()
    {

    }

    // 合影 函数
    public void TakePhoto()                  //截图
    {
        StartCoroutine(ScreenshotManager.Save("手智绘AR森林聚会", "手智绘AR森林聚会", true, true));
    }

    void ScreenshotSaved()
    {
        //Debug.Log ("screenshot finished saving");
        //saved = true;
    }

    void ImageSaved()
    {
        //Debug.Log (texture.name + " finished saving");
        //saved2 = true;
    }

    public void ShowPicture()
    {
        /*
        Debug.Log ("Screen.width :" + Screen.width.ToString());
        float thumbnailWidth = (float) (((Screen.width * 0.575) - 50) * 0.25);
        Debug.Log ("thumbnailWidth :" + thumbnailWidth);
        float thumbnailHeight = thumbnailWidth/1.778f;
        Debug.Log ("thumbnailHeight :" + thumbnailHeight);
        imageContentManager.cellSize = new Vector2 (thumbnailWidth,thumbnailHeight);
        imageContentManager.spacing = new Vector2 (10, 10);
        */
        showImagePanel.SetActive(true);
      //  showImagePanel.SetActive(true);
        string[] dir = Directory.GetFiles(Application.persistentDataPath + "/../../../../DCIM/手智绘AR森林聚会/", "*.png");
        //string[] dir = Directory.GetFiles ("E:/CompanyProject/CloudBookBoy", "*.png");    //获取每张图片的详细路径
        for (int j = 0; j < dir.Length; j++)
        {                                                     //便利每一个路径               
            if (filePaths.Contains(dir[j]) == false)
            {
                filePaths.Add(dir[j]);
            }//把便利的路径添加到数组中 
        }
        float imageContentHeight;                                  //显示图片容器的高度
        if ((float)(((int)(filePaths.Count / 3) + 1) * 225) < 784.0f)
        {
            imageContentHeight = 784.0f;
        }
        else
        {
            imageContentHeight = ((float)((int)(filePaths.Count / 3) + 1) * 225);
        }
        imageContent.GetComponent<RectTransform>().sizeDelta = new Vector2(1339f, imageContentHeight);

        for (i = 0; i < filePaths.Count; i++)
        {                                                    //便利路径数组中的每一条路径

            newImageObj = new GameObject("myimage" + i, typeof(Image));
            newImageObj.transform.parent = imageContent.transform;
            if (allimagename.Contains(newImageObj.name) == false)
            {
                allimagename.Add(newImageObj.name);
            }

            Texture2D texture = new Texture2D(350, 205);                                     //设置图像大小
            texture.LoadImage(getImageByte(filePaths[i]));                                      //（LoadImage：加载图像）（getImageByte：根据图片路径返回图片方法）

            Sprite sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), Vector2.zero);
            newImageObj.GetComponent<Image>().sprite = sprite;
            newImageObj.GetComponent<RectTransform>().localScale = new Vector3(1, 1, 1);

            allTex2D.Add(texture);


        }

        foreach (string imagename in allimagename)
        {
            GameObject objbu = GameObject.Find(imagename);
            objbu.AddComponent<Button>();
            button = objbu.GetComponent<Button>();
            button.onClick.AddListener(delegate()
            {
                this.TouchClick(objbu);
            }
            );
        }
    }

    public void CloseImageContent()                                                  //删除所有图片
    {

        for (int i = 0; i < (imageContent.transform.childCount); i++)
        {
            Destroy(GameObject.Find(imageContent.transform.GetChild(i).name));
        }
        imageContent.GetComponent<RectTransform>().anchoredPosition3D = new Vector3(670, 0, 0);
        showImagePanel.SetActive(false);
    }
    /// <summary>
    /// 根据图片路径返回图片的字节流byte[] 
    /// </summary>
    /// <returns>The image byte.</returns>
    /// <returns>返回的字节流</returns> 
    private static byte[] getImageByte(string imagePath)
    {
        FileStream files = new FileStream(imagePath, FileMode.Open);    //(FileStream:能够对对系统上的文件进行读、写、打开、关闭等操作)(FileMode（是指确定如何打开或创建文件,枚举类型）.Open:打开指定现有文件（打开现有文件）)
        byte[] imgByte = new byte[files.Length];
        files.Read(imgByte, 0, imgByte.Length);                         //对文件的读访问。可从文件中读取数据 (Read())
        files.Close();                                                  //关闭流并释放所有资源，同时将缓冲区的没有写入的数据，写入然后再关闭。
        return imgByte;
    }

    public void TouchClick(GameObject image)                             //双击放大
    {

        t1 = t2;
        t2 = DateTime.Now;
        if (t2 - t1 <= new TimeSpan(0, 0, 0, 0, 500))
        {
            showGreatPicturePanel.GetComponent<Image>().sprite = image.GetComponent<Image>().sprite;
            showGreatPicturePanelPos.anchoredPosition3D = new Vector3(0, 0, 0);
            //father.SetActive (false);
            showImagePanel.SetActive(false);
        }
    }

    public void CloseImagePictureBtnClick()             //关闭大图
    {
        showGreatPicturePanelPos.anchoredPosition3D = new Vector3(-1920.0f, -0.0f, 0);
        showImagePanel.SetActive(true);
    }
}