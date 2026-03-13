using Assets.Project.Scripts.Desserts;

namespace Assets.Project.Scripts.System.DessertCreator
{
    public interface IDessertSpawner
    {
        void PreparePool(int copiesPerDessert);
        DessertController SpawnByIndex(int index);
    }
}
