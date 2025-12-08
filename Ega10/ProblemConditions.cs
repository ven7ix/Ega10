namespace Ega10
{
    internal class ProblemConditions(int applications, int machines, int[][] executionTimes, int[] dueTimes, int[] penaltyMultiplyers)
    {
        public int Applications { get; } = applications;
        public int Machines { get; } = machines;
        public int[][] ExecutionTimes { get; } = executionTimes;
        public int[] DueTimes { get; } = dueTimes;
        public int[] PenaltyMultiplyers { get; } = penaltyMultiplyers;
    }
}
