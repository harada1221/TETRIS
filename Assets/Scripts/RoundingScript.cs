using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class RoundingScript
{
    //’l‚ÌØ‚èã‚°‚ğs‚¤
    public static Vector3 Round(Vector3 i)
    {
        return new Vector3(Mathf.Round(i.x), Mathf.Round(i.y));
    }
}
