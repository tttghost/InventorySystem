using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookAt : MonoBehaviour
{
    void Update()
    {
        transform.LookAt(Camera.main.transform);
        transform.localEulerAngles = Vector3.up * transform.localEulerAngles.y;
    }
}
