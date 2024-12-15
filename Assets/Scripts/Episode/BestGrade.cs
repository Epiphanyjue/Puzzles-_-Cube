using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class BestGrade : MonoBehaviour
{
    [Header("关卡名称")]
    public List<string> episodeNames;
	[Header("不同关卡在开始页面的文本")]
    public List<TextMeshProUGUI> initialTexts;

    void OnEnable()
    {
        EventCenter.Instance.Subscribe("ClearRecord",ClearRecord);
    }
    void OnDisable()
    {
        EventCenter.Instance.Unsubscribe("ClearRecord",ClearRecord);
    }

    void Start()
    {
        for(int i=0;i<episodeNames.Count;i++)
        {
            Debug.Log(PlayerPrefs.GetString(episodeNames[i]));
            if(PlayerPrefs.GetString(episodeNames[i])!="")
            {
                initialTexts[i].text=PlayerPrefs.GetString(episodeNames[i]);
            }
            else
            {
                initialTexts[i].text="00:00.00";
            }
        }
    }

    void ClearRecord()
    {
        for(int i=0;i<episodeNames.Count;i++)
        {
            initialTexts[i].text="00:00.00";
        }
    }
}
