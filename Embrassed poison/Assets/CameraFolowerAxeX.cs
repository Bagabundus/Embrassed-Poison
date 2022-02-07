using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFolowerAxeX : MonoBehaviour
{
    public Transform targetToFollow;

    private void Update()
    {   
        transform.position = new Vector3 (targetToFollow.position.x, transform.position.y, transform.position.z) ;
    }
    

}
