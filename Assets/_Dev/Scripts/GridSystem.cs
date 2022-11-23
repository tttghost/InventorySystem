using BitBenderGames;
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
    //public GameObject cameraRotateY;
    //public GameObject cameraPivotZ;
    public float rotateSpeed;
    public float zoomSpeed;
    private bool isValid;

    public List<Texture2D> thumbnailList = new List<Texture2D>();

    public Material mat_Ghost;
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

    /// <summary>
    /// 설치완료
    /// </summary>
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

        invenLock?.Invoke(false);
    }

    public enum eItemState
    {
        idle,
        move,
    }
    public eItemState itemState;


    /// <summary>
    /// 오브젝트 이동 시작
    /// </summary>
    public void OnClick_MoveDown()
    {
        CurrentMethodName();
        inMove.raycastTarget = false;
        outMove.SetActive(false);
        cover.SetActive(false);
        itemState = eItemState.move;
        touchInputController.enabled = false;
        if (curObj.TryGetComponent(out Animation animation))
        {
            animation.enabled = true;
            
            ghostObj = new GameObject();
            ghostObj.transform.SetParent(curObj.transform.parent);
            ghostObj.transform.localPosition = Vector3.zero;

            var mesh = ghostObj.AddComponent<MeshFilter>();
            mesh.mesh = curObj.GetComponent<MeshFilter>().mesh;

            var renderer = ghostObj.AddComponent<MeshRenderer>();
            renderer.material = Instantiate(mat_Ghost);
        }
    }
    GameObject ghostObj;
    /// <summary>
    /// 오브젝트 이동 끝
    /// </summary>
    public void OnClick_MoveUp()
    {
        CurrentMethodName();
        inMove.raycastTarget = true;
        outMove.SetActive(true);
        cover.SetActive(true);
        touchInputController.enabled = true;
        StartCoroutine(Co_Wait());
        if (curObj.TryGetComponent(out Animation animation))
        {
            animation.enabled = false;
            curObj.transform.position = new Vector3(curObj.transform.position.x, 0f, curObj.transform.position.z);
            Destroy(ghostObj);
        }
    }

    IEnumerator Co_Wait()
    {
        yield return new WaitForEndOfFrame();
        itemState = eItemState.idle;
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
        //pivot.transform.SetParent(null);
        GridManagerAccessor.GridManager.CancelPlacement();
        invenLock?.Invoke(false);
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
        invenLock?.Invoke(true);
        cover.SetActive(true);

        pivot.SetActive(true);
        //pivot.transform.SetParent(obj.transform);
        //pivot.transform.localPosition = Vector3.zero;

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

    string subPath = "RoomItem.json";

    string GetPath(string subPath)
    {
        string oriPath =  Path.Combine(Application.streamingAssetsPath, subPath);
#if UNITY_ANDROID
        WWW reader = new WWW(oriPath);
        while (!reader.isDone) { }
        var realPath = Application.persistentDataPath + "/db";
        File.WriteAllBytes(realPath, reader.bytes);

        oriPath = File.ReadAllText(realPath);
#endif
        return oriPath;
    }

    public void Save()
    {
        Debug.Log(MethodBase.GetCurrentMethod().Name);
        gridSaveManager.HandleSaveGridObjectsPressed();
    }
    public void Load()
    {
        Debug.Log(MethodBase.GetCurrentMethod().Name);
        //pivot.transform.SetParent(null);
        //string path = Application.dataPath + "/StreamingAssets/RoomItem.json";
        string path = GetPath(subPath);
        //if (!File.Exists(path))
        //{
        //    return;
        //}
        //string saveDataAsJson = path;
        //string saveDataAsJson = PlayerPrefs.GetString("GRIDMANAGER");
        _GridData _gridData = JsonUtility.FromJson<_GridData>(path);
        if(_gridData==null)
        {
            return;
        }
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

    public Transform cameraPivot;
    public void SetRotate(int val)
    {
        StartCoroutine(Co_Rotate(val));
    }

    IEnumerator Co_Rotate(int val)
    {
        float curTime = 0f;
        float durTime = .5f;
        while (curTime < 1f)
        {
            curTime += Time.deltaTime / durTime;
            cameraPivot.Rotate(Vector3.up * 90f * val * Time.deltaTime / durTime);
            yield return null;
        }
    }

    public delegate void RoomItem(int idx);

    public RoomItem InitRoomItem; //룸아이템 초기셋업
    public RoomItem MinusRoomItem; //룸아이템 빼기

    private void Update()
    {
        if (curObj != null)
        {
            pivot.transform.position = new Vector3(curObj.transform.position.x, 0f, curObj.transform.position.z);
        }

        //if (Input.GetKeyDown(KeyCode.Alpha9))
        //{
        //    Save();
        //}
        //if (Input.GetKeyDown(KeyCode.Alpha0))
        //{
        //    Load();
        //}

        //if (Input.GetKey(KeyCode.A))
        //{
        //    cameraRotateY.transform.Rotate(Vector3.up * 360f * rotateSpeed * Time.deltaTime);
        //}
        //if (Input.GetKey(KeyCode.D))
        //{
        //    cameraRotateY.transform.Rotate(Vector3.up * 360f * -rotateSpeed * Time.deltaTime);
        //}
        //if (Input.GetKey(KeyCode.W))
        //{
        //    if (cameraPivotZ.transform.localPosition.z < 2f)
        //    {
        //        cameraPivotZ.transform.Translate(Vector3.forward * zoomSpeed * Time.deltaTime);
        //    }
        //}
        //if (Input.GetKey(KeyCode.S))
        //{
        //    if (cameraPivotZ.transform.localPosition.z > -2f)
        //    {
        //        cameraPivotZ.transform.Translate(-Vector3.forward * zoomSpeed * Time.deltaTime);
        //    }
        //}
        if (curObj != null)
        {
            return;
        }
        //if (EventSystem.current.IsPointerOverGameObject())
        //{
        //    return;
        //}
        int layerMask = -1 - (1 << LayerMask.NameToLayer("CinemachineCollider"));
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition, Camera.MonoOrStereoscopicEye.Mono);
        if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, layerMask))
        {
            if (hit.transform.GetComponent<GridHeightPositioner>() != null)
            {
                if (Input.GetMouseButtonDown(0))
                {
                    selectObj = hit.transform.gameObject;
                }
                if (Input.GetMouseButtonUp(0))
                {
                    if (selectObj == hit.transform.gameObject)
                    {
                        SelectRoomItem(hit.transform.gameObject);
                    }
                    selectObj = null;
                }
            }
        }
    }
    public delegate void InvenLock(bool bLock);

    public InvenLock invenLock;

    public TouchInputController touchInputController;
    private GameObject selectObj;
}
