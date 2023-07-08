using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fish;

public class Sardine : FishAI
{
    private void Start()
    {
        rb.velocity = transform.rotation * Vector3.right;
    }
}
