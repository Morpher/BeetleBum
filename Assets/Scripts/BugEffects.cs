using Cinemachine;
using Components;
using UnityEngine;

[RequireComponent(typeof(CinemachineImpulseSource))]
//Generates pop effect
public class BugEffects : MonoBehaviour
{
    [SerializeField]
    private Transform popEffectPrefab;
    private CinemachineImpulseSource impulseSource;
    
    void Start()
    {
        impulseSource = GetComponent<CinemachineImpulseSource>();
    }

    public void GeneratePopEffect(float x, float y, BugColor bugColor)
    {
        impulseSource.GenerateImpulse();
        if (popEffectPrefab)
        {
            var effect = Instantiate(popEffectPrefab, new Vector3(x, y, 0), Quaternion.identity);
            var particles = effect.GetChild(0)?.GetComponent<ParticleSystem>();
            if (particles)
            {
                var color = Color.white;
                switch (bugColor)
                {
                    case BugColor.Blue:
                        color =  new Color(0.0f, 0.3f, 1f, 1f);
                        break;
                    case BugColor.Green:
                        color = Color.green;
                        break;
                    case BugColor.Purple:
                        color = Color.magenta;
                        break;
                    case BugColor.Red:
                        color = Color.red;
                        break;
                    case BugColor.Yellow:
                        color = Color.yellow;
                        break;
                }
                var main = particles.main;
                main.startColor = new ParticleSystem.MinMaxGradient(color);
            }
        }
    }
}
