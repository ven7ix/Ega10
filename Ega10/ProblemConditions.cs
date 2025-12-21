namespace Ega10
{
    internal class ProblemConditions(int applications, int machines, int[][] executionTimes, int[] dueTimes, int[] penaltyMultiplyers)
    {
        public int Applications { get; } = applications;
        public int Machines { get; } = machines;
        public int[][] ExecutionTimes { get; } = executionTimes;
        public int[] DueTimes { get; } = dueTimes;
        public int[] PenaltyMultiplyers { get; } = penaltyMultiplyers;

        public void Print()
        {
            Console.Clear();

            Console.WriteLine($"Applications: {Applications}");
            Console.WriteLine($"Machines: {Machines}");

            Console.WriteLine("Execution times:");
            for (int m = 0; m < ExecutionTimes.Length; m++)
            {
                for (int a = 0; a < ExecutionTimes[m].Length; a++)
                {
                    Console.Write($"{(ExecutionTimes[m][a] < 10 ? ' ' : string.Empty)}{ExecutionTimes[m][a]} ");
                }
                Console.WriteLine();
            }
            Console.WriteLine();

            Console.Write("Due times: ");
            for (int d = 0; d < DueTimes.Length; d++)
            {
                Console.Write($"{DueTimes[d]} ");
            }
            Console.WriteLine();

            Console.Write("Penalty multiplyers: ");
            for (int p = 0; p < PenaltyMultiplyers.Length; p++)
            {
                Console.Write($"{PenaltyMultiplyers[p]} ");
            }
            Console.WriteLine();

            Console.WriteLine("---------------------------------------------------------------------");
        }
    }
}
