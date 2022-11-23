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
