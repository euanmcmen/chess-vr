using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public static class EditorHelper
{
    #region Get Serialized Property from SerializedObject

    public static SerializedProperty GetSerializedPropertyByConvention(SerializedObject serializedObject, string serializedPropertyName)
    {
        var convention = "Property";

        var propertyName = serializedPropertyName.Remove(serializedPropertyName.Length - convention.Length);

        return GetSerializedProperty(serializedObject, propertyName);
    }

    public static SerializedProperty GetSerializedProperty(SerializedObject serializedObject, string propertyName)
    {
        var serializedProperty = serializedObject.FindProperty(propertyName);

        if (serializedProperty == null)
        {
            Debug.LogErrorFormat("Unable to find property '{0}' on SerializedObject '{1}'.  This will likely lead to incorrect behaviour.", propertyName, serializedObject);
        }

        return serializedProperty;
    }

    #endregion

    #region Show Window
    public static void ShowEditorWindow<TWindow>()
    {
        var window = EditorWindow.GetWindow(typeof(TWindow), false);
        window.Show();
    }
    #endregion

    #region Control Drawing
    public static void DrawEditorControlsInGroup(string groupName, bool isDisabled, Action groupDrawAction)
    {
        EditorGUI.BeginDisabledGroup(isDisabled);

        EditorGUILayout.BeginVertical(EditorStyles.textArea);

        EditorGUILayout.LabelField(groupName, EditorStyles.whiteLabel);

        groupDrawAction();

        EditorGUILayout.EndVertical();

        EditorGUI.EndDisabledGroup();

        EditorGUILayout.Space();
    }

    public static void DrawInScrollGroup(string groupName, Action groupDrawAction, ref Vector2 scrollPosition)
    {
        scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition, EditorStyles.textArea);

        EditorGUILayout.LabelField(groupName, EditorStyles.whiteLargeLabel);

        groupDrawAction();

        EditorGUILayout.EndScrollView();

        EditorGUILayout.Space();
    }

    public static int DrawIntFOVSlider(string label, int fov)
    {
        return EditorGUILayout.IntSlider(label, fov, 1, 179);
    }

    public static (Vector3 outputPosition, Quaternion outputRotation) DrawTransformControl(string groupName, Transform input)
    {
        EditorGUILayout.BeginVertical(EditorStyles.textArea);

        EditorGUILayout.LabelField(groupName, EditorStyles.whiteLabel);

        var outputPosition = EditorGUILayout.Vector3Field("Position", input.position);

        var outputRotation = Quaternion.Euler(EditorGUILayout.Vector3Field("Euler Rotation", input.rotation.eulerAngles));

        EditorGUILayout.EndVertical();

        EditorGUILayout.Space();

        return (outputPosition, outputRotation);
    }

    public static Vector3 DrawImmediateVerticalVector3Control(string groupName, Vector3 input)
    {
        var floatX = input.x;
        var floatY = input.y;
        var floatZ = input.z;

        EditorGUILayout.BeginVertical(EditorStyles.textArea);

        EditorGUILayout.LabelField(groupName, EditorStyles.whiteLabel);

        floatX = EditorGUILayout.FloatField("X", floatX);
        floatY = EditorGUILayout.FloatField("Y", floatY);
        floatZ = EditorGUILayout.FloatField("Z", floatZ);

        EditorGUILayout.EndVertical();

        EditorGUILayout.Space();

        return new Vector3(floatX, floatY, floatZ);
    }

    public static T DrawTypedObjectField<T>(string label, T obj, bool allowSceneObjects = false, params GUILayoutOption[] gUILayoutOptions) where T : UnityEngine.Object
    {
        return (T)(EditorGUILayout.ObjectField(label, obj, typeof(T), allowSceneObjects, gUILayoutOptions));
    }

    public static List<T> DrawTypedObjectListField<T>(string label, IEnumerable<T> currentObjs, bool allowSceneObjects = false, bool showSizeIntControl = false) where T : UnityEngine.Object
    {
        var newList = new List<T>(currentObjs);

        if (showSizeIntControl)
        {
            var newCount = Mathf.Max(0, newList.Count);
            while (newCount < newList.Count)
                newList.RemoveAt(newList.Count - 1);
            while (newCount > newList.Count)
                newList.Add(default);
        }

        EditorGUILayout.BeginVertical(EditorStyles.textArea);
        EditorGUILayout.LabelField(label, EditorStyles.whiteLabel);

        for (int i = 0; i < newList.Count; i++)
            newList[i] = (T)(EditorGUILayout.ObjectField($"{typeof(T).Name} {i + 1}:", newList[i], typeof(T), allowSceneObjects));

        EditorGUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();
        if (GUILayout.Button("+", GUILayout.Width(40))) { newList.Add(null); }
        if (GUILayout.Button("-", GUILayout.Width(40))) { newList.RemoveAt(newList.Count - 1); }
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.EndVertical();

        return newList;
    }
    #endregion
}