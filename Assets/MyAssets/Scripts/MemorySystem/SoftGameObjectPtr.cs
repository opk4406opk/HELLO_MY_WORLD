using UnityEngine;

public class SoftGameObjectPtr
{
    public string ObjectPath;
    private GameObject Obj;
   
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
}
