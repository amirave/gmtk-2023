using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationTest : MonoBehaviour
{
    private Quaternion[] quats = new[]
    {
        new Quaternion(0.0f, 0.0f, 0.0f, 1.0f),
        new Quaternion(-0.09314386546611786f, -0.042154453694820404f, 0.015463929623365402f, 0.9946397542953491f),
        new Quaternion(0.11648590117692947f, -0.008897109888494015f, -0.0786137729883194f, 0.9900363683700562f),
        new Quaternion(0.2157028466463089f, 0.05324333533644676f, 0.04998594522476196f, 0.973724365234375f),
        new Quaternion(0.0f, 0.0f, 0.0f, 1.0f),
        new Quaternion(-0.12351036816835403f, -0.009336305782198906f, -0.015379211865365505f, 0.9921802282333374f),
        new Quaternion(0.12067223340272903f, -0.023375604301691055f, 0.04902255907654762f, 0.991205632686615f),
        new Quaternion(0.20697017014026642f, 0.1070951521396637f, -0.10073521733283997f, 0.9672366976737976f),
        new Quaternion(0.022906487807631493f, -0.00426446832716465f, 0.059047698974609375f, 0.9979832768440247f),
        new Quaternion(-0.0030147223733365536f, -0.021120421588420868f, -0.02060694992542267f, 0.9995599985122681f),
        new Quaternion(0.002597096608951688f, -0.0014679576270282269f, -0.04923492297530174f, 0.9987828135490417f),
        new Quaternion(0.03202705830335617f, 0.01729159988462925f, -0.04051540791988373f, 0.9985159039497375f),
        new Quaternion(-0.017618224024772644f, 0.039132971316576004f, 0.03035151958465576f, 0.9986175298690796f),
        new Quaternion(-0.007865432649850845f, -0.09177303314208984f, 0.03659495711326599f, 0.9950762987136841f)
    };

    private string[] names = new[]
    {
        "Main Controller",
        "Bone",
        "Shoulder L",
        "Arm L",
        "Bone.003",
        "Bone.004",
        "Shoulder R",
        "Arm R",
        "Leg L",
        "Foot L",
        "Toe L",
        "Leg R",
        "Foot R",
        "Toe R"
    };

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.H) || Input.GetKey(KeyCode.J))
        {
            for (int i = 0; i < names.Length; i++)
            {
                GameObject.Find(names[i]).transform.localRotation *= quats[i];
            }
        }
    }
}
