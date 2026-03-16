namespace Assets.Project.Scripts.System.DessertCreator.Dtos
{
    public readonly struct DessertCountsDto
    {
        public int TotalDessertsCount { get; }
        public int RemainingDessertsCount { get; }
        public int ActiveDessertsCount { get; }
        public int FieldDessertsCount { get; }

        public DessertCountsDto(
            int totalDessertsCount,
            int remainingDessertsCount,
            int activeDessertsCount,
            int fieldDessertsCount)
        {
            TotalDessertsCount = totalDessertsCount;
            RemainingDessertsCount = remainingDessertsCount;
            ActiveDessertsCount = activeDessertsCount;
            FieldDessertsCount = fieldDessertsCount;
        }
    }
}
