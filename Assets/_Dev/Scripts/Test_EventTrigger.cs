using BitBenderGames;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Test_EventTrigger : MonoBehaviour
{
    public TouchInputController touchInputController;
    public RectTransform rt;

    private void Update()
    {
        if(Input.GetMouseButtonDown(0) && IsMouseOverPanel(rt))
        {
            touchInputController.enabled = false;
        }
        else if(Input.GetMouseButtonUp(0) && !touchInputController.enabled)
        {
            touchInputController.enabled = true;
        }

    }


    /// <summary>
    /// �׷��ȷ���ĳ��Ʈ ���̸� ���� UI������Ʈ ����
    /// </summary>
    bool IsGraphicRaycast()
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

    /// <summary>
    /// ���콺�� �г����� �ִ��� üũ
    /// </summary>
    bool IsMouseOverPanel(RectTransform rt)
    {
        return RectTransformUtility.RectangleContainsScreenPoint(rt, Input.mousePosition);  
    }
}
