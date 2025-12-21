namespace Ega10
{
    internal interface IParentPairsSelector
    {
        List<(IChromosome, IChromosome)> Select(in List<IChromosome> chromosomes);
    }

    internal class ParentPairsSelectorRandom : IParentPairsSelector
    {
        public List<(IChromosome, IChromosome)> Select(in List<IChromosome> chromosomes)
        {
            int pairsCount = chromosomes.Count / 2;
            var pairs = new List<(IChromosome, IChromosome)>(pairsCount);

            List<int> availableIndices = [.. Enumerable.Range(0, chromosomes.Count)];

            for (int i = 0; i < pairsCount; i++)
            {
                int index1 = Tools.Random.Next(0, availableIndices.Count);
                int parentIndex1 = availableIndices[index1];
                availableIndices.RemoveAt(index1);

                int index2 = Tools.Random.Next(0, availableIndices.Count);
                int parentIndex2 = availableIndices[index2];
                availableIndices.RemoveAt(index2);

                pairs.Add((chromosomes[parentIndex1], chromosomes[parentIndex2]));
            }

            return pairs;
        }
    }

    internal class ParentPairsSelectorInbreeding(int maxDistance) : IParentPairsSelector
    {
        private IChromosome GetPartnerInbreeding(in List<IChromosome> chromosomes, List<int> availableIndices, in IChromosome parent1)
        {
            int index = Tools.Random.Next(0, availableIndices.Count);
            int parterIndex = availableIndices[index];
            IChromosome partner = chromosomes[parterIndex];

            for (; index < availableIndices.Count; index++)
            {
                parterIndex = availableIndices[index];

                if (ChromosomeOperations.DistanceBetweenChromosomes(parent1, chromosomes[parterIndex]) < maxDistance)
                {
                    availableIndices.RemoveAt(index);
                    return chromosomes[parterIndex];
                }
            }

            return partner;
        }

        public List<(IChromosome, IChromosome)> Select(in List<IChromosome> chromosomes)
        {
            int pairsCount = chromosomes.Count / 2;
            var pairs = new List<(IChromosome, IChromosome)>(pairsCount);

            List<int> availableIndices = [.. Enumerable.Range(0, chromosomes.Count)];

            for (int i = 0; i < pairsCount; i++)
            {
                int index1 = Tools.Random.Next(0, availableIndices.Count);
                int parentIndex1 = availableIndices[index1];
                availableIndices.RemoveAt(index1);

                pairs.Add((chromosomes[parentIndex1], GetPartnerInbreeding(chromosomes, availableIndices, chromosomes[parentIndex1])));
            }

            return pairs;
        }
    }

    internal class ParentPairsSelectorOutbreeding(int minDistance) : IParentPairsSelector
    {
        private IChromosome GetPartnerOutbreeding(in List<IChromosome> chromosomes, List<int> availableIndices, in IChromosome parent1)
        {
            int index = Tools.Random.Next(0, availableIndices.Count);
            int parterIndex = availableIndices[index];
            IChromosome partner = chromosomes[parterIndex];

            for (; index < availableIndices.Count; index++)
            {
                parterIndex = availableIndices[index];

                if (ChromosomeOperations.DistanceBetweenChromosomes(parent1, chromosomes[parterIndex]) > minDistance)
                {
                    availableIndices.RemoveAt(index);
                    return chromosomes[parterIndex];
                }
            }

            return partner;
        }

        public List<(IChromosome, IChromosome)> Select(in List<IChromosome> chromosomes)
        {
            int pairsCount = chromosomes.Count / 2;
            var pairs = new List<(IChromosome, IChromosome)>(pairsCount);

            List<int> availableIndices = [.. Enumerable.Range(0, chromosomes.Count)];

            for (int i = 0; i < pairsCount; i++)
            {
                int index1 = Tools.Random.Next(0, availableIndices.Count);
                int parentIndex1 = availableIndices[index1];
                availableIndices.RemoveAt(index1);

                pairs.Add((chromosomes[parentIndex1], GetPartnerOutbreeding(chromosomes, availableIndices, chromosomes[parentIndex1])));
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

        private void PrepareEvaluatedPoplationPositive(in List<EvaluatedChromosome> evaluatedChromosomes, ref double populationValue, in List<IChromosome> chromosomes)
        {
            for (int i = 0; i < chromosomes.Count; i++)
            {
                evaluatedChromosomes.Add(evaluator.EvaluateChromosome(chromosomes[i], problemConditions));
            }

            evaluatedChromosomes.Sort();

            for (int i = 0; i < evaluatedChromosomes.Count; i++)
            {
                evaluatedChromosomes[i] = new EvaluatedChromosome(evaluatedChromosomes[i].Genes, evaluatedChromosomes[^(i + 1)].Value);
            }

            for (int i = 0; i < evaluatedChromosomes.Count; i++)
            {
                populationValue += evaluatedChromosomes[i].Value;
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
                int parentIndex1 = PopApplicant(evaluatedPopuplaion, ref populationValue);

                int parentIndex2 = PopApplicant(evaluatedPopuplaion, ref populationValue);

                pairs.Add((population[parentIndex1], population[parentIndex2]));
            }

            return pairs;
        }
    }
}
