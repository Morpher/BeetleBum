using System.Collections;
using UnityEngine;

namespace Utilities
{
    public class DestroyAfterDelay : MonoBehaviour
    {
        [SerializeField] 
        private float delaySec = 1f; 
        
        private IEnumerator Start()
        {
            yield return new WaitForSeconds(delaySec);
            Destroy(gameObject);
        }
    }
}
