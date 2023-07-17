using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainCamera : MonoBehaviour
{
    [SerializeField]
    protected Transform target;
    protected float xOffset;
    protected float yOffset;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        //Reset camera position based on the target’s position9
        transform.position = new Vector3(target.position.x + xOffset, target.position.y + yOffset, transform.position.z);
    }
}
