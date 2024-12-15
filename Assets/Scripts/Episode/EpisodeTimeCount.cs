using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;


//在玩家开始移动时开始计时,并且在游戏结束时上传成绩

public class EpisodeTimeCount : MonoBehaviour
{
    //公共变量
	[Header("当前关卡名称")]
    public string episodeName;
    [Header("获取玩家位置")]
    public Transform player_Trans;
    [Header("获取当前场景的计时器")]
    public TextMeshProUGUI timer;
    [Header("当前场景新纪录文字框")]
    public GameObject newRecord;
    private TextMeshProUGUI newRecordText;
    //私有变量
    private Vector3 startPos;
    private bool isStart=false;
    private bool isEnd=false;
    private DateTime m_StartTime;//开始计时时间
    void Awake()
    {
        newRecordText=newRecord.GetComponent<TextMeshProUGUI>();
        newRecord.SetActive(false);
        EventCenter.Instance.Subscribe("Win",LoadAndCompare);
    }
    void OnDisable()
    {
        EventCenter.Instance.Unsubscribe("Win",LoadAndCompare);
    }

    void Start()
    {
        startPos=player_Trans.position;
    }

    void Update()
    {
        if(!isStart&&Vector3.Distance(startPos,player_Trans.position)>2.0f)
        {
            isStart=true;
            m_StartTime=DateTime.Now;
        }
        if(isStart&&!isEnd)
        {
            UpdateTimer();
        }
    }

    void UpdateTimer()
    {
        TimeSpan timeSpan=DateTime.Now-m_StartTime;
        timer.text = timeSpan.ToString(@"mm\:ss\.ff");

    }

    void LoadAndCompare()
    {
        Debug.Log(PlayerPrefs.GetString(episodeName));
        if(PlayerPrefs.HasKey(episodeName))
        {
            string current=PlayerPrefs.GetString(episodeName);
            string now=timer.text;
            switch (string.Compare(now,current))
            {
                case -1:
                    PlayerPrefs.SetString(episodeName,now);
                    newRecordText.text="新纪录"+"\r\n"+now;
                    AudioManager.Instance.Play("NewRecord");
                    newRecord.SetActive(true);
                    return;
                case 0:
                    return;
                case 1:
                    return;
            }
        }

        else
        {
            PlayerPrefs.SetString(episodeName,timer.text);
            newRecordText.text="新纪录"+"\r\n"+timer.text;
            AudioManager.Instance.Play("NewRecord");
            newRecord.SetActive(true);

        }

    }
}
