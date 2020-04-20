using Unity.Entities;
using Unity.Mathematics;

namespace Components
{
    /// <summary>
    /// Spawn additional bugs
    /// </summary>
    [GenerateAuthoringComponent]
    public struct SpawnMarkComponent : IComponentData
    {
        public int Count;
    }
}