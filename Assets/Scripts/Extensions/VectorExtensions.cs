using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class VectorExtensions
{
    public static Vector3 ToVector3(this Vector2 v, float z)
    {
        return new Vector3(v.x, v.y, z);
    }
    
    public static Vector3 ToVector3(this Vector2 v)
    {
        return new Vector3(v.x, v.y, 0);
    }

    public static Vector3 SetX(this Vector3 v, float x)
    {
        v.x = x;
        return v;
    }
    
    public static Vector3 SetY(this Vector3 v, float y)
    {
        v.y = y;
        return v;
    }
    
    public static Vector3 SetZ(this Vector3 v, float z)
    {
        v.z = z;
        return v;
    }

    public static Vector2 xy(this Vector3 v)
    {
        return new Vector2(v.x, v.y);
    }
    
    public static Vector2 yz(this Vector3 v)
    {
        return new Vector2(v.y, v.z);
    }
    
    public static Vector2 xz(this Vector3 v)
    {
        return new Vector2(v.x, v.z);
    }
    
    public static Vector3 ClampMagnitude(this Vector3 v, float min, float max)	
    {	
        double sm = v.sqrMagnitude;	
        if(sm > max * max) return v.normalized * max;	
        if(sm < min * min) return v.normalized * min;	
        return v;	
    }	
    public static Vector3 Rotate(this Vector3 v, float angle)	
    {	
        return Quaternion.AngleAxis(angle, Vector3.up) * v;	
    }

    public static Vector2 Rotate(this Vector2 v, float degrees) {
        float sin = Mathf.Sin(degrees * Mathf.Deg2Rad);
        float cos = Mathf.Cos(degrees * Mathf.Deg2Rad);
         
        float tx = v.x;
        float ty = v.y;
        v.x = (cos * tx) - (sin * ty);
        v.y = (sin * tx) + (cos * ty);
        return v;
    }

    public static Vector3 Inverse(this Vector3 v)
    {
        return v / v.sqrMagnitude;
    }
}
