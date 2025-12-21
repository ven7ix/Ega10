namespace Ega10
{
    internal static class Tools
    {
        public static Random Random { get; } = new Random(0);
    }

    internal class Program
    {
        private static int Applications { get; } = 15;
        private static int Machines { get; } = 5;
        private static int[][] ExecutionTimes { get; } =
            [
            [ 3, 13,  7, 15,  5,  5,  3, 17, 13,  3,  1,  3,  4,  4,  9],
            [23,  9, 11,  8, 11, 22, 10, 15,  4,  6,  5,  5,  7,  9, 10],
            [ 7, 21,  9,  9,  7, 23,  6,  2,  8, 23, 23, 16,  1, 11,  6],
            [ 4, 15,  1,  1, 12,  1,  7, 24, 19, 24,  9, 12, 20, 11,  6],
            [ 5, 16,  1, 17, 23,  5,  7,  9,  4, 16, 22, 20, 16, 22,  2]
            ];
        private static int[] DueTimes { get; } = [20, 25, 30, 35, 40, 45, 50, 55, 60, 65, 70, 75, 80, 85, 90];
        private static int[] PenaltyMultiplyers { get; } = [1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1];

        public static void Main()
        {
            var problemConditions = new ProblemConditions(Applications, Machines, ExecutionTimes, DueTimes, PenaltyMultiplyers);
            problemConditions.Print();

            var solution = new Solution(problemConditions, 200, false);
            solution.SolveMaxIterarions(
                new CyclicChromosomeFactory(),
                new RandomControlledInitialPopalionGenerator(),
                new ParentPairsSelectorRandom(),
                new CyclicCrossoverOperator(),
                new RandomControlledMutator(0.1f, 0.5f),
                new CyclicModifyRestriction(new CyclicChromosomeFactory()),
                new EvaluatorCyclic(),
                new SelectorRandom(0.8f),
                new NewPopulationGeneratorDefault(),
                50);
        }
    }
}
