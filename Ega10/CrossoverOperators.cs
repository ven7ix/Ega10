namespace Ega10
{
    internal interface ICrossoverOperator
    {
        protected List<IChromosome> Crossover(in IChromosome parent1, in IChromosome parent2);

        List<IChromosome> CrossoverPairs(in List<(IChromosome, IChromosome)> parentPairs)
        {
            List<IChromosome> children = [];

            foreach (var (first, second) in parentPairs)
            {
                children = [.. children, .. Crossover(first, second)];
            }

            return children;
        }
    }

    internal class CyclicCrossoverOperator : ICrossoverOperator
    {
        private static List<int[]> ListAllCycles(in int[] permutationIndices, in int[] permutationValues)
        {
            int permutationLength = permutationIndices.Length;

            Dictionary<int, int> permutation = [];
            for (int i = 0; i < permutationLength; i++)
            {
                permutation[permutationIndices[i]] = permutationValues[i];
            }

            var visited = new HashSet<int>(permutationIndices.Length);
            List<int[]> permutationCycles = [];

            for (int i = 0; i < permutationIndices.Length; i++)
            {
                if (visited.Contains(permutationIndices[i]))
                    continue;

                int[] cycle = new int[permutationLength];
                int current = permutationIndices[i];

                while (!visited.Contains(current))
                {
                    cycle[Array.FindIndex(permutationIndices, value => value == current)] = 1;
                    _ = visited.Add(current);

                    _ = permutation.TryGetValue(current, out current);
                }

                permutationCycles.Add(cycle);
            }

            return permutationCycles;
        }

        List<IChromosome> ICrossoverOperator.Crossover(in IChromosome parent1, in IChromosome parent2)
        {
            if (!(parent1 is CyclicChromosome && parent2 is CyclicChromosome))
            {
                throw new Exception();
            }

            List<int[]> permutationCycles = ListAllCycles(parent1.Genes, parent2.Genes);

            int childrenCount = 1 << permutationCycles.Count;
            var children = new List<IChromosome>(childrenCount);

            for (int c = 0; c < childrenCount; c++)
            {
                int[] childGenes = new int[parent1.Genes.Length];

                for (int p = 0; p < permutationCycles.Count; p++)
                {
                    int[] parentGenes = ((c >> p) & 1) == 1 ? parent1.Genes : parent2.Genes;

                    for (int gen = 0; gen < parentGenes.Length; gen++)
                    {
                        childGenes[gen] += parentGenes[gen] * permutationCycles[p][gen];
                    }
                }

                if (childGenes.SequenceEqual(parent1.Genes) || childGenes.SequenceEqual(parent2.Genes))
                    continue;

                children.Add(new CyclicChromosome(childGenes));
            }

            return children;
        }
    }

    internal class OrdinalCrossoverOperator : ICrossoverOperator
    {
        private static int[] GenerateCuts(int genesCount)
        {
            int cutsCount = Tools.Random.Next(1, 5);

            int[] cutPositions = new int[cutsCount];
            List<int> possibleCutPositions = [.. Enumerable.Range(1, genesCount)];

            for (int gen = 0; gen < cutsCount; gen++)
            {
                int cutPosition = Tools.Random.Next(0, possibleCutPositions.Count);
                cutPositions[gen] = possibleCutPositions[cutPosition];
                possibleCutPositions.RemoveAt(cutPosition);
            }

            return cutPositions;
        }

        private static List<int[]> GenerateCutCombinations(int[] array1, int[] array2, int[] cuts)
        {
            int arrayLength = array1.Length;
            List<int[]> result = [];

            List<int> boundaries = [0, .. cuts.Order(), arrayLength];

            List<int[]> parts1 = [];
            List<int[]> parts2 = [];

            for (int i = 0; i < boundaries.Count - 1; i++)
            {
                int start = boundaries[i];
                int end = boundaries[i + 1];
                int length = end - start;

                int[] part1 = new int[length];
                int[] part2 = new int[length];

                Array.Copy(array1, start, part1, 0, length);
                Array.Copy(array2, start, part2, 0, length);

                parts1.Add(part1);
                parts2.Add(part2);
            }

            int partsCount = parts1.Count;
            int totalCombinations = 1 << partsCount;

            for (int mask = 0; mask < totalCombinations; mask++)
            {
                List<int> combination = [];

                for (int partIndex = 0; partIndex < partsCount; partIndex++)
                {
                    int[] selectedPart = ((mask >> partIndex) & 1) == 1 ? parts2[partIndex] : parts1[partIndex];
                    combination.AddRange(selectedPart);
                }

                result.Add([.. combination]);
            }

            return result;
        }

        List<IChromosome> ICrossoverOperator.Crossover(in IChromosome parent1, in IChromosome parent2)
        {
            if (!(parent1 is OrdinalChromosome && parent2 is OrdinalChromosome))
            {
                throw new Exception();
            }

            int genesAmount = parent1.Genes.Length;

            List<int[]> childrenGenes = GenerateCutCombinations(parent1.Genes, parent2.Genes, GenerateCuts(genesAmount));

            List<IChromosome> children = [];

            for (int c = 0; c < childrenGenes.Count; c++)
            {
                int[] childGenes = childrenGenes[c];

                if (childGenes.SequenceEqual(parent1.Genes) || childGenes.SequenceEqual(parent2.Genes))
                    continue;

                children.Add(new OrdinalChromosome(childGenes, encodeGenes: false));
            }

            return children;
        }
    }
}
