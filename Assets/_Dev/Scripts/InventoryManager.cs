using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    public InventorySystem inventory;
    public GridSystem test_Grid;

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
        test_Grid.Save();
    }

    private void Load()
    {
        ItemDatabase.instance.LoadDB(); //���ε���
        inventory.Init(); //�κ��丮 �ʱ�ȭ
        test_Grid.Load(); //�ʱ�ȭ�� �κ��丮���� �׸��������Ʈ��ŭ ����
    }

    private void OnEnable()
    {
        test_Grid.InitRoomItem += inventory.InitInvenItem; //�������, �κ������� �ʱ�ȭ (�κ����� ���� �ʱ�ȭ �� ������۰�����ŭ �����Ͽ� �κ�ǥ��)

        inventory.MinusInvenItem += test_Grid.PlusRoomItem; //�κ����� �������� ������ �뿡 ������ �߰�
        test_Grid.MinusRoomItem += inventory.PlusInvenItem; //�뿡�� �������� ������ �κ��� ������ �߰�
    }

    private void OnDisable()
    {
        test_Grid.InitRoomItem -= inventory.InitInvenItem;

        inventory.MinusInvenItem -= test_Grid.PlusRoomItem;
        test_Grid.MinusRoomItem += inventory.PlusInvenItem;
    }

}
