namespace Project.Scripts.UI.UseCases
{
    public interface IDessertCountUseCase
    {
        int CurrentValue { get; }
        void Refresh();
        void Reset();
    }
}
