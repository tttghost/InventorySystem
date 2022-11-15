using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using System;
using System.Linq;

public class Slot : MonoBehaviour, IDropHandler
{
	public int slotId;
    public int itemId;
	private InventorySystem inven;

    private void Awake()
	{
		inven = GameObject.Find("InventorySystem").GetComponent<InventorySystem>();
	}

    /// <summary>
    /// 온드래그 : 슬롯에 드래그 했을 시
    /// </summary>
    /// <param name="eventData"></param>
    public void OnDrop(PointerEventData eventData)
    {
        ItemData droppedItemData = eventData.pointerDrag.GetComponent<ItemData>(); //드롭된 아이템
        if (droppedItemData == null || droppedItemData.itemState != ItemData.eItemState.move)
        {
            return;
        }
        Slot oriSlot = inven.slotList[droppedItemData.slotId];
        if (itemId == -1) //물체가 빈 슬롯위라면
        {
            oriSlot.itemId = -1;
            droppedItemData.slotId = slotId;
        }
        else if(itemId != droppedItemData.invenItem.itemId) //물체가 다른물체위라면
        {
            ItemData curItemData = transform.GetComponentInChildren<ItemData>(); //자식오브젝트의 아이템데이터
            Swap(ref oriSlot.itemId, ref itemId);

            curItemData.slotId = oriSlot.slotId;
            droppedItemData.slotId = slotId;
        }
        inven.itemDataList = inven.itemDataList.OrderBy(x => x.slotId).ToList();
    }

    /// <summary>
    /// 스왑
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="t1"></param>
    /// <param name="t2"></param>
    private void Swap<T>(ref T t1, ref T t2)
    {
        T swapT = t1;
        t1 = t2;
        t2 = swapT;
    }
}