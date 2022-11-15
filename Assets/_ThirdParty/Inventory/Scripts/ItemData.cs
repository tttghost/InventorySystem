using System;
using System.Collections;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// <summary>
/// 아이템에 달아주는 클래스
/// </summary>
public class ItemData : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerDownHandler, IPointerEnterHandler, IPointerExitHandler, IPointerUpHandler, IPointerClickHandler
{

    private Inventory inv;
    private ItemDatabase database;

    public InvenItem invenItem { private set ; get; } //인벤토리
    public Text txt_Stack;
    public string category; //아이템 카테고리
    
    private Tooltip tooltip;
    private Vector2 offset;
    private Coroutine coroutine;

    //슬롯아이디
    private int _slotId;
    public int slotId
    {
        get => _slotId;
        set
        {
            _slotId = value;
            if (slotId == -1)
            {
                transform.SetParent(null);
                gameObject.SetActive(false);
                return;
            }
            Slot slot = inv.slotList[slotId];
            slot.itemId = invenItem.itemId;
            transform.SetParent(slot.transform, false);
            transform.localPosition = Vector3.zero;
            transform.localScale = Vector3.one;
            gameObject.SetActive(true);
        }
    }

    //남은스택
    private int _leftStack;
    public int leftStack
    {
        get => _leftStack;
        set
        {
            _leftStack = value;
            if (value == 0)
            {
                inv.RemoveItem(this);
            }
            txt_Stack.text = _leftStack.ToString();
        }
    }

    /// <summary>
    /// 아이템셋팅
    /// </summary>
    /// <param name="itemId"></param>
    void OnClick_InvenItemMinus(int itemId)
    {
        inv.MinusInvenItem(itemId);
    }

    public enum eItemState
    {
        idle, //기본상태
        press, //누른상태
        hold, //길게눌러서 홀드한 상태
        move, //길게눌러 홀드한 상태에서 이동
        drag, //길게누르지않고 드래그
    }
    public eItemState _itemState;
    public eItemState itemState
    {
        get => _itemState;
        set
        {
            _itemState = value;
            //Debug.Log("itemState: " + itemState);
            switch (value)
            {
                case eItemState.idle:
                case eItemState.drag:
                    {
                        GetComponent<RectTransform>().localScale = Vector3.one * 1f;
                        GetComponent<CanvasGroup>().alpha = 1f;
                        StopPress();
                    }
                    break;
                case eItemState.press:
                    {
                        GetComponent<RectTransform>().localScale = Vector3.one * 0.9f;
                        GetComponent<CanvasGroup>().alpha = 1f;
                        StartPress();
                    }
                    break;
                case eItemState.hold:
                case eItemState.move:
                    {
                        GetComponent<RectTransform>().localScale = Vector3.one * 1.1f;
                        GetComponent<CanvasGroup>().alpha = 0.5f;
                    }
                    break;
                default:
                    break;
            }
        }
    }


    void Awake()
    {
        database = ItemDatabase.instance;
        inv = GameObject.Find("InventorySystem").GetComponent<Inventory>();
        tooltip = inv.GetComponent<Tooltip>();
    }

    public static Sprite Tex2Sprite(Texture2D _tex)
    {
        return Sprite.Create(_tex, new Rect(0, 0, _tex.width, _tex.height), new Vector2(0.5f, 0.5f));
    }

    /// <summary>
    /// 데이터 초기화
    /// </summary>
    /// <param name="slotId"></param>
    /// <param name="invenItem"></param>
    public void SetItemData(int slotId, InvenItem invenItem)
    {
        this.invenItem = invenItem;
        ItemType itemType = database.GetItemType(invenItem.itemId);
        leftStack = this.invenItem.stack;
        gameObject.name = "Item: " + itemType.title;
        category = itemType.categoryType.ToString();
        //transform.GetComponent<Image>().sprite = Resources.Load<Sprite>("Sprites/Items/" + itemType.slug);
        transform.GetComponent<Image>().sprite = Tex2Sprite(ItemDatabase.instance.test_Grid.thumbnailList[slotId]);
        if (inv.categoryType != -1 || inv.categoryType == itemType.categoryType)
        {
            this.slotId = slotId;
        }
        else
        {
            this.slotId = -1;
        }
   
    }

