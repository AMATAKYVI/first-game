using UnityEngine;

public class BlackDoorHingeController : MonoBehaviour
{


    public float openAngle = 90f;
    public float openSpeed = 2f;
    public bool isOpen = false;

    private Quaternion defaultRotation;
    private Quaternion openRotation;

    private float currentDirection = -1f; // -1 for left, 1 for right

    public
    void Start()
    {
        defaultRotation = transform.rotation;
        openRotation = Quaternion.Euler(transform.eulerAngles + new Vector3(0, openAngle, 0));
    }

    void OnTriggerEnter(Collider other)
    {
        Debug.Log("OnTriggerEnter: " + other.name);
    }
    void Update()
    {
        transform.rotation = Quaternion.Lerp(transform.rotation,
            isOpen ? openRotation : defaultRotation, Time.deltaTime * openSpeed);
    }


    public void ChangeDoorSide(string doorSide)
    {
        if (doorSide == "left")
        {
            currentDirection = -1f;
        }
        else if (doorSide == "right")
        {
            currentDirection = 1f;
        }
        else
        {
            Debug.LogWarning("Invalid door side specified: " + doorSide);
            return;
        }
        // Update the open rotation based on the specified door side
        openRotation = Quaternion.Euler(defaultRotation.eulerAngles + new Vector3(0, openAngle * currentDirection, 0));
    }

    public void ToggleDoor()
    {
        isOpen = !isOpen;
    }
}
