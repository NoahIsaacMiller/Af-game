using System;
using UnityEngine;

public class CircleController : MonoBehaviour
{
    public float r = 1f; // 圆的半径
    public int segments = 50; // 圆的分段数，值越大圆越平滑
    [Range(0.01f, 1f)]
    public float lineWidth = 0.1f; // 线条的粗细
    public bool isTrigger = true; // 是否勾选 Is Trigger，默认为 true
    public float colliderRadiusOffset = 0.1f; // 为了触发，球形碰撞器可以略大于线圈

    public float rotateDirection = 1f;

    public float period = 1f;
    private LineRenderer linearRenderer;
    private CircleCollider2D circleCollider; // 使用 CircleCollider

    private AudioSource audioSource;
    void Start()
    {
        if ((linearRenderer = gameObject.GetComponent<LineRenderer>()) == null)
        {
            linearRenderer = gameObject.AddComponent<LineRenderer>();
        }
        linearRenderer.useWorldSpace = false; // 使用局部坐标
        linearRenderer.startWidth = lineWidth;
        linearRenderer.endWidth = lineWidth;
        linearRenderer.positionCount = segments + 1;
        if ((circleCollider = gameObject.GetComponent<CircleCollider2D>()) == null)
        {
            circleCollider = gameObject.AddComponent<CircleCollider2D>(); // 添加 CircleCollider
        }
        circleCollider.isTrigger = isTrigger; // 设置 Is Trigger 的初始状态
        circleCollider.radius = r * transform.localScale.x + colliderRadiusOffset; // 设置球形碰撞器的半径，略大于线圈
        circleCollider.offset = Vector3.zero; // 确保球心与 GameObject 的局部原点重合

        DrawSolidCircle();
        audioSource = gameObject.GetComponent<AudioSource>();
    }

    void OnValidate()
    {
        // 在 Inspector 面板中值改变时自动更新
        if (linearRenderer != null)
        {
            linearRenderer.startWidth = lineWidth;
            linearRenderer.endWidth = lineWidth;
            DrawSolidCircle();
        }
        // Collider 存在时更新 isTrigger 和 radius
        if (circleCollider != null)
        {
            circleCollider.isTrigger = isTrigger;
            circleCollider.radius = r * transform.localScale.x + colliderRadiusOffset;
            circleCollider.offset = Vector3.zero; // 确保球心与 GameObject 的局部原点重合
        }
    }

    void DrawSolidCircle()
    {
        float angle = 2f * Mathf.PI / segments;
        for (int i = 0; i <= segments; i++)
        {
            float x = r * Mathf.Cos(angle * i) * transform.localScale.x; // 圆心为局部原点，所以直接使用 r * cos 和 r * sin
            float y = r * Mathf.Sin(angle * i) * transform.localScale.x;
            linearRenderer.SetPosition(i, new Vector3(x, y, 0f)); // z 坐标保持为 0，因为是在局部坐标系下
        }

        // 设置材质为普通的白色材质
        linearRenderer.material = new Material(Shader.Find("Sprites/Default"));
        linearRenderer.material.color = Color.white;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("player"))
        {
            audioSource.Play();
            PlayerController playerController = other.gameObject.GetComponent<PlayerController>();
            // 获取玩家的中心坐标 (使用 Renderer bounds)
            Vector3 playerCenter = GetGameObjectCenter(other.gameObject);
            Vector3 center = new Vector3(transform.position.x, transform.position.y + lineWidth / 2, transform.position.z);

            playerController.rotateDirection = rotateDirection;
            playerController.setRing(gameObject);
            // 获取整个世界向右的方向向量
            float angleDegrees = Vector3.Angle(playerCenter - center, Vector3.right);
            playerController.angleStartToRotate = (playerCenter.y > center.y ? angleDegrees : 360 - angleDegrees)*Mathf.PI / 180;
            playerController.SetOnOrbit(center, period, r * transform.localScale.x); // 使用圆的半径作为环绕半径
        }
    }

    // 获取 GameObject 的渲染中心
    public static Vector3 GetGameObjectCenter(GameObject targetObject)
    {
        if (targetObject == null)
        {
            Debug.LogError("目标 GameObject 为空！");
            return Vector3.zero;
        }

        Renderer[] renderers = targetObject.GetComponentsInChildren<Renderer>();
        if (renderers.Length == 0)
        {
            return targetObject.transform.position;
        }

        Bounds bounds = renderers[0].bounds;
        for (int i = 1; i < renderers.Length; i++)
        {
            bounds.Encapsulate(renderers[i].bounds);
        }

        return bounds.center;
    }
}