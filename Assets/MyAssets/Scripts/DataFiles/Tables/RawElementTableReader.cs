using CsvHelper;
using CsvHelper.Configuration.Attributes;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;

public class RawElementTableRow : ATableRow
{
    [Name("ResourceName")]
    public string ResourceName { get; set; }
}

public class RawElementTableReader : ATableReader<RawElementTableReader, RawElementTableRow>
{
    public override void Initialize(string TablePath)
    {
        Instance = this;
        using (var reader = new StreamReader(TablePath, Encoding.UTF8))
        {
            using (var csv = new CsvReader(reader))
            {
                var records = csv.GetRecords<RawElementTableRow>();
                foreach (var row in records)
                {
                    TableRows.Add(row.UniqueID, row);
                }
            }
        }
    }
}