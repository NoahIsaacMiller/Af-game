using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using MyGame;


public class PlayerController : MonoBehaviour
{

    private Rigidbody2D rb;
    private OnScenceSetup onSceneSetup;
    private bool isColliding;
    private ContactPoint2D currentContactPoint;
    private AudioSource audioSource;

    public float rotateDirection = 1f;
    public float angleStartToRotate = 0f;
    private LevelCompleteManager levelCompleteManager;

    [Header("移动")]
    public float moveSpeed = 10f;

    [Header("脱离")]
    public float detachForce = 5f;

    [Header("环绕")]
    private bool isOrbiting = false; // 是否进行环绕运动
    public Vector3 orbitCenter = Vector3.zero; // 环绕中心的世界坐标
    private float orbitPeriod;      // 环绕周期 (秒)
    private float orbitRadius;      // 环绕半径


    private float orbitTime = 0f;
    private GameObject ring;

    public void setRing(GameObject ring) { this.ring = ring; }
    public bool IsOrbiting() { return isOrbiting; }
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        audioSource = GetComponent<AudioSource>();
        levelCompleteManager = FindObjectOfType<LevelCompleteManager>();
        onSceneSetup = FindObjectOfType<OnScenceSetup>();
        if (onSceneSetup == null)
        {
            Debug.LogError("GameLogicManager: 场景中未找到 OnSceneSetup 实例。请确保它挂载在某个GameObject上。");
        }
        if (levelCompleteManager == null)
        {
            Debug.LogError("GameLogicManager: 场景中未找到 LevelCompleteManager 实例。请确保它挂载在某个GameObject上。");
        }
        if (rb == null)
        {
            enabled = false; // 如果没有 Rigidbody，禁用脚本
        }
    }

    void Update()
    {
        // 坠落到一定位置算作死亡
        if (transform.position.y < -10f)
        {
            levelCompleteManager.ShowLevelEndScreen(false, 0, 0, 0, 0);
        }

        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            switch (touch.phase)
            {
                case TouchPhase.Began:
                    SetOffOrbit(ring.transform.position);
                    ring = null;
                    break;
                case TouchPhase.Moved:
                    break;
                case TouchPhase.Ended:
                case TouchPhase.Canceled:
                    break;
                case TouchPhase.Stationary:
                    // 可选处理静止触摸
                    break;
            }
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }


        // 脱离
        if (Input.GetKeyDown(KeyCode.Space) && ring != null)
        {
            // 脱离环
            SetOffOrbit(ring.transform.position);
            ring = null;
        }

        // 环绕运动的控制
        if (isOrbiting)
        {
            OrbitAroundCenter();
        }
    }

    private void FixedUpdate()
    {
        if (isOrbiting && rb != null)
        {
            orbitTime += Time.fixedDeltaTime;
            // 计算当前角度 (弧度)
            float currentAngle = ((orbitTime / orbitPeriod) * 2) * rotateDirection * Mathf.PI + angleStartToRotate;

            // 计算在 XOY 平面上的目标位置 (相对于环绕中心)
            float offsetX = orbitRadius * Mathf.Cos(currentAngle);
            float offsetY = orbitRadius * Mathf.Sin(currentAngle);

            // 计算世界坐标系中的目标位置
            Vector3 targetPosition = new Vector3(orbitCenter.x + offsetX, orbitCenter.y + offsetY, orbitCenter.z);

            // 直接设置位置以实现精确的圆周运动
            rb.MovePosition(targetPosition);

        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        audioSource.Play();
        isColliding = true;
        orbitTime = 0f;
        currentContactPoint = collision.GetContact(0);
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        isColliding = false;
    }

    // 调用此函数开始或停止环绕运动
    public void SetOnOrbit(Vector3 center, float period, float radius)
    {
        // 环绕时禁用重力
        rb.gravityScale = 0;
        isOrbiting = true;
        orbitCenter = center;
        orbitPeriod = period;
        orbitRadius = radius;
        orbitTime = 0f; // 重置环绕时间
    }

    public void SetOffOrbit(Vector3 center)
    {
        rb.gravityScale = 0.80f;
        isOrbiting = false;
        Vector3 direction = Vector3.Cross(Vector3.forward, new Vector3(transform.position.x - center.x, transform.position.y - center.y, transform.position.z)).normalized;
        if (rotateDirection < 0)
        {
            direction = -direction;
        }
        rb.linearVelocity = direction * (float)(Math.PI * 2 / orbitPeriod) * orbitRadius;
    }

    // 内部使用的环绕函数
    private void OrbitAroundCenter()
    {
        // 实际的环绕逻辑在 FixedUpdate 中执行
    }

}