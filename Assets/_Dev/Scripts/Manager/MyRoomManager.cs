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
    public Camera mainCamera;
    private MobileTouchCamera touchCamera;
    private TouchInputController touchController;
    


    private void Awake()
    {
        instance = this;
        gameObject.AddComponent<ItemDatabase>();

        touchCamera = mainCamera.GetComponent<MobileTouchCamera>();
        touchController = mainCamera.GetComponent<TouchInputController>();
    }

    private IEnumerator Start()
    {
        yield return null;
        OnClick_Load();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha9))
        {
            OnClick_Save();
        }
        else if (Input.GetKeyDown(KeyCode.Alpha0))
        {
            OnClick_Load();
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

        controllerSystem.handlerPanelPressed += OnTouchInputController; //특정 패널이 프레스 되었을 때 이동회전기능 제어
        gridSystem.handlerMoveRoomObject +=  OnTouchInputController; //아이템을 옮길 때 이동회전기능 제어
        gridSystem.handlerItemType += inventorySystem.OnItemType; //아이템을 선택했을 때 정보 출력
    }

   

    private void OnDisable()
    {
        gridSystem.handlerInitRoomItem -= inventorySystem.OnInitRoomItem;

        inventorySystem.handlerPlusRoomItem -= gridSystem.OnPlusRoomItem;
        gridSystem.handlerPlusInvenItem -= inventorySystem.OnPlusInvenItem;

        gridSystem.handlerInvenLock -= inventorySystem.OnInvenLock;

        controllerSystem.handlerPanelPressed -= OnTouchInputController;
        gridSystem.handlerMoveRoomObject -= OnTouchInputController;
        gridSystem.handlerItemType -= inventorySystem.OnItemType;
    }

    public void OnClick_ResetCamera()
    {
        StartCoroutine(Co_OnClick_ResetCamera());
    }

    private IEnumerator Co_OnClick_ResetCamera()
    {
        mainCamera.transform.eulerAngles = Vector3.right * 90f;
        //yield return Co_Rotation(mainCamera.transform, 90f);
        yield return null;
        mainCamera.transform.position = Vector3.up * touchCamera.CamZoomMax;
    }

    private IEnumerator Co_Rotation(Transform tr, float targetRot)
    {
        float oriRot = tr.eulerAngles.x;
        float curTime = 0f;
        float durTime = 1f;
        while (curTime < 1f)
        {
            curTime += Time.deltaTime / durTime;
            mainCamera.transform.eulerAngles = Vector3.right * Mathf.Lerp(oriRot, targetRot, curTime);
            yield return null;
        }
    }

    public void OnClick_InvenLock()
    {
        gridSystem.handlerInvenLock?.Invoke(!inventorySystem.img_Block.enabled);
    }

    public void OnClick_Save()
    {
        gridSystem.Save();
    }

    public void OnClick_Load()
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
