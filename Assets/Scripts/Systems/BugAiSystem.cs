using Components;
using Unity.Entities;
using Unity.Jobs;
using Unity.Physics;
using Random = Unity.Mathematics.Random;

namespace Systems
{
    /// <summary>
    /// Simple AI system for bugs to make them look more alive
    /// </summary>
    public class BugAiSystem : JobComponentSystem
    {
        private float timeSinceStart;
    
        protected override JobHandle OnUpdate(JobHandle inputDeps)
        {
            var time = Time.DeltaTime;
            timeSinceStart += time;
            var random = new Random((uint)(timeSinceStart * 1000f));
        
            return Entities.WithBurst().ForEach((ref BugAiComponent ai, ref PhysicsVelocity velocity, in BugComponent bug) =>
                {
                    ai.TimeTillNextTurn -= time;
                    if (ai.TimeTillNextTurn <= 0)
                    {
                        velocity.Angular = random.NextFloat3(ai.RotationRange.x, ai.RotationRange.y);
                        velocity.Angular = random.NextBool() ? velocity.Angular : -velocity.Angular;
                        ai.TimeTillNextTurn = random.NextFloat(ai.IntervalRangeSec.x, ai.IntervalRangeSec.y);
                    }
                })
                .Schedule(inputDeps);
        }
    }
}