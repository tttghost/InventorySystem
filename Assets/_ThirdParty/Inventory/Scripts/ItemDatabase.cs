using UnityEngine;
using LitJson;
using System.Collections.Generic;
using System.IO;
using Hypertonic.GridPlacement.Example.BasicDemo;
using System;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Linq;

public class ItemDatabase : MonoBehaviour
{
    public static ItemDatabase instance;

    public Dictionary<string, HomeType> db_HomeType = new Dictionary<string, HomeType>();
    public Dictionary<int, GridType> db_GridType = new Dictionary<int, GridType>();
    public Dictionary<int, ItemType> db_ItemType = new Dictionary<int, ItemType>();
    public List<InvenItem> db_InvenItem = new List<InvenItem>();
    public List<RoomItem> db_RoomItem = new List<RoomItem>();

    private string subPath = "InvenItem.json";

    private void Awake()
    {
        instance = this;
        //LoadDB();
    }


    private string GetPath(string subPath)
    {
        string oriPath = Path.Combine(Application.streamingAssetsPath, subPath);
#if UNITY_ANDROID
        WWW reader = new WWW(oriPath);
        while (!reader.isDone) { }
        var realPath = Application.persistentDataPath + "/db";
        File.WriteAllBytes(realPath, reader.bytes);

        oriPath = File.ReadAllText(realPath);
#endif
        return oriPath;
    }
    /// <summary>
    /// 
    /// </summary>
    public void LoadDB()
    {
        string path = GetPath(subPath);
        JObject jobj = (JObject)JsonConvert.DeserializeObject(path);
        //JObject jobj = (JObject)JsonConvert.DeserializeObject(File.ReadAllText(Application.dataPath + "/StreamingAssets/InvenItem.json"));
        foreach (var x in jobj)
        {
            string data = x.Value.ToString();
            switch (x.Key)
            {
                case "HomeType": db_HomeType = JsonConvert.DeserializeObject<HomeType[]>(data).ToDictionary(x => x.homeId, x => x); break;
                case "GridType": db_GridType = JsonConvert.DeserializeObject<GridType[]>(data).ToDictionary(x => x.gridId, x => x); break;
                case "ItemType": db_ItemType = JsonConvert.DeserializeObject<ItemType[]>(data).ToDictionary(x => x.itemId, x => x); break;
                case "InvenItem": db_InvenItem = JsonConvert.DeserializeObject<InvenItem[]>(data).ToList(); break;
                case "RoomItem": db_RoomItem = JsonConvert.DeserializeObject<RoomItem[]>(data).ToList(); break;
            }
        }
    }

    /// <summary>
    /// gridId로 그리드타입 추출
    /// </summary>
    /// <param name="gridId"></param>
    /// <returns></returns>
    public GridType GetGridType(int gridId)
    {
        return db_GridType[gridId];
    }

    /// <summary>
    /// prefabName으로 그리드타입 추출
    /// </summary>
    /// <param name="prefabName"></param>
    /// <returns></returns>
    public GridType GetGridType(string prefabName)
    {
        return db_GridType.FirstOrDefault(x => x.Value.gridId == GetItemType(prefabName).gridId).Value;
    }

    /// <summary>
    /// prefabName으로 아이템타입 추출
    /// </summary>
    /// <param name="prefabName">프리팹이름</param>
    /// <returns></returns>
    public ItemType GetItemType(string prefabName)
    {
        return db_ItemType.FirstOrDefault(x => x.Value.prefabName == prefabName).Value;
    }

    /// <summary>
    /// itemId로 아이템타입 추출
    /// </summary>
    /// <param name="itemId">아이템아이디</param>
    /// <returns></returns>
    public ItemType GetItemType(int itemId)
    {
        return db_ItemType[itemId];
    }

    /// <summary>
    /// itemId로  인벤아이템 추출
    /// </summary>
    /// <param name="itemId"></param>
    /// <returns></returns>
    public InvenItem GetInvenItem(int itemId)
    {
        return db_InvenItem.FirstOrDefault(x => x.itemId == itemId);
    }

   


}

/// <summary>
/// 홈타입 정의
/// </summary>
[Serializable]
public class HomeType
{
    public string homeId { get; set; }
    public int x { get; set; }
    public int y { get; set; }
    public int z { get; set; }
}

/// <summary>
/// 그리드타입 정의
/// </summary>
[Serializable]
public class GridType
{
    public int gridId { get; set; }
    public string gridName { get; set; }
}


/// <summary>
/// 아이템타입 정의
/// </summary>
[Serializable]
public class ItemType
{
    public int itemId { get; set; }
    public int gridId { get; set; }
    public string prefabName { get; set; }
    public int categoryType { get; set; }
    public int priceType { get; set; }
    public string title { get; set; }
    public string description { get; set; }
    public string slug { get; set; }
    public bool stackable { get; set; }
    public int capacity { get; set; }
    public ItemType()
    {
        this.itemId = -1;
    }
}

/// <summary>
/// 인벤아이템 정보
/// </summary>
[Serializable]
public class InvenItem
{
    public InvenItem()
    {
        this.itemId = -1;
    }

    public InvenItem(int itemId, int stack)
    {
        this.itemId = itemId;
        this.stack = stack;
    }

    public int itemId { get; set; }
    public int stack { get; set; }
}

/// <summary>
/// 룸아이템 정보
/// </summary>
[Serializable]
public class RoomItem
{
    public int itemId { get; set; }
    public int x { get; set; }
    public int z { get; set; }
    public int rotate { get; set; }
}
