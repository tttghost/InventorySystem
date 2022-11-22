using Lean.Touch;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class Test_Zoom : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        LeanTouch.OnFingerDown += LeanTouch_OnFingerDown;
        LeanTouch.OnGesture += LeanTouch_OnGesture;
    }

    // Update is called once per frame
    void Update()
    {
        sld.value = Mathf.Lerp(sld.value, value, Time.deltaTime);

        if (Input.GetMouseButtonDown(0))
        {
            RayCastA(screenCenter, rayStart);
        }
        if (Input.GetMouseButton(0))
        {
            RayCastA(Input.mousePosition, rayUpdate);
        }
        if (Vector3.Distance(center.position, new Vector3(screenStart.x, 0, screenStart.y)) > 1f)
        {

        center.position = Vector3.Lerp(center.position, center.position + targetCenter.normalized, 3f * Time.deltaTime);
        }
        //center.Translate(new Vector3(targetCenter.x, 0f, targetCenter.y).normalized * 0.1f * dist * Time.deltaTime);
    }

    void RayCastA(Vector2 rayTr, Transform targetTr)
    {
        Ray tempRay = Camera.main.ScreenPointToRay(rayTr);
        float maxDistance = 100;
        RaycastHit hit;
        // Physics.Raycast (레이저를 발사할 위치, 발사 방향, 충돌 결과, 최대 거리)
        bool isHit = Physics.Raycast(tempRay, out hit, maxDistance);
        if (isHit)
        {
            targetTr.position = hit.point;
        }
    }


    public Transform center;
    public Transform rayStart;
    public Transform rayUpdate;
    public Vector2 screenStart;
    public Vector2 screenUpdate;

    public Slider sld;
    public float value;
    public Text txt;
    public void SetValue(float value)
    {
        this.value = value;

        //투핑거상태
        //이동 : 패러럴
        //확대 / 축소 : 줌
    }
    private void OnDrawGizmos()
    {
        //Ray tempRay = Camera.main.ScreenPointToRay(screenCenter);
        //float maxDistance = 100;
        //RaycastHit hit;
        //// Physics.Raycast (레이저를 발사할 위치, 발사 방향, 충돌 결과, 최대 거리)
        //bool isHit = Physics.Raycast(tempRay, out hit, maxDistance);

        //if (isHit)
        //{
        //    Gizmos.color = Color.green;
        //    Gizmos.DrawRay(tempRay.origin, tempRay.direction * hit.distance);
        //}
        //else
        //{
        //    Gizmos.color = Color.red;
        //    Gizmos.DrawRay(tempRay.origin, tempRay.direction * maxDistance);
        //}

     
    }


    public Image[] img;
    public Image[] screenImg;

    Vector2 screenCenter;
    Vector3 targetCenter;
    float dist;


    private void LeanTouch_OnGesture(List<LeanFinger> obj)
    {
        var fingerList = obj.Skip(1).ToList();
        if (obj.Count == 3)
        {
            //center.position = Vector3.Lerp(center.position, rayStart.position, Time.deltaTime);
            screenImg[1].transform.position = LeanGesture.GetScreenCenter(fingerList);
            screenImg[2].transform.position = 2f * screenImg[0].transform.position - screenImg[1].transform.position;
            targetCenter = new Vector3(screenImg[2].transform.position.x, 0f, screenImg[2].transform.position.y) - new Vector3(screenStart.x, 0f, screenStart.y);
            dist = Vector2.Distance(screenImg[2].transform.position, screenStart);
            Debug.Log("screenImg[2].transform.position.normalized : " + targetCenter.normalized);
            
        }

        if (obj.Count == 3 && !isTwoTap)
        {
            rayStart.position = default;
            rayUpdate.position = default;
            screenStart = screenCenter = LeanGesture.GetScreenCenter(fingerList);
            screenImg[0].transform.position = Input.mousePosition;
            tapState = eTapState.none;
            isTwoTap = true;
            if (coroutine != null)
            {
                coroutine = null;
            }
            coroutine = StartCoroutine(Co_StartIsTwoTap(fingerList));
        }
        else if(obj.Count != 3 && isTwoTap)
        {
            tapState = eTapState.none;
            isTwoTap = false;
            StopCoroutine(coroutine);
        }
    }

    Coroutine coroutine;

    private void LeanTouch_OnFingerDown(LeanFinger obj)
    {
        
    }

    bool isTwoTap = false;

    public enum eTapState
    {
        none,
        parallel,
        zoom,
    }

    public eTapState tapState;

    IEnumerator Co_StartIsTwoTap(List<LeanFinger> obj)
    {
        float startDist =  LeanGesture.GetLastScaledDistance(obj);
        List<Vector2> dist = new List<Vector2>();
        for (int i = 0; i < obj.Count; i++)
        {
            dist.Add(img[i].transform.position = obj[i].ScreenPosition);
        }

        bool isEnd = false;
        do{
            for (int i = 0; i < obj.Count; i++)
            {
                float tempDist = Vector2.Distance(dist[i], obj[i].ScreenPosition);
                if (tempDist > f1)
                {
                    isEnd = true;
                    break;
                }
            }
            yield return null;
        } while (!isEnd);
        float lastDist = LeanGesture.GetLastScaledDistance(obj);
        float resultDist = Mathf.Abs(startDist - lastDist);
        if (resultDist > f2)
        {
            Debug.Log("zoom");
            tapState = eTapState.zoom;
        }
        else
        {
            Debug.Log("parallel");
            tapState = eTapState.parallel;
        }
    }

    public float f1;
    public float f2;

}
