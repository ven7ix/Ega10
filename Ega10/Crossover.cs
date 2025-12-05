namespace Ega10
{
    internal static class Crossover //2
    {
        public static List<IApplicant> CrossoverPairs(List<(IApplicant, IApplicant)> parentPairs)
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
