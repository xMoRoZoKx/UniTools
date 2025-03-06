using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class PhysicsTools
{
    public static bool IsInLayerMask(this Component component, LayerMask mask)
    {
        return ((1 << component.gameObject.layer) & mask) != 0;
    }
}
