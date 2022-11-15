using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Test_VirtualCamera : MonoBehaviour
{
    public CinemachineVirtualCamera[] cinemachineVirtualCamera;
    public Transform dollyPath;

    public int idx = 0;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            SetData(1);
        }
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            SetData(-1);
        }
        if (Input.GetKey(KeyCode.PageUp))
        {
            Dolly_Offset(1);
        }
        if (Input.GetKey(KeyCode.PageDown))
        {
            Dolly_Offset(-1);
        }
        float v = Input.GetAxisRaw("Vertical");
        DolbyPath(v);
    }

    private void SetData(int val)
    {
        Dolly_m_PathPosition(val);
        //SetRotate(val);
        //SetPriority(val);
    }

    private void DolbyPath(float val)
    {
        //val = Mathf.Lerp(1f, -1f, Mathf.InverseLerp(-1f, 1f, val));

        float y = dollyPath.localScale.y;
        y =  Mathf.Clamp(y += Time.deltaTime * -val, 0.5f, 2f);
        dollyPath.localScale = Vector3.one * y;
    }
    private void Dolly_Offset(float val)
    {
        CinemachineTrackedDolly cinemachineTrackedDolly = cinemachineVirtualCamera[0].GetCinemachineComponent<CinemachineTrackedDolly>();
        float y = Mathf.Clamp(cinemachineTrackedDolly.m_PathOffset.y += Time.deltaTime / 0.1f * val, 1f, 10f);
        cinemachineTrackedDolly.m_PathOffset = Vector3.up * y;
    }

    private void Dolly_m_PathPosition(int val)
    {
        CinemachineTrackedDolly cinemachineTrackedDolly = cinemachineVirtualCamera[0].GetCinemachineComponent<CinemachineTrackedDolly>();
        cinemachineTrackedDolly.m_PathPosition += val;
    }

    private void SetRotate(int val)
    {
        StartCoroutine(Co_Rotate(val));
    }

    IEnumerator Co_Rotate(int val)
    {
        float curTime = 0f;
        float durTime = 1f;
        while (curTime < 1f)
        {
            curTime += Time.deltaTime / durTime;
            cinemachineVirtualCamera[0].Follow.Rotate(Vector3.up * 90f * val * Time.deltaTime);
            yield return null;
        }
    }

    private void SetPriority(int val)
    {
        cinemachineVirtualCamera = cinemachineVirtualCamera.Select(x => { x.Priority = 0; return x; }).ToArray();
        cinemachineVirtualCamera[idx = (int)Mathf.Repeat(idx + val, cinemachineVirtualCamera.Length)].Priority = 1;
    }
}
