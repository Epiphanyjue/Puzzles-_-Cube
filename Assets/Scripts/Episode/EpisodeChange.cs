using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EpisodeChange : MonoBehaviour
{
	[Header("目标关卡名")]
    public string episodeName;
    AsyncOperation async;

    void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            async=SceneManager.LoadSceneAsync(episodeName);
            async.allowSceneActivation=false;
        }
    }
    
    void OnTriggerExit(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            async.allowSceneActivation=true;
        }
    }
}
