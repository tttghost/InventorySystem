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
        ItemDatabase.instance.LoadDB(); //디비로드후
        inventory.Init(); //인벤토리 초기화
        test_Grid.Load(); //초기화된 인벤토리에서 그리드오브젝트만큼 제거
    }

    private void OnEnable()
    {
        test_Grid.InitRoomItem += inventory.InitInvenItem; //룸아이템, 인벤아이템 초기화 (인벤부터 개수 초기화 후 룸아이템개수만큼 차감하여 인벤표시)

        inventory.MinusInvenItem += test_Grid.PlusRoomItem; //인벤에서 아이템이 빠지면 룸에 아이템 추가
        test_Grid.MinusRoomItem += inventory.PlusInvenItem; //룸에서 아이템이 빠지면 인벤에 아이템 추가
    }

    private void OnDisable()
    {
        test_Grid.InitRoomItem -= inventory.InitInvenItem;

        inventory.MinusInvenItem -= test_Grid.PlusRoomItem;
        test_Grid.MinusRoomItem += inventory.PlusInvenItem;
    }

}
