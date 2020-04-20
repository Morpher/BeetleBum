using Unity.Entities;

namespace Components
{
    /// <summary>
    /// Level border component
    /// </summary>
    [GenerateAuthoringComponent]
    public struct LevelBorderComponent : IComponentData
    {
        public BorderPosition Position;
    }

    public enum BorderPosition
    {
        Left,
        Right,
        Top,
        Bottom
    }
}