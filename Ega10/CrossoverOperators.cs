using System.Linq;

namespace Ega10
{
    internal interface ICrossoverOperator
    {
        protected List<IChromosome> Crossover(in IChromosome firstParent, in IChromosome secondParent);

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

        List<IChromosome> ICrossoverOperator.Crossover(in IChromosome firstParent, in IChromosome secondParent)
        {
            List<int[]> permutationCycles = ListAllCycles(firstParent.Genes, secondParent.Genes);

            int childrenCount = 1 << permutationCycles.Count;
            var children = new List<IChromosome>(childrenCount);

            for (int c = 0; c < childrenCount; c++)
            {
                int[] childGenes = new int[firstParent.Genes.Length];

                for (int p = 0; p < permutationCycles.Count; p++)
                {
                    int[] parentGenes = ((c >> p) & 1) == 1 ? firstParent.Genes : secondParent.Genes;

                    for (int gen = 0; gen < parentGenes.Length; gen++)
                    {
                        childGenes[gen] += parentGenes[gen] * permutationCycles[p][gen];
                    }
                }

                if (childGenes.SequenceEqual(firstParent.Genes) || childGenes.SequenceEqual(secondParent.Genes))
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

        private static List<int[]> GenerateCutCombinations(int[] arrayA, int[] arrayB, int[] cuts)
        {
            int arrayLength = arrayA.Length;
            List<int[]> result = [];

            List<int> boundaries = [0, .. cuts.Order(), arrayLength];

            List<int[]> partsA = [];
            List<int[]> partsB = [];

            for (int i = 0; i < boundaries.Count - 1; i++)
            {
                int start = boundaries[i];
                int end = boundaries[i + 1];
                int length = end - start;

                int[] partA = new int[length];
                int[] partB = new int[length];

                Array.Copy(arrayA, start, partA, 0, length);
                Array.Copy(arrayB, start, partB, 0, length);

                partsA.Add(partA);
                partsB.Add(partB);
            }

            int partsCount = partsA.Count;
            int totalCombinations = 1 << partsCount;

            for (int mask = 0; mask < totalCombinations; mask++)
            {
                List<int> combination = [];

                for (int partIndex = 0; partIndex < partsCount; partIndex++)
                {
                    int[] selectedPart = ((mask >> partIndex) & 1) == 1 ? partsB[partIndex] : partsA[partIndex];
                    combination.AddRange(selectedPart);
                }

                result.Add([.. combination]);
            }

            return result;
        }

        List<IChromosome> ICrossoverOperator.Crossover(in IChromosome firstParent, in IChromosome secondParent)
        {
            int genesAmount = firstParent.Genes.Length;

            List<int[]> childrenGenes = GenerateCutCombinations(firstParent.Genes, secondParent.Genes, GenerateCuts(genesAmount));

            List<IChromosome> children = [];

            for (int c = 0; c < childrenGenes.Count; c++)
            {
                int[] childGenes = childrenGenes[c];

                if (childGenes.SequenceEqual(firstParent.Genes) || childGenes.SequenceEqual(secondParent.Genes))
                    continue;

                children.Add(new OrdinalChromosome(childGenes, encodeGenes: false));
            }

            return children;
        }
    }
}
