using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

//该脚本负责死亡后重生,以及记录重生点
public class StatusManager : MonoBehaviour
{
    [Header("重生点")]
    public List<Transform> transforms;
    [Header("死亡高度")]
    public int deathHeight;
	private Transform player_Transform;
    private Rigidbody player_Rigidbody;
    //减少内存压力,定时检测是否死亡
    private float m_Counter=0f;
    //检测是否需要使用重生点
    private float point_Counter=0f;
    //当前记录的位置
    private Vector3 jumpPos;


    void Awake()
    {
        EventCenter.Instance.Subscribe("Jump",RecordPos);
        player_Transform=GameObject.FindWithTag("Player").GetComponent<Transform>();
        player_Rigidbody=GameObject.FindWithTag("Player").GetComponent<Rigidbody>();
    }

    void Update()
    {
        m_Counter-=Time.deltaTime;

        point_Counter+=Time.deltaTime;


            CheckPlayer();

    }

    void CheckPlayer()
    {
        if(player_Transform.position.y<deathHeight)
        {
            if(point_Counter>5.0f)
            {
                ReAlive();
            }
            else
            {
                PointAlive();
            }
        }
    }

    void RecordPos()
    {
        jumpPos=player_Transform.position;
    }

    //重生
    void ReAlive()
    {
        player_Transform.position=jumpPos+3*Vector3.up;
        player_Rigidbody.velocity=new Vector3(0,0,0);
        point_Counter=0.0f;
    }

    void PointAlive()
    {
        foreach(Transform transform in transforms)
        {
            if(transform.position.z<player_Transform.position.z-3.0f)
            {
                player_Transform.position=transform.position;
                return;
            }
        }
    }
}
