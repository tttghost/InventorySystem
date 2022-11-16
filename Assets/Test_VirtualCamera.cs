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
            SetData(2);
        }
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            SetData(-2);
        }
        if (Input.GetKeyDown(KeyCode.PageUp))
        {
            SideTop(eViewState.top);
            //Dolly_Offset(1);
        }
        if (Input.GetKeyDown(KeyCode.PageDown))
        {
            SideTop(eViewState.side);
            //Dolly_Offset(-1);
        }
        float v = Input.GetAxisRaw("Vertical");
        //ZoomInOut(v);

        if (Input.GetKey(KeyCode.UpArrow))
        {
            ZoomInOut(1);
        }
        if (Input.GetKey(KeyCode.DownArrow))
        {
            ZoomInOut(-1);
        }

        //Dolly_Path(v);
    }

    private void SetData(int val)
    {
        Dolly_m_PathPosition(val);
        //SetRotate(val);
        //SetPriority(val);
    }

    public float pathMin = 0.5f;
    public float pathMax = 2f;

    private void Dolly_Path(float val)
    {
        //val = Mathf.Lerp(1f, -1f, Mathf.InverseLerp(-1f, 1f, val));

        float y = dollyPath.localScale.y;
        y = Mathf.Clamp(y += Time.deltaTime * -val, pathMin, pathMax);
        dollyPath.localScale = Vector3.one * y;
    }

    enum eViewState
    {
        side,
        top,
    }

    eViewState viewState = eViewState.side;

    float LerpInverseLerp(float fromA, float fromB, float toA, float toB, float val)
    {
        return Mathf.Lerp(toA, toB, Mathf.InverseLerp(fromA, fromB, val));
    }

    private void SideTop(eViewState viewState)
    {
        if (this.viewState == viewState)
        {
            return;
        }
        this.viewState = viewState;
        switch (viewState)
        {
            case eViewState.top:

                GetCinemachineTrackedDolly().m_PathOffset = Vector3.up * LerpInverseLerp(0.01f, 2f, 0f, 12f, dollyPath.localScale.y);
                dollyPath.localScale = Vector3.one * 0.01f;
                break;
            case eViewState.side:
                dollyPath.localScale = Vector3.one * LerpInverseLerp(0f, 12f, 0.01f, 2f, GetCinemachineTrackedDolly().m_PathOffset.y);
                GetCinemachineTrackedDolly().m_PathOffset = Vector3.up * 1.5f;
                break;
            default:
                break;
        }
    }

    //dollyTrack 스케일 side : 
    //cinemachine 높이 side : 

    private void ZoomInOut(float val)
    {
        switch (viewState)
        {
            case eViewState.side:
                Dolly_Path(val);
                break;
            case eViewState.top:
                Dolly_Offset(-val);
                break;
            default:
                break;
        }
    }

    CinemachineTrackedDolly GetCinemachineTrackedDolly()
    {
        return cinemachineVirtualCamera[0].GetCinemachineComponent<CinemachineTrackedDolly>();
    }

    private void Dolly_Offset(float val)
    {
        CinemachineTrackedDolly cinemachineTrackedDolly = GetCinemachineTrackedDolly();
        float y = Mathf.Clamp(cinemachineTrackedDolly.m_PathOffset.y += Time.deltaTime / 0.1f * val, 1f, 10f);
        cinemachineTrackedDolly.m_PathOffset = Vector3.up * y;
    }

    private void Dolly_m_PathPosition(int val)
    {
        CinemachineTrackedDolly cinemachineTrackedDolly = GetCinemachineTrackedDolly();
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
