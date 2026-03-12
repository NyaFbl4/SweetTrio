using Assets.Project.Scripts.Desserts;

namespace Project.System
{
    public interface IActionBar
    {
        bool TryAddDessert(DessertController dessert);
        void ClearField();
    }
}