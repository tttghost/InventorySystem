using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    public InventorySystem inventorySystem;
    public GridSystem gridSystem;

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

    private void OnEnable()
    {
        gridSystem.InitRoomItem += inventorySystem.InitInvenItem; //�������, �κ������� �ʱ�ȭ (�κ����� ���� �ʱ�ȭ �� ������۰�����ŭ �����Ͽ� �κ�ǥ��)

        inventorySystem.MinusInvenItem += gridSystem.PlusRoomItem; //�κ����� �������� ������ �뿡 ������ �߰�
        gridSystem.MinusRoomItem += inventorySystem.PlusInvenItem; //�뿡�� �������� ������ �κ��� ������ �߰�

        gridSystem.invenLock += inventorySystem.SetInvenLock;
    }

    private void OnDisable()
    {
        gridSystem.InitRoomItem -= inventorySystem.InitInvenItem;

        inventorySystem.MinusInvenItem -= gridSystem.PlusRoomItem;
        gridSystem.MinusRoomItem += inventorySystem.PlusInvenItem;

        gridSystem.invenLock -= inventorySystem.SetInvenLock;
    }

}
