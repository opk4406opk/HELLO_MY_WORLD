using CsvHelper;
using CsvHelper.Configuration.Attributes;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;
public class BlockProductTableRow : ATableRow
{
    [Name("BlockID")]
    public string BlockID { get; set; }
    [Name("ResourceName")]
    public string ResourceName { get; set; }
}

public class BlockProductTableReader : ATableReader<BlockProductTableReader, BlockProductTableRow>
{
    public override void Initialize(string TablePath)
    {
        Instance = this;
        using (var reader = new StreamReader(TablePath, Encoding.UTF8))
        {
            using (var csv = new CsvReader(reader))
            {
                var records = csv.GetRecords<BlockProductTableRow>();
                foreach (var row in records)
                {
                    TableRows.Add(row.UniqueID, row);
                }
            }
        }
    }
}
