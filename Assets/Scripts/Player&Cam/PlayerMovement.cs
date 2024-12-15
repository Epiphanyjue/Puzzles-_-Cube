using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("移动设置")]
    public float moveSpeed = 5f;                  // 基础移动速度
    public float moveAccelerationDuration = 0.5f;  // 按键保持后加速时长（秒）
    public float moveDecelerationDuration = 0.5f;  // 按键松开后的减速时长（秒）
    public float maxMoveSpeed = 10f;              // 最大移动速度
    public float raycastOffset = 0.2f;            // 射线检测偏移量
    public float minMoveSpeed = 5f;               // 最低移动速度
    public float bonusSpeed = 10f;                // 加速时的bonus速度

    [Space(30)]
    [Header("跳跃设置")]
    public float jumpForce = 7f;                  // 跳跃力度
    public float gravityScaleDuringAscend = 2f;   // 跳跃上升阶段的重力
    public float gravityScaleDuringDescend = 4f;  // 跳跃下落阶段的重力
    public float jumpWindowDuration = 0.2f;       // 跳跃窗口期的时长（秒）
    private float jumpWindowTimer;
    [Header("卡死判断时间")]
    [Range(1.0f,3.0f)]
    public float stuckMaxTime=2.0f;
    private float jumpStuckTimer=0.0f;

    private Rigidbody rb;
    public bool isGrounded;
    private Vector3 moveDirection;
    private Vector3 targetVelocity;  // 当前目标速度

    // 平面移动状态
    private Vector3 currentVelocity;
    private bool isAccelerating = false;   // 是否正在加速（根据方向输入）
    private bool isDecelerating = false;   // 是否正在减速（松开按键）
    private float timeSinceDirectionChange = 0f;  // 记录切换方向后的时间，用于计算bonus加速

    private float initialMoveSpeed=0f;        // 起始速度（用于加速）
    private bool isBonusSpeedActive = false;   // 标记是否加速状态

    // 跳跃相关
    private bool wasGroundedLastFrame = true;
    [SerializeField]
    private bool isJumping = false;
    private bool isLeavingGround = false;
    private float initialJumpSpeedXZ = 0f; // 记录起跳瞬间的 xz 速度的平方和
    private bool hasJumped = false;
    public bool isWalled=false; //检测角色周围是否有墙壁

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;  // 防止物理引擎影响角色的旋转
        targetVelocity = Vector3.zero;
    }

    void Update()
    {
        HandleMovement();
        HandleJumping();

    }
    void FixedUpdate()
    {
        ApplyCustomGravity();
    }
    void HandleMovement()
    {
        // 获取水平和垂直方向的输入（WASD控制）
        float horizontal = Input.GetAxis("Horizontal");  // A/D 或 左右箭头键
        float vertical = Input.GetAxis("Vertical");      // W/S 或 上下箭头键

        // 计算玩家的前进和右侧方向
        Vector3 forward = transform.forward; // 玩家前进方向
        Vector3 right = transform.right;     // 玩家右侧方向

        // 计算最终的移动方向，基于玩家的朝向
        moveDirection = forward * vertical + right * horizontal;

        // 保证方向是单位向量（即标准化方向），这样可以确保统一的移动速度
        moveDirection = moveDirection.normalized;

        // 检查物体是否在地面上
        isGrounded =Physics.Raycast(transform.position + Vector3.right * raycastOffset, Vector3.down, 1.1f) ||
                    Physics.Raycast(transform.position + Vector3.left * raycastOffset, Vector3.down, 1.1f) ||
                    Physics.Raycast(transform.position + Vector3.forward * raycastOffset, Vector3.down, 1.1f) ||
                    Physics.Raycast(transform.position + Vector3.back * raycastOffset, Vector3.down, 1.1f);

        isWalled=Physics.Raycast(transform.position,transform.forward,1f);

        // 处理加速
        if (moveDirection.magnitude > 0 &&!isWalled)
        {
            // timeSinceDirectionChange += Time.deltaTime;  // 记录按住方向键的时间
            //撞墙时清空加速计时
            if(isWalled)
            {
                timeSinceDirectionChange=0.0f;
            }

            // if ( timeSinceDirectionChange < moveAccelerationDuration)
            // {
            //     // 开始加速
            //     isAccelerating = true;
            //     isDecelerating = false;
            //     moveSpeed=minMoveSpeed;
            // }
            // 激活加速
            // else
            // {
            //     moveSpeed = bonusSpeed;
            // }
            moveSpeed=minMoveSpeed;
            // 设置目标速度
            targetVelocity = moveDirection * moveSpeed;
        }
        else
        {
            // 没有输入方向，进入减速状态
            if (isAccelerating && isGrounded)
            {
                // 开始减速
                isDecelerating = true;
                isAccelerating=false;
                isBonusSpeedActive = false;
                moveSpeed=initialMoveSpeed;
                timeSinceDirectionChange=0;
            }

            // 渐变减速
            if (isDecelerating)
            {
                moveSpeed = initialMoveSpeed;
                if (moveSpeed <= initialMoveSpeed + 0.1f)  // 防止减速过多
                {
                    isDecelerating = false;
                }
            }

            targetVelocity = moveDirection * moveSpeed;
        }
        
        // 施加目标速度
        currentVelocity = targetVelocity;  // 使用插值平滑过渡
        rb.velocity = new Vector3(currentVelocity.x, rb.velocity.y, currentVelocity.z);  // 保持y轴速度不变
    }

    void HandleJumping()
    {
        if (isGrounded && Mathf.Abs(rb.velocity.y) < 2f)
        {
            isJumping = false;
        }

        // 只有在地面上时才允许跳跃
        if (isGrounded || (jumpWindowTimer > 0 && !isJumping))
        {
            if (Input.GetButtonDown("Jump") && !isJumping)
            {
                jumpStuckTimer=0.0f;
                AudioManager.Instance.Play("Jump");
                isJumping = true;
                hasJumped = true;

                // 只有在地面上按下空格键时才跳跃，保持水平速度不变
                rb.velocity = new Vector3(rb.velocity.x, jumpForce, rb.velocity.z);  // 只修改y轴速度
                EventCenter.Instance.TriggerEvent("Jump");
            }
        }
        if(isJumping)
        {
            jumpStuckTimer+=Time.deltaTime;
            if(jumpStuckTimer>=stuckMaxTime)
            {
                isJumping=false;
                jumpStuckTimer=0.0f;
            }
        }
        // 离开地面开始计时
        if (!isGrounded)
        {
            jumpWindowTimer += Time.deltaTime;
            if (jumpWindowTimer > jumpWindowDuration)
            {
                jumpWindowTimer = 0f;
            }
        }


        wasGroundedLastFrame = isGrounded;
    }

    void ApplyCustomGravity()
    {
        // 根据跳跃阶段应用不同的重力
        if (rb.velocity.y > 0)  // 上升阶段
        {
            rb.AddForce(Vector3.down * gravityScaleDuringAscend, ForceMode.Acceleration);
        }
        else if (rb.velocity.y < 0)  // 下落阶段
        {
            rb.AddForce(Vector3.down * gravityScaleDuringDescend, ForceMode.Acceleration);
        }
    }
}
