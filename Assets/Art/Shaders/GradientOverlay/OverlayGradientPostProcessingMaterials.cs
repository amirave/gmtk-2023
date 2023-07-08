using UnityEngine;

namespace Art.Shaders
{
    [System.Serializable, CreateAssetMenu(fileName = "OverlayGradientPostProcessingMaterials", menuName = "Game/OverlayGradientPostProcessingMaterials")]
    public class OverlayGradientPostProcessingMaterials : ScriptableObject
    {
        //---Your Materials---
        public Material material;
    
        //---Accessing the data from the Pass---
        static OverlayGradientPostProcessingMaterials _instance;

        public static OverlayGradientPostProcessingMaterials Instance
        {
            get
            {
                if (_instance != null) return _instance;
                // TODO check if application is quitting
                // and avoid loading if that is the case

                _instance = Resources.Load("OverlayGradientPostProcessingMaterials") as OverlayGradientPostProcessingMaterials;
                return _instance;
            }
        }
    }
}