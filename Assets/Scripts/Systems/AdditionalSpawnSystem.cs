using Components;
using Unity.Entities;
using UnityEngine;
using Random = Unity.Mathematics.Random;

namespace Systems
{
    /// <summary>
    /// Spawn additional bugs to replace removed ones
    /// </summary>
    [UpdateAfter(typeof(FindChainsSystem))]
    public class AdditionalSpawnSystem : SystemBase
    {
        private EntityCommandBufferSystem barrier;
        private BugSpawner spawner;
        private float timeSinceStart;
        
        protected override void OnCreate()
        {
            base.OnCreate();
            barrier = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
        
            //TODO: inject the dependency with DI framework
            spawner = GameObject.FindObjectOfType<BugSpawner>();
        }
        
        protected override void OnUpdate()
        {
            var time = Time.DeltaTime;
            timeSinceStart += time;
            var random = new Random((uint)(timeSinceStart * 1000f));

            var spawner = this.spawner;
            var commandBuffer = barrier.CreateCommandBuffer();
            Entities.WithoutBurst().ForEach((Entity entity, int nativeThreadIndex, int entityInQueryIndex, 
                    in SpawnMarkComponent mark) =>
                {
                    spawner.Spawn(mark.Count, random.NextInt(500, 5000));
                    commandBuffer.RemoveComponent<SpawnMarkComponent>(entity);
                })
                .Run();
        
            barrier.AddJobHandleForProducer(Dependency);
        }
    }
}