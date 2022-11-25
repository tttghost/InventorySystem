using BitBenderGames;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CheckMouseOverPanel : MonoBehaviour
{
    public RectTransform target;

    private void Start()
    {
        target = target == null ? (RectTransform)transform : target;
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0) && IsMouseOverPanel(target))
        {
            MyRoomManager.instance.OnTouchInputController(false);
        }
        else if(Input.GetMouseButtonUp(0) )
        {
            MyRoomManager.instance.OnTouchInputController(true);
        }

    }

    /// <summary>
    /// ���콺�� �г����� �ִ��� üũ
    /// </summary>
    private bool IsMouseOverPanel(RectTransform rt)
    {
        return RectTransformUtility.RectangleContainsScreenPoint(rt, Input.mousePosition);
    }

    /// <summary>
    /// �׷��ȷ���ĳ��Ʈ ���̸� ���� UI������Ʈ ����
    /// </summary>
    private bool IsGraphicRaycast()
    {
        GraphicRaycaster gr = GetComponent<GraphicRaycaster>();
        var ped = new PointerEventData(null);
        ped.position = Input.mousePosition;
        List<RaycastResult> results = new List<RaycastResult>();
        gr.Raycast(ped, results);

        if (results.Count <= 0) return false;
        
        // �̺�Ʈ ó���κ�
        return true;
    }


}
