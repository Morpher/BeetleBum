using Unity.Entities;

namespace Components
{
    /// <summary>
    /// Main component of the bugs game
    /// </summary>
    [GenerateAuthoringComponent]
    public struct BugComponent : IComponentData
    {
        public BugColor Color;
        public float Radius;
    }

    /// <summary>
    /// Color of the bug
    /// </summary>
    public enum BugColor
    {
        Blue,
        Green,
        Purple,
        Red,
        Yellow
    }
}