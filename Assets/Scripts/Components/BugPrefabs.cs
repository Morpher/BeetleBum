using Unity.Entities;

namespace Components
{
    /// <summary>
    /// Stores prefabs for spawning bugs
    /// </summary>
    [GenerateAuthoringComponent]
    struct BugPrefabs : IComponentData
    {
        public Entity BlueBug;
        public Entity GreenBug;
        public Entity PurpleBug;
        public Entity RedBug;
        public Entity YellowBug;
    }
}
