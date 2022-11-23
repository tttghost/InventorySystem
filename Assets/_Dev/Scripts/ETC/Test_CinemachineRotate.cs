using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 시네머신 회전 테스트
/// </summary>
public class Test_CinemachineRotate : MonoBehaviour
{
    public Transform target;
    public Camera camera;
    public CinemachineCameraOffset cinemachineCamera;
    public CinemachineVirtualCamera cinemachineVirtualCamera;
    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        cinemachineVirtualCamera.m_Lens.FieldOfView = camera.fieldOfView;
        cinemachineCamera.m_Offset = camera.transform.localPosition;
    }

    public void Rotate_Left()
    {
        StartCoroutine(Co_Rotate(-1));
    }
    public void Rotate_Right()
    {
        StartCoroutine(Co_Rotate(1));
    }
    IEnumerator Co_Rotate(int val)
    {
        float curTime = 0f;
        float durTime = 0.5f;
        while (curTime < 1f)
        {
            curTime += Time.deltaTime / durTime;
            target.Rotate(Vector3.up * 90f * val * Time.deltaTime / durTime);
            yield return null;
        }
    }
}
