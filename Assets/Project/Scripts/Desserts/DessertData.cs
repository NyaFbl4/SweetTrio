namespace Assets.Project.Scripts.Desserts
{
    public class DessertData
    {
        private EDessertType _dessertType;
        public EDessertType DessertType => _dessertType;

        public DessertData(EDessertType dessertType)
        {
            _dessertType = dessertType;
        }
    }
}