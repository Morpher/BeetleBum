using Components;
using Unity.Entities;
using Unity.Transforms;
using UnityEngine;
using Random = Unity.Mathematics.Random;

namespace Systems
{
    /// <summary>
    /// Add screenshake + particles on bug removal
    /// </summary>
    [UpdateBefore(typeof(EntityRemovalSystem))]
    public class BugEffectsSystem : SystemBase
    {
        //private float timeSinceStart;
        private BugEffects effectsManager;
        
        protected override void OnCreate()
        {
            base.OnCreate();
            //TODO: somehow inject the dependency
            effectsManager = GameObject.FindObjectOfType<BugEffects>();
        }
        
        protected override void OnUpdate()
        {
            // var time = Time.DeltaTime;
            // timeSinceStart += time;
            // var random = new Random((uint)(timeSinceStart * 1000f));
            var effectsManager = this.effectsManager;
            Entities.WithoutBurst().ForEach((in RemoveMarkComponent removeMark, in Translation position, in BugComponent bug) =>
                {
                    effectsManager.GeneratePopEffect(position.Value.x, position.Value.y, bug.Color);
                })
                .Run();
        }
    }
}