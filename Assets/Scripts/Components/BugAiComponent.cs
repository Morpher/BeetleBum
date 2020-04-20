using Unity.Entities;
using Unity.Mathematics;

namespace Components
{
    /// <summary>
    /// Simple bugs AI component
    /// </summary>
    [GenerateAuthoringComponent]
    public struct BugAiComponent : IComponentData
    {
        public float TimeTillNextTurn;
        public float2 IntervalRangeSec;
        public float2 RotationRange;
    }
}