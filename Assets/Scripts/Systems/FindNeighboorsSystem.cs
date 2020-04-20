using Components;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Physics;
using Unity.Physics.Systems;
using Unity.Transforms;

namespace Systems
{
    /// <summary>
    /// Find adjacent bugs and write them to a DynamicBuffer
    /// </summary>
    [UpdateBefore(typeof(EndFramePhysicsSystem)), UpdateAfter(typeof(StepPhysicsWorld))]
    public class FindNeighboorsSystem : JobComponentSystem
    {
        [ReadOnly] BuildPhysicsWorld buildPhysicsWorld; 
        [ReadOnly] StepPhysicsWorld stepPhysicsWorld;
    
        private enum Layer
        {
            Bug = (1 << 0),
            Obstacle = (1 << 1),
        };

        private static CollisionFilter LayerFilter(Layer layer, Layer disabled)
        {
            return new CollisionFilter
            {
                BelongsTo = (uint)layer,
                CollidesWith = ~(uint)disabled,
                GroupIndex = 0
            };
        }

        protected override void OnCreate()
        {
            base.OnCreate();
            buildPhysicsWorld = World.GetOrCreateSystem<BuildPhysicsWorld>();
            stepPhysicsWorld = World.GetOrCreateSystem<StepPhysicsWorld>();
        }
    
        protected override JobHandle OnUpdate(JobHandle inputDeps)
        {
            PhysicsWorld physicsWorld = buildPhysicsWorld.PhysicsWorld;
            inputDeps = JobHandle.CombineDependencies(inputDeps, buildPhysicsWorld.FinalJobHandle);
            inputDeps = JobHandle.CombineDependencies(inputDeps, stepPhysicsWorld.FinalJobHandle);
        
            var jobHandle = Entities.ForEach((ref DynamicBuffer<EntityBufferElement> adjacentEntities, 
                    in Translation position, in BugComponent bug) =>
                {
                    NativeList<DistanceHit> distanceHits = new NativeList<DistanceHit>(Allocator.Temp);
                    var pointDistanceInput = new PointDistanceInput
                    {
                        Position = position.Value,
                        MaxDistance = bug.Radius,
                        Filter = LayerFilter(Layer.Bug, Layer.Obstacle)
                    };

                    physicsWorld.CalculateDistance(pointDistanceInput, ref distanceHits);
                    adjacentEntities.Clear();
                    var adjacent = adjacentEntities.Reinterpret<Entity>();
                    for (int i = 0; i < distanceHits.Length; i++)
                    { 
                        adjacent.Add(distanceHits[i].Entity);
                    }
                    
                    distanceHits.Dispose();
                })
                .Schedule(inputDeps);

            jobHandle.Complete();
            return jobHandle;
        }
    }
}