using UnityEditor;
using UnityEngine;

public static class EditorUIUtils
{
    private static readonly Color DefaultBgColor = new Color(0.3f, 0.3f, 0.3f, 0.15f);
    private const float BarWidth = 3f;

    private static readonly string[] ColorPool = new string[]
    {
        "#00BCD4",
        "#4CAF50",
        "#FF9800",
        "#9C27B0",
        "#F44336",
        "#2196F3",
        "#E91E63",
        "#FFEB3B",
        "#607D8B",
    };

    public static bool DrawColorGroup(
        string title,
        Color color,
        bool isExpanded,
        System.Action content
    )
    {
        // --- 1. 创建并定制文字样式 ---
        // 克隆官方的折叠头样式，防止修改全局样式
        GUIStyle customFoldoutStyle = new GUIStyle(EditorStyles.foldoutHeader);

        // 将文字颜色设置为传入的竖线颜色
        customFoldoutStyle.normal.textColor = color;
        customFoldoutStyle.onNormal.textColor = color; // 展开时的颜色
        customFoldoutStyle.hover.textColor = color; // 鼠标悬停时的颜色
        customFoldoutStyle.onHover.textColor = color;
        customFoldoutStyle.active.textColor = color; // 点击时的颜色
        customFoldoutStyle.onActive.textColor = color;
        customFoldoutStyle.focused.textColor = color;
        customFoldoutStyle.onFocused.textColor = color;

        // 设置字体加粗（可选，通常颜色亮了加粗更好看）
        customFoldoutStyle.fontStyle = FontStyle.Bold;

        // --- 2. 绘制 Header ---
        Rect headerRect = EditorGUILayout.GetControlRect(
            true,
            EditorGUIUtility.singleLineHeight + 4
        );

        // 绘制左侧彩色竖条
        EditorGUI.DrawRect(
            new Rect(headerRect.x, headerRect.y + 2, BarWidth, headerRect.height - 4),
            color
        );

        // 使用我们定制的 customFoldoutStyle 来绘制折叠标题
        bool foldout = EditorGUI.Foldout(
            new Rect(headerRect.x + 12, headerRect.y, headerRect.width - 12, headerRect.height),
            isExpanded,
            title,
            true,
            customFoldoutStyle // 注入自定义样式
        );

        if (foldout)
        {
            Rect contentRect = EditorGUILayout.BeginVertical();

            if (Event.current.type == EventType.Repaint)
            {
                EditorGUI.DrawRect(contentRect, DefaultBgColor);
                EditorGUI.DrawRect(
                    new Rect(headerRect.x, contentRect.y, BarWidth, contentRect.height),
                    color
                );
            }

            EditorGUILayout.BeginHorizontal();
            GUILayout.Space(15);
            EditorGUILayout.BeginVertical();
            GUILayout.Space(5);

            content?.Invoke();

            GUILayout.Space(5);
            EditorGUILayout.EndVertical();
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.EndVertical();
        }

        return foldout;
    }

    public static Color GetColorFromPool(string hex, string sectionName)
    {
        if (
            !string.IsNullOrEmpty(hex)
            && ColorUtility.TryParseHtmlString(hex, out Color customColor)
        )
            return customColor;

        int hash = Mathf.Abs(sectionName.GetHashCode());
        int index = hash % ColorPool.Length;
        ColorUtility.TryParseHtmlString(ColorPool[index], out Color pooledColor);
        return pooledColor;
    }
}
