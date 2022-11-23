using BitBenderGames;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyRoomManager : MonoBehaviour
{
    public static MyRoomManager instance;

    public InventorySystem inventorySystem;
    public GridSystem gridSystem;
    public TouchInputController touchInputController;
    public delegate TouchInputController TouchInputControllerHandler(bool enable);
    public TouchInputControllerHandler touchInputControllerHandler;

    private void Awake()
    {
        instance = this;
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

    }

    private void OnEnable()
    {
        gridSystem.InitRoomItem += inventorySystem.OnInitRoomItem; //�������, �κ������� �ʱ�ȭ (�κ����� ���� �ʱ�ȭ �� ������۰�����ŭ �����Ͽ� �κ�ǥ��)

        inventorySystem.MinusInvenItem += gridSystem.OnPlusRoomItem; //�κ�������-, �������+
        gridSystem.MinusRoomItem += inventorySystem.OnPlusInvenItem; //�������-, �κ�������+

        gridSystem.invenLock += inventorySystem.OnInvenLock; // ������ ���õǾ����� �κ� ��

        //touchInputControllerHandler += gridSystem.on
    }


    private void OnDisable()
    {
        gridSystem.InitRoomItem -= inventorySystem.OnInitRoomItem;

        inventorySystem.MinusInvenItem -= gridSystem.OnPlusRoomItem;
        gridSystem.MinusRoomItem += inventorySystem.OnPlusInvenItem;

        gridSystem.invenLock -= inventorySystem.OnInvenLock;


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
}
