using UnityEngine;

public class CameraController : MonoBehaviour
{
    public bool clickToMoveCamera = false;    // Only rotate camera while right mouse button is held
    public bool canZoom = true;               // Allow zooming with the scroll wheel
    public float sensitivity = 5f;            // Mouse look and zoom sensitivity
    public Vector2 cameraLimit = new Vector2(-45, 40);  // Vertical angle limits (min, max)

    float mouseX;                             // Current horizontal rotation amount
    float mouseY;                             // Current vertical rotation amount
    float offsetDistanceY;                    // Height offset from the player

    Transform player;                         // Reference to the player’s transform

    void Start()
    {
        // Find the player and record how high above them this camera sits
        player = GameObject.FindWithTag("Player").transform;
        offsetDistanceY = transform.position.y;

        // If not using click-to-move, lock and hide the cursor for free look
        if (!clickToMoveCamera)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }

    void Update()
    {
        // Keep the camera positioned above the player at the set offset
        transform.position = player.position + new Vector3(0, offsetDistanceY, 0);

        // Zoom camera in/out with scroll wheel if allowed
        if (canZoom && Input.GetAxis("Mouse ScrollWheel") != 0)
        {
            Camera.main.fieldOfView -= Input.GetAxis("Mouse ScrollWheel") * sensitivity * 2;
        }

        // If click-to-move is enabled but right mouse button is not held, skip rotation
        if (clickToMoveCamera && Input.GetAxisRaw("Fire2") == 0)
        {
            return;
        }

        // Accumulate mouse movement for looking around
        mouseX += Input.GetAxis("Mouse X") * sensitivity;
        mouseY += Input.GetAxis("Mouse Y") * sensitivity;

        // Clamp vertical look angle to prevent flipping over
        mouseY = Mathf.Clamp(mouseY, cameraLimit.x, cameraLimit.y);

        // Apply the rotation to the camera
        transform.rotation = Quaternion.Euler(-mouseY, mouseX, 0);
    }
}
