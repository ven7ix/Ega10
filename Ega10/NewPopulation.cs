using static Ega10.Tools;

namespace Ega10
{
    internal static class NewPopulation //1
    {
        public static List<Applicant> GenerateBESTCHILDREN(List<EvaluatedApplicant> children, int populationSize)
        {
            int newPopulationSize = Math.Min(children.Count, populationSize);

            List<Applicant> newPopulation = new(newPopulationSize);

            for (int i = 0; i < newPopulationSize; i++)
            {
                newPopulation.Add(new Applicant(children[i].Genes));
            }

            return newPopulation;
        }
    }
}
