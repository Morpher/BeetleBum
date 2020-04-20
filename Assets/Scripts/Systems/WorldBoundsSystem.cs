using Components;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Physics.Systems;
using Unity.Transforms;
using UnityEngine;

namespace Systems
{
    /// <summary>
    /// Entities, that are outside of the world bounds should be marked to destroy 
    /// </summary>
    [UpdateBefore(typeof(EndFramePhysicsSystem)), UpdateAfter(typeof(StepPhysicsWorld))]
    public class WorldBoundsSystem : JobComponentSystem
    {
        [ReadOnly]
        private Rect worldBounds = new Rect
        {
            min = new Vector2(-20, -10),
            max = new Vector2(20, 20),
        };
    
        private EntityCommandBufferSystem barrier;
    
        protected override void OnCreate()
        {
            base.OnCreate();
            barrier = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
        }
    
        protected override JobHandle OnUpdate(JobHandle inputDeps)
        {
            var commandBuffer = barrier.CreateCommandBuffer().ToConcurrent();
            var bounds = worldBounds;
            inputDeps = Entities.WithBurst().ForEach((Entity entity, int nativeThreadIndex, in Translation position) =>
                {
                    if (!bounds.Contains(position.Value))
                    {
                        commandBuffer.AddComponent<RemoveMarkComponent>(nativeThreadIndex, entity);                    
                    }
                })
                .Schedule(inputDeps);
        
            barrier.AddJobHandleForProducer(inputDeps);
            return inputDeps;
        }
    }
}