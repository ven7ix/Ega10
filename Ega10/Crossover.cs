namespace Ega10
{
    internal static class Crossover //2
    {
        /// <summary>
        /// Crossovers parent pairs using <see cref="IApplicant.CrossoverWith(IApplicant)"/>
        /// </summary>
        /// <param name="parentPairs">Parent pairs</param>
        /// <returns>List of children who do not equal their parents</returns>
        public static List<IApplicant> CrossoverPairs(in List<(IApplicant, IApplicant)> parentPairs)
        {
            List<IApplicant> children = [];

            foreach (var (first, second) in parentPairs)
            {
                if (first.GetType() == second.GetType())
                {
                    children = [.. children, .. first.CrossoverWith(second)];
                }
            }

            return children;
        }
    }
}
