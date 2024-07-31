using UnityEngine;

public class CameraController : MonoBehaviour
{
    public float rotationSpeed = 3f;
#if UNITY_EDITOR
    void Update()
    {
        float mouseX = Input.GetAxis("Mouse X");
        float mouseY = Input.GetAxis("Mouse Y");

        // ���X����ת�����ˮƽ����Y����ת�������ֱ����
        transform.Rotate(Vector3.up * mouseX * rotationSpeed);
        transform.Rotate(Vector3.left * mouseY * rotationSpeed);

        // ����������ڴ�ֱ�����ϵ���ת��������ȷ�ת
        float currentXRotation = transform.eulerAngles.x;
        if (currentXRotation > 180f)
            currentXRotation -= 360f;
        currentXRotation = Mathf.Clamp(currentXRotation, -90f, 90f);
        transform.rotation = Quaternion.Euler(currentXRotation, transform.eulerAngles.y, 0f);
    }
#endif
}
