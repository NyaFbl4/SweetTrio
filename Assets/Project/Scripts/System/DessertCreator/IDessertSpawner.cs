using Assets.Project.Scripts.Desserts;

namespace Assets.Project.Scripts.System.DessertCreator
{
    public interface IDessertSpawner
    {
        void PrepareDeck();
        DessertController SpawnNext();
        void ClearDeck();
    }
}
