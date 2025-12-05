using static Ega10.Tools;

namespace Ega10
{
    internal static class Parents //5
    {
        private const int MaxDistance = 40;
        private const int MinDistance = 40;

        public static List<(IApplicant First, IApplicant Second)> PickPairsRANDOM(List<IApplicant> population)
        {
            int pairCount = population.Count / 2;
            var pairs = new List<(IApplicant First, IApplicant Second)>(pairCount);

            List<int> availableIndices = [.. Enumerable.Range(0, population.Count)];

            for (int i = 0; i < pairCount; i++)
            {
                int firstIndex = Tools.Random.Next(0, availableIndices.Count);
                int firstParentIndex = availableIndices[firstIndex];
                availableIndices.RemoveAt(firstIndex);

                int secondIndex = Tools.Random.Next(0, availableIndices.Count);
                int secondParentIndex = availableIndices[secondIndex];
                availableIndices.RemoveAt(secondIndex);

                pairs.Add((population[firstParentIndex], population[secondParentIndex]));
            }

            return pairs;
        }


        private static IApplicant GetPartnerINBREEDING(List<IApplicant> population, List<int> availableIndices, IApplicant firstParent, int maxDistance)
        {
            int index = Tools.Random.Next(0, availableIndices.Count);
            int parterIndex = availableIndices[index];
            IApplicant partner = population[parterIndex];

            for (; index < availableIndices.Count; index++)
            {
                parterIndex = availableIndices[index];

                if (firstParent.DistanceToApplicant(population[parterIndex]) < maxDistance)
                {
                    availableIndices.RemoveAt(index);
                    return population[parterIndex];
                }
            }

            return partner;
        }

        public static List<(IApplicant First, IApplicant Second)> PickINBREEDING(List<IApplicant> population, int maxDistance = MaxDistance)
        {
            int pairCount = population.Count / 2;
            var pairs = new List<(IApplicant First, IApplicant Second)>(pairCount);

            List<int> availableIndices = [.. Enumerable.Range(0, population.Count)];

            for (int i = 0; i < pairCount; i++)
            {
                int firstIndex = Tools.Random.Next(0, availableIndices.Count);
                int firstParentIndex = availableIndices[firstIndex];
                availableIndices.RemoveAt(firstIndex);

                pairs.Add((population[firstParentIndex], GetPartnerINBREEDING(population, availableIndices, population[firstParentIndex], maxDistance)));
            }

            return pairs;
        }


        private static IApplicant GetPartnerOUTBREEDING(List<IApplicant> population, List<int> availableIndices, IApplicant firstParent, int minDistance)
        {
            int index = Tools.Random.Next(0, availableIndices.Count);
            int parterIndex = availableIndices[index];
            IApplicant partner = population[parterIndex];

            for (; index < availableIndices.Count; index++)
            {
                parterIndex = availableIndices[index];

                if (firstParent.DistanceToApplicant(population[parterIndex]) > minDistance)
                {
                    availableIndices.RemoveAt(index);
                    return population[parterIndex];
                }
            }

            return partner;
        }

        public static List<(IApplicant First, IApplicant Second)> PickOUTBREEDING(List<IApplicant> population, int minDistance = MinDistance)
        {
            int pairCount = population.Count / 2;
            var pairs = new List<(IApplicant First, IApplicant Second)>(pairCount);

            List<int> availableIndices = [.. Enumerable.Range(0, population.Count)];

            for (int i = 0; i < pairCount; i++)
            {
                int firstIndex = Tools.Random.Next(0, availableIndices.Count);
                int firstParentIndex = availableIndices[firstIndex];
                availableIndices.RemoveAt(firstIndex);

                pairs.Add((population[firstParentIndex], GetPartnerOUTBREEDING(population, availableIndices, population[firstParentIndex], minDistance)));
            }

            return pairs;
        }


        public static List<(IApplicant First, IApplicant Second)> PickPOSITIVE(List<IApplicant> population)
        {
            List<(IApplicant First, IApplicant Second)> parentPairs = [];

            return parentPairs;
        }
    }
}
