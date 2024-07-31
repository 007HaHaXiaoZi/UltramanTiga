using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 5f;

#if UNITY_EDITOR
    void Update()
    {
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");

        Vector3 movement = new Vector3(horizontalInput, 0f, verticalInput) * moveSpeed * Time.deltaTime;
        transform.Translate(movement);

        // 如果你的角色是在地面上移动，并且有刚体组件（例如Rigidbody），可以考虑使用以下方式来移动:
        // GetComponent<Rigidbody>().MovePosition(transform.position + movement);
    }
#endif
}
