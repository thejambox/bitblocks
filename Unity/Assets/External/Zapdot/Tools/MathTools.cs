using UnityEngine;
using System.Collections;

public static class MathTools
{
    // this function assumes the vectors are being placed on the xy-plane.
    public static float AngleBetween(Vector3 a, Vector3 b)
    {
        return Mathf.Acos(Vector3.Dot(a, b) / (a.magnitude * b.magnitude));
    }

    // this function assumes the vectors are being placed on the xy-plane.
    public static float AngleBetweenSigned(Vector3 a, Vector3 b)
    {
        return AngleBetween(a, b) * Mathf.Sign(Vector3.Cross(a, b).z);
    }

    public static int Wrap(int val, int max)
    {
        return (val < 0) ? max + val : val % max;
    }

    // padding should be a value from [0..1] defining how close to the edge t
    public static Vector3 ScreenSpaceForVector(Vector3 vec, float padding)
    {
        Vector3 result = Vector3.zero;

        float maxHeight = Screen.height * padding * 0.5f;
        float maxWidth = Screen.width * padding * 0.5f;

        float absx = Mathf.Abs(vec.x);
        float absy = Mathf.Abs(vec.y);

        result.y = (vec.x == 0 ? maxHeight : Mathf.Min(maxHeight, (absy / absx) * maxWidth)) * ((vec.y >= 0) ? 1f : -1f);
        result.x = (vec.y == 0 ? maxWidth : Mathf.Min(maxWidth, (absx / absy) * maxHeight)) * ((vec.x >= 0) ? 1f : -1f);

        return result;
    }

    // returns a smoothed value from 0 to 1 for any x = [0,1]
    public static float SmoothStep(float x)
    {
        // y = x² * (3 - 2x)
        x = Mathf.Clamp01(x);
        return x * x * (3f - (2f * x));
    }

    // returns a quadratic out value from 0 to 1 for any x = [0,1]
    public static float QuadOutStep(float x)
    {
        return x * (x - 2f);
    }

    // returns a cosine wave that ranges from 0 to 1 for any x
    public static float CosineWave(float x)
    {
        // y = (1 - cos(x*pi))/2
        return (1 - Mathf.Cos(x * Mathf.PI)) / 2f;
    }

    public static bool IsPowerOfTwo(ulong x)
    {
        return (x != 0) && ((x & (x - 1)) == 0);
    }

    /* Not implemented:
     * 
     * for tile-based games:
     * IndexFor(x,y) = x + y * width
     * PositionFor(i) = (i % width,floor(i / width))
     * 
     * normalize function
     * 
     * http://en.wikipedia.org/wiki/Fast_inverse_square_root
     * 
     * http://dinodini.wordpress.com/2010/04/05/normalized-tunable-sigmoid-functions/
     * 
     * http://en.wikipedia.org/wiki/Perlin_noise
     * 
     * http://www.altdevblogaday.com/2011/03/21/moving-beyond-the-linear-bezier/
     * 
     * http://devmaster.net/forums/topic/9796-ease-in-ease-out-algorithm/
     * 
     * suggestions taken from: http://www.reddit.com/r/gamedev/comments/iomn3/what_kinds_of_equations_do_you_find_useful_or/
     */
}