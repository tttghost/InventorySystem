using BitBenderGames;
using Hypertonic.GridPlacement;
using Hypertonic.GridPlacement.CustomSizing;
using Hypertonic.GridPlacement.Example.BasicDemo;
using Hypertonic.GridPlacement.Models;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEngine.UI;

public class GridSystem : MonoBehaviour
{

    #region ����
    private GridSaveManager_Custom  gridSaveManager;
    private GameObject              curObj;
    private GameObject              selectObj;
    private bool                    isValid;
    private string                  path = "RoomItem.json";

    public GameObject[]             prefabs;
    public GameObject               cover;
    public GameObject               pivot;
    public Image                    inMove;
    public GameObject               outMove;
    public List<Texture2D>          thumbnailList = new List<Texture2D>();


    public eItemState               itemState;
    public enum eItemState
    {
        idle,
        move,
    }


    public delegate void IntParam(int i);
    public IntParam                 handlerInitRoomItem; //������� �ʱ�¾�
    public IntParam                 handlerPlusInvenItem; //������� ���� �κ������ۿ� �߰�

    public delegate void BoolParam(bool b);
    public BoolParam                handlerInvenLock;
    public BoolParam                handlerMoveRoomObject;

    #endregion

    #region �Լ�

    #region ����Ƽ �Լ�
    private void Awake()
    {
        gridSaveManager = gameObject.AddComponent<GridSaveManager_Custom>();
        for (int i = 0; i < prefabs.Length; i++)
        {
            thumbnailList.Add(RuntimePreviewGenerator.GenerateModelPreview(prefabs[i].transform, 512, 512));
        }
    }

    private void Start()
    {
        
    }

