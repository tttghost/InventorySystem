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
        ItemDatabase.instance.LoadDB(); //디비로드후
        inventorySystem.Init(); //인벤토리 초기화
        gridSystem.Load(); //초기화된 인벤토리에서 그리드오브젝트만큼 제거
    }

    private void OnEnable()
    {
        gridSystem.InitRoomItem += inventorySystem.InitInvenItem; //룸아이템, 인벤아이템 초기화 (인벤부터 개수 초기화 후 룸아이템개수만큼 차감하여 인벤표시)

        inventorySystem.MinusInvenItem += gridSystem.PlusRoomItem; //인벤에서 아이템이 빠지면 룸에 아이템 추가
        gridSystem.MinusRoomItem += inventorySystem.PlusInvenItem; //룸에서 아이템이 빠지면 인벤에 아이템 추가

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
