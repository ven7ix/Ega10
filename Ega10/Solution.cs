using static Ega10.Tools;

namespace Ega10
{
    internal static class Solution
    {
        public static int Applications { get; } = 15;
        public static int Machines { get; } = 5;
        public static int[] PenaltyMultiplyers { get; } = [1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1];
        public static int[][] ExecutionTimes { get; } =
            [
            [ 3, 13,  7, 15,  5,  5,  3, 17, 13,  3,  1,  3,  4,  4,  9],
            [23,  9, 11,  8, 11, 22, 10, 15,  4,  6,  5,  5,  7,  9, 10],
            [ 7, 21,  9,  9,  7, 23,  6,  2,  8, 23, 23, 16,  1, 11,  6],
            [ 4, 15,  1,  1, 12,  1,  7, 24, 19, 24,  9, 12, 20, 11,  6],
            [ 5, 16,  1, 17, 23,  5,  7,  9,  4, 16, 22, 20, 16, 22,  2]
            ]; //сколько выполнятеся работа (столбец) на станке (строка)
        public static int[] DueTimes { get; } = [20, 25, 30, 35, 40, 45, 50, 55, 60, 65, 70, 75, 80, 85, 90];

        public static int PopulationSize { get; } = 200;
        public static List<Applicant> Population { get; set; } = [];
        public static EvaluatedApplicant Best { get; set; } = new([], int.MaxValue);

        public static void Solve()
        {
            PrintInitialConditions(Applications, Machines, ExecutionTimes, DueTimes, PenaltyMultiplyers);

            Population = InitialPopulation.GenerateHEURISTICS(PopulationSize, Applications, Machines, ExecutionTimes, DueTimes, PenaltyMultiplyers);
            
            for (int i = 0, iterationsWithSameBest = 0; i < 1000 && iterationsWithSameBest < 1000; i++, iterationsWithSameBest++) //2
            {
                Population = 
                    NewPopulation.GenerateBESTCHILDREN(
                        Evaluation.EvaluatePENALTY(
                            HandlingRestrictions.MODIFY(
                                Mutation.MutateRANDOM(
                                    Crossover.OrgyORDINAL(
                                        Parents.PickRANDOM(Population)))),
                                Applications, Machines, ExecutionTimes, DueTimes, PenaltyMultiplyers),
                            PopulationSize);

                if (Population.Count < 1)
                    break;
                
                EvaluatedApplicant currentApplicant = Evaluation.EvaluateApplicantPENALTY(Population[0], Applications, Machines, ExecutionTimes, DueTimes, PenaltyMultiplyers);

                if (currentApplicant.Value < Best.Value)
                {
                    Best = currentApplicant;
                    Console.WriteLine(Best);
                    iterationsWithSameBest = 0;
                }
            }
        }
    }
}
