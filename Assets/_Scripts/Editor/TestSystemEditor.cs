using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(TestSystem))]
public class TestSystemEditor : Editor
{
    private void OnEnable()
    {
        if (target == null) return;
    }

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        TestSystem script = target as TestSystem;

        if (GUILayout.Button("点击执行编辑器方法"))
        {
            script.UnitSkillTest();
        }
    }
}
