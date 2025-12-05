namespace Ega10
{
    internal static class Evaluation
    {
        public static List<EvaluatedApplicant> EvaluatePENALTY(List<IApplicant> applicants, int applications, int machines, int[][] executionTimes, int[] dueTimes, int[] penaltyMultiplyers)
        {
            List<EvaluatedApplicant> evaluatedApplicant = [];

            foreach (IApplicant applicant in applicants)
            {
                evaluatedApplicant.Add(applicant.Evaluate(applications, machines, executionTimes, dueTimes, penaltyMultiplyers));
            }

            evaluatedApplicant.Sort();

            return evaluatedApplicant;
        }
    }
}