    private void Update()
    {
        if (curObj != null)
        {
            pivot.transform.position = new Vector3(curObj.transform.position.x, 0f, curObj.transform.position.z);
            return;
        }
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition, Camera.MonoOrStereoscopicEye.Mono);
        if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity))
        {
            if (hit.transform.GetComponent<GridHeightPositioner>() != null)
            {
                if (Input.GetMouseButtonDown(0))
                {
                    selectObj = hit.transform.gameObject;
                }
            }
        }
        if (Input.GetMouseButtonUp(0))
        {
            if (selectObj != null && hit.transform.gameObject != null && selectObj == hit.transform.gameObject 
                && !MyRoomManager.instance.GetMousePressedMove())
            {
                SelectRoomItem(selectObj);
            }
            selectObj = null;
        }

    }
    #endregion

    #region ��ư �Լ�
    /// <summary>
    /// ��ư_�������Ʈ�� �κ�������Ʈ�� �̵�
    /// </summary>
    public void OnClick_Inven()
    {
        ItemType itemType = ItemDatabase.instance.GetItemType(curObj.name);

        handlerPlusInvenItem?.Invoke(itemType.itemId);
        handlerInvenLock?.Invoke(false);
        cover.SetActive(false);
        pivot.SetActive(false);
        curObj = null;

        GridManagerAccessor.GridManager.CancelPlacement(); // ������ ȸ��
    }
    /// <summary>
    /// ��ư_������Ʈ �̵� ����
    /// </summary>
    public void OnClick_MoveDown()
    {
        inMove.raycastTarget = false; //Ÿ�ٳ�����(�׸����̵�������)
        cover.SetActive(false); //�������� Ŀ������ �����̰��ϱ�
        outMove.SetActive(false); //�����ư ��Ȱ��ȭ
        handlerMoveRoomObject?.Invoke(false);//��ġ�̵� ���̱�
    }



    /// <summary>
    /// ��ư_������Ʈ �̵� ��
    /// </summary>
    public void OnClick_MoveUp()
    {
        inMove.raycastTarget = true;
        cover.SetActive(true);
        outMove.SetActive(true);
        handlerMoveRoomObject?.Invoke(true);
    }

    /// <summary>
    /// ��ư_������Ʈ ������ȸ��
    /// </summary>
    public void OnClick_TurnRight()
    {
        HandleRotate(curObj, 90);
    }

    /// <summary>
    /// ��ư_������Ʈ ����ȸ��
    /// </summary>
    public void OnClick_TurnLeft()
    {
        HandleRotate(curObj, -90);
    }


    /// <summary>
    /// ��ư_��ġ�Ϸ�
    /// </summary>
    public void OnClick_Confirm()
    {
        if (!isValid)
        {
            return;
        }

        //�⺻����
        cover.SetActive(false);
        pivot.SetActive(false);
        handlerInvenLock?.Invoke(false);

        //������ ��ġ�޷�
        GridManagerAccessor.GridManager.ConfirmPlacement();
        
        //�׸��� ������Ʈ ����
        curObj = null;
    }
    #endregion

    #region �ݹ� �Լ�

    /// <summary>
    /// �̺�Ʈ_�κ�������-, �������+
    /// </summary>
    /// <param name="itemId"></param>
    public void OnPlusRoomItem(int itemId)
    {
        GameObject obj = Instantiate(prefabs[itemId], GridManagerAccessor.GridManager.GetGridPosition(), new Quaternion());
        obj.name = prefabs[itemId].name;
        SelectRoomItem(obj);
    }

    #endregion

    #region ���� ���
    /// <summary>
    /// ������� ����
    /// </summary>
    /// <param name="obj"></param>
    private void SelectRoomItem(GameObject obj)
    {
        //�׸��带 ���� �⺻����
        handlerInvenLock?.Invoke(true);
        cover.SetActive(true);
        pivot.SetActive(true);

        //�׸��� ����
        string gridKey;
        if (obj.TryGetComponent(out GridObjectInfo objectInfo))
        {
            gridKey = objectInfo.GridKey;
        }
        else
        {
            gridKey = ItemDatabase.instance.GetGridType(obj.name).gridName;
        }
        SelectedGridManager(gridKey);

        //�׸��� ��ġ��� ����
        GridManagerAccessor.GridManager.EnterPlacementMode(obj);

        //�׸����ϴ� ������Ʈ ����
        curObj = obj;
    }




    /// <summary>
    /// �ȵ���̵��϶��� �����͸���϶� �н� ���ϱ�
    /// </summary>
    /// <param name="path"></param>
    /// <returns></returns>
    private string GetPath(string path)
    {
        string oriPath = Path.Combine(Application.streamingAssetsPath, path);
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
    /// �����ϱ� (�������, �κ�������)
    /// </summary>
    public void Save()
    {
        Debug.Log(MethodBase.GetCurrentMethod().Name);
        gridSaveManager.HandleSaveGridObjectsPressed();
    }

    /// <summary>
    /// �ҷ����� (�������, �κ�������)
    /// </summary>
    public void Load()
    {
        Debug.Log(MethodBase.GetCurrentMethod().Name);
        _GridData _gridData = JsonUtility.FromJson<_GridData>(GetPath(path));
        if (_gridData == null)
        {
            return;
        }
        for (int i = 0; i < _gridData._saveDatas.Count; i++)
        {
            _SaveData _saveData = _gridData._saveDatas[i];
            RoomItemInit(_saveData._gridObjectSaveDatas);
            SelectedGridManager(_saveData.gridId); //�׸��� �¾�
            gridSaveManager.HandleLoadGridObjectsPressed(_gridData._saveDatas[i]);
        }
    }

    /// <summary>
    /// ������� �ʱ�¾�
    /// </summary>
    /// <param name="_GridObjectSaveDatas"></param>
    private void RoomItemInit(List<_GridObjectSaveData> _GridObjectSaveDatas)
    {
        for (int i = 0; i < _GridObjectSaveDatas.Count; i++)
        {
            int itemId = ItemDatabase.instance.GetItemType(_GridObjectSaveDatas[i].prefabName).itemId;
            handlerInitRoomItem?.Invoke(itemId);
        }
    }
    #endregion

    #region ��Ÿ ���

    /// <summary>
    /// ������Ʈ ��ġ�����ѿ��� �ǽð� üũ
    /// </summary>
    /// <param name="isValid"></param>
    private void GridManager_OnPlacementValidated(bool isValid)
    {
        this.isValid = isValid;
    }


    /// <summary>
    /// ������Ʈ ȸ�� ������
    /// </summary>
    /// <param name="obj"></param>
    /// <param name="rotateY"></param>
    private void HandleRotate(GameObject obj, float rotateY)
    {
        obj.transform.Rotate(new Vector3(0, rotateY, 0));
        GridManagerAccessor.GridManager.HandleGridObjectRotated();
    }


    /// <summary>
    /// �׸��� ����
    /// </summary>
    /// <param name="gridKey"></param>
    private void SelectedGridManager(string gridKey)
    {
        //Debug.Log("Select : " + gridKey);
        GridManagerAccessor.GridManager.OnPlacementValidated -= GridManager_OnPlacementValidated;
        GridManagerAccessor.SetSelectedGridManager(gridKey);
        GridManagerAccessor.GridManager.OnPlacementValidated += GridManager_OnPlacementValidated;
    }
    #endregion

    #endregion

}





































































































