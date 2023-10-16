using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UniTools;
using UnityEngine;
public class Row
{
    public List<string> cells;
    public Row header;
    public Row(string data, Row header = null)
    {
        this.header = header;
        cells = GoogleTableParser.GetCells(data).ToList();
        for(int i = 0; i < cells.Count; i++) {
            cells[i] = cells[i].Remove(0, 1);
            cells[i] = cells[i].Remove(cells[i].Length - 1);
        }
    }
    public string GetCell(string columnName)
    {
        int idx = header.cells.FindIndex(c => c == columnName);
        if (!cells.HasIndex(idx)) return null;
        return cells[idx];
    }
}
public class GoogleTable
{
    string _data;
    public List<Row> rows { get; private set; } = new List<Row>();
    public GoogleTable(string data)
    {
        SetData(data);
    }
    public Row GetRow(int idx) => rows[idx];
    public void SetData(string data)
    {
        _data = data;
        rows.Clear();
        GoogleTableParser.GetRows(data).ToList().ForEach(textRow =>
        {
            var header = rows.Count == 0 ? new Row(textRow) : rows[0].header;
            rows.Add(new Row(textRow, header));
        });
    }
}
public static class GoogleTableParser
{
    public static string[] GetRows(this string data)
    {
        return data.Split(GoogleTableParser.PlatformLineEnd());
    }
    public static string[] GetCells(this string rowData)
    {
        return rowData.Split(',');
    }
    public static Vector3 ParseVector3(this string s)
    {
        string[] vectorComponents = s.Split(';');
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

    public static int ParseInt(this string s)
    {
        int result = -1;
        if (!int.TryParse(s, System.Globalization.NumberStyles.Integer, System.Globalization.CultureInfo.GetCultureInfo("en-US"), out result))
        {
            Debug.Log("Can't parse int, wrong text");
        }

        return result;
    }

    public static bool ParseBool(this string s)
    {
        if (s == null) return false;
        s = s.ToLower();
        return s == "true" || s == "истина";
    }
    public static float ParseFloat(this string s)
    {
        float result = -1;
        if (!float.TryParse(s, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.GetCultureInfo("en-US"), out result))
        {
            Debug.Log("Can't pars float,wrong text ");
        }

        return result;
    }

    private static char PlatformLineEnd()
    {
        char lineEnding = '\n';
#if UNITY_IOS
        lineEnding = '\r';
#endif
        return lineEnding;
    }
}
