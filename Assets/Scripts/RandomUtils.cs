using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class UtilsRandom
{

    /// <summary>
    /// Returns a random colour with given alpha transparancy 
    /// </summary>
    /// <param name="Alpha">The transparancy with max 1</param>
    /// <returns>Random Colour</returns>
    public static Color Colour(float Alpha)
    {
        float r = Random.value;
        Random.InitState((int)(r* 10000 * 3));
        float g = Random.value;
        Random.InitState((int)(g * 10000 * 7));
        float b = Random.value;
        Random.InitState((int)(b * 10000 * 11));
        return new Color(r, g, b, Alpha);
    }

    /// <summary>
    /// Gets a random colour by initiating the random state
    /// </summary>
    /// <returns>Random Colour</returns>
    public static Color Colour()
    {
        return Colour(1);
    }
}
