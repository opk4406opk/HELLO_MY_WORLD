using CsvHelper;
using CsvHelper.Configuration.Attributes;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class BlockElementTableRow : ATableRow
{
   
}

public class BlockElementTableReader : ATableReader<BlockElementTableReader, BlockElementTableRow>
{
    public override void Initialize(string TablePath)
    {
        Instance = this;
        using (var reader = new StreamReader(TablePath))
        {
            using (var csv = new CsvReader(reader))
            {
                var records = csv.GetRecords<BlockElementTableRow>();
                foreach (var row in records)
                {
                    TableRows.Add(row.UniqueID, row);
                }
            }
        }
    }
}