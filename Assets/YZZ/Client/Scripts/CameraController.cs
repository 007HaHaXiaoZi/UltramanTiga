using UnityEngine;

public class CameraController : MonoBehaviour
{
    public float rotationSpeed = 3f;
#if UNITY_EDITOR
    void Update()
    {
        float mouseX = Input.GetAxis("Mouse X");
        float mouseY = Input.GetAxis("Mouse Y");

        // 鼠标X轴旋转摄像机水平方向，Y轴旋转摄像机垂直方向
        transform.Rotate(Vector3.up * mouseX * rotationSpeed);
        transform.Rotate(Vector3.left * mouseY * rotationSpeed);

        // 限制摄像机在垂直方向上的旋转，避免过度翻转
        float currentXRotation = transform.eulerAngles.x;
        if (currentXRotation > 180f)
            currentXRotation -= 360f;
        currentXRotation = Mathf.Clamp(currentXRotation, -90f, 90f);
        transform.rotation = Quaternion.Euler(currentXRotation, transform.eulerAngles.y, 0f);
    }
#endif
}
