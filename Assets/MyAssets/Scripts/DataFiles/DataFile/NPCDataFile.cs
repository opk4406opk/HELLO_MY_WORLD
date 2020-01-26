using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCDataFile : BaseDataFile
{
    public static NPCDataFile Instance = null;
    /// <summary>
    /// Key : UniqueID, Value : SpawnData
    /// </summary>
    public Dictionary<int, NPCSpawnData> NpcSpawnDatas { get; private set; } = new Dictionary<int, NPCSpawnData>();

	public override void Init()
    {
        JsonFile = Resources.Load(ConstFilePath.TXT_RESOURCE_NPC_DATAS) as TextAsset;
        JsonObject = new JSONObject(JsonFile.text);
        AccessData(JsonObject);

        if (Instance == null) Instance = this;
    }

    protected override void AccessData(JSONObject jsonObj)
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
            spawnData.NpcType = KojeomUtility.StringToEnum<NPC_TYPE>(outValue);
            //
            data.TryGetValue("RESOURCE_ID", out outValue);
            spawnData.ResourceID = outValue;

            data.TryGetValue("UNIQUE_ID", out outValue);
            spawnData.UniqueID = int.Parse(outValue);
            //
            NpcSpawnDatas.Add(spawnData.UniqueID, spawnData);
        }
    }

}
