using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class Inter_Cube : MonoBehaviour
{
    [Header("方块对象列表")]
    public List<string> cubeName;
    [Header("预制件列表")]
    public List<GameObject> prefabs;
    [Header("通过数字键或者滚轮更改高光选中")]
    public Image outline;
    private int currentIndex=0;//当前选中的对象
    // 设置射线检测的Layer
    public LayerMask raycastLayer;
    private Material mat;
    private Collider m_Collider = null;
    [Header("射线检测距离")]
    public float m_distance=3.5f;
    [Header("灰色提示")]
    public TextMeshProUGUI grayPromt;
    void Awake()
    {
        outline.rectTransform.anchoredPosition=new Vector2(-450,-450);
    }
    void Update()
    {
        // 获取屏幕中心的屏幕坐标
        Vector3 screenCenter = new Vector3(Screen.width / 2, Screen.height / 2, 0);

        // 将屏幕坐标转换为世界坐标
        Ray ray = Camera.main.ScreenPointToRay(screenCenter);

        // 检测输入
        HandleCubeSelection();

        // 射线碰撞检测
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, m_distance, raycastLayer))
        {
            // if (m_Collider == null)
            // {
            //     m_Collider = hit.collider;
            // }

            // // 碰撞体特效
            // hit.collider.gameObject.GetComponent<PerObjectMaterialProperties>().rimThreshold=0.001f;
            // if (m_Collider != hit.collider)
            // {
            //     m_Collider.gameObject.GetComponent<PerObjectMaterialProperties>().rimThreshold=0.73f;
            //     m_Collider = hit.collider;
            // }

            // 获取命中的物体（假设是一个立方体）
            GameObject hitObject = hit.collider.gameObject;

            if (Input.GetMouseButtonDown(1)&&BagManager.Instance.GetItemCount(cubeName[currentIndex])>0&&!hitObject.CompareTag("GrayCube")&&!hitObject.CompareTag("PurpleCube"))
            {
                BagManager.Instance.RemoveItem(cubeName[currentIndex],1);
                // 获取碰撞的面法线
                Vector3 hitNormal = hit.normal;

                // 判断是哪一面
                Vector3 cubePosition = hitObject.transform.position;

                // 创建新的立方体
                GameObject newCube = Instantiate(prefabs[currentIndex]);
                AudioManager.Instance.Play("Put");

                // 根据法线确定放置新的立方体的位置
                Vector3 placePosition = cubePosition + hitNormal; // 0.5f是新立方体的大小一半
                newCube.transform.position = placePosition;

                // 将新立方体放置在合适的面上
                newCube.transform.rotation = Quaternion.LookRotation(hitNormal);
                Debug.Log("生成新立方体");
            }
            //试图在灰色方块上放置时出现提示
            else if((hitObject.CompareTag("GrayCube")||hitObject.CompareTag("PurpleCube"))&&Input.GetMouseButtonDown(1)&&BagManager.Instance.GetItemCount(cubeName[currentIndex])>0)
            {
                grayPromt.text="无法放置";
                StartCoroutine(grayPromtShow());
            }
            else if(Input.GetMouseButtonDown(1)&&BagManager.Instance.GetItemCount(cubeName[currentIndex])<=0)
            {
                Debug.Log("数量不足");
            }

            // 收取立方体
            if (Input.GetMouseButtonDown(0) && hitObject.GetComponent<CubeParent>().isMove)
            {
                BagManager.Instance.AddItem(hitObject.tag, 1);
                AudioManager.Instance.Play("Get");
                Destroy(hitObject);
            }
        }
    }
    //协程处理Promt
    IEnumerator grayPromtShow()
    {
        yield return new WaitForSeconds(1.0f);
        grayPromt.text="";
    }
    void HandleCubeSelection()
    {
        float scrollInput=Input.GetAxis("Mouse ScrollWheel");
        // 如果滚轮向上滚动
        if (scrollInput > 0f)
        {
            // 增加数字
            currentIndex--;
        }
        // 如果滚轮向下滚动
        else if (scrollInput < 0f)
        {
            // 减小数字
            currentIndex++;
        }

        // 保证数字在0到3之间循环
        if (currentIndex > 3)
        {
            currentIndex = 0; // 超过3时，循环回到0
        }
        else if (currentIndex < 0)
        {
            currentIndex = 3; // 低于0时，循环回到3
        }
        if(Input.GetKeyDown(KeyCode.Alpha1))
        {
            currentIndex=0;

        }
        else if(Input.GetKeyDown(KeyCode.Alpha2))
        {
            currentIndex=1;

        }
        else if(Input.GetKeyDown(KeyCode.Alpha3))
        {
            currentIndex=2;

        }
        else if(Input.GetKeyDown(KeyCode.Alpha4))
        {
            currentIndex=4;
   
        }

        switch (currentIndex)
        {
            case 0:
                outline.rectTransform.anchoredPosition=new Vector2(-450,-450);
                break;
            case 1:
                outline.rectTransform.anchoredPosition=new Vector2(-150,-450);
                break;
            case 2:
                outline.rectTransform.anchoredPosition=new Vector2(150,-450);
                break;
            case 3:
                outline.rectTransform.anchoredPosition=new Vector2(450,-450);
                break;
        }

    }
}
