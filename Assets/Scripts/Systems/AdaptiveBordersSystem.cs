using Components;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Physics.Systems;
using Unity.Transforms;
using UnityEngine;
using BoxCollider = Unity.Physics.BoxCollider;

namespace Systems
{
    /// <summary>
    /// Aligns level borders with screen borders
    /// </summary>
    [UpdateBefore(typeof(BuildPhysicsWorld))]
    public class AdaptiveBordersSystem : JobComponentSystem
    {
        [BurstCompile]
        private struct AlignCollidersWithScreenJob : IJobForEach<PhysicsCollider, LevelBorderComponent, Translation>
        {
            [ReadOnly] public Rect ScreenRect;

            public unsafe void Execute(ref PhysicsCollider collider, ref LevelBorderComponent border,
                ref Translation translation)
            {
                // make sure we are dealing with boxes
                if (collider.ColliderPtr->Type != ColliderType.Box)
                {
                    return;
                }

                translation.Value = float3.zero;
                // tweak the physical representation of the box
                // grab the box pointer
                BoxCollider* scPtr = (BoxCollider*) collider.ColliderPtr;
                var box = scPtr->Geometry;
                var minSize = 0.1f;
                switch (border.Position)
                {
                    case BorderPosition.Left:
                        box.Center = new float3(ScreenRect.xMin, ScreenRect.center.y, 0);
                        box.Size = new float3(minSize, ScreenRect.height, minSize);
                        break;
                    case BorderPosition.Right:
                        box.Center = new float3(ScreenRect.xMax, ScreenRect.center.y, 0);
                        box.Size = new float3(minSize, ScreenRect.height, minSize);
                        break;
                    case BorderPosition.Top:
                        box.Center = new float3(ScreenRect.center.x, ScreenRect.yMax, 0);
                        box.Size = new float3(ScreenRect.width, minSize, minSize);
                        break;
                    case BorderPosition.Bottom:
                        box.Center = new float3(ScreenRect.center.x, ScreenRect.yMin, 0);
                        box.Size = new float3(ScreenRect.width, minSize, minSize);
                        break;
                }

                // update the collider geometry
                box.Orientation = quaternion.identity;
                box.BevelRadius = 0;
                scPtr->Geometry = box;
            }
        }

        protected override JobHandle OnUpdate(JobHandle inputDeps)
        {
            var camera = Camera.main;
            var min = camera.ViewportToWorldPoint(new Vector2(0, 0));
            var max = camera.ViewportToWorldPoint(new Vector2(1, 1));
            var screenRect = new Rect
            {
                min = min,
                max = max
            };
            var job = new AlignCollidersWithScreenJob()
            {
                ScreenRect = screenRect
            }.Schedule(this, inputDeps);
            return job;
        }
    }
}

