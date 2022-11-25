using UnityEngine;

public abstract class Singleton<T> : MonoBehaviour where T : Singleton<T>
{
    private static T _instance = null; //진짜 싱글톤 인스턴스

    private static object objectLock = new object();

    public static T instance
    {
        get => GetInstance();
    }

    public static T GetInstance()
    {

        lock (objectLock)
        {
            if (_instance == null) // 찐 인스턴스는 없는데,
            {
                if (FindObjectOfType<T>() == null) // 씬에 존재하는 인스턴스도 없다? (그럼 생성하여 찐 인스턴스에 넣어줌)
                {
                    _instance = new GameObject(typeof(T).ToString()).AddComponent<T>();
                }
                else // 씬에 존재하는 인스턴스가 있다 ?
                {
                    _instance = FindObjectOfType<T>();
                }
                DontDestroyOnLoad(_instance.gameObject);
            }
        }

        return _instance;
    }


    public static void DuplicationDelete(T pInstance) //4
    {
        T[] pList = GameObject.FindObjectsOfType<T>();
        for (int i = 0; i < pList.Length; i++) //찾은 오브젝트가 1개 이상이면
        {
            if (pInstance.GetInstanceID() != pList[i].GetInstanceID()) //내 오브젝트와 찾은 오브젝트의 인스턴스ID비교하여 다르면
            {
                DestoryObject(pList[i].gameObject);
            }
        }
    }


    /// <summary>
    /// 오브젝트 제거
    /// </summary>
    public static void DestoryObject(UnityEngine.Object pObject)
    {
        if (pObject == null)
            return;

        DestroyImmediate(pObject);
    }


    protected virtual void Start()
    {
        if (_instance != null)
        {
            DuplicationDelete(_instance);
        }
    }

    /// <summary>
    /// 메모리릭 누수 방지.. 추측.. 프로세스터지는 현상..
    /// </summary>
    protected virtual void OnDestroy()
    {
        _instance = null;
    }

    protected virtual void OnApplicationQuit()
    {

    }
}