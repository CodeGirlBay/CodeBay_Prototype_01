﻿using UnityEngine;
using UnityEditor;
using ActionCat;

public class SkillData_Editor
{

}

[CustomEditor(typeof(SkillDataSpreadShot))]
public class SpreadShot_DataEditor : Editor
{
    SerializedObject sobject;

    SerializedProperty idProp;
    SerializedProperty nameProp;
    SerializedProperty descProp;
    SerializedProperty typeProp;
    SerializedProperty levelProp;

    SerializedProperty arrowCountProp;
    SerializedProperty spreadAngleProp;

    public void OnEnable()
    {
        sobject = new SerializedObject(target);

        idProp    = sobject.FindProperty("SkillId");
        nameProp  = sobject.FindProperty("SkillName");
        descProp  = sobject.FindProperty("SkillDesc");
        typeProp  = sobject.FindProperty("SkillType");
        levelProp = sobject.FindProperty("SkillLevel");

        arrowCountProp  = sobject.FindProperty("ArrowShotCount");
        spreadAngleProp = sobject.FindProperty("SpreadAngle");
    }

    public override void OnInspectorGUI()
    {
        //base.OnInspectorGUI();
        EditorGUI.BeginDisabledGroup(true);
        EditorGUILayout.ObjectField("Script", MonoScript.FromScriptableObject((SkillDataSpreadShot)target),
                                                                       typeof(SkillDataSpreadShot), false);
        EditorGUI.EndDisabledGroup();

        sobject.Update();

        GUILayout.Space(5f);
        GUILayout.BeginVertical("HelpBox");

        #region BASIC_SKILL_DATA
        //Start Default Data Fields
        GUILayout.Label("Default Skill Info", EditorStyles.boldLabel);
        GUILayout.BeginVertical("GroupBox");

        //ID Field
        EditorGUILayout.PropertyField(idProp);

        //Name Field
        EditorGUILayout.PropertyField(nameProp, true);

        //Description Field
        GUILayout.Label("Skill Description");
        descProp.stringValue = EditorGUILayout.TextArea(descProp.stringValue, GUILayout.Height(50f));

        //Type Field [LOCK]
        EditorGUI.BeginDisabledGroup(true);
        EditorGUILayout.PropertyField(typeProp, true);
        EditorGUI.EndDisabledGroup();

        //Level Field
        EditorGUILayout.PropertyField(levelProp, true);

        //End Field
        GUILayout.EndVertical();
        #endregion

        #region ANOTHER_SKILL_DATA
        GUILayout.Label("Spread Shot Info", EditorStyles.boldLabel);
        GUILayout.BeginVertical("GroupBox");

        //Arrow Shot Count Field
        EditorGUILayout.PropertyField(arrowCountProp);

        //Spread Angle Field
        EditorGUILayout.PropertyField(spreadAngleProp);

        GUILayout.EndVertical();
        #endregion

        GUILayout.EndVertical();

        //Save Property
        sobject.ApplyModifiedProperties();
    }
}

[CustomEditor(typeof(SkillDataRapidShot))]
public class RapidShot_DataEditor : Editor
{
    SerializedObject sobject;

    SerializedProperty idProp;
    SerializedProperty nameProp;
    SerializedProperty descProp;
    SerializedProperty typeProp;
    SerializedProperty levelProp;

    SerializedProperty arrowCountProp;
    SerializedProperty intervalProp;

    public void OnEnable()
    {
        sobject = new SerializedObject(target);

        idProp    = sobject.FindProperty("SkillId");
        nameProp  = sobject.FindProperty("SkillName");
        descProp  = sobject.FindProperty("SkillDesc");
        typeProp  = sobject.FindProperty("SkillType");
        levelProp = sobject.FindProperty("SkillLevel");

        arrowCountProp  = sobject.FindProperty("ArrowShotCount");
        intervalProp    = sobject.FindProperty("ShotInterval");
    }

