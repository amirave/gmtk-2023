using System;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace Art.Shaders
{
    [Serializable, VolumeComponentMenuForRenderPipeline("Custom/OverlayGradient", typeof(UniversalRenderPipeline))]
    public class OverlayGradientEffect : VolumeComponent, IPostProcessComponent
    {
        public NoInterpColorParameter topLeftColor = new (Color.clear);
        public NoInterpColorParameter topRightColor = new (new Color(251, 86, 7));
        public NoInterpColorParameter bottomLeftColor = new (Color.clear);
        public NoInterpColorParameter bottomRightColor = new (new Color(58, 134, 255));
        
        public ClampedFloatParameter radius = new ClampedFloatParameter(value: 0, min: 0, max: 2, overrideState: true);
        public ClampedFloatParameter feather = new ClampedFloatParameter(value: 0, min: 0, max: 20, overrideState: true);
        
        public bool IsActive() => radius.value > 0;

        public bool IsTileCompatible() => true;
    }
}