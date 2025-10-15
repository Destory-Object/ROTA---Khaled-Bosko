using UnityEngine;

public class cameraMovement : MonoBehaviour
{

    [SerializeField] float cameraSpeed;
    [SerializeField] Transform playerTransform;
    [SerializeField] float zOffset;
    [SerializeField] float yOffset;

    void Update()
    {
        transform.position = Vector3.Lerp(transform.position, playerTransform.position + new Vector3(0, yOffset, -zOffset), Time.deltaTime * cameraSpeed);
    }
}