//221123����
//using BitBenderGames;
//using Hypertonic.GridPlacement;
//using Hypertonic.GridPlacement.CustomSizing;
//using Hypertonic.GridPlacement.Example.BasicDemo;
//using Hypertonic.GridPlacement.Models;
//using Newtonsoft.Json.Linq;
//using System.Collections;
//using System.Collections.Generic;
//using System.IO;
//using System.Linq;
//using System.Reflection;
//using UnityEngine;
//using UnityEngine.EventSystems;
//using UnityEngine.UI;

//public class GridSystem : MonoBehaviour
//{
//    public GameObject[] prefabs;
//    public GameObject cover;
//    public GameObject pivot;

//    public Image inMove;
//    public GameObject outMove;

//    private GameObject curObj;
//    private GridSaveManager_Custom gridSaveManager;
//    //public GameObject cameraRotateY;
//    //public GameObject cameraPivotZ;
//    public float rotateSpeed;
//    public float zoomSpeed;
//    private bool isValid;

//    public List<Texture2D> thumbnailList = new List<Texture2D>();

//    public Material mat_Ghost; public enum eItemState
//    {
//        idle,
//        move,
//    }
//    public eItemState itemState;

//    string path = "RoomItem.json";

//    public Transform cameraPivot;

//    public delegate void RoomItem(int idx);
//    public RoomItem InitRoomItem; //������� �ʱ�¾�
//    public RoomItem MinusRoomItem; //������� ����

//    public delegate void InvenLock(bool bLock);
//    public InvenLock invenLock;

//    public TouchInputController touchInputController;
//    private GameObject selectObj;

//    private void Awake()
//    {
//        gridSaveManager = gameObject.AddComponent<GridSaveManager_Custom>();
//        for (int i = 0; i < prefabs.Length; i++)
//        {
//            thumbnailList.Add(RuntimePreviewGenerator.GenerateModelPreview(prefabs[i].transform, 512, 512));
//        }
//    }


//    private void GridManager_OnPlacementValidated(bool isValid)
//    {
//        this.isValid = isValid;
//    }

//    /// <summary>
//    /// ��ġ�Ϸ�
//    /// </summary>
//    public void OnClick_Cover()
//    {
//        CurrentMethodName();
//        if (!isValid)
//        {
//            return;
//        }
//        cover.SetActive(false);
//        pivot.SetActive(false);
//        curObj = null;
//        GridManagerAccessor.GridManager.ConfirmPlacement();

//        invenLock?.Invoke(false);
//    }


//    public void SetTouchInputController(bool enable)
//    {
//        touchInputController.enabled = enable;
//    }

//    /// <summary>
//    /// ������Ʈ �̵� ����
//    /// </summary>
//    public void OnClick_MoveDown()
//    {
//        CurrentMethodName();
//        inMove.raycastTarget = false; //Ÿ�ٳ�����
//        outMove.SetActive(false); //�������� ��Ȱ��ȭ
//        cover.SetActive(false); //�������� Ŀ������ �����̰��ϱ�
//        SetTouchInputController(false); //��ġ�̵� ���̱�

//        //itemState = eItemState.move; //�����̴»��� �̳Ѱ� ����
//        //if (curObj.TryGetComponent(out Animation animation))
//        //{
//        //    animation.enabled = true;

//        //    ghostObj = new GameObject();
//        //    ghostObj.transform.SetParent(curObj.transform.parent);
//        //    ghostObj.transform.localPosition = Vector3.zero;

//        //    var mesh = ghostObj.AddComponent<MeshFilter>();
//        //    mesh.mesh = curObj.GetComponent<MeshFilter>().mesh;

//        //    var renderer = ghostObj.AddComponent<MeshRenderer>();
//        //    renderer.material = Instantiate(mat_Ghost);
//        //}
//    }


//    //GameObject ghostObj;
//    /// <summary>
//    /// ������Ʈ �̵� ��
//    /// </summary>
//    public void OnClick_MoveUp()
//    {
//        CurrentMethodName();
//        inMove.raycastTarget = true;
//        outMove.SetActive(true);
//        cover.SetActive(true);
//        SetTouchInputController(true);
//        //StartCoroutine(Co_Wait());
//        //if (curObj.TryGetComponent(out Animation animation))
//        //{
//        //    animation.enabled = false;
//        //    curObj.transform.position = new Vector3(curObj.transform.position.x, 0f, curObj.transform.position.z);
//        //    Destroy(ghostObj);
//        //}
//    }

