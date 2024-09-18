using UnityEngine;

public static class ColorExtension
{
    public static Color ToTransparent(this Color color)
    {
        var transparentColor = color;
        transparentColor.a = 0f;
        return transparentColor;
    }

    public static Color ToOpaque(this Color color)
    {
        var transparentColor = color;
        transparentColor.a = 1f;
        return transparentColor;
    }
}