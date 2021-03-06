﻿using CsvHelper;
using CsvHelper.Configuration;
using CsvHelper.Configuration.Attributes;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public abstract class ATableRow
{
    [Name("UniqueID")]
    public string UniqueID { get; set; }
}

public abstract class ATableReader<ReaderClass, RowClass>
{
    protected Dictionary<string, RowClass> TableRows = new Dictionary<string, RowClass>();
    protected static ReaderClass Instance;
    protected Configuration DefaultConfiguration = new Configuration()
    {
        Encoding = System.Text.Encoding.GetEncoding("euc-kr")
    };
    public abstract void Initialize(string TablePath);
    public static ReaderClass GetInstance()
    {
        return Instance;
    }

    public RowClass GetTableRow(string UniqueID)
    {
        RowClass value;
        TableRows.TryGetValue(UniqueID, out value);
        return value;
    }

    public List<RowClass> GetTableRowList()
    {
        return new List<RowClass>(TableRows.Values);
    }
}
