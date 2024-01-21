using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ExtensionMethods
{
    /// <summary>
    /// translate a given vector from one linear space to another
    /// </summary>
    /// <param name="value">the Vector to be maped</param>
    /// <param name="from1">minimum vector for the old space</param>
    /// <param name="to1">maximum vector for the old space</param>
    /// <param name="from2">minimum vector for the new space</param>
    /// <param name="to2">maximum vector for the new space</param>
    /// <returns>the value vectors equivalent in the new space</returns>
    public static Vector3 Map(Vector3 value, Vector3 from1, Vector3 to1, Vector3 from2, Vector3 to2)
    {
        float x = Map(value.x, from1.x, to1.x, from2.x, to2.x);
        float y = Map(value.y, from1.y, to1.y, from2.y, to2.y);
        float z = Map(value.z, from1.z, to1.z, from2.z, to2.z);

        return new Vector3(x, y, z);
    }

    /// <summary>
    /// translate a given value from one linear range to another
    /// </summary>
    /// <param name="value">the value to be maped</param>
    /// <param name="from1">minimum value for the old range</param>
    /// <param name="to1">maximum value for the old range</param>
    /// <param name="from2">minimum value for the new range</param>
    /// <param name="to2">maximum value for the new range</param>
    /// <returns>the values equivalent in the new range</returns>
    public static float Map(float value, float from1, float to1, float from2, float to2)
    {
        return (value - from1) / (to1 - from1) * (to2 - from2) + from2;
    }
}

