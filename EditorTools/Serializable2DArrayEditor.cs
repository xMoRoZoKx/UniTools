using UnityEditor;
using UnityEngine;
using System;
using System.Reflection;

public class Serializable2DArrayEditor 
{
    private Vector2 scrollPosition;
    private UnityEngine.Object target;

    public void OnInspectorGUI(UnityEngine.Object target)
    {
        this.target = target;

        Type targetType = target.GetType();
        FieldInfo[] fields = targetType.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

        foreach (var field in fields)
        {
            if (Attribute.IsDefined(field, typeof(HideInInspector))) continue; // Пропускаем скрытые поля

            if (field.FieldType.IsGenericType && field.FieldType.GetGenericTypeDefinition() == typeof(Serializable2DArray<>))
            {
                DrawArrayEditor(field);
            }
        }
    }

    private void DrawArrayEditor(FieldInfo field)
    {
        object arrayInstance = field.GetValue(target);
        if (arrayInstance == null)
        {
            EditorGUILayout.HelpBox($"{field.Name} is null. Initializing...", MessageType.Warning);
            field.SetValue(target, Activator.CreateInstance(field.FieldType, new object[] { 3, 3 }));
            EditorUtility.SetDirty(target);
            return;
        }

        Type elementType = field.FieldType.GetGenericArguments()[0];

        GUILayout.Space(10);
        GUILayout.Label($"{field.Name} ({elementType.Name})", EditorStyles.boldLabel);

        PropertyInfo rowsProperty = field.FieldType.GetProperty("Rows");
        PropertyInfo columnsProperty = field.FieldType.GetProperty("Columns");
        MethodInfo resizeMethod = field.FieldType.GetMethod("Resize");

        int rows = (int)rowsProperty.GetValue(arrayInstance);
        int columns = (int)columnsProperty.GetValue(arrayInstance);

        int newRows = Mathf.Max(1, EditorGUILayout.IntField("Rows", rows));
        int newColumns = Mathf.Max(1, EditorGUILayout.IntField("Columns", columns));

        if (newRows != rows || newColumns != columns)
        {
            Undo.RecordObject(target, "Resize Grid");
            resizeMethod.Invoke(arrayInstance, new object[] { newColumns, newRows }); // ✅ Исправлен порядок аргументов
            EditorUtility.SetDirty(target);
        }

        MethodInfo getValueMethod = field.FieldType.GetMethod("GetValue");
        MethodInfo setValueMethod = field.FieldType.GetMethod("SetValue");

        float maxWidth = EditorGUIUtility.currentViewWidth - 30; // Максимальная ширина доступного пространства
        scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition, GUILayout.Width(maxWidth), GUILayout.Height(200));

        for (int row = 0; row < newRows; row++) // ✅ Теперь строка сначала, затем колонки
        {
            GUILayout.BeginHorizontal();
            for (int col = 0; col < newColumns; col++)
            {
                object value = getValueMethod.Invoke(arrayInstance, new object[] { col, row });

                if (value == null && typeof(UnityEngine.Object).IsAssignableFrom(elementType))
                {
                    value = null; // Оставляем возможность установить объект вручную
                }
                else if (value == null)
                {
                    value = Activator.CreateInstance(elementType);
                    setValueMethod.Invoke(arrayInstance, new object[] { col, row, value });
                }

                object newValue = DrawField(value, elementType);
                setValueMethod.Invoke(arrayInstance, new object[] { col, row, newValue });
            }
            GUILayout.EndHorizontal();
        }

        EditorGUILayout.EndScrollView();

        if (GUI.changed)
        {
            EditorUtility.SetDirty(target);
        }
    }


    private object DrawField(object value, Type fieldType)
    {
        Type valueType = value?.GetType() ?? fieldType;

        if (typeof(UnityEngine.Object).IsAssignableFrom(valueType))
        {
            return EditorGUILayout.ObjectField((UnityEngine.Object)value, valueType, true, GUILayout.Width(120));
        }

        if (value == null)
        {
            GUILayout.Label($"NULL ({fieldType.Name})", EditorStyles.boldLabel);
            return null;
        }

        if (valueType.IsClass && valueType.IsSerializable)
        {
            return DrawSerializedClass(value, valueType);
        }

        EditorGUIUtility.labelWidth = 50;

        if (value is int) return EditorGUILayout.IntField((int)value, GUILayout.Width(120));
        if (value is float) return EditorGUILayout.FloatField((float)value, GUILayout.Width(120));
        if (value is string) return EditorGUILayout.TextField((string)value, GUILayout.Width(120));
        if (value is bool) return EditorGUILayout.Toggle((bool)value, GUILayout.Width(50));
        if (value is Enum) return EditorGUILayout.EnumPopup((Enum)value, GUILayout.Width(120));

        if (value is Vector2) return EditorGUILayout.Vector2Field("", (Vector2)value, GUILayout.Width(150));
        if (value is Vector3) return EditorGUILayout.Vector3Field("", (Vector3)value, GUILayout.Width(180));
        if (value is Vector4) return EditorGUILayout.Vector4Field("", (Vector4)value, GUILayout.Width(200));
        if (value is Color) return EditorGUILayout.ColorField("", (Color)value, GUILayout.Width(120));
        if (value is Rect) return EditorGUILayout.RectField("", (Rect)value, GUILayout.Width(200));
        if (value is Quaternion q)
        {
            Vector3 euler = EditorGUILayout.Vector3Field("Rot", q.eulerAngles, GUILayout.Width(150));
            return Quaternion.Euler(euler);
        }

        GUILayout.Label($"Unsupported Type: {valueType.Name}", EditorStyles.boldLabel);
        return value;
    }



    private object DrawSerializedClass(object obj, Type objectType)
    {
        EditorGUILayout.BeginVertical("box", GUILayout.Width(180), GUILayout.MinWidth(150), GUILayout.ExpandWidth(false)); // Фиксируем ширину
        EditorGUILayout.LabelField(objectType.Name, EditorStyles.boldLabel, GUILayout.Width(160));

        FieldInfo[] fields = objectType.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

        foreach (var field in fields)
        {
            if (field.IsNotSerialized || (!field.IsPublic && !Attribute.IsDefined(field, typeof(SerializeField))))
                continue;

            EditorGUILayout.BeginHorizontal(); // Поля выравниваются в строку
            EditorGUILayout.LabelField(field.Name, GUILayout.Width(70)); // Компактный Label
            object fieldValue = field.GetValue(obj);
            object newValue = DrawField(fieldValue, field.FieldType);
            EditorGUILayout.EndHorizontal();

            if (!Equals(fieldValue, newValue))
            {
                field.SetValue(obj, newValue);
            }
        }

        EditorGUILayout.EndVertical();
        return obj;
    }



}
