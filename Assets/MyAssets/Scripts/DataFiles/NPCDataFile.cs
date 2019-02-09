using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct NPCSpawnData
{
    public int HP;
    public int MP;
    public int AP;
    public string NAME;
    public NPC_TYPE TYPE;
}

public enum NPC_TYPE
{
    MERCHANT,
    GUARD
}

public class NPCDataFile {
    
    private JSONObject JsonObject;
    private TextAsset JsonFile;
 
    public static NPCDataFile Instance = null;
    //
    public List<NPCSpawnData> NpcSpawnDatas { get; private set; } = new List<NPCSpawnData>();

	public void Init()
    {
        JsonFile = Resources.Load(ConstFilePath.TXT_NPC_DATAS) as TextAsset;
        JsonObject = new JSONObject(JsonFile.text);
        AccessData(JsonObject);

        if (Instance == null) Instance = this;
    }

    private void AccessData(JSONObject jsonObj)
    {
        foreach(var json in jsonObj.list[0].list)
        {
            NPCSpawnData spawnData = new NPCSpawnData();
            //
            var data = json.ToDictionary();
            string outValue;
            data.TryGetValue("HP", out outValue);
            spawnData.HP = int.Parse(outValue);
            //
            data.TryGetValue("MP", out outValue);
            spawnData.MP = int.Parse(outValue);
            //
            data.TryGetValue("AP", out outValue);
            spawnData.AP = int.Parse(outValue);
            //
            data.TryGetValue("NAME", out outValue);
            spawnData.NAME = outValue;
            //
            data.TryGetValue("TYPE", out outValue);
            spawnData.TYPE = KojeomUtility.StringToEnum<NPC_TYPE>(outValue);
            //
            NpcSpawnDatas.Add(spawnData);
        }
    }

}
