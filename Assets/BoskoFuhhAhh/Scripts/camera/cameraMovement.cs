using UnityEngine;

public class cameraMovement : MonoBehaviour
{
    [SerializeField] float cameraSpeed;
    [SerializeField] Transform playerTransform;
    [SerializeField] float zOffset;
    [SerializeField] float yOffset;

    [Header("Zoom")]
    [SerializeField] float orthographicSize = 8f; 

    private void Start()
    {
        Camera.main.orthographicSize = orthographicSize;
    }

    void Update()
    {
        if (playerTransform == null) return;
        transform.position = Vector3.Lerp(transform.position, playerTransform.position + new Vector3(0, yOffset, -zOffset), Time.deltaTime * cameraSpeed);
    }
}