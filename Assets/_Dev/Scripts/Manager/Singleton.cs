using UnityEngine;

public abstract class Singleton<T> : MonoBehaviour where T : Singleton<T>
{
    private static T _instance = null; //��¥ �̱��� �ν��Ͻ�

    private static object objectLock = new object();

    public static T instance
    {
        get => GetInstance();
    }

    public static T GetInstance()
    {

        lock (objectLock)
        {
            if (_instance == null) // �� �ν��Ͻ��� ���µ�,
            {
                if (FindObjectOfType<T>() == null) // ���� �����ϴ� �ν��Ͻ��� ����? (�׷� �����Ͽ� �� �ν��Ͻ��� �־���)
                {
                    _instance = new GameObject(typeof(T).ToString()).AddComponent<T>();
                }
                else // ���� �����ϴ� �ν��Ͻ��� �ִ� ?
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
        for (int i = 0; i < pList.Length; i++) //ã�� ������Ʈ�� 1�� �̻��̸�
        {
            if (pInstance.GetInstanceID() != pList[i].GetInstanceID()) //�� ������Ʈ�� ã�� ������Ʈ�� �ν��Ͻ�ID���Ͽ� �ٸ���
            {
                DestoryObject(pList[i].gameObject);
            }
        }
    }


    /// <summary>
    /// ������Ʈ ����
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
    /// �޸𸮸� ���� ����.. ����.. ���μ��������� ����..
    /// </summary>
    protected virtual void OnDestroy()
    {
        _instance = null;
    }

    protected virtual void OnApplicationQuit()
    {

    }
}