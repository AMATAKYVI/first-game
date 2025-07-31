using UnityEngine;

public class BlackDoorLightController : MonoBehaviour
{
    public Light lightDoor;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

        if (lightDoor == null)
            lightDoor = GetComponent<Light>();

        // Set initial light states
        lightDoor.enabled = false;

    }

    // Update is called once per frame
    void Update()
    {

    }

}
