namespace Project.Scripts.UI.UseCases
{
    public interface ITimerCountdownUseCase
    {
        float RemainingSeconds { get; }
        void Reset(float seconds);
        void SubtractSeconds(float seconds);
    }
}
