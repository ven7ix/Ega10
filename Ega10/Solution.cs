using static Ega10.Tools;

namespace Ega10
{
    internal static class Solution
    {
        private static int Applications { get; } = 15;
        private static int Machines { get; } = 5;
        private static int[] PenaltyMultiplyers { get; } = [1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1];
        private static int[][] ExecutionTimes { get; } =
            [
            [ 3, 13,  7, 15,  5,  5,  3, 17, 13,  3,  1,  3,  4,  4,  9],
            [23,  9, 11,  8, 11, 22, 10, 15,  4,  6,  5,  5,  7,  9, 10],
            [ 7, 21,  9,  9,  7, 23,  6,  2,  8, 23, 23, 16,  1, 11,  6],
            [ 4, 15,  1,  1, 12,  1,  7, 24, 19, 24,  9, 12, 20, 11,  6],
            [ 5, 16,  1, 17, 23,  5,  7,  9,  4, 16, 22, 20, 16, 22,  2]
            ];
        private static int[] DueTimes { get; } = [20, 25, 30, 35, 40, 45, 50, 55, 60, 65, 70, 75, 80, 85, 90];

        private static int PopulationSize { get; } = 200;
        private static List<IApplicant> Population { get; set; } = [];
        private static EvaluatedApplicant Best { get; set; } = new([], int.MaxValue);


        public static void PrintInitialConditions(int applications, int machines, int[][] executionTimes, int[] dueTimes, int[] penaltyMultiplyers)
        {
            Console.Clear();

            Console.WriteLine($"Applications: {applications}");
            Console.WriteLine($"Machines: {machines}");

            Console.WriteLine("Execution times:");
            for (int m = 0; m < executionTimes.Length; m++)
            {
                for (int a = 0; a < executionTimes[m].Length; a++)
                {
                    Console.Write($"{(executionTimes[m][a] < 10 ? ' ' : string.Empty)}{executionTimes[m][a]} ");
                }
                Console.WriteLine();
            }
            Console.WriteLine();

            Console.Write("Due times: ");
            for (int d = 0; d < dueTimes.Length; d++)
            {
                Console.Write($"{dueTimes[d]} ");
            }
            Console.WriteLine();

            Console.Write("Penalty multiplyers: ");
            for (int p = 0; p < penaltyMultiplyers.Length; p++)
            {
                Console.Write($"{penaltyMultiplyers[p]} ");
            }
            Console.WriteLine();

            Console.WriteLine("---------------------------------------------------------------------");
        }

        public static void Solve()
        {
            PrintInitialConditions(Applications, Machines, ExecutionTimes, DueTimes, PenaltyMultiplyers);

            static IApplicant applicantFactory(int[] genes)
            {
                return new ApplicantCyclic(genes);
            }

            Population = InitialPopulation.GenerateHEURISTICS(applicantFactory, PopulationSize, Applications, Machines, ExecutionTimes, DueTimes, PenaltyMultiplyers);
            
            for (int i = 0, iterationsWithSameBest = 0; i < 1000 && iterationsWithSameBest < 1000; i++, iterationsWithSameBest++) //2
            {
                if (Population.Count < 1)
                    break;

                EvaluatedApplicant currentApplicant = Population[0].Evaluate(Applications, Machines, ExecutionTimes, DueTimes, PenaltyMultiplyers);

                if (currentApplicant.Value < Best.Value)
                {
                    Best = currentApplicant;
                    Console.WriteLine(Best);
                    iterationsWithSameBest = 0;
                }

                List<(IApplicant First, IApplicant Second)> pairs = Parents.PickINBREEDING(Population);
                List<IApplicant> children = Crossover.CrossoverPairs(pairs);
                List<IApplicant> mutatedChildren = Mutation.MutateCOMPLEMENT(children);
                List<IApplicant> applicants = HandlingRestrictions.MODIFY(mutatedChildren, applicantFactory);
                List<EvaluatedApplicant> evaluatedApplicants = Evaluation.EvaluatePENALTY(applicants, Applications, Machines, ExecutionTimes, DueTimes, PenaltyMultiplyers);
                Population = NewPopulation.GenerateBESTCHILDREN(evaluatedApplicants, PopulationSize, applicantFactory);
            }
        }
    }
}
