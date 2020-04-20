using Unity.Entities;

namespace Components
{
    /// <summary>
    /// Buffer for storing adjacent entities
    /// </summary>
    [GenerateAuthoringComponent]
    [InternalBufferCapacity(10)]
    public struct EntityBufferElement: IBufferElementData
    {
        public Entity Value;
    }
}
