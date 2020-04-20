using Unity.Entities;

namespace Components
{
    /// <summary>
    /// Adding this component triggers FindChainsSystem to remove all adjacent bugs of one color
    /// </summary>
    [GenerateAuthoringComponent]
    public struct ChainMarkComponent : IComponentData
    {
        public BugColor NeededColor;
    }
}
