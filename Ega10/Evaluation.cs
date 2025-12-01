using static Ega10.Tools;

namespace Ega10
{
    internal static class Evaluation
    {
        public static EvaluatedApplicant EvaluateApplicantPENALTY(Applicant applicant, int applications, int machines, int[][] executionTimes, int[] dueTimes, int[] penaltyMultiplyers)
        {
            int[] machineTimes = new int[machines];
            int totalPenalty = 0;

            for (int a = 0; a < applications; a++)
            {
                int minDueTimeIndex = applicant.Genes[a];

                for (int m = 0; m < machines; m++)
                {
                    int taskExecutionTime = executionTimes[m][minDueTimeIndex];

                    if (m > 0)
                    {
                        if (machineTimes[m] < machineTimes[m - 1])
                        {
                            machineTimes[m] += machineTimes[m - 1] - machineTimes[m];
                        }
                    }

                    machineTimes[m] += taskExecutionTime;
                }

                totalPenalty += penaltyMultiplyers[a] * Math.Max(0, machineTimes[^1] - dueTimes[minDueTimeIndex]);
            }

            return new EvaluatedApplicant(applicant.Genes, totalPenalty);
        }

        public static List<EvaluatedApplicant> EvaluatePENALTY(List<Applicant> applicants, int applications, int machines, int[][] executionTimes, int[] dueTimes, int[] penaltyMultiplyers)
        {
            List<EvaluatedApplicant> evaluatedApplicant = [];

            foreach (Applicant applicant in applicants)
            {
                evaluatedApplicant.Add(EvaluateApplicantPENALTY(new Applicant(DecodePermutation(applicant.Genes)), applications, machines, executionTimes, dueTimes, penaltyMultiplyers));
            }

            evaluatedApplicant.Sort();

            return evaluatedApplicant;
        }
    }
}
