using Components;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;
using Random = Unity.Mathematics.Random;

namespace Systems
{
    /// <summary>
    /// Main game logic system, finds chains of bugs of one color and marks them with RemoveMarkComponent
    /// </summary>
    [UpdateAfter(typeof(FindNeighboorsSystem))]
    public class FindChainsSystem : SystemBase
    {
        private EntityCommandBufferSystem barrier;
        private int minimumChainSize = 2;
        private float timeSinceStart;
        
        protected override void OnCreate()
        {
            base.OnCreate();
            barrier = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
        }

        protected override void OnUpdate()
        {
            ComponentDataFromEntity<BugComponent> bugComponent = GetComponentDataFromEntity<BugComponent>();
            BufferFromEntity<EntityBufferElement> entityBuffer = GetBufferFromEntity<EntityBufferElement>();
        
            var time = Time.DeltaTime;
            timeSinceStart += time;
            var random = new Random((uint)(timeSinceStart * 1000f));
            
            //Recursive method of finding chains of one color, some kind of flood fill
            void FindChainRec(
                Entity entity,
                BugComponent ball,
                BugColor neededColor,
                DynamicBuffer<EntityBufferElement> adjacentEntities,
                NativeHashMap<Entity, bool> chainMap)
            {
                if (!chainMap.ContainsKey(entity) && ball.Color == neededColor)
                {
                    chainMap.Add(entity, true);
                }
            
                for (int i = 0; i < adjacentEntities.Length; i++)
                {
                    var adjacentEntity = adjacentEntities[i].Value;
                    var adjacentBall = bugComponent[adjacentEntity];
                    if (!chainMap.ContainsKey(adjacentEntity) && adjacentBall.Color == neededColor)
                    {
                        chainMap.Add(adjacentEntity, true);
                        var nextBall = bugComponent[adjacentEntity];
                        var nextAdjacent = entityBuffer[adjacentEntity];
                        FindChainRec(adjacentEntity, nextBall, neededColor, nextAdjacent, chainMap);
                    }
                }
            }

            var minChainSize = minimumChainSize;
            var commandBuffer = barrier.CreateCommandBuffer().ToConcurrent();
            Dependency = Entities.ForEach((Entity entity, int nativeThreadIndex, int entityInQueryIndex, 
                    in BugComponent bug, in DynamicBuffer<EntityBufferElement> adjacentEntities, in ChainMarkComponent mark) =>
                {
                    commandBuffer.RemoveComponent<ChainMarkComponent>(nativeThreadIndex, entity);
                    BugColor neededColor = mark.NeededColor;
                    
                    //Should be HashSet as we don't need value here, but there is no NativeHashSet at the moment
                    NativeHashMap<Entity, bool> chainMap = new NativeHashMap<Entity, bool>(100, Allocator.Temp);
                    FindChainRec(entity, bug, neededColor, adjacentEntities, chainMap);
                    var keys = chainMap.GetKeyArray(Allocator.Temp);
                    if (keys.Length >= minChainSize)
                    {
                        //Trigger spawning additional bugs
                        commandBuffer.AddComponent<SpawnMarkComponent>(nativeThreadIndex, entity);
                        commandBuffer.SetComponent(nativeThreadIndex, entity, new SpawnMarkComponent
                        {
                            Count = keys.Length
                        });

                        for (var i = 0; i < keys.Length; i++)
                        {
                            //Remove popped bugs with delay
                            commandBuffer.AddComponent<RemoveMarkComponent>(nativeThreadIndex, keys[i]);
                            commandBuffer.SetComponent(nativeThreadIndex, keys[i], new RemoveMarkComponent
                            {
                                DelayMs = random.NextInt(0, 20)
                            });
                        }
                    }
                    chainMap.Dispose();
                })
                .Schedule(Dependency);
        
            barrier.AddJobHandleForProducer(Dependency);
        }
    }
}