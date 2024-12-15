using UnityEngine;

[DisallowMultipleComponent]
public class SkyboxControl : MonoBehaviour
{
    public Material mat;
    
    [SerializeField]
    [Header("旋转角度")]
    float rotateAngle=0f;



    void FixedUpdate()
    {
        rotateAngle+=3*Time.fixedDeltaTime;
        mat.SetFloat("_Rotation",rotateAngle);
    }
}