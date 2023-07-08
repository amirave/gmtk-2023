using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fish;

public class SardineAI : FishAI
{
    private void Start()
    {
        rb.velocity = transform.rotation * new Vector3(1, 0);
    }
}
