using Assets.Project.Scripts.Desserts;

namespace Assets.Project.Scripts.System.DessertCreator
{
    public interface IDessertSpawner
    {
        int RemainingDessertsCount { get; }
        int ActiveDessertsCount { get; }

        void PrepareDeck();
        DessertController SpawnNext();
        void ClearDeck();
    }
}
