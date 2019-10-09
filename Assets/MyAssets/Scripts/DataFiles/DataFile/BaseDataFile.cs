using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseDataFile
{
    protected JSONObject JsonObject;
    protected TextAsset JsonFile;

    abstract public void Init();
    abstract protected void AccessData(JSONObject jsonObj);
}
