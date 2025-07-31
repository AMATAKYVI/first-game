using UnityEngine;

public class DetechFromInsideBlack : MonoBehaviour
{
    public GameObject doorHingeDetect;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    void OnTriggerEnter(Collider other)

    {
        Debug.Log("OnTriggerEnter: " + other.name);
        if (other.CompareTag("Player"))
        {
            // Assuming the door hinge controller is attached to the same GameObject
            BlackDoorHingeController doorHingeController = doorHingeDetect.GetComponent<BlackDoorHingeController>();
            if (doorHingeController != null)
            {
                doorHingeController.ChangeDoorSide("left");
            }
        }
    }
    // Update is called once per frame
    void Update()
    {

    }
}
