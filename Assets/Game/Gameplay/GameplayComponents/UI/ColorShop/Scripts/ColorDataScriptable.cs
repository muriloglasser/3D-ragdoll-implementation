using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ColorData", menuName = "ScriptableObjects/ColorData")]
public class ColorDataScriptable : ScriptableObject
{
    public List<ColorData> colors;
}
