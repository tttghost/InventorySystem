using BitBenderGames;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ControllerSystem : MonoBehaviour
{
    private bool isPanelPressed = false;

    public RectTransform target;

    public delegate void BoolParam(bool b);
    public BoolParam handlerPanelPressed;

    private void OnEnable()
    {
        MyRoomManager.instance.gridSystem.handlerInvenLock += MyRoomManager.instance.inventorySystem.OnInvenLock; // 아이템 선택되었을때 인벤 락
    }
    private void OnDisable()
    {
        MyRoomManager.instance.gridSystem.handlerInvenLock -= MyRoomManager.instance.inventorySystem.OnInvenLock;
    }

    private void Start()
    {
        target = target == null ? (RectTransform)transform : target;
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0) && IsMouseOverPanel(target))
        {
            handlerPanelPressed?.Invoke(isPanelPressed = false);
        }
        else if(Input.GetMouseButtonUp(0) && !isPanelPressed)
        {
            handlerPanelPressed?.Invoke(isPanelPressed = true);
        }

    }

    /// <summary>
    /// 마우스가 패널위에 있는지 체크
    /// </summary>
    private bool IsMouseOverPanel(RectTransform rt)
    {
        return RectTransformUtility.RectangleContainsScreenPoint(rt, Input.mousePosition);
    }

    /// <summary>
    /// 그래픽레이캐스트 레이를 쏴서 UI오브젝트 검출
    /// </summary>
    private bool IsGraphicRaycast()
    {
        GraphicRaycaster gr = GetComponent<GraphicRaycaster>();
        var ped = new PointerEventData(null);
        ped.position = Input.mousePosition;
        List<RaycastResult> results = new List<RaycastResult>();
        gr.Raycast(ped, results);

        if (results.Count <= 0) return false;
        
        // 이벤트 처리부분
        return true;
    }


}
