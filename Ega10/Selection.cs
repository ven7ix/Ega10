namespace Ega10
{
    internal static class Selection //2
    {
        /// <summary>
        /// Evaluated only children
        /// </summary>
        /// <param name="populationSize">Population size</param>
        /// <param name="applicants">Children</param>
        /// <param name="applications">Number of applications</param>
        /// <param name="machines">Number of machines</param>
        /// <param name="executionTimes">Execution times of each application</param>
        /// <param name="dueTimes">Due times of each application</param>
        /// <param name="penaltyMultiplyers">Penalty multiplyers of each application</param>
        /// <returns>List of <paramref name="populationSize"/> or <paramref name="applicants"/>.Count (whichever is smaller) evaluated applicants</returns>
        public static List<EvaluatedApplicant> EvaluateChildren(in List<IApplicant> applicants, in int populationSize, in int applications, in int machines, in int[][] executionTimes, in int[] dueTimes, in int[] penaltyMultiplyers)
        {
            int applicantsCount = applicants.Count;
            var evaluatedApplicants = new List<EvaluatedApplicant>(applicantsCount);

            for (int i = 0; i < applicantsCount; i++)
            {
                evaluatedApplicants.Add(applicants[i].Evaluate(applications, machines, executionTimes, dueTimes, penaltyMultiplyers));
            }

            evaluatedApplicants.Sort();

            return evaluatedApplicants.GetRange(0, Math.Min(populationSize, applicantsCount));
        }

        /// <summary>
        /// Evaluated only children and current pupulation
        /// </summary>
        /// <param name="populationSize">Population size</param>
        /// <param name="population">Current population</param>
        /// <param name="applicants">Children</param>
        /// <param name="applications">Number of applications</param>
        /// <param name="machines">Number of machines</param>
        /// <param name="executionTimes">Execution times of each application</param>
        /// <param name="dueTimes">Due times of each application</param>
        /// <param name="penaltyMultiplyers">Penalty multiplyers of each application</param>
        /// <returns>List of <paramref name="populationSize"/> or <paramref name="applicants"/>.Count (whichever is smaller) evaluated applicants</returns>
        public static List<EvaluatedApplicant> EvaluatePopulationAndChildren(in List<IApplicant> population, in int populationSize, in List<IApplicant> applicants, in int applications, in int machines, in int[][] executionTimes, in int[] dueTimes, in int[] penaltyMultiplyers)
        {
            int applicantsCount = applicants.Count;
            var evaluatedApplicants = new List<EvaluatedApplicant>(population.Count + applicantsCount);

            for (int i = 0; i < population.Count; i++)
            {
                evaluatedApplicants.Add(population[i].Evaluate(applications, machines, executionTimes, dueTimes, penaltyMultiplyers));
            }

            for (int i = 0; i < applicantsCount; i++)
            {
                evaluatedApplicants.Add(applicants[i].Evaluate(applications, machines, executionTimes, dueTimes, penaltyMultiplyers));
            }

            evaluatedApplicants.Sort();

            return evaluatedApplicants.GetRange(0, Math.Min(populationSize, applicantsCount));
        }

        /// <summary>
        /// Works like trash because it drastically cuts the number of children
        /// </summary>
        /// <param name="populationSize">Population size</param>
        /// <param name="tournamentFreeSpaces">Number of children who will fight for survival</param>
        /// <param name="applicants">Children</param>
        /// <param name="applications">Number of applications</param>
        /// <param name="machines">Number of machines</param>
        /// <param name="executionTimes">Execution times of each application</param>
        /// <param name="dueTimes">Due times of each application</param>
        /// <param name="penaltyMultiplyers">Penalty multiplyers of each application</param>
        /// <returns>List of <paramref name="populationSize"/> or <paramref name="applicants"/>.Count (whichever is smaller) evaluated applicants</returns>
        public static List<EvaluatedApplicant> BetaTournament(in List<IApplicant> applicants, in int populationSize, in int tournamentFreeSpaces, in int applications, in int machines, in int[][] executionTimes, in int[] dueTimes, in int[] penaltyMultiplyers)
        {
            int applicantsCount = applicants.Count;

            var evaluatedApplicants = new List<EvaluatedApplicant>(applicantsCount);
            var competingApplicants = new List<EvaluatedApplicant>(tournamentFreeSpaces);

            List<int> availableIndices = [.. Enumerable.Range(0, applicantsCount)];

            for (int i = 0; i < applicantsCount; i++)
            {
                int index = Tools.Random.Next(0, availableIndices.Count);
                int applicantIndex = availableIndices[index];
                availableIndices.RemoveAt(index);

                competingApplicants.Add(applicants[applicantIndex].Evaluate(applications, machines, executionTimes, dueTimes, penaltyMultiplyers));

                if (competingApplicants.Count >= tournamentFreeSpaces)
                {
                    evaluatedApplicants.Add(competingApplicants.Max());
                    competingApplicants.Clear();
                }
            }

            evaluatedApplicants.Sort();

            return evaluatedApplicants.GetRange(0, Math.Min(populationSize, applicantsCount));
        }
    }
}
