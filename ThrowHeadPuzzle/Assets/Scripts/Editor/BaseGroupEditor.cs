using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(UnityEngine.Object), true)] // 匹配所有对象
[CanEditMultipleObjects]
public class BaseGroupEditor : Editor
{
    private Dictionary<string, bool> _foldoutStates = new Dictionary<string, bool>();

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        SerializedProperty iterator = serializedObject.GetIterator();
        bool enterChildren = true;
        string currentSection = null;
        List<SerializedProperty> sectionProps = new List<SerializedProperty>();

        // 遍历所有属性并分组
        while (iterator.NextVisible(enterChildren))
        {
            enterChildren = false;
            if (iterator.name == "m_Script")
                continue;

            // 获取 Section 属性
            var attr = GetAttribute<SectionAttribute>(iterator);

            if (attr != null)
            {
                // 绘制之前的组
                DrawSection(currentSection, sectionProps);

                // 开始新组
                currentSection = attr.Name;
                sectionProps.Clear();
            }

            if (currentSection != null)
                sectionProps.Add(iterator.Copy());
            else
                EditorGUILayout.PropertyField(iterator, true); // 没有组的直接画
        }

        // 绘制最后一个组
        DrawSection(currentSection, sectionProps);

        serializedObject.ApplyModifiedProperties();
    }

    // 在 BaseGroupEditor.cs 的 DrawSection 方法中修改：
    private void DrawSection(string name, List<SerializedProperty> props)
    {
        if (string.IsNullOrEmpty(name) || props.Count == 0)
            return;
        if (!_foldoutStates.ContainsKey(name))
            _foldoutStates[name] = true;

        var attr = GetAttribute<SectionAttribute>(props[0]);

        // 【修改点】调用新的 GetColorFromPool 方法
        Color sectionColor = EditorUIUtils.GetColorFromPool(attr?.ColorHex, name);

        _foldoutStates[name] = EditorUIUtils.DrawColorGroup(
            name,
            sectionColor,
            _foldoutStates[name],
            () =>
            {
                foreach (var p in props)
                    EditorGUILayout.PropertyField(p, true);
            }
        );
    }

    private T GetAttribute<T>(SerializedProperty prop)
        where T : class
    {
        var targetType = serializedObject.targetObject.GetType();
        var field = targetType.GetField(
            prop.name,
            System.Reflection.BindingFlags.Public
                | System.Reflection.BindingFlags.NonPublic
                | System.Reflection.BindingFlags.Instance
        );
        if (field == null)
            return null;
        return System.Attribute.GetCustomAttribute(field, typeof(T)) as T;
    }
}
