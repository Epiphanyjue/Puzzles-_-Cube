using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Destination : MonoBehaviour
{
    [Header("转换场景前延时时间")]
    public float waitTime=0.5f;
	void Awake()
    {
        
    }

    void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            EventCenter.Instance.TriggerEvent("Win");

            StartCoroutine(loadNewScene());
        }
    }

    private IEnumerator loadNewScene()
    {
        yield return new WaitForSeconds(waitTime);
        SceneManager.LoadScene("Initial");
    }
}
