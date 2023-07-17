using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Parallax : MonoBehaviour
{
    //Reference to the main camera
    private Transform cameraTransform;
    //Last camera X position
    private float lastCamPosX;

    //Parallax effect speed
    [SerializeField] float pSpeed = 1;

    // Start is called before the first frame update
    void Start()
    {
        cameraTransform = Camera.main.transform;
        lastCamPosX = Camera.main.transform.position.x;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void LateUpdate()
    {
        //Current camera X position
        float curCamPosX = cameraTransform.position.x;
        //Difference between current camera X position and the last
        float deltaMov = curCamPosX - lastCamPosX;
        //Move sprite to opposite direction of the character by a factor of pSpeed
        transform.position = new Vector3(transform.position.x + deltaMov * pSpeed * -0.1f, transform.position.y, transform.position.z);
        //Update last camera X position
        lastCamPosX = cameraTransform.position.x;
    }
}
