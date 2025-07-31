using UnityEngine;

public class DoorHingeController : MonoBehaviour
{


    public float openAngle = 90f;
    public float openSpeed = 2f;
    public bool isOpen = false;

    // private string doorSide = "left"; // Default side, can be "left" or "right"
    private Quaternion defaultRotation;
    private Quaternion openRotation;
    private float currentDirection = -1f; // -1 for left, 1 for right


    public
    void Start()
    {
        defaultRotation = transform.rotation;
        openRotation = Quaternion.Euler(transform.eulerAngles + new Vector3(0, -openAngle, 0));
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
        // if (doorSide == "left")
        // {
        //     openRotation = Quaternion.Euler(transform.eulerAngles + new Vector3(0, -openAngle, 0));
        // }
        // else if (doorSide == "right")
        // {
        //     openRotation = Quaternion.Euler(transform.eulerAngles + new Vector3(0, openAngle, 0));
        // }
        // else
        // {
        //     Debug.LogWarning("Invalid door side specified: " + doorSide);
        //     return;
        // }
        isOpen = !isOpen;
    }
}
