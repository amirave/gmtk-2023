using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotationTest : MonoBehaviour
{
    public Transform obj;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawRay(transform.position, transform.forward * 4);
        
        Gizmos.color = Color.blue;
        var dir = obj.forward;
        Gizmos.DrawRay(transform.position, dir * 4);
        
        Gizmos.color = Color.green;
        var newDir = Quaternion.LookRotation(transform.forward);
        var x = Utils.ShortestRotation(newDir, Quaternion.LookRotation(dir));
        var i = 0;
        while (i < 50 && Quaternion.Angle(newDir, Quaternion.LookRotation(dir)) > 0.01f)
        {
            i++;
            newDir = Quaternion.RotateTowards(newDir, x, 5);
            newDir.ToAngleAxis(out var angle, out var axis);
            Gizmos.DrawRay(transform.position, newDir * transform.forward * angle);
        }
    }
}
