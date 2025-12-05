using static Ega10.Tools;

namespace Ega10
{
    internal static class NewPopulation //1
    {
        public static List<IApplicant> GenerateBESTCHILDREN(List<EvaluatedApplicant> children, int populationSize, Func<int[], IApplicant> childFactory)
        {
            int newPopulationSize = Math.Min(children.Count, populationSize);

            List<IApplicant> newPopulation = new(newPopulationSize);

            for (int i = 0; i < newPopulationSize; i++)
            {
                var child = childFactory(children[i].Genes);
                newPopulation.Add(child);
            }

            return newPopulation;
        }
    }
}
