using Cinemachine;
using Components;
using Unity.Entities;
using Unity.Physics;
using Unity.Physics.Systems;
using UnityEngine;
using RaycastHit = Unity.Physics.RaycastHit;

/// <summary>
/// Mark clicked bugs with special component ChainMarkComponent, which triggers FindChainsSystem
/// </summary>
public class Input : MonoBehaviour
{
    private static readonly int RAYCAST_DISTANCE = 1000;
    
    [SerializeField]
    private Camera cam;
    private PhysicsWorld PhysicsWorld => World.DefaultGameObjectInjectionWorld.GetExistingSystem<BuildPhysicsWorld>().PhysicsWorld;
    private EntityManager EntityManager => World.DefaultGameObjectInjectionWorld.EntityManager;

    private void Awake()
    {
        if (cam == null)
        {
            cam = Camera.main;
        }
    }

    void LateUpdate()
    {
        if (UnityEngine.Input.GetMouseButtonDown(0) && cam)
        {
            var screenPointToRay = cam.ScreenPointToRay(UnityEngine.Input.mousePosition);
            var rayInput = new RaycastInput
            {
                Start = screenPointToRay.origin,
                End = screenPointToRay.GetPoint(RAYCAST_DISTANCE),
                Filter = CollisionFilter.Default
            };

            if (PhysicsWorld.CastRay(rayInput, out RaycastHit hit))
            {
                MarkSelected(hit);
            }
        }
    }

    private void MarkSelected(RaycastHit hit)
    {
        var selectedEntity = PhysicsWorld.Bodies[hit.RigidBodyIndex].Entity;
        var ballComponent = EntityManager.HasComponent<BugComponent>(selectedEntity) ? EntityManager.GetComponentData<BugComponent>(selectedEntity) : new BugComponent();
        EntityManager.AddComponent<ChainMarkComponent>(selectedEntity);
        EntityManager.SetComponentData(selectedEntity, new ChainMarkComponent { NeededColor = ballComponent.Color});
    }
}