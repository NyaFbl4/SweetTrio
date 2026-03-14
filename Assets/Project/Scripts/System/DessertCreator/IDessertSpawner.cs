using Assets.Project.Scripts.Desserts;

namespace Assets.Project.Scripts.System.DessertCreator
{
    public interface IDessertSpawner
    {
        void PrepareDeck(int copiesPerDessert);
        DessertController SpawnNext();
    }
}
