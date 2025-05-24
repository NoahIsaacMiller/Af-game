using UnityEngine;

public class PeriodicRotation : MonoBehaviour
{
    [SerializeField] private float period = 2f; // 旋转周期(秒)
    [SerializeField] private float degreesPerRotation = 360f; // 每次旋转的角度
    [SerializeField] private bool clockwise = false; // 是否顺时针旋转
    
    private float angularSpeed; // 角速度(度/秒)
    private float startTime; // 开始时间
    
    void Start()
    {
        // 计算角速度 (度/秒)
        angularSpeed = degreesPerRotation / period;
        if (clockwise) angularSpeed = -angularSpeed;
        
        // 记录开始时间
        startTime = Time.time;
    }
    
    void Update()
    {
        // 计算从开始到现在的时间
        float elapsedTime = Time.time - startTime;
        
        // 计算当前应旋转的角度 (考虑时间循环)
        float currentAngle = (elapsedTime % period) * angularSpeed;
        
        // 应用旋转 (绕Z轴，2D旋转)
        transform.rotation = Quaternion.Euler(0f, 0f, currentAngle);
    }
}    