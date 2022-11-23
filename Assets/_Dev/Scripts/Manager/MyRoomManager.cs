using BitBenderGames;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyRoomManager : MonoBehaviour
{
    public static MyRoomManager instance;

    public ControllerSystem controllerSystem;
    public InventorySystem inventorySystem;
    public GridSystem gridSystem;

    public MobileTouchCamera touchCamera;
    public TouchInputController touchController;


    private void Awake()
    {
        instance = this;
        gameObject.AddComponent<ItemDatabase>();
    }

    private IEnumerator Start()
    {
        yield return null;
        Load();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha9))
        {
            Save();
        }
        else if (Input.GetKeyDown(KeyCode.Alpha0))
        {
            Load();
        }
        CheckMousePressedMove();
    }

    /// <summary>
    /// 마우스 클릭후 움직임 판단
    /// </summary>
    private void CheckMousePressedMove()
    {
        if (touchCamera.IsDragging && !isDragging)
        {
            isDragging = true;
        }
        if (touchCamera.IsPinching && !isPinching)
        {
            isPinching = true;
        }
        if (Input.GetMouseButtonUp(0))
        {
            StartCoroutine(Co_Wait());
        }
    }

    public bool GetMousePressedMove()
    {
        return (isDragging || isPinching);
    }

    public bool isDragging;
    public bool isPinching;

    private IEnumerator Co_Wait()
    {
        yield return null;
        isDragging = false;
        isPinching = false;
    }

    private void OnEnable()
    {
        gridSystem.handlerInitRoomItem += inventorySystem.OnInitRoomItem; //룸아이템, 인벤아이템 초기화 (인벤부터 개수 초기화 후 룸아이템개수만큼 차감하여 인벤표시)

        inventorySystem.handlerPlusRoomItem += gridSystem.OnPlusRoomItem; //인벤아이템-, 룸아이템+
        gridSystem.handlerPlusInvenItem += inventorySystem.OnPlusInvenItem; //룸아이템-, 인벤아이템+

        gridSystem.handlerInvenLock += inventorySystem.OnInvenLock; // 아이템 선택되었을때 인벤 락

        controllerSystem.handlerPanelPressed += OnTouchInputController;
        gridSystem.handlerMoveRoomObject += OnTouchInputController;
    }

   

    private void OnDisable()
    {
        gridSystem.handlerInitRoomItem -= inventorySystem.OnInitRoomItem;

        inventorySystem.handlerPlusRoomItem -= gridSystem.OnPlusRoomItem;
        gridSystem.handlerPlusInvenItem -= inventorySystem.OnPlusInvenItem;

        gridSystem.handlerInvenLock -= inventorySystem.OnInvenLock;

        gridSystem.handlerMoveRoomObject -= OnTouchInputController;
        controllerSystem.handlerPanelPressed -= OnTouchInputController;
    }

    private void Save()
    {
        gridSystem.Save();
    }

    private void Load()
    {
        ItemDatabase.instance.LoadDB(); //디비로드후
        inventorySystem.Init(); //인벤토리 초기화
        gridSystem.Load(); //초기화된 인벤토리에서 그리드오브젝트만큼 제거
    }

    /// <summary>
    /// 터치인풋컨트롤러 활성화/비활성화 : 카메라회전,이동,줌 제어용
    /// </summary>
    /// <param name="enable"></param>
    private void OnTouchInputController(bool enable)
    {
        touchController.enabled = enable;
    }
}
