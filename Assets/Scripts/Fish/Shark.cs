using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Fish;
using DefaultNamespace;

public class Shark : FishAI
{
    private void Start()
    {
        rb.velocity = new Vector3(1, 0, 0);
    }
    // Update is called once per frame
    void Update()
    {
        rb.velocity = Quaternion.Euler(0, 0, Vector3.Angle(transform.position, transform.position)) * rb.velocity;
    }
}
