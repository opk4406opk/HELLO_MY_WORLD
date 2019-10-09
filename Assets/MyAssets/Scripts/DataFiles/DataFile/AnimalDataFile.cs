using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimalDataFile : BaseDataFile
{
    public static AnimalDataFile Instance = null;
    /// <summary>
    /// Key : UniqueID, Value : SpawnData
    /// </summary>
    public Dictionary<int, AnimalSpawnData> AnimalSpawnDatas { get; private set; } = new Dictionary<int, AnimalSpawnData>();

    public override void Init()
    {
        JsonFile = Resources.Load(ConstFilePath.TXT_ANIMAL_DATAS) as TextAsset;
        JsonObject = new JSONObject(JsonFile.text);
        AccessData(JsonObject);

        if (Instance == null) Instance = this;
    }
    protected override void AccessData(JSONObject jsonObj)
    {
        foreach (var json in jsonObj.list[0].list)
        {
            AnimalSpawnData spawnData = new AnimalSpawnData();
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
            spawnData.AnimalType = KojeomUtility.StringToEnum<ANIMAL_TYPE>(outValue);
            //
            data.TryGetValue("RESOURCE_ID", out outValue);
            spawnData.ResourceID = outValue;
            //
            data.TryGetValue("UNIQUE_ID", out outValue);
            spawnData.UniqueID = int.Parse(outValue);
            //
            data.TryGetValue("CATEGORY", out outValue);
            spawnData.AnimalCategory = KojeomUtility.StringToEnum<ANIMAL_CATEGORY>(outValue);
            //
            AnimalSpawnDatas.Add(spawnData.UniqueID, spawnData);
        }
    }
}
