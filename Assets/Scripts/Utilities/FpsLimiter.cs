using UnityEngine;

namespace Utilities
{
    /// <summary>
    /// Currently, DOTS physics speed depends on FPS, so we have to limit it. Otherwise, it will run extremely fast at ~500 fps
    /// </summary>
    public class FpsLimiter : MonoBehaviour
    {
        [SerializeField]
        private int targetFramerate = 60;
    
        private void Awake()
        {
            Application.targetFrameRate = targetFramerate;
        }
    }
}
