using Assets.Project.Scripts.Desserts;

namespace Assets.Project.Scripts.System.DessertCreator
{
    public interface IDessertSpawner
    {
        int TotalDessertsCount { get; }
        int RemainingDessertsCount { get; }
        int ActiveDessertsCount { get; }
        int FieldDessertsCount { get; }

        void PrepareDeck();
        DessertController SpawnNext();
        void RespawnFieldWithShuffle();
        void ClearDeck();
    }
}
