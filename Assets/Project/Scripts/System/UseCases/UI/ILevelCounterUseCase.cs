namespace Project.Scripts.UI.UseCases
{
    public interface ILevelCounterUseCase
    {
        int CurrentValue { get; }

        void SetValue(int value);
        void Increment(int amount = 1);
        void Decrement(int amount = 1);
        void Reset();
    }
}
