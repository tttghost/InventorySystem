using Hypertonic.GridPlacement;
using Hypertonic.GridPlacement.CustomSizing;
using Hypertonic.GridPlacement.Example.BasicDemo;
using Hypertonic.GridPlacement.Models;
using Newtonsoft.Json.Linq;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class GridSystem : MonoBehaviour
{
    public GameObject[] prefabs;
    public GameObject cover;
    public GameObject pivot;

    public Image inMove;
    public GameObject outMove;

    private GameObject curObj;
    private GridSaveManager_Custom gridSaveManager;
    public GameObject cameraRotateY;
    public GameObject cameraPivotZ;
    public float rotateSpeed;
    public float zoomSpeed;
    private bool isValid;
    public List<Texture2D> thumbnailList = new List<Texture2D>();

    /// <summary>
    /// 인벤시스템 구현하자..!!
    /// </summary>
    private void Awake()
    {
        gridSaveManager = gameObject.AddComponent<GridSaveManager_Custom>();
        for (int i = 0; i < prefabs.Length; i++)
        {
            thumbnailList.Add(RuntimePreviewGenerator.GenerateModelPreview(prefabs[i].transform, 512, 512));
        }
    }


    private void GridManager_OnPlacementValidated(bool isValid)
    {
        this.isValid = isValid;
    }

    public void OnClick_Cover()
    {
        CurrentMethodName();
        if (!isValid)
        {
            return;
        }
        cover.SetActive(false);
        pivot.SetActive(false);
        curObj = null;
        GridManagerAccessor.GridManager.ConfirmPlacement();
    }

    public void OnClick_MoveDown()
    {
        CurrentMethodName();
        inMove.raycastTarget = false;
        outMove.SetActive(false);
        cover.SetActive(false);
    }
    public void OnClick_MoveUp()
    {
        CurrentMethodName();
        inMove.raycastTarget = true;
        outMove.SetActive(true);
        cover.SetActive(true);
    }
    public void OnClick_TurnRight()
    {
        CurrentMethodName();
        HandleRotate(curObj, 90);
    }
    public void OnClick_TurnLeft()
    {
        CurrentMethodName();
        HandleRotate(curObj, -90);
    }
    public void OnClick_Inven()
    {
        CurrentMethodName();

        int itemId = ItemDatabase.instance.db_ItemType.FirstOrDefault(x => x.Value.prefabName == curObj.name).Key;
        MinusRoomItem(itemId);

        cover.SetActive(false);
        pivot.SetActive(false);
        curObj = null;
        pivot.transform.SetParent(null);
        GridManagerAccessor.GridManager.CancelPlacement();
    }

    private void CurrentMethodName()
    {
        return;
        Debug.Log(MethodBase.GetCurrentMethod().Name);
    }

    private void OnGUI()
    {
        return;
        if (curObj != null)
        {
            return;
        }
        for (int i = 0; i < prefabs.Length; i++)
        {
            if (GUI.Button(new Rect(i * 150, 0, 150, 150), new GUIContent(thumbnailList[i])))
            {
                int capture = i;
                PlusRoomItem(capture);
            }
        }
    }

    /// <summary>
    /// 룸아이템 추가
    /// </summary>
    /// <param name="itemId"></param>
    public void PlusRoomItem(int itemId)
    {
        GameObject obj = Instantiate(prefabs[itemId], GridManagerAccessor.GridManager.GetGridPosition(), new Quaternion());
        obj.name = prefabs[itemId].name;
        SelectRoomItem(obj);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="prefabName"></param>
    /// <returns></returns>
    public ItemType GetPrefabName2ItemType(string prefabName)
    {
        return ItemDatabase.instance.db_ItemType.FirstOrDefault(x => x.Value.prefabName == prefabName).Value;
    }

    /// <summary>
    /// 오브젝트 설치
    /// </summary>
    /// <param name="obj"></param>
    public void SelectRoomItem(GameObject obj)
    {
        cover.SetActive(true);

        pivot.SetActive(true);
        pivot.transform.SetParent(obj.transform);
        pivot.transform.localPosition = Vector3.zero;

        if (obj.TryGetComponent(out GridObjectInfo objectInfo))
        {
            SelectedGridManager(objectInfo.GridKey);
        }
        else
        {
            SelectedGridManager(ItemDatabase.instance.GetGridType(GetPrefabName2ItemType(obj.name).gridId).gridName);
        }

        GridManagerAccessor.GridManager.EnterPlacementMode(obj);
        curObj = obj;
    }

    private void HandleRotate(GameObject obj, float rotateY)
    {
        obj.transform.Rotate(new Vector3(0, rotateY, 0));
        GridManagerAccessor.GridManager.HandleGridObjectRotated();
    }

    private void SelectedGridManager(string gridKey)
    {
        //Debug.Log("Select : " + gridKey);
        GridManagerAccessor.GridManager.OnPlacementValidated -= GridManager_OnPlacementValidated;
        GridManagerAccessor.SetSelectedGridManager(gridKey);
        GridManagerAccessor.GridManager.OnPlacementValidated += GridManager_OnPlacementValidated;
    }

    public void Save()
    {
        Debug.Log(MethodBase.GetCurrentMethod().Name);
        gridSaveManager.HandleSaveGridObjectsPressed();
    }
    public void Load()
    {
        Debug.Log(MethodBase.GetCurrentMethod().Name);
        pivot.transform.SetParent(null);
        string path = Application.dataPath + "/StreamingAssets/RoomItem.json";
        if (!File.Exists(path))
        {
            return;
        }
        string saveDataAsJson = File.ReadAllText(path);
        //string saveDataAsJson = PlayerPrefs.GetString("GRIDMANAGER");
        _GridData _gridData = JsonUtility.FromJson<_GridData>(saveDataAsJson);
        for (int i = 0; i < _gridData._saveDatas.Count; i++)
        {
            _SaveData _saveData = _gridData._saveDatas[i];
            RoomItemInit(_saveData._gridObjectSaveDatas);
            SelectedGridManager(_saveData.gridId);
            gridSaveManager.HandleLoadGridObjectsPressed(_gridData._saveDatas[i]);
        }
    }

    /// <summary>
    /// 룸아이템 초기셋업
    /// </summary>
    /// <param name="_GridObjectSaveDatas"></param>
    private void RoomItemInit(List<_GridObjectSaveData> _GridObjectSaveDatas)
    {
        for (int i = 0; i < _GridObjectSaveDatas.Count; i++)
        {
            int itemId = ItemDatabase.instance.db_ItemType.FirstOrDefault(x => x.Value.prefabName == _GridObjectSaveDatas[i].prefabName).Key;
            InitRoomItem(itemId);
        }
    }

    public delegate void RoomItem(int idx);

    public RoomItem InitRoomItem; //룸아이템 초기셋업
    public RoomItem MinusRoomItem; //룸아이템 빼기

    private void Update()
    {
        //if (Input.GetKeyDown(KeyCode.Alpha9))
        //{
        //    Save();
        //}
        //if (Input.GetKeyDown(KeyCode.Alpha0))
        //{
        //    Load();
        //}

        if (Input.GetKey(KeyCode.A))
        {
            cameraRotateY.transform.Rotate(Vector3.up * 360f * rotateSpeed * Time.deltaTime);
        }
        if (Input.GetKey(KeyCode.D))
        {
            cameraRotateY.transform.Rotate(Vector3.up * 360f * -rotateSpeed * Time.deltaTime);
        }
        if (Input.GetKey(KeyCode.W))
        {
            if (cameraPivotZ.transform.localPosition.z < 2f)
            {
                cameraPivotZ.transform.Translate(Vector3.forward * zoomSpeed * Time.deltaTime);
            }
        }
        if (Input.GetKey(KeyCode.S))
        {
            if (cameraPivotZ.transform.localPosition.z > -2f)
            {
                cameraPivotZ.transform.Translate(-Vector3.forward * zoomSpeed * Time.deltaTime);
            }
        }
        if (curObj != null)
        {
            return;
        }
        if (Input.GetMouseButtonDown(0))
        {
            if(EventSystem.current.IsPointerOverGameObject())
            {
                return;
            }
            Vector3 mousePosition = Input.mousePosition;
            Ray ray = Camera.main.ScreenPointToRay(mousePosition, Camera.MonoOrStereoscopicEye.Mono);
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                if (hit.transform.GetComponent<GridHeightPositioner>() != null)
                {
                    SelectRoomItem(hit.transform.gameObject);
                }
            }
        }
    }
}
