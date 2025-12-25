namespace Ega10
{
    internal static class Tools
    {
        public static Random Random { get; } = new Random();
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
            var solutionElite = new Solution(problemConditions, 200, true);

            int choice = 3;

            switch (choice)
            {
                case 0:
                {
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
                    break;
                }
                case 1:
                {
                    solution.SolveMaxIterarions(
                        new OrdinalChromosomeFactory(true),
                        new HeuristicInitialPopalionGenerator(problemConditions),
                        new ParentPairsSelectorOutbreeding(40),
                        new OrdinalCrossoverOperator(),
                        new RandomControlledMutator(0.1f, 0.5f),
                        new EliminateRestriction(),
                        new EvaluatorOrdinal(),
                        new SelectorRandom(0.9f),
                        new NewPopulationGeneratorDefault(),
                        2000);
                    break;
                }
                case 2:
                {
                    solutionElite.SolveMaxIterarions(
                        new CyclicChromosomeFactory(),
                        new HeuristicInitialPopalionGenerator(problemConditions),
                        new ParentPairsSelectorInbreeding(50),
                        new CyclicCrossoverOperator(),
                        new CyclicComplementaryMutator(0.05f, 0.4f),
                        new CyclicModifyRestriction(new CyclicChromosomeFactory()),
                        new EvaluatorCyclic(),
                        new SelectorRandom(0.75f),
                        new NewPopulationGeneratorDefault(),
                        50);
                    break;
                }
                case 3:
                {
                    solutionElite.SolveMaxIterarions(
                        new OrdinalChromosomeFactory(true),
                        new RandomControlledInitialPopalionGenerator(),
                        new ParentPairsSelectorRandom(),
                        new OrdinalCrossoverOperator(),
                        new RandomControlledMutator(0.05f, 0.3f),
                        new EliminateRestriction(),
                        new EvaluatorOrdinal(),
                        new SelectorBetaTournament(2),
                        new NewPopulationGeneratorDefault(),
                        50);
                    break;
                }
            }
        }
    }
}
