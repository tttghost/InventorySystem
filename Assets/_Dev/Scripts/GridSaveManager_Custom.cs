using Hypertonic.GridPlacement.Enums;
using Hypertonic.GridPlacement.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;


namespace Hypertonic.GridPlacement.Example.BasicDemo
{

    /// <summary>
    /// This is an example implementation of how you can get the grid data and save it. Then load it back into the grid.
    /// For this simple demo the class will use PlayerPrefs as a way to persist a serialised form of the data in this example.
    /// </summary>
    public class GridSaveManager_Custom : MonoBehaviour
    {
        //[SerializeField]
        //private List<GameObject> _gridObjectPrefabs = new List<GameObject>();
        private GridSystem grid;

        private void Start()
        {
            grid = GetComponent<GridSystem>();
        }

        public void HandleSaveGridObjectsPressed()
        {
            List<GridType> gridTypeList = new List<GridType>(ItemDatabase.instance.db_GridType.Values);
            _GridData _gridData = new _GridData();
            for (int j = 0; j < gridTypeList.Count; j++)
            {
                string key = gridTypeList[j].gridName;

                GridData gridData = GridManagerAccessor.GetGridManagerByKey(key).GridData;

                _SaveData _saveData = new _SaveData();

                for (int i = 0; i < gridData.GridObjectPositionDatas.Count; i++)
                {
                    GridObjectPositionData gridObjectPositionData = gridData.GridObjectPositionDatas[i];

                    _saveData.gridId = key;

                    _GridObjectSaveData _gridObjectSaveData = new _GridObjectSaveData(
                        gridObjectPositionData.GridObject.name,
                        gridObjectPositionData.GridCellIndex.x,
                        gridObjectPositionData.GridCellIndex.y,
                        (int)gridObjectPositionData.GridObject.transform.localEulerAngles.y);
                    _saveData._gridObjectSaveDatas.Add(_gridObjectSaveData);
                }
                _gridData._saveDatas.Add(_saveData);

            }
            string saveDataAsJson = JsonUtility.ToJson(_gridData);
            //Debug.Log("saveDataAsJson: " + saveDataAsJson);
            //PlayerPrefs.SetString(GRIDMANAGER, saveDataAsJson);
            File.WriteAllText(Application.dataPath + "/StreamingAssets/RoomItem.json", saveDataAsJson);
        }

        /// <summary>
        /// 룸(그리드)오브젝트 로드 핸들러
        /// </summary>
        /// <param name="_saveData"></param>
        public void HandleLoadGridObjectsPressed(_SaveData _saveData)
        {
            _ = LoadGridData(_saveData);
        }

        //string GRIDMANAGER = "GRIDMANAGER";

        private async Task LoadGridData(_SaveData _saveData)
        {
            //if (!PlayerPrefs.HasKey(GRIDMANAGER))
            //{
            //    Debug.LogWarning("There is no save data stored yet. You must save the grid data before being able to load it.");
            //    return;
            //}

            //string saveDataAsJson = PlayerPrefs.GetString(playerPrefabsKey);

            //_GridData _gridData = JsonUtility.FromJson<_GridData>(saveDataAsJson);

            List<GridObjectPositionData> gridObjectPositionDatas = new List<GridObjectPositionData>();
            //GridManagerAccessor.SetSelectedGridManager(_saveData.gridId);
            foreach (_GridObjectSaveData _gridObjectSaveData in _saveData._gridObjectSaveDatas)
            {
                //GameObject prefab = _gridObjectPrefabs.Where(x => x.name.Equals(_gridObjectSaveData.prefabName)).FirstOrDefault();
                GameObject prefab = grid.prefabs.Where(x => x.name.Equals(_gridObjectSaveData.prefabName)).FirstOrDefault();
                

                if (prefab == null)
                {
                    Debug.LogErrorFormat("The save game manager does not have a prefab reference for the object: {0}", _gridObjectSaveData.prefabName);
                    continue;
                }

                GameObject gridObject = Instantiate(prefab);
                gridObject.transform.position = Vector3.right * 1000f; //깜빡임 방지

                // Set the rotation back to the saved rotation of the object
                gridObject.transform.localEulerAngles = Vector3.up * _gridObjectSaveData.rotation;

                // Remove the "(Clone)" from instantiated name.
                gridObject.name = prefab.name;


                if (!gridObject.TryGetComponent(out ExampleGridObject exampleGridObjectComponent))
                {
                    gridObject.AddComponent<ExampleGridObject>();
                }


                GridObjectPositionData gridObjectPositionData = new GridObjectPositionData(gridObject, new Vector2Int(_gridObjectSaveData.x, _gridObjectSaveData.y), ObjectAlignment.CENTER);
                gridObjectPositionDatas.Add(gridObjectPositionData);
            }

            GridData gridData = new GridData(gridObjectPositionDatas);
            await GridManagerAccessor.GridManager.PopulateWithGridData(gridData, true);


        }
    }


    [Serializable]

    public class _GridData
    {
        public List<_SaveData> _saveDatas = new List<_SaveData>();
    }
    [Serializable]
    public class _SaveData
    {
        public string gridId;
        public List<_GridObjectSaveData> _gridObjectSaveDatas = new List<_GridObjectSaveData>();
    }

    [Serializable]
    public class _GridObjectSaveData
    {
        public string prefabName;
        public int x;
        public int y;
        public int rotation;

        public _GridObjectSaveData(string prefabName, int x, int y, int rotation)
        {
            this.prefabName = prefabName;
            this.x = x;
            this.y = y;
            this.rotation = rotation;
        }
    }
}