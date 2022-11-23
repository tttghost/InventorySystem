using Cinemachine;
using Lean.Touch;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public class Test_VirtualCamera1 : MonoBehaviour
{
    public CinemachineVirtualCamera[] cinemachineVirtualCamera;
    public Transform dollyPath;

    public MyRoomManager inventoryManager;

    public int idx = 0;
    private void Start()
    {
        LeanTouch.OnFingerSwipe += LeanTouch_OnFingerSwipe;
        LeanTouch.OnGesture += LeanTouch_OnGesture;
        LeanTouch.OnFingerExpired += LeanTouch_OnFingerExpired;
        LeanTouch.OnFingerDown += LeanTouch_OnFingerDown;
        LeanTouch.OnFingerUp += LeanTouch_OnFingerUp;
        LeanTouch.OnFingerSwipe += (a) => { };
        LeanTouch.OnFingerUpdate += LeanTouch_OnFingerUpdate;
    }

    private void LeanTouch_OnFingerUpdate(LeanFinger obj)
    {
        if(obj.Swipe)
        {
            Debug.Log("aa");
        }
        //if(obj.)
    }

    private void LeanTouch_OnFingerUp(LeanFinger obj)
    {
        fingerCnt--;
        if (fingerCnt == 0)
        {
            isPinch = false;
        }
    }

    int fingerCnt = 0;
    bool isPinch = false;
    private void LeanTouch_OnFingerDown(LeanFinger obj)
    {
        fingerCnt++;
        if (fingerCnt == 2)
        {
            isPinch = true;
        }
    }

    private void LeanTouch_OnFingerExpired(LeanFinger obj)
    {
        //fingerCnt = 0;
        //isPinch = false;
    }

    private void LeanTouch_OnGesture(List<LeanFinger> obj)
    {
    }

    private void LeanTouch_OnFingerSwipe(LeanFinger obj)
    {
    }

    private bool IsParallel()
    {
        float ff = Mathf.Abs(LeanGesture.GetTwistDegrees());

        if (ff > 10f || ff == 0f) // 패러럴
        {
            return true;
        }
        return false;
    }

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
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            SideTop(-1);
            //Dolly_Offset(1);
        }
        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            SideTop(1);
            //Dolly_Offset(-1);
        }

        
        //float v = Input.GetAxisRaw("Vertical");
        //ZoomInOut(v);
        //float scrollWhell = Input.GetAxis("Mouse ScrollWheel");
        //ZoomInOut(scrollWhell * 30f);
        //if (Input.GetKey(KeyCode.UpArrow))
        //{
        //    ZoomInOut(scrollWhell * 10f);
        //}
        //if (Input.GetKey(KeyCode.DownArrow))
        //{
        //    ZoomInOut(-1);
        //}

        //Dolly_Path(v);
    }

    private void OnGUI()
    {

        GUI.Label(new Rect(0, 500, 1000, 100), "LeanTouch.Fingers.Count : " + LeanTouch.Fingers.Count);
    }

    public void SetData(int val)
    {
        if (inventoryManager.gridSystem.itemState == GridSystem.eItemState.move)
        {
            return;
        }
        if (isPinch)
        {
            return;
        }

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

    public int viewState = 1;

    float LerpInverseLerp(float fromA, float fromB, float toA, float toB, float val)
    {
        return Mathf.Lerp(toA, toB, Mathf.InverseLerp(fromA, fromB, val));
    }
    public static T String2Enum<T>(string _str)
    {
        try
        {
            return (T)Enum.Parse(typeof(T), _str);
        }
        catch
        {
            return (T)Enum.Parse(typeof(T), "none");
        }
    }

    public void OnParallel(Vector2 vector2)
    {
        StartCoroutine(Co_OnParallel(vector2));
    }

    IEnumerator Co_OnParallel(Vector2 vector2)
    {
        CinemachineCameraOffset cinemachineCameraOffset = cinemachineVirtualCamera[0].GetComponent<CinemachineCameraOffset>();
        Vector3 ori = cinemachineCameraOffset.m_Offset;
        Vector3 target;
        if (vector2.x < 0)
        {
            target = ori + Vector3.left;
        }
        else
        {
            target = ori + Vector3.right;
        }

        float curTime = 0f;
        float durTime = 0.25f;
        while (curTime < 1f)
        {
            curTime += Time.deltaTime / durTime;
            cinemachineCameraOffset.m_Offset = Vector3.Lerp(ori, target, curTime);
            yield return null;
        }
    }


    public void SideTop(int viewState)
    {

        if (inventoryManager.gridSystem.itemState == GridSystem.eItemState.move)
        {
            return;
        }
        int newVal = Mathf.Clamp(this.viewState + viewState, 0, 2);
        if (this.viewState == newVal)
        {
            return;
        }
        this.viewState = newVal;
        var v = GetCinemachineTrackedDolly();
        switch (this.viewState)
        {
            case 0:
                v.m_PathOffset = Vector3.up * 0f;
                break;
            case 1:
                if (viewState == -1)
                {
                    dollyPath.localScale = Vector3.one * LerpInverseLerp(0f, 12f, 0.01f, 2f, v.m_PathOffset.y);
                }
                v.m_PathOffset = Vector3.up * 6f;
                break;
            case 2:
                v.m_PathOffset = Vector3.up * LerpInverseLerp(0.01f, 2f, 0f, 12f, dollyPath.localScale.y);
                dollyPath.localScale = Vector3.one * 0.01f;
                break;
            default:
                break;
        }
    }

    //dollyTrack 스케일 side : 
    //cinemachine 높이 side : 

    public void ZoomInOut(float val)
    {
        //if(IsParallel())
        //{
        //    return;
        //}
        Debug.Log("ZoomInOut: " + val);
        switch (viewState)
        {
            case 0:
            case 1:
                Dolly_Path((val - 1) * 100f);
                break;
            case 2:
                Dolly_Offset(-(val - 1) * 100f);
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
