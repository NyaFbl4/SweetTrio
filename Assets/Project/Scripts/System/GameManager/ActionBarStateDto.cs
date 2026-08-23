namespace Project.Scripts.GameManager
{
    public class ActionBarStateDto
    {
        public int CurrentCount { get; }

        public ActionBarStateDto(int currentCount)
        {
            CurrentCount = currentCount;
        }
    }
}