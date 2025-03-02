using System;
using UnityEngine;

[Serializable]
public class Serializable2DArray<T>
{
    [SerializeField] private int rows = 3;
    [SerializeField] private int columns = 3;
    [SerializeField] private T[] data; 

    public int Rows => rows;
    public int Columns => columns;
    public T[] Data => data;

    public Serializable2DArray(int columns, int rows)
    {
        this.rows = Mathf.Max(1, rows);
        this.columns = Mathf.Max(1, columns);
        data = new T[this.rows * this.columns];
    }

    public T GetValue(int col, int row)
    {
        int index = row * columns + col;
        if (index < 0 || index >= data.Length)
        {
            return default;
        }
        return data[index];
    }

    public void SetValue(int col, int row, T value)
    {
        int index = row * columns + col;
        if (index < 0 || index >= data.Length)
        {
            Debug.LogError($"❌ SetValue({col}, {row}) is wrong! Index: {index}, Columns: {columns}, Rows: {rows}, ArraySize: {data.Length}");
            return;
        }
        data[index] = value;
    }

    public void Resize(int newColumns, int newRows)
    {
        T[] newData = new T[newRows * newColumns];

        for (int row = 0; row < Mathf.Min(rows, newRows); row++)
        {
            for (int col = 0; col < Mathf.Min(columns, newColumns); col++)
            {
                int oldIndex = row * columns + col;
                int newIndex = row * newColumns + col;

                if (oldIndex < data.Length && newIndex < newData.Length)
                    newData[newIndex] = data[oldIndex];
            }
        }

        rows = newRows;
        columns = newColumns;
        data = newData;
    }
}
