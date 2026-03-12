using Assets.Project.Scripts.Desserts;

namespace Project.System
{
    public interface IActionBar
    {
        void AddDessert(DessertController dessert);
        void ClearField();
    }
}