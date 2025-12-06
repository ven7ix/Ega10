namespace Ega10
{
    internal static class NewPopulation //1
    {
        /// <summary>
        /// Generates new population
        /// </summary>
        /// <param name="children">Evaluated applicants</param>
        /// <param name="childFactory">How applicants will be created</param>
        /// <returns>New population</returns>
        public static List<IApplicant> Generate(in List<EvaluatedApplicant> children, Func<int[], IApplicant> childFactory)
        {
            List<IApplicant> newPopulation = new(children.Count);

            for (int i = 0; i < children.Count; i++)
            {
                var child = childFactory(children[i].Genes);
                newPopulation.Add(child);
            }

            return newPopulation;
        }
    }
}
