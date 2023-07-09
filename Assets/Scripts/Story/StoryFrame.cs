using System;
using UnityEngine;

namespace Story
{
    [Serializable]
    public struct StoryFrame
    {
        public Sprite[] animSprites;
        public float animSpeed;
        public bool playOnce;
        public string[] dialogLines;
    }
}