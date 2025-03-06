
using UnityEngine;

public static class ColorTools
{
    public static Color WithAlpha(this Color color, float alpha) => new Color() { r = color.r, g = color.g, b = color.b, a = alpha };
}
