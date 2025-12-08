namespace Ega10
{
    internal interface IParentPairsSelector
    {
        List<(IChromosome, IChromosome)> Select(in List<IChromosome> population);
    }

    internal class ParentPairsSelectorRandom : IParentPairsSelector
    {
        public List<(IChromosome, IChromosome)> Select(in List<IChromosome> population)
        {
            int pairsCount = population.Count / 2;
            var pairs = new List<(IChromosome, IChromosome)>(pairsCount);

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
    }

    internal class ParentPairsSelectorInbreeding(int maxDistance) : IParentPairsSelector
    {
        private IChromosome GetPartnerInbreeding(in List<IChromosome> population, List<int> availableIndices, in IChromosome firstParent)
        {
            int index = Tools.Random.Next(0, availableIndices.Count);
            int parterIndex = availableIndices[index];
            IChromosome partner = population[parterIndex];

            for (; index < availableIndices.Count; index++)
            {
                parterIndex = availableIndices[index];

                if (ChromosomeOperations.DistanceBetweenChromosomes(firstParent, population[parterIndex]) < maxDistance)
                {
                    availableIndices.RemoveAt(index);
                    return population[parterIndex];
                }
            }

            return partner;
        }

        public List<(IChromosome, IChromosome)> Select(in List<IChromosome> population)
        {
            int pairsCount = population.Count / 2;
            var pairs = new List<(IChromosome, IChromosome)>(pairsCount);

            List<int> availableIndices = [.. Enumerable.Range(0, population.Count)];

            for (int i = 0; i < pairsCount; i++)
            {
                int firstIndex = Tools.Random.Next(0, availableIndices.Count);
                int firstParentIndex = availableIndices[firstIndex];
                availableIndices.RemoveAt(firstIndex);

                pairs.Add((population[firstParentIndex], GetPartnerInbreeding(population, availableIndices, population[firstParentIndex])));
            }

            return pairs;
        }
    }

    internal class ParentPairsSelectorOutbreeding(int minDistance) : IParentPairsSelector
    {
        private IChromosome GetPartnerOutbreeding(in List<IChromosome> population, List<int> availableIndices, in IChromosome firstParent)
        {
            int index = Tools.Random.Next(0, availableIndices.Count);
            int parterIndex = availableIndices[index];
            IChromosome partner = population[parterIndex];

            for (; index < availableIndices.Count; index++)
            {
                parterIndex = availableIndices[index];

                if (ChromosomeOperations.DistanceBetweenChromosomes(firstParent, population[parterIndex]) > minDistance)
                {
                    availableIndices.RemoveAt(index);
                    return population[parterIndex];
                }
            }

            return partner;
        }

        public List<(IChromosome, IChromosome)> Select(in List<IChromosome> population)
        {
            int pairsCount = population.Count / 2;
            var pairs = new List<(IChromosome, IChromosome)>(pairsCount);

            List<int> availableIndices = [.. Enumerable.Range(0, population.Count)];

            for (int i = 0; i < pairsCount; i++)
            {
                int firstIndex = Tools.Random.Next(0, availableIndices.Count);
                int firstParentIndex = availableIndices[firstIndex];
                availableIndices.RemoveAt(firstIndex);

                pairs.Add((population[firstParentIndex], GetPartnerOutbreeding(population, availableIndices, population[firstParentIndex])));
            }

            return pairs;
        }
    }

    internal class ParentPairsSelectorPositive(IEvaluator evaluator, ProblemConditions problemConditions) : IParentPairsSelector
    {
        private static int PopApplicant(in List<EvaluatedChromosome> evaluatedPopuplaion, ref double populationValue)
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

        private void PrepareEvaluatedPoplationPositive(in List<EvaluatedChromosome> evaluatedPopuplaion, ref double populationValue, in List<IChromosome> population)
        {
            for (int i = 0; i < population.Count; i++)
            {
                evaluatedPopuplaion.Add(evaluator.EvaluateChromosome(population[i], problemConditions));
            }

            evaluatedPopuplaion.Sort();

            for (int i = 0; i < evaluatedPopuplaion.Count; i++)
            {
                evaluatedPopuplaion[i] = new EvaluatedChromosome(evaluatedPopuplaion[i].Genes, evaluatedPopuplaion[^(i + 1)].Value);
            }

            for (int i = 0; i < evaluatedPopuplaion.Count; i++)
            {
                populationValue += evaluatedPopuplaion[i].Value;
            }
        }

        public List<(IChromosome, IChromosome)> Select(in List<IChromosome> population)
        {
            int pairsCount = population.Count / 2;
            var pairs = new List<(IChromosome, IChromosome)>(pairsCount);

            var evaluatedPopuplaion = new List<EvaluatedChromosome>(population.Count);
            double populationValue = 0;

            PrepareEvaluatedPoplationPositive(evaluatedPopuplaion, ref populationValue, population);

            for (int i = 0; i < pairsCount; i++)
            {
                int firstParentIndex = PopApplicant(evaluatedPopuplaion, ref populationValue);

                int secondParentIndex = PopApplicant(evaluatedPopuplaion, ref populationValue);

                pairs.Add((population[firstParentIndex], population[secondParentIndex]));
            }

            return pairs;
        }
    }
}