    /// <summary>
    /// 누른상태에서 드래그 시작할때
    /// </summary>
    /// <param name="eventData"></param>
    public void OnBeginDrag(PointerEventData eventData)
    {
        if (itemState == eItemState.press)
        {
            GetComponentInParent<ScrollRect>().OnBeginDrag(eventData);
            itemState = eItemState.drag;
            return;
        }
        itemState = eItemState.move;

        this.transform.SetParent(this.transform.parent.parent);
        this.transform.position = eventData.position - offset;

        GetComponent<CanvasGroup>().blocksRaycasts = false;

    }



    /// <summary>
    /// 누른상태에서 드래그 중일때
    /// </summary>
    /// <param name="eventData"></param>
    public void OnDrag(PointerEventData eventData)
    {
        if (itemState != eItemState.move)
        {
            GetComponentInParent<ScrollRect>().OnDrag(eventData);
            return;
        }
        this.transform.position = eventData.position - offset;
        //if (database.GetInvenItem(invenItem.itemId) != null)
        //{
        //}
    }


    /// <summary>
    /// 누른상태에서 드래그 끊났을때
    /// </summary>
    /// <param name="eventData"></param>
    public void OnEndDrag(PointerEventData eventData)
    {
        if (itemState != eItemState.move)
        {
            GetComponentInParent<ScrollRect>().OnEndDrag(eventData);
            return;
        }
        itemState = eItemState.idle;

        this.transform.SetParent(inv.slotList[slotId].transform);
        this.transform.position = inv.slotList[slotId].transform.position;

        GetComponent<CanvasGroup>().blocksRaycasts = true;
    }

    /// <summary>
    /// 누른상태, 0.2초 지속해야 누른것으로 인정
    /// </summary>
    /// <param name="eventData"></param>
    public void OnPointerDown(PointerEventData eventData)
    {
        itemState = eItemState.press;

        offset = eventData.position - new Vector2(this.transform.position.x, this.transform.position.y);
    }

    void StartPress()
    {
        StopPress();
        coroutine = StartCoroutine(Co_StartHolding());
    }
    private IEnumerator Co_StartHolding()
    {
        float curTime = 0f;
        float durTime = 0.3f;
        while (curTime < 1f)
        {
            curTime += Time.deltaTime / durTime;
            if (itemState != eItemState.press)
            {
                yield break;
            }
            yield return null;
        }

        itemState = eItemState.hold;
    }

    private void StopPress()
    {
        if (coroutine != null)
        {
            StopCoroutine(coroutine);
        }
    }

    /// <summary>
    /// 뗀상태, 레이캐스트 끊겨도 유지됨
    /// </summary>
    /// <param name="eventData"></param>
    public void OnPointerUp(PointerEventData eventData)
    {
        if (itemState == eItemState.press) //클릭시
        {
            itemState = eItemState.idle;
            leftStack--;
            int itemId = eventData.pointerDrag.GetComponent<ItemData>().invenItem.itemId;
            OnClick_InvenItemMinus(itemId);
        }
        if(itemState == eItemState.hold)
        {
            itemState = eItemState.idle;
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        tooltip.Activate(database.GetItemType(invenItem.itemId));
    }

    /// <summary>
    /// 벗어난 상태, 레이캐스트 끊겨도 벗어난 상태가 될수 있다.
    /// </summary>
    /// <param name="eventData"></param>
    public void OnPointerExit(PointerEventData eventData)
    {
        tooltip.Deactivate();
    }

    public void OnPointerClick(PointerEventData eventData)
    {

    }
}
