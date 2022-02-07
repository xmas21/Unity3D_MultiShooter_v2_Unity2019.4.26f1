﻿using UnityEngine;

public class scr_PlayerController : MonoBehaviour
{
    [SerializeField] [Header("滑鼠水平靈敏度")] float mouseSensitivity_X;
    [SerializeField] [Header("滑鼠垂直靈敏度")] float mouseSensitivity_Y;
    [SerializeField] [Header("走路 - 速度")] float walkSpeed;
    [SerializeField] [Header("跑步 - 速度")] float runSpeed;
    [SerializeField] [Header("移動滑順時間")] float moveSmoothTime;
    [SerializeField] [Header("跳躍力道")] float jumpForce;

    [SerializeField] [Header("攝影機座標")] GameObject cameraHolder;
    [SerializeField] [Header("玩家攝影機")] Camera playerCamera;

    [HideInInspector] public bool isGrounded;

    bool cursorLocked = true;
    bool isRunning = false;

    float lookRotation;         // 上下視角旋轉值
    float walkFOV;
    float runFOV;

    Vector3 moveSmoothVelocity; // 滑順加速度
    Vector3 moveDir;            // 移動到的位置

    Rigidbody rig;

    void Awake()
    {
        playerCamera = transform.GetChild(1).GetChild(0).GetComponent<Camera>();
        rig = GetComponent<Rigidbody>();
    }

    void Start()
    {
        walkFOV = playerCamera.fieldOfView;
        runFOV = walkFOV * 1.2f;
    }

    void Update()
    {
        Move();
        View();
        Jump();
        CursorLock();
    }

    void FixedUpdate()
    {
        rig.MovePosition(rig.position + transform.TransformDirection(moveDir) * Time.deltaTime);
    }

    /// <summary>
    /// 鼠標消失
    /// </summary>
    void CursorLock()
    {
        if (cursorLocked)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;

            if (Input.GetKeyDown(KeyCode.Escape))
            {
                cursorLocked = false;
            }
        }
        else
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;

            if (Input.GetKeyDown(KeyCode.Escape))
            {
                cursorLocked = true;
            }
        }
    }

    /// <summary>
    /// 移動
    /// </summary>
    void Move()
    {
        Vector3 direction = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical")).normalized;

        isRunning = Input.GetKey(KeyCode.LeftShift) & Input.GetKey(KeyCode.W);  // 判斷是否在跑步

        if (isRunning) playerCamera.fieldOfView = Mathf.Lerp(playerCamera.fieldOfView, runFOV, Time.deltaTime * 10f);
        else playerCamera.fieldOfView = Mathf.Lerp(playerCamera.fieldOfView, walkFOV, Time.deltaTime * 10f); ;

        moveDir = Vector3.SmoothDamp(moveDir, direction * (isRunning ? runSpeed : walkSpeed), ref moveSmoothVelocity, moveSmoothTime);
    }

    /// <summary>
    /// 視角
    /// </summary>
    void View()
    {
        transform.Rotate(Vector3.up * Input.GetAxisRaw("Mouse X") * mouseSensitivity_X * Time.deltaTime * 60f);  // 角色直接左右旋轉 (X軸)

        lookRotation += Input.GetAxisRaw("Mouse Y") * mouseSensitivity_Y * Time.deltaTime * 60f;
        lookRotation = Mathf.Clamp(lookRotation, -80, 80);

        cameraHolder.transform.localEulerAngles = -Vector3.right * lookRotation;         // 攝影機角度轉換 (Y軸)
    }

    /// <summary>
    /// 跳躍
    /// </summary>
    void Jump()
    {
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            rig.AddForce(transform.up * jumpForce);
        }
    }

}
