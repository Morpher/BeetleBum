using System.Threading.Tasks;
using UnityEngine;

namespace Utilities
{
    /// <summary>
    /// Simple fps counter
    /// </summary>
    public class FpsCounter : MonoBehaviour 
    {
        private string label = "";
	
        private async void Start()
        {
            GUI.depth = 2;
            while (true) 
            {
                var fps = 1 / Time.unscaledDeltaTime;
                label = $"FPS : {Mathf.Round (fps)}";
                await Task.Delay(500);
            }
        }
	
        private void OnGUI()
        {
            GUI.Label(new Rect (Screen.width - 105, 5, 100, 25), label);
        }
    }
}