// using UnityEngine;

// public class CameraScript : MonoBehaviour
// {

//     public Transform player;       // The player/capsule transform
//     public float minDistance = 0.5f;   // Min distance from player
//     public float maxDistance = 3f;     // Max distance camera can be away
//     private float currentDistance;

//     void Start()
//     {
//         currentDistance = maxDistance;
//     }

//     void LateUpdate()
//     {
//         Vector3 desiredPosition = player.position - player.forward * maxDistance;
//         RaycastHit hit;

//         if (Physics.Linecast(player.position, desiredPosition, out hit))
//         {
//             currentDistance = Mathf.Clamp(hit.distance, minDistance, maxDistance);
//         }
//         else
//         {
//             currentDistance = maxDistance;
//         }

//         transform.position = player.position - player.forward * currentDistance;
//         transform.LookAt(player.position);
//     }
// }
using UnityEngine;
public class CameraScript : MonoBehaviour
{
    public float mouseSensitivity = 100f;
    public Transform playerBody;

    float xRotation = 0f;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {

        if (Cursor.lockState != CursorLockMode.Locked)
            return;


        float mouseX = Input.GetAxis("Mouse X");
        float mouseY = Input.GetAxis("Mouse Y");

        float deadzone = 5f;
        if (Mathf.Abs(mouseX) < deadzone) mouseX = 0f;
        if (Mathf.Abs(mouseY) < deadzone) mouseY = 0f;

        if (Mathf.Abs(mouseX) > deadzone || Mathf.Abs(mouseY) > deadzone)
        {
            mouseX *= mouseSensitivity * Time.deltaTime;
            mouseY *= mouseSensitivity * Time.deltaTime;

            xRotation -= mouseY;
            xRotation = Mathf.Clamp(xRotation, -90f, 90f);

            transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
            playerBody.Rotate(Vector3.up * mouseX);

            Vector3 euler = playerBody.eulerAngles;
            euler.x = 0f;
            euler.z = 0f;
            playerBody.eulerAngles = euler;
        }
    }
}
