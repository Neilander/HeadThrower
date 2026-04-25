using System;
using UnityEngine;

[AttributeUsage(AttributeTargets.Field, Inherited = true, AllowMultiple = false)]
public class SectionAttribute : PropertyAttribute
{
    public string Name;
    public string ColorHex;

    // 将 colorHex 设为可选参数，默认值为 null 或空
    public SectionAttribute(string name, string colorHex = "")
    {
        Name = name;
        ColorHex = colorHex;
    }
}