//    //private IEnumerator Co_Wait()
//    //{
//    //    yield return new WaitForEndOfFrame();
//    //    itemState = eItemState.idle;
//    //}

//    /// <summary>
//    /// ������Ʈ ������ȸ��
//    /// </summary>
//    public void OnClick_TurnRight()
//    {
//        CurrentMethodName();
//        HandleRotate(curObj, 90);
//    }

//    /// <summary>
//    /// ������Ʈ ����ȸ��
//    /// </summary>
//    public void OnClick_TurnLeft()
//    {
//        CurrentMethodName();
//        HandleRotate(curObj, -90);
//    }

//    /// <summary>
//    /// ������Ʈ ȸ�� ������
//    /// </summary>
//    /// <param name="obj"></param>
//    /// <param name="rotateY"></param>
//    private void HandleRotate(GameObject obj, float rotateY)
//    {
//        obj.transform.Rotate(new Vector3(0, rotateY, 0));
//        GridManagerAccessor.GridManager.HandleGridObjectRotated();
//    }

//    /// <summary>
//    /// ������Ʈ �κ��� �ֱ�
//    /// </summary>
//    public void OnClick_Inven()
//    {
//        CurrentMethodName();

//        int itemId = ItemDatabase.instance.db_ItemType.FirstOrDefault(x => x.Value.prefabName == curObj.name).Key;
//        MinusRoomItem(itemId);

//        cover.SetActive(false);
//        pivot.SetActive(false);
//        curObj = null;
//        //pivot.transform.SetParent(null);
//        GridManagerAccessor.GridManager.CancelPlacement();
//        invenLock?.Invoke(false);
//    }

//    /// <summary>
//    /// ���� �Լ��̸� ���
//    /// </summary>
//    private void CurrentMethodName()
//    {
//        //Debug.Log(MethodBase.GetCurrentMethod().Name);
//    }

//    /// <summary>
//    /// ����� ONGUI�� ���
//    /// </summary>
//    //private void OnGUI()
//    //{
//    //    return;
//    //    if (curObj != null)
//    //    {
//    //        return;
//    //    }
//    //    for (int i = 0; i < prefabs.Length; i++)
//    //    {
//    //        if (GUI.Button(new Rect(i * 150, 0, 150, 150), new GUIContent(thumbnailList[i])))
//    //        {
//    //            int capture = i;
//    //            PlusRoomItem(capture);
//    //        }
//    //    }
//    //}

//    /// <summary>
//    /// ������� �߰�
//    /// </summary>
//    /// <param name="itemId"></param>
//    public void PlusRoomItem(int itemId)
//    {
//        GameObject obj = Instantiate(prefabs[itemId], GridManagerAccessor.GridManager.GetGridPosition(), new Quaternion());
//        obj.name = prefabs[itemId].name;
//        SelectRoomItem(obj);
//    }

//    /// <summary>
//    /// �������̸����� ������Ÿ�� ����
//    /// </summary>
//    /// <param name="prefabName"></param>
//    /// <returns></returns>
//    public ItemType GetPrefabName2ItemType(string prefabName)
//    {
//        return ItemDatabase.instance.db_ItemType.FirstOrDefault(x => x.Value.prefabName == prefabName).Value;
//    }

//    /// <summary>
//    /// ������� ����
//    /// </summary>
//    /// <param name="obj"></param>
//    private void SelectRoomItem(GameObject obj)
//    {
//        curObj = obj;

//        invenLock?.Invoke(true);
//        cover.SetActive(true);
//        pivot.SetActive(true);
//        //pivot.transform.SetParent(obj.transform);
//        //pivot.transform.localPosition = Vector3.zero;

//        if (obj.TryGetComponent(out GridObjectInfo objectInfo))
//        {
//            SelectedGridManager(objectInfo.GridKey);
//        }
//        else
//        {
//            ItemType itemType = GetPrefabName2ItemType(obj.name);
//            string gridName = ItemDatabase.instance.GetGridType(itemType.gridId).gridName;
//            SelectedGridManager(gridName);
//        }

//        GridManagerAccessor.GridManager.EnterPlacementMode(obj); //�׸��� ��ġ��� ����
//    }



//    private void SelectedGridManager(string gridKey)
//    {
//        //Debug.Log("Select : " + gridKey);
//        GridManagerAccessor.GridManager.OnPlacementValidated -= GridManager_OnPlacementValidated;
//        GridManagerAccessor.SetSelectedGridManager(gridKey);
//        GridManagerAccessor.GridManager.OnPlacementValidated += GridManager_OnPlacementValidated;
//    }


