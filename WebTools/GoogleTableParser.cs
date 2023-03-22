using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
public class Row
{
    private List<string> cells;
    private List<string> headerCells;
    public Row(string data)
    {
        cells = GoogleTableParser.GetCells(data).ToList();
    }
    public string GetCell(int idx)
    {
        return cells[idx];
    }
    // public string GetCell(string columnName)
    // {
    //     return cells.Find();
    // }
}
public class GoogleTable
{
    string _data;
    private List<Row> rows = new List<Row>();
    public GoogleTable(string data)
    {
        SetData(data);
    }
    public Row GetRow(int idx) => rows[idx];
    public void SetData(string data)
    {
        _data = data;
        rows.Clear();
        GoogleTableParser.GetRows(data).ToList().ForEach(textRow => rows.Add(new Row(textRow)));
    }
}
public static class GoogleTableParser
{
    public const char CellSeporator = ',';
    public const char InCellSeporator = ';';

    public static string[] GetRows(string data)
    {
        return data.Split(GoogleTableParser.GetPlatformSpecificLineEnd());
    }
    public static string[] GetCells(string rowData)
    {
        return rowData.Split(GoogleTableParser.CellSeporator);
    }
    public static Vector3 ParseVector3(string s)
    {
        string[] vectorComponents = s.Split(InCellSeporator);
        if (vectorComponents.Length < 3)
        {
            Debug.Log("Can't parse Vector3. Wrong text format");
            return default;
        }

        float x = ParseFloat(vectorComponents[0]);
        float y = ParseFloat(vectorComponents[1]);
        float z = ParseFloat(vectorComponents[2]);
        return new Vector3(x, y, z);
    }

    public static int ParseInt(string s)
    {
        int result = -1;
        if (!int.TryParse(s, System.Globalization.NumberStyles.Integer, System.Globalization.CultureInfo.GetCultureInfo("en-US"), out result))
        {
            Debug.Log("Can't parse int, wrong text");
        }

        return result;
    }

    public static float ParseFloat(string s)
    {
        float result = -1;
        if (!float.TryParse(s, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.GetCultureInfo("en-US"), out result))
        {
            Debug.Log("Can't pars float,wrong text ");
        }

        return result;
    }

    public static char GetPlatformSpecificLineEnd()
    {
        char lineEnding = '\n';
#if UNITY_IOS
        lineEnding = '\r';
#endif
        return lineEnding;
    }
}