    public override void OnInspectorGUI()
    {
        //base.OnInspectorGUI();
        EditorGUI.BeginDisabledGroup(true);
        EditorGUILayout.ObjectField("Script", MonoScript.FromScriptableObject((SkillDataRapidShot)target),
                                                                       typeof(SkillDataRapidShot), false);
        EditorGUI.EndDisabledGroup();

        sobject.Update();

        GUILayout.Space(5);
        GUILayout.BeginVertical("HelpBox");

        #region BASIC_SKILL_DATA
        //Start Default Data Fields
        GUILayout.Label("Default Skill Info", EditorStyles.boldLabel);
        GUILayout.BeginVertical("GroupBox");

        //ID Field
        EditorGUILayout.PropertyField(idProp);

        //Name Field
        EditorGUILayout.PropertyField(nameProp, true);

        //Description Field
        GUILayout.Label("Skill Description");
        descProp.stringValue = EditorGUILayout.TextArea(descProp.stringValue, GUILayout.Height(50f));

        //Type Field [LOCK]
        EditorGUI.BeginDisabledGroup(true);
        EditorGUILayout.PropertyField(typeProp, true);
        EditorGUI.EndDisabledGroup();

        //Level Field
        EditorGUILayout.PropertyField(levelProp, true);

        //End Field
        GUILayout.EndVertical();
        #endregion

        #region ANOTHER_SKILL_DATA
        GUILayout.Label("Rapid Shot Info", EditorStyles.boldLabel);
        GUILayout.BeginVertical("GroupBox");

        //Arrow Count Field
        EditorGUILayout.PropertyField(arrowCountProp);

        //Spread Angle Field
        EditorGUILayout.PropertyField(intervalProp);

        GUILayout.EndVertical();
        #endregion

        GUILayout.EndVertical();

        //Save Property
        sobject.ApplyModifiedProperties();
    }
}

[CustomEditor(typeof(SkillDataArrowRain))]
public class RainArrow_DataEditor : Editor
{
    SerializedObject sobject;

    SerializedProperty idProp;
    SerializedProperty nameProp;
    SerializedProperty descProp;
    SerializedProperty typeProp;
    SerializedProperty levelProp;

    SerializedProperty arrowCountProp;
    SerializedProperty intervalProp;

    public void OnEnable()
    {
        sobject = new SerializedObject(target);

        idProp    = sobject.FindProperty("SkillId");
        nameProp  = sobject.FindProperty("SkillName");
        descProp  = sobject.FindProperty("SkillDesc");
        typeProp  = sobject.FindProperty("SkillType");
        levelProp = sobject.FindProperty("SkillLevel");

        arrowCountProp = sobject.FindProperty("ArrowShotCount");
        intervalProp   = sobject.FindProperty("ShotInterval");
    }

    public override void OnInspectorGUI()
    {
        //base.OnInspectorGUI();
        EditorGUI.BeginDisabledGroup(true);
        EditorGUILayout.ObjectField("Script", MonoScript.FromScriptableObject((SkillDataArrowRain)target),
                                                                       typeof(SkillDataArrowRain), false);
        EditorGUI.EndDisabledGroup();

        sobject.Update();

        GUILayout.Space(5);
        GUILayout.BeginVertical("HelpBox");

        #region BASIC_SKILL_DATA
        //Start Default Data Fields
        GUILayout.Label("Default Skill Info", EditorStyles.boldLabel);
        GUILayout.BeginVertical("GroupBox");

        //ID Field
        EditorGUILayout.PropertyField(idProp);

        //Name Field
        EditorGUILayout.PropertyField(nameProp, true);

        //Description Field
        GUILayout.Label("Skill Description");
        descProp.stringValue = EditorGUILayout.TextArea(descProp.stringValue, GUILayout.Height(50f));

        //Type Field [LOCK]
        EditorGUI.BeginDisabledGroup(true);
        EditorGUILayout.PropertyField(typeProp, true);
        EditorGUI.EndDisabledGroup();

        //Level Field
        EditorGUILayout.PropertyField(levelProp, true);

        //End Field
        GUILayout.EndVertical();
        #endregion

        #region ANOTHER_SKILL_DATA
        GUILayout.Label("Rain Arrow Info", EditorStyles.boldLabel);
        GUILayout.BeginVertical("GroupBox");

        //Arrow Count Field
        EditorGUILayout.PropertyField(arrowCountProp);

        //Interval Field
        EditorGUILayout.PropertyField(intervalProp);

        GUILayout.EndVertical();
        #endregion

        GUILayout.EndVertical();

        //Save Property
        sobject.ApplyModifiedProperties();
    }
}

[CustomEditor(typeof(SkillData_Empty))]
public class Empty_DataEditor : Editor
{
    SerializedObject sobject;

    SerializedProperty idProp;
    SerializedProperty nameProp;
    SerializedProperty descProp;
    SerializedProperty typeProp;
    SerializedProperty levelProp;

    public void OnEnable()
    {
        sobject = new SerializedObject(target);

        idProp    = sobject.FindProperty("SkillId");
        nameProp  = sobject.FindProperty("SkillName");
        descProp  = sobject.FindProperty("SkillDesc");
        typeProp  = sobject.FindProperty("SkillType");
        levelProp = sobject.FindProperty("SkillLevel");

        //levelProperty = sobject.FindProperty(nameof(SkillData_Empty.SkillName));
    }

    public override void OnInspectorGUI()
    {
        //base.OnInspectorGUI();
        EditorGUI.BeginDisabledGroup(true);
        EditorGUILayout.ObjectField("Script", MonoScript.FromScriptableObject((SkillData_Empty)target), 
                                                                       typeof(SkillData_Empty), false);
        EditorGUI.EndDisabledGroup();

        sobject.Update();

        GUILayout.Space(5);
        GUILayout.BeginVertical("HelpBox");

        #region BASIC_SKILL_DATA
        //Start Default Data Fields
        GUILayout.Label("Default Skill Info", EditorStyles.boldLabel);
        GUILayout.BeginVertical("GroupBox");

        //ID Field
        EditorGUILayout.PropertyField(idProp);

        //Name Field
        EditorGUILayout.PropertyField(nameProp, true);

        //Description Field
        GUILayout.Label("Skill Description");
        descProp.stringValue = EditorGUILayout.TextArea(descProp.stringValue, GUILayout.Height(50f));

        //Type Field [LOCK]
        EditorGUI.BeginDisabledGroup(true);
        EditorGUILayout.PropertyField(typeProp, true);
        EditorGUI.EndDisabledGroup();

        //Level Field
        EditorGUILayout.PropertyField(levelProp, true);

        //End Field
        GUILayout.EndVertical();
        #endregion

        #region ANOTHER_SKILL_DATA

        #endregion

        GUILayout.EndVertical();

        //Save Property
        sobject.ApplyModifiedProperties();
    }
}

/// <summary>
/// Add Skill Data Scriptable Asset Create Button to ActionCat Menu
/// </summary>
public static class CreateSkillDataAsset
{
    [MenuItem("ActionCat/Scriptable Object/Bow Skill Asset/Spread Shot")]
    public static void CreateSpreadShotAsset()
    {

    }

    [MenuItem("ActionCat/Scriptable Object/Bow Skill Asset/Rapid Shot")]
    public static void CreateRapidShotAsset()
    {

    }

    [MenuItem("ActionCat/Scriptable Object/Bow Skill Asset/Rain Arrow")]
    public static void CreateRainArrowAsset()
    {

    }

    [MenuItem("ActionCat/Scriptable Object/Bow Skill Asset/Empty Slot")]
    public static void CreateEmptySlotAsset()
    {

    }
}



