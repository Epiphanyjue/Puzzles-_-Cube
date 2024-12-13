using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum CubeName
{
    WhiteCube,
    GreenCube
}

public class Inter_Cube : MonoBehaviour
{
    [Header("方块对象列表")]
    public CubeName cubeName;
    // 设置射线检测的Layer
    public LayerMask raycastLayer;
    private Material mat;
    private Collider m_Collider = null;
    public GameObject cubePrefab;
    [Header("射线检测距离")]
    public int m_distance=5;
    private string currentCube="WhiteCube";

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
            if (m_Collider == null)
            {
                m_Collider = hit.collider;
            }

            // 碰撞体特效
            hit.collider.gameObject.GetComponent<Renderer>().material.SetColor("_LightColor",Color.black);
            if (m_Collider != hit.collider)
            {
                m_Collider.gameObject.GetComponent<Renderer>().material.SetColor("_LightColor",Color.white);
                m_Collider = hit.collider;
                Debug.Log("发现新碰撞体");
            }

            // 获取命中的物体（假设是一个立方体）
            GameObject hitObject = hit.collider.gameObject;

            if (Input.GetKeyDown(KeyCode.F)&&BagManager.Instance.GetItemCount(currentCube)>0)
            {
                Debug.Log("尝试放置方块");
                BagManager.Instance.RemoveItem(currentCube,1);
                // 获取碰撞的面法线
                Vector3 hitNormal = hit.normal;

                // 判断是哪一面
                Vector3 cubePosition = hitObject.transform.position;

                // 创建新的立方体
                GameObject newCube = Instantiate(cubePrefab);

                // 根据法线确定放置新的立方体的位置
                Vector3 placePosition = cubePosition + hitNormal; // 0.5f是新立方体的大小一半
                newCube.transform.position = placePosition;

                // 将新立方体放置在合适的面上
                newCube.transform.rotation = Quaternion.LookRotation(hitNormal);
                Debug.Log("生成新立方体");
            }
            else if(BagManager.Instance.GetItemCount(currentCube)<=0)
            {
                Debug.Log("数量不足");
            }

            // 收取立方体
            if (Input.GetKeyDown(KeyCode.R) && hitObject.GetComponent<CubeParent>().isMove)
            {
                BagManager.Instance.AddItem(hitObject.tag, 1);
                Destroy(hitObject);
            }
        }
        else
        {
            // 如果射线没有碰到任何物体
            Debug.Log("No hit");
        }
    }

    // 处理数字键1和2的输入，来改变currentCube
    private void HandleCubeSelection()
    {
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            currentCube = CubeName.GreenCube.ToString();
            Debug.Log("Current Cube: BlackCube");
        }
        else if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            currentCube = CubeName.WhiteCube.ToString();
            Debug.Log("Current Cube: WhiteCube");
        }
    }
}
