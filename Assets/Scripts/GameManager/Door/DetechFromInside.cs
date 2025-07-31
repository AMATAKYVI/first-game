using UnityEngine;

public class DetechFromInside : MonoBehaviour
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
            DoorHingeController doorHingeController = doorHingeDetect.GetComponent<DoorHingeController>();
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
