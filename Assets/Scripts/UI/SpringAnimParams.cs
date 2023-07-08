using UnityEngine;

namespace UI
{
    [CreateAssetMenu]
    public class SpringAnimParams : ScriptableObject
    {
        public float initialVelocity;
        public float damping;
        public float stiffness;
        public Vector3 positionModifier;
        public Vector3 rotationModifier;
        public Vector3 scaleModifier = Vector3.one;

        public SpringAnimParams GetCopy()
        {
            return (SpringAnimParams)MemberwiseClone();
        }
    }
}