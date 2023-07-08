using UnityEngine;

namespace UI
{
    [CreateAssetMenu()]
    public class AnimParams : ScriptableObject
    {
        public float duration;
        public AnimationCurve translateX;
        public AnimationCurve translateY;
        public AnimationCurve translateYAdditive;
        public AnimationCurve rotate;
        public AnimationCurve rotateAdditive;
        public AnimationCurve scaleX;
        public AnimationCurve scaleY;
        public AnimationCurve scaleZ;
        public AnimationCurve deltaScale;

        public AnimParams GetCopy()
        {
            return (AnimParams)MemberwiseClone();
        }
    }
}
