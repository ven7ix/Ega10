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


        public static void SolveSameBest(Func<int[], IApplicant> applicantFactory, int maxIterations)
        {
            PrintInitialConditions();
            GenerateInitialPopulation(applicantFactory);

            for (int iterationsWithSameBest = 0; iterationsWithSameBest < maxIterations; iterationsWithSameBest++)
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

                GenerateNewPopulation(applicantFactory);
            }
        }


        public static void SolveMaxIterarions(Func<int[], IApplicant> applicantFactory, int maxIterations)
        {
            PrintInitialConditions();
            GenerateInitialPopulation(applicantFactory);

            for (int i = 0; i < maxIterations; i++)
            {
                if (Population.Count < 1)
                    break;

                EvaluatedApplicant currentApplicant = Population[0].Evaluate(Applications, Machines, ExecutionTimes, DueTimes, PenaltyMultiplyers);

                if (currentApplicant.Value < Best.Value)
                {
                    Best = currentApplicant;
                    Console.WriteLine(Best);
                }

                GenerateNewPopulation(applicantFactory);
            }
        }


        private static int GetPopulationDiversity(List<IApplicant> population)
        {
            int diversity = 0;

            for (int i = 0; i < population.Count; i++)
            {
                for (int j = 0; j < population.Count; j++)
                {
                    diversity += population[i].DistanceToApplicant(population[j]);
                }
            }

            return diversity;
        }

        public static void SolveGeneticDiversity(Func<int[], IApplicant> applicantFactory, double geneticDiversity, int checkDelay)
        {
            PrintInitialConditions();
            GenerateInitialPopulation(applicantFactory);

            double diversityRandom = 0;
            diversityRandom = GetPopulationDiversity(InitialPopulation.GenerateRANDOMCONTROL(applicantFactory, PopulationSize, Applications));

            for (int i = 0; ; i++)
            {
                if (i % checkDelay == 0)
                {
                    if (GetPopulationDiversity(Population) / diversityRandom < geneticDiversity)
                        break;
                }

                if (Population.Count < 1)
                    break;

                EvaluatedApplicant currentApplicant = Population[0].Evaluate(Applications, Machines, ExecutionTimes, DueTimes, PenaltyMultiplyers);

                if (currentApplicant.Value < Best.Value)
                {
                    Best = currentApplicant;
                    Console.WriteLine(Best);
                }

                GenerateNewPopulation(applicantFactory);
            }
        }


        private static void PrintInitialConditions()
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

        private static void GenerateInitialPopulation(Func<int[], IApplicant> applicantFactory)
        {
            Population = InitialPopulation.GenerateHEURISTIC(applicantFactory, PopulationSize, Applications, Machines, ExecutionTimes, DueTimes, PenaltyMultiplyers);
        }

        private static void GenerateNewPopulation(Func<int[], IApplicant> applicantFactory)
        {
            Population =
                NewPopulation.Generate(
                    Selection.EvaluateChildren(
                        HandlingRestrictions.DECODE(
                            Mutation.MutateCOMPLEMENT(
                                Crossover.CrossoverPairs(
                                    Parents.PickPairsRANDOM(Population)),
                                5, 10),
                           applicantFactory),
                        PopulationSize, Applications, Machines, ExecutionTimes, DueTimes, PenaltyMultiplyers), 
                    applicantFactory);
        }
    }
}
