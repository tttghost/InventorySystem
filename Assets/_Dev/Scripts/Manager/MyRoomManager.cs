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
        gridSystem.InitRoomItem += inventorySystem.OnInitRoomItem; //룸아이템, 인벤아이템 초기화 (인벤부터 개수 초기화 후 룸아이템개수만큼 차감하여 인벤표시)

        inventorySystem.MinusInvenItem += gridSystem.OnPlusRoomItem; //인벤아이템-, 룸아이템+
        gridSystem.MinusRoomItem += inventorySystem.OnPlusInvenItem; //룸아이템-, 인벤아이템+

        gridSystem.invenLock += inventorySystem.OnInvenLock; // 아이템 선택되었을때 인벤 락

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
        ItemDatabase.instance.LoadDB(); //디비로드후
        inventorySystem.Init(); //인벤토리 초기화
        gridSystem.Load(); //초기화된 인벤토리에서 그리드오브젝트만큼 제거
    }
}
