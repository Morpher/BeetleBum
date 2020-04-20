using Unity.Entities;

namespace Components
{
    /// <summary>
    /// Simply marks entity for removal
    /// </summary>
    [GenerateAuthoringComponent]
    public struct RemoveMarkComponent : IComponentData
    {
        public int DelayMs;
    }
}
