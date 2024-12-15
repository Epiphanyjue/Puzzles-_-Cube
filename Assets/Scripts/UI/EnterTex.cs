using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnterTex : MonoBehaviour
{
    public Texture2D tex;
    [Header("深度")]
    [Range(0.3f,0.9f)]
    public float depth=0.6f;
    private Material material;
	void Awake()
    {
        material=this.GetComponent<Material>();
    }

    void Start()
    {
        material.SetTexture("_RoomTex",tex);
        material.SetFloat("_Depth",depth);
    }
}
