using UnityEngine;

// when you want only one layer, it's ~(1 << LayerMask.NameToLayer(""))

public static class Layers
{
    public static int Default = LayerMask.NameToLayer("Default");
    public static int UI = LayerMask.NameToLayer("UI");
    public static int Player = LayerMask.NameToLayer("Player");
    public static int Ground = LayerMask.NameToLayer("Ground");

    public static int MaskOnly(int layer)
    {
        return 1 << layer;
    }

    public static int MaskAllBut(int layer)
    {
        return ~(1 << layer);
    }
}