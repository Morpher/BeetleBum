using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Physics.Systems;
using Unity.Transforms;

namespace Systems
{
    /// <summary>
    /// As there is no 2d DOTS physics, here we force 3d physics to become 2d
    /// </summary>
    [UpdateAfter(typeof(ExportPhysicsWorld))]
    public class PhysicConstraintsSystem : JobComponentSystem
    {
        protected override JobHandle OnUpdate(JobHandle inputDeps)
        {
            return Entities.WithBurst().ForEach((ref PhysicsVelocity velocity, ref Translation position, ref Rotation rotation) =>
                {
                    velocity.Linear = new float3(velocity.Linear.x, velocity.Linear.y, 0);
                    velocity.Angular = new float3(0, 0, velocity.Angular.z);
                    position.Value = new float3(position.Value.x, position.Value.y, 0);
                    rotation.Value = quaternion.Euler(0, 0, rotation.Value.ToEuler().z);
                })
                .Schedule(inputDeps);
        }
    }

    public static class QuaternionExtensions
    {
        /// <summary>
        /// Converts quaternion representation to euler
        /// </summary>
        public static float3 ToEuler(this quaternion quaternion) {
            float4 q = quaternion.value;
            double3 res;
 
            double sinr_cosp = +2.0 * (q.w * q.x + q.y * q.z);
            double cosr_cosp = +1.0 - 2.0 * (q.x * q.x + q.y * q.y);
            res.x = math.atan2(sinr_cosp, cosr_cosp);
 
            double sinp = +2.0 * (q.w * q.y - q.z * q.x);
            if (math.abs(sinp) >= 1) {
                res.y = math.PI / 2 * math.sign(sinp);
            } else {
                res.y = math.asin(sinp);
            }
 
            double siny_cosp = +2.0 * (q.w * q.z + q.x * q.y);
            double cosy_cosp = +1.0 - 2.0 * (q.y * q.y + q.z * q.z);
            res.z = math.atan2(siny_cosp, cosy_cosp);
 
            return (float3) res;
        }
    }
}