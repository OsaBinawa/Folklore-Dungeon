using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(EnemyData))]
public class EnemyDataEditor : Editor
{
    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        SerializedProperty isUniqueProp = serializedObject.FindProperty("isUnique");
        SerializedProperty attacksProp = serializedObject.FindProperty("attacks");

        DrawDefaultInspector();

        if (!isUniqueProp.boolValue && attacksProp != null)
        {
            for (int i = 0; i < attacksProp.arraySize; i++)
            {
                SerializedProperty attack = attacksProp.GetArrayElementAtIndex(i);
                SerializedProperty cooldown = attack.FindPropertyRelative("Cooldown");

                if (cooldown != null)
                    cooldown.intValue = 0;
            }

            EditorGUILayout.HelpBox(
                "This enemy is a WEAKLING.\nAttack cooldowns are disabled.",
                MessageType.Info
            );
        }

        serializedObject.ApplyModifiedProperties();
    }
}
