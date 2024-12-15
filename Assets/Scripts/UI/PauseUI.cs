using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Antlr3.Runtime.Tree;
using UnityEngine;
using UnityEngine.UI;

public class PauseUI : MonoBehaviour
{
    public static PauseUI Instance{get;private set;}


	[Header("模糊背景")]
    public Image background;

    public bool isPaused;
    [Header("暂停UI物体对象")]
    public List<GameObject> uiObjects;

        void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(this);
            return;
        }
    }
    void Start()
    {
        foreach(GameObject objs in uiObjects)
        {
            objs.SetActive(false);
        }
    }
    void Update()
    {
        //暂停部分功能
        if(Input.GetKeyDown(KeyCode.Escape)&&!isPaused)
        {
            Show();
        }
        //恢复功能
        else if(Input.GetKeyDown(KeyCode.Escape)&&isPaused)
        {

            Unshow();
        }
    }

    public void Show()
    {
        isPaused=true;
        Time.timeScale=0;
        Cursor.visible=true;
        Cursor.lockState=CursorLockMode.None;
        foreach(GameObject objs in uiObjects)
        {
            objs.SetActive(true);
        }
    }

    public void Unshow()
    {
        isPaused=false;
        Time.timeScale=1;
        Cursor.visible=false;
        Cursor.lockState=CursorLockMode.Locked;
        foreach(GameObject objs in uiObjects)
        {
            objs.SetActive(false);
        }
    }
}
