using UnityEngine;
using System.Collections;

public static class ColorExtensions 
{
    /// <summary>
    /// Returns a copy of the color as transparent.
    /// </summary>
    public static Color GetTransparent(this Color c)
    {
        return new Color(c.r, c.g, c.b, 0f);
    }

    /// <summary>
    /// Returns a copy of the color as transparent with a set alpha.
    /// </summary>
    public static Color GetTransparent(this Color c, float alpha)
    {
        return new Color(c.r, c.g, c.b, alpha);
    }

    /// <summary>
    /// Returns a copy of the color as fully-opaque.
    /// </summary>
    public static Color GetOpaque(this Color c)
    {
        return new Color(c.r, c.g, c.b, 1f);
    }
}
