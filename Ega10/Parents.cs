namespace Ega10
{
    internal static class Parents //7
    {
        private const int MaxDistance = 40;
        private const int MinDistance = 40;

        /// <summary>
        /// Picks pairs of parents completly at random
        /// </summary>
        /// <param name="population">Current population</param>
        /// <returns>List of pairs, each containing two parents</returns>
        public static List<(IApplicant, IApplicant)> PickPairsRANDOM(in List<IApplicant> population)
        {
            int pairsCount = population.Count / 2;
            var pairs = new List<(IApplicant, IApplicant)>(pairsCount);

            List<int> availableIndices = [.. Enumerable.Range(0, population.Count)];

            for (int i = 0; i < pairsCount; i++)
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


        private static IApplicant GetPartnerINBREEDING(in List<IApplicant> population, List<int> availableIndices, in IApplicant firstParent, int maxDistance)
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

        /// <summary>
        /// Picks pairs of parents with minimal distance between them, using <see cref="IApplicant.DistanceToApplicant(IApplicant)"/>
        /// </summary>
        /// <param name="population">Current population</param>
        /// <param name="maxDistance">Max distance between two permutations. Uses <see cref="IApplicant.DistanceToApplicant(IApplicant)"/>to determine it</param>
        /// <returns>List of pairs, each containing two parents</returns>
        public static List<(IApplicant, IApplicant)> PickINBREEDING(in List<IApplicant> population, int maxDistance = MaxDistance)
        {
            int pairsCount = population.Count / 2;
            var pairs = new List<(IApplicant, IApplicant)>(pairsCount);

            List<int> availableIndices = [.. Enumerable.Range(0, population.Count)];

            for (int i = 0; i < pairsCount; i++)
            {
                int firstIndex = Tools.Random.Next(0, availableIndices.Count);
                int firstParentIndex = availableIndices[firstIndex];
                availableIndices.RemoveAt(firstIndex);

                pairs.Add((population[firstParentIndex], GetPartnerINBREEDING(population, availableIndices, population[firstParentIndex], maxDistance)));
            }

            return pairs;
        }


        private static IApplicant GetPartnerOUTBREEDING(in List<IApplicant> population, List<int> availableIndices, in IApplicant firstParent, int minDistance)
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

        /// <summary>
        /// Picks pairs of parents with biggest distance between them.
        /// Uses <see cref="IApplicant.DistanceToApplicant(IApplicant)"/>to determine it
        /// </summary>
        /// <param name="population">Current population</param>
        /// <param name="minDistance">Min distance between two permutations, using <see cref="IApplicant.DistanceToApplicant(IApplicant)"/>
        /// <returns>List of pairs, each containing two parents</returns>
        public static List<(IApplicant, IApplicant)> PickOUTBREEDING(in List<IApplicant> population, int minDistance = MinDistance)
        {
            int pairsCount = population.Count / 2;
            var pairs = new List<(IApplicant, IApplicant)>(pairsCount);

            List<int> availableIndices = [.. Enumerable.Range(0, population.Count)];

            for (int i = 0; i < pairsCount; i++)
            {
                int firstIndex = Tools.Random.Next(0, availableIndices.Count);
                int firstParentIndex = availableIndices[firstIndex];
                availableIndices.RemoveAt(firstIndex);

                pairs.Add((population[firstParentIndex], GetPartnerOUTBREEDING(population, availableIndices, population[firstParentIndex], minDistance)));
            }

            return pairs;
        }


        private static int PopApplicant(ref List<EvaluatedApplicant> evaluatedPopuplaion, ref double populationValue)
        {
            double xi = Tools.Random.Next(1, (int)populationValue + 1);
            double sum = 0;

            for (int i = 0; i < evaluatedPopuplaion.Count; i++)
            {
                sum += evaluatedPopuplaion[i].Value;

                if (xi <= sum)
                {
                    populationValue -= evaluatedPopuplaion[i].Value;
                    evaluatedPopuplaion.RemoveAt(i);
                    return i;
                }
            }

            return -1;
        }

        private static void PrepareEvaluatedPoplationPOSITIVE(ref List<EvaluatedApplicant> evaluatedPopuplaion, ref double populationValue, in List<IApplicant> population, in int applications, in int machines, in int[][] executionTimes, in int[] dueTimes, in int[] penaltyMultiplyers)
        {
            for (int i = 0; i < population.Count; i++)
            {
                evaluatedPopuplaion.Add(population[i].Evaluate(applications, machines, executionTimes, dueTimes, penaltyMultiplyers));
            }

            evaluatedPopuplaion.Sort();

            for (int i = 0; i < evaluatedPopuplaion.Count; i++)
            {
                evaluatedPopuplaion[i] = new EvaluatedApplicant(evaluatedPopuplaion[i].Genes, evaluatedPopuplaion[^(i + 1)].Value);
            }

            for (int i = 0; i < evaluatedPopuplaion.Count; i++)
            {
                populationValue += evaluatedPopuplaion[i].Value;
            }
        }

        /// <summary>
        /// Picks pairs of parents randomly, weighted by their <see cref="EvaluatedApplicant.Value"/>
        /// </summary>
        /// <param name="population">Current population</param>
        /// <param name="applications">Number of applications</param>
        /// <param name="machines">Number of machines</param>
        /// <param name="executionTimes">Execution times of each application</param>
        /// <param name="dueTimes">Due times of each application</param>
        /// <param name="penaltyMultiplyers">Penalty multiplyers of each application</param>
        /// <returns>List of pairs, each containing two parents</returns>
        public static List<(IApplicant, IApplicant)> PickPOSITIVE(in List<IApplicant> population, in int applications, in int machines, in int[][] executionTimes, in int[] dueTimes, in int[] penaltyMultiplyers)
        {
            int pairsCount = population.Count / 2;
            var pairs = new List<(IApplicant, IApplicant)>(pairsCount);

            var evaluatedPopuplaion = new List<EvaluatedApplicant>(population.Count);
            double populationValue = 0;

            PrepareEvaluatedPoplationPOSITIVE(ref evaluatedPopuplaion, ref populationValue, population, applications, machines, executionTimes, dueTimes, penaltyMultiplyers);

            for (int i = 0; i < pairsCount; i++)
            {
                int firstParentIndex = PopApplicant(ref evaluatedPopuplaion, ref populationValue);

                int secondParentIndex = PopApplicant(ref evaluatedPopuplaion, ref populationValue);

                pairs.Add((population[firstParentIndex], population[secondParentIndex]));
            }

            return pairs;
        }
    }
}
