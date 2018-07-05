using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class CoolStuff {

    /// <summary>
    /// http://devmag.org.za/2011/04/05/bzier-curves-a-tutorial/
    /// </summary>
    /// <param name="origin"></param>
    /// <param name="height"></param>
    /// <param name="target"></param>
    /// <param name="rate"></param>
    /// <returns></returns>
    public static IEnumerable<Vector2> PositionOverParabola(Vector2 origin, Vector2 height, Vector2 target, float time = 2f)
    {
        for (float t = 0f; t < 1f; t += Time.deltaTime / time )
        {
            yield return (Mathf.Pow(1f - t,2f) * origin) + (2f * (1f - t) * t * height) + (Mathf.Pow(t, 2f) * target);
        }

        /* End by returning the target */
        yield return target;
    }
}
