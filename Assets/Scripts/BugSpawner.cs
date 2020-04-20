using System;
using System.Threading.Tasks;
using Components;
using Reese.Spawning;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Transforms;
using UnityEngine;
using Random = UnityEngine.Random;

/// <summary>
/// Monobehaviour responsible for spawning bugs from entity prefabs
/// Based on this https://reeseschultz.com/spawning-prefabs-with-unity-ecs/
/// </summary>
public class BugSpawner : MonoBehaviour
{
    [SerializeField] 
    private int count = 50;

    [SerializeField] 
    private int delayMs = 200;

    [SerializeField] 
    private Vector2 viewportSpawnPosition = new Vector2(0.5f, 1f);

    [SerializeField] 
    private float spawnRadius = 0.5f;

    [SerializeField] 
    private Vector2 minLinearVelocity = new Vector2(2, -10);

    [SerializeField] 
    private Vector2 maxLinearVelocity = new Vector2(5, -8);

    [SerializeField] 
    private Vector2 angularVelocityRange = new Vector2(-10, 10);

    [SerializeField] 
    private Vector2 bugMovementIntervalRange = new Vector2(2, 7);
    
    [SerializeField] 
    private Vector2 bugAngularRotationRange = new Vector2(10, 20);
        
    [SerializeField] 
    private Camera cam;

    private BugPrefabs prefabs;
    private Entity[] entityPrefabs;
    
    // Get the default world containing all entities:
    private EntityManager EntityManager => World
        .DefaultGameObjectInjectionWorld
        .EntityManager;

    private void Start()
    {
        if (cam == null)
        {
            cam = Camera.main;
        }
        
        // Get the entity associated with the prefab:
        prefabs = EntityManager
            .CreateEntityQuery(typeof(BugPrefabs))
            .GetSingleton<BugPrefabs>();

        entityPrefabs = new[]
        {
            prefabs.BlueBug,
            prefabs.GreenBug,
            prefabs.PurpleBug,
            prefabs.RedBug,
            prefabs.YellowBug,
        };

        var initialSpawnCount = count * cam.aspect;
        Spawn((int)initialSpawnCount);
    }
    
    public void Spawn()
    {
        RunSpawnAsync(count, delayMs, entityPrefabs);          
    }

    public void Spawn(int count)
    {
        RunSpawnAsync(count, delayMs, entityPrefabs);          
    }
    
    public void Spawn(int count, int delayMs)
    {
        RunSpawnAsync(count, delayMs, entityPrefabs);       
    }
    
    private async void RunSpawnAsync(int count, int delayMs, Entity[] entityPrefas)
    {
        try
        {
            await SpawnRandomAsync(count, delayMs, entityPrefabs);
        }
        catch (Exception e)
        {
            Debug.LogException(e);
        }
    }
    
    private async Task SpawnRandomAsync(int count, int delayMs, Entity[] entityPrefas)
    {
        for (int i = 0; i < count; i++)
        {
            // Enqueue spawning (SpawnSystem and Spawn are from Reese.Spawning):
            SpawnSystem.Enqueue(MakeRandomSpawn(entityPrefas));           
            await Task.Delay(delayMs);            
        }
    }

    private Spawn MakeRandomSpawn(Entity[] entityPrefas)
    {
        var index = Random.Range(0, entityPrefas.Length);
        var position = cam.ViewportToWorldPoint(viewportSpawnPosition) + Random.insideUnitSphere * spawnRadius;
        var rotation = Random.rotation;
        var angularVelocity = Random.Range(Mathf.Min(angularVelocityRange.x, angularVelocityRange.y), Mathf.Max(angularVelocityRange.x, angularVelocityRange.y));
        var linearVelocity = GetRandomRange(minLinearVelocity, maxLinearVelocity);

        var spawn = new Spawn()
            .WithPrefab(entityPrefas[index]) //  Optional prefab entity.
            .WithComponentList(
                new Translation {Value = new float3(position.x, position.y, 0)},
                new Rotation {Value = new quaternion(rotation.x, rotation.y, rotation.z, rotation.w)},
                new PhysicsVelocity
                {
                    Angular = new float3(0, 0, angularVelocity),
                    Linear = new float3(linearVelocity.x, linearVelocity.y, 0)
                },
                new BugAiComponent
                {
                    IntervalRangeSec = new float2(bugMovementIntervalRange.x, bugMovementIntervalRange.y), 
                    RotationRange = new float2(bugAngularRotationRange.x, bugAngularRotationRange.y)
                })
            .WithBufferList( // Optional comma-delimited list of IBufferElementData.
                new EntityBufferElement() { }
            );
        return spawn;
    }
    
    private Vector2 GetRandomRange(Vector2 a, Vector2 b)
    {
        var t = Random.value;
        return Vector2.Lerp(a, b, t);
    }
}
