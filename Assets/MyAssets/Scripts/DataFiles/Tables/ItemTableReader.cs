using CsvHelper;
using CsvHelper.Configuration.Attributes;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public enum EItemType
{
    Element = 0,
    Equipment = 1,
    Product = 2,
    None,
    Count = None
}

/// <summary>
/// 게임 내 아이템들의 정보를 가진 데이터 구조체.
/// </summary>
public struct ItemInfo
{
    public string UniqueID;
    public string Name;
    public EItemType Type;
    public string FlavorText;
}


public class ItemTableRow : ATableRow
{
    [Name("Type")]
    public string Type { get; set; }
    [Name("Name")]
    public string Name { get; set; }
    [Name("FlavorText")]
    public string FlavorText { get; set; }
}

public class ItemTableReader : ATableReader<ItemTableReader, ItemTableRow>
{
    public override void Initialize(string TablePath)
    {
        Instance = this;
        using (var reader = new StreamReader(new MemoryStream(Resources.Load<TextAsset>(TablePath).bytes), Encoding.GetEncoding("euc-kr")))
        {
            using (var csv = new CsvReader(reader, DefaultConfiguration))
            {
                var records = csv.GetRecords<ItemTableRow>();
                foreach (var row in records)
                {
                    TableRows.Add(row.UniqueID, row);
                }
            }
        }
    }

    public ItemInfo GetItemInfo(string UniqueID)
    {
        ItemInfo item;
        item.Name = "";
        item.Type = EItemType.None;
        item.UniqueID = "";
        item.FlavorText = "";
        //
        if (TableRows.TryGetValue(UniqueID, out ItemTableRow row) == true)
        {
            item.Name = row.Name;
            item.Type = KojeomUtility.StringToEnum<EItemType>(row.Type);
            item.FlavorText = row.FlavorText;
            item.UniqueID = row.UniqueID;
        }
        return item;
    }
}