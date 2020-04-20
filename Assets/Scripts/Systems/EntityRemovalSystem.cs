using Components;
using Unity.Entities;
using Unity.Jobs;

namespace Systems
{
    /// <summary>
    /// System responsible for removing entities, which were marked with RemoveMarkComponent
    /// </summary>
    public class EntityRemovalSystem : JobComponentSystem
    {
        private EntityCommandBufferSystem barrier;
    
        protected override void OnCreate()
        {
            base.OnCreate();
            barrier = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
        }
    
        protected override JobHandle OnUpdate(JobHandle inputDeps)
        {
            var deltaTimeMs = Time.DeltaTime * 1000f;
            var commandBuffer = barrier.CreateCommandBuffer().ToConcurrent();
            inputDeps = Entities.WithBurst().ForEach((Entity entity, int nativeThreadIndex, ref RemoveMarkComponent removeMark) =>
                {
                    removeMark.DelayMs -= (int) deltaTimeMs;
                    if (removeMark.DelayMs <= 0)
                    {
                        commandBuffer.DestroyEntity(nativeThreadIndex, entity);                        
                    }
                })
                .Schedule(inputDeps);
        
            barrier.AddJobHandleForProducer(inputDeps);
            return inputDeps;
        }
    }
}