using UnityEngine;

public class SoftObjectPtr
{
    public string ObjectPath;
    private GameObject Obj;

    public SoftObjectPtr(string objectPath)
    {
        ObjectPath = objectPath;
    }
   
    public GameObject LoadSynchro()
    {
        Obj = Resources.Load<GameObject>(ObjectPath);
        return Obj;
    }

    public bool IsValid()
    {
        if(Obj == null)
        {
            return false;
        }
        return true;
    }

    public void Release()
    {
        ObjectPath = string.Empty;
        Resources.UnloadAsset(Obj);
        Obj = null;
    }
}

public class SoftObjectPtrGroup
{
    public SoftObjectPtr[] Group;
}
