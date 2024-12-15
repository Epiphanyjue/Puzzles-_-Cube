using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

public class WinnerCamera : MonoBehaviour
{
    [Header("动画时长")]
    [Range(0.3f,3.0f)]
    public float changeDuration=1.0f;
    // 在 Inspector 中拖拽 PostProcessVolume
    public PostProcessVolume postProcessVolume;

    private DepthOfField depthOfField;
    
    void OnEnable()
    {
        EventCenter.Instance.Subscribe("Win",DepthChange);
    }
    void OnDisable()
    {
        EventCenter.Instance.Unsubscribe("Win",DepthChange);
    }

    void Start()
    {
        if (postProcessVolume != null)
        {
            // 获取 PostProcessProfile
            PostProcessProfile profile = postProcessVolume.profile;

            // 尝试获取 DepthOfField 设置
            if (profile.TryGetSettings(out depthOfField))
            {
                // 初始化时修改景深的默认值
                SetDepthOfFieldSettings(15.0f, 1.0f);
            }
            else
            {
                Debug.LogWarning("No DepthOfField effect found in PostProcessProfile.");
            }
        }
        else
        {
            Debug.LogError("PostProcessVolume is not assigned.");
        }
    }

    // 设置景深参数（焦距和光圈）
    void SetDepthOfFieldSettings(float focalDistance, float aperture)
    {
        if (depthOfField != null)
        {
            depthOfField.focusDistance.value = focalDistance; // 设置焦距
            depthOfField.aperture.value = aperture;         // 设置光圈
        }
    }

    void DepthChange()
    {
        StartCoroutine(enumerator());
    }

    IEnumerator enumerator()
    {
        float start_FocalLength=depthOfField.focusDistance.value;
        float elapsedTime=0;
        while(elapsedTime<changeDuration)
        {
            SetDepthOfFieldSettings(Mathf.Lerp(start_FocalLength,0.1f,elapsedTime/changeDuration),1.0f);
            elapsedTime+=Time.deltaTime;
            yield return null;
        }
    }
}
