using System;
using static Ega10.Tools;

namespace Ega10
{
    internal static class InitialPopulation //6
    {
        public static List<Applicant> GenerateRANDOM(int populationSize, int applications)
        {
            List<Applicant> initialPopulation = [];

            for (int applicant = 0; applicant < populationSize; applicant++)
            {
                int[] applicantGenes = new int[applications];

                for (int a = 0; a < applications; a++)
                {
                    applicantGenes[a] = Tools.Random.Next(0, applications);
                }

                initialPopulation.Add(new Applicant(applicantGenes));
            }

            return initialPopulation;
        }


        public static List<Applicant> GenerateRANDOMCONTROL(int populationSize, int applications)
        {
            List<Applicant> initialPopulation = [];

            for (int applicant = 0; applicant < populationSize; applicant++)
            {
                List<int> applicationIDs = [.. Enumerable.Range(0, applications)];
                int[] applicantGenes = new int[applications];

                for (int a = 0; a < applications; a++)
                {
                    int applicationID = applicationIDs[Tools.Random.Next(0, applicationIDs.Count)];
                    _ = applicationIDs.Remove(applicationID);

                    applicantGenes[a] = applicationID;
                }

                initialPopulation.Add(new Applicant(applicantGenes));
            }

            return initialPopulation;
        }


        private static void GenerateOrdering(IndexValuePair[] ordering, ref double orderingSum, int applications, int machines, int[][] executionTimes, int[] dueTimes, int[] penaltyMultiplyers)
        {
            for (int a = 0; a < applications; a++)
            {
                int value = 0;

                for (int m = 0; m < machines; m++)
                {
                    value += executionTimes[m][a];
                }

                ordering[a] = new(a, (value - dueTimes[a]) * penaltyMultiplyers[a]);
            }

            double orderingMin = ordering.Min().Value;
            for (int a = 0; a < applications; a++)
            {
                ordering[a].Value += Math.Abs(orderingMin) + 1;
                orderingSum += ordering[a].Value;
            }
        }

        private static IndexValuePair PopApplication(IndexValuePair[] ordering, ref double orderingSum, int applications)
        {
            double xi = Tools.Random.Next(1, (int)orderingSum + 1);
            double sum = 0;
            for (int a = 0; a < applications; a++)
            {
                sum += ordering[a].Value;

                if (xi <= sum)
                {
                    IndexValuePair currentApplication = ordering[a];
                    orderingSum -= currentApplication.Value;
                    ordering[a] = new(currentApplication.Index, 0); //set value to 0 to skip it
                    return currentApplication;
                }
            }

            return new(-1, int.MinValue);
        }

        private static Applicant GenerateApplicant(IndexValuePair[] ordering, double orderingSum, int applications, int machines, int[][] executionTimes, int[] dueTimes, int[] penaltyMultiplyers)
        {
            IndexValuePair[] applicantOrdering = new IndexValuePair[ordering.Length];
            Array.Copy(ordering, applicantOrdering, ordering.Length);

            int[] applicationsPermutation = new int[applications];
            Array.Fill(applicationsPermutation, -1);

            int totalPenalty = 0;

            int[] machineTimes = new int[machines];

            Array.Sort(applicantOrdering);

            IndexValuePair[] orderingByProbability = new IndexValuePair[applications];
            for (int a = 0; a < applications; a++)
            {
                orderingByProbability[a] = new(applicantOrdering[a].Index, applicantOrdering[^(a + 1)].Value);
            }

            for (int a = 0; a < applications; a++)
            {
                IndexValuePair currentApplication = PopApplication(orderingByProbability, ref orderingSum, applications);

                applicationsPermutation[a] = currentApplication.Index;

                for (int m = 0; m < machines; m++)
                {
                    int taskExecutionTime = executionTimes[m][currentApplication.Index];

                    if (m > 0)
                    {
                        if (machineTimes[m] < machineTimes[m - 1])
                        {
                            machineTimes[m] += machineTimes[m - 1] - machineTimes[m];
                        }
                    }

                    machineTimes[m] += taskExecutionTime;
                }

                totalPenalty += penaltyMultiplyers[a] * Math.Max(0, machineTimes[machines - 1] - dueTimes[currentApplication.Index]);
            }

            return new Applicant(applicationsPermutation);
        }

        public static List<Applicant> GenerateHEURISTICS(int populationSize, int applications, int machines, int[][] executionTimes, int[] dueTimes, int[] penaltyMultiplyers)
        {
            List<Applicant> initialPopulation = [];

            IndexValuePair[] ordering = new IndexValuePair[applications];
            double orderingSum = 0;

            GenerateOrdering(ordering, ref orderingSum, applications, machines, executionTimes, dueTimes, penaltyMultiplyers);

            for (int i = 0; i < populationSize; i++)
            {
                initialPopulation.Add(GenerateApplicant(ordering, orderingSum, applications, machines, executionTimes, dueTimes, penaltyMultiplyers));
            }

            return initialPopulation;
        }
    }
}
