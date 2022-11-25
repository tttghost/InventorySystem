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
    /// ���콺 Ŭ���� ������ �Ǵ�
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
        gridSystem.handlerInitRoomItem += inventorySystem.OnInitRoomItem; //�������, �κ������� �ʱ�ȭ (�κ����� ���� �ʱ�ȭ �� ������۰�����ŭ �����Ͽ� �κ�ǥ��)

        inventorySystem.handlerPlusRoomItem += gridSystem.OnPlusRoomItem; //�κ�������-, �������+
        gridSystem.handlerPlusInvenItem += inventorySystem.OnPlusInvenItem; //�������-, �κ�������+

        gridSystem.handlerInvenLock += inventorySystem.OnInvenLock; // ������ ���õǾ����� �κ� ��

        controllerSystem.handlerPanelPressed += OnTouchInputController; //Ư�� �г��� ������ �Ǿ��� �� �̵�ȸ����� ����
        gridSystem.handlerMoveRoomObject +=  OnTouchInputController; //�������� �ű� �� �̵�ȸ����� ����
        gridSystem.handlerItemType += inventorySystem.OnItemType; //�������� �������� �� ���� ���
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
        ItemDatabase.instance.LoadDB(); //���ε���
        inventorySystem.Init(); //�κ��丮 �ʱ�ȭ
        gridSystem.Load(); //�ʱ�ȭ�� �κ��丮���� �׸��������Ʈ��ŭ ����
    }

    /// <summary>
    /// ��ġ��ǲ��Ʈ�ѷ� Ȱ��ȭ/��Ȱ��ȭ : ī�޶�ȸ��,�̵�,�� �����
    /// </summary>
    /// <param name="enable"></param>
    private void OnTouchInputController(bool enable)
    {
        touchController.enabled = enable;
    }
}