//    string GetPath(string path)
//    {
//        string oriPath = Path.Combine(Application.streamingAssetsPath, path);
//#if UNITY_ANDROID
//        WWW reader = new WWW(oriPath);
//        while (!reader.isDone) { }
//        var realPath = Application.persistentDataPath + "/db";
//        File.WriteAllBytes(realPath, reader.bytes);

//        oriPath = File.ReadAllText(realPath);
//#endif
//        return oriPath;
//    }

//    public void Save()
//    {
//        Debug.Log(MethodBase.GetCurrentMethod().Name);
//        gridSaveManager.HandleSaveGridObjectsPressed();
//    }
//    public void Load()
//    {
//        Debug.Log(MethodBase.GetCurrentMethod().Name);
//        //pivot.transform.SetParent(null);
//        //string path = Application.dataPath + "/StreamingAssets/RoomItem.json";
//        //if (!File.Exists(path))
//        //{
//        //    return;
//        //}
//        //string saveDataAsJson = path;
//        //string saveDataAsJson = PlayerPrefs.GetString("GRIDMANAGER");
//        _GridData _gridData = JsonUtility.FromJson<_GridData>(GetPath(path));
//        if (_gridData == null)
//        {
//            return;
//        }
//        for (int i = 0; i < _gridData._saveDatas.Count; i++)
//        {
//            _SaveData _saveData = _gridData._saveDatas[i];
//            RoomItemInit(_saveData._gridObjectSaveDatas);
//            SelectedGridManager(_saveData.gridId);
//            gridSaveManager.HandleLoadGridObjectsPressed(_gridData._saveDatas[i]);
//        }
//    }

//    /// <summary>
//    /// ������� �ʱ�¾�
//    /// </summary>
//    /// <param name="_GridObjectSaveDatas"></param>
//    private void RoomItemInit(List<_GridObjectSaveData> _GridObjectSaveDatas)
//    {
//        for (int i = 0; i < _GridObjectSaveDatas.Count; i++)
//        {
//            int itemId = ItemDatabase.instance.db_ItemType.FirstOrDefault(x => x.Value.prefabName == _GridObjectSaveDatas[i].prefabName).Key;
//            InitRoomItem(itemId);
//        }
//    }


//    private void Update()
//    {
//        if (curObj != null)
//        {
//            pivot.transform.position = new Vector3(curObj.transform.position.x, 0f, curObj.transform.position.z);
//        }

//        //if (Input.GetKeyDown(KeyCode.Alpha9))
//        //{
//        //    Save();
//        //}
//        //if (Input.GetKeyDown(KeyCode.Alpha0))
//        //{
//        //    Load();
//        //}

//        //if (Input.GetKey(KeyCode.A))
//        //{
//        //    cameraRotateY.transform.Rotate(Vector3.up * 360f * rotateSpeed * Time.deltaTime);
//        //}
//        //if (Input.GetKey(KeyCode.D))
//        //{
//        //    cameraRotateY.transform.Rotate(Vector3.up * 360f * -rotateSpeed * Time.deltaTime);
//        //}
//        //if (Input.GetKey(KeyCode.W))
//        //{
//        //    if (cameraPivotZ.transform.localPosition.z < 2f)
//        //    {
//        //        cameraPivotZ.transform.Translate(Vector3.forward * zoomSpeed * Time.deltaTime);
//        //    }
//        //}
//        //if (Input.GetKey(KeyCode.S))
//        //{
//        //    if (cameraPivotZ.transform.localPosition.z > -2f)
//        //    {
//        //        cameraPivotZ.transform.Translate(-Vector3.forward * zoomSpeed * Time.deltaTime);
//        //    }
//        //}
//        if (curObj != null)
//        {
//            return;
//        }
//        //if (EventSystem.current.IsPointerOverGameObject())
//        //{
//        //    return;
//        //}
//        int layerMask = -1 - (1 << LayerMask.NameToLayer("CinemachineCollider"));
//        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition, Camera.MonoOrStereoscopicEye.Mono);
//        if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, layerMask))
//        {
//            if (hit.transform.GetComponent<GridHeightPositioner>() != null)
//            {
//                if (Input.GetMouseButtonDown(0))
//                {
//                    selectObj = hit.transform.gameObject;
//                }
//                if (Input.GetMouseButtonUp(0))
//                {
//                    if (selectObj == hit.transform.gameObject)
//                    {
//                        SelectRoomItem(hit.transform.gameObject);
//                    }
//                    selectObj = null;
//                }
//            }
//        }
//    }
//}
