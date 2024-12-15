using UnityEngine;

[DisallowMultipleComponent]
public class PerObjectMaterialProperties : MonoBehaviour
{
    static int lightColorId = Shader.PropertyToID("_LightColor");//定义shaderID
    static int darkColorId=Shader.PropertyToID("_DarkColor");
    static int outlineColorId=Shader.PropertyToID("_OutLineColor");
    static int _RimThreshold=Shader.PropertyToID("_RimThreshold");
    [SerializeField]
    [Header("亮面颜色")]
    Color lightColor = Color.white;//开放颜色汇入接口
    [Header("暗面颜色")]
    public Color darkColor=Color.white;
    [Space(30)]
    [Header("描边HDR强度")]
    [Range(1,5)]
    public float m_Intensity=1;
    [Header("描边颜色")]
    public Color outlineColor=Color.white;
    [Header("菲涅尔效果强度")]
    [Range(0,1)]
    public float rimThreshold=0.732f;

    static MaterialPropertyBlock block;//为了设置材质的属性而定义的接口

    void OnValidate()
    {
        if (block == null)
        {
            block = new MaterialPropertyBlock();//Block缺失
        }
        block.SetColor(lightColorId,lightColor);//设置颜色到接口上
        block.SetColor(darkColorId,darkColor);
        block.SetColor(outlineColorId,outlineColor*m_Intensity);
        block.SetFloat(_RimThreshold,rimThreshold);
        GetComponent<Renderer>().SetPropertyBlock(block);//设置接口到我们的渲染里面
    }
    //【使用函数】
    void Awake()
    {
        OnValidate();
    }

}