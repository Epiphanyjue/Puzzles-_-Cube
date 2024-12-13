using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public  class CubeParent : MonoBehaviour
{
    [Header("可以移动")]
	public bool isMove=true;
    [Header("会受重力")]
    public bool isGravity=true;



    //私有非序列化变量
    private Rigidbody rb;


    void Awake()
    {
        rb=this.GetComponent<Rigidbody>();
    }

    void Start()
    {
        if(isGravity)
        {
            rb.useGravity=true;
        }
        else
        {
            rb.useGravity=false;
            rb.constraints=RigidbodyConstraints.FreezeAll;
        }
    }

    
}
