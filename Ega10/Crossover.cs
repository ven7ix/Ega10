using static Ega10.Tools;

namespace Ega10
{
    internal static class Crossover
    {
        public static List<int[]> ListAllCycles(int[] permutationIndices, int[] permutationValues)
        {
            int permutationLength = permutationIndices.Length;

            Dictionary<int, int> permutation = [];
            for (int i = 0; i < permutationLength; i++)
            {
                permutation[permutationIndices[i]] = permutationValues[i];
            }

            HashSet<int> visited = [];
            List<int[]> permutationCycles = [];

            foreach (int element in permutationIndices)
            {
                if (visited.Contains(element))
                    continue;

                int[] cycle = new int[permutationLength];
                int current = element;

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

        public static List<Applicant> CrossoverCYCLIC(Applicant firstParent, Applicant secondParent)
        {
            List<int[]> permutationCycles = ListAllCycles(firstParent.Genes, secondParent.Genes);

            int childrenAmount = 1 << permutationCycles.Count;
            List<Applicant> children = new(childrenAmount);

            for (int c = 0; c < childrenAmount; c++)
            {
                int[] childGenes = new int[firstParent.Genes.Length];

                string childIDBinar = GetLongBinar(c, permutationCycles.Count);

                for (int p = 0; p < permutationCycles.Count; p++)
                {
                    int[] parentGenes = childIDBinar[p] == '0' ? firstParent.Genes : secondParent.Genes;

                    for (int gen = 0; gen < parentGenes.Length; gen++)
                    {
                        childGenes[gen] += parentGenes[gen] * permutationCycles[p][gen];
                    }
                }

                if (childGenes.SequenceEqual(firstParent.Genes) || childGenes.SequenceEqual(secondParent.Genes))
                    continue;

                children.Add(new Applicant(childGenes));
            }

            return children;
        }

        public static List<Applicant> OrgyCYCLIC(List<Tuple<Applicant, Applicant>> parentPairs)
        {
            List<Applicant> children = [];

            foreach (var pair in parentPairs)
            {
                children = [.. children, .. CrossoverCYCLIC(pair.Item1, pair.Item2)];
            }

            return children;
        }


        public static int[] GenerateCuts(int genesAmount)
        {
            int cutsAmount = Tools.Random.Next(1, 5);

            int[] cuts = new int[cutsAmount]; //positions
            List<int> possibleCutPositions = [.. Enumerable.Range(1, genesAmount)];

            for (int gen = 0; gen < cutsAmount; gen++)
            {
                int cutPosition = Tools.Random.Next(0, possibleCutPositions.Count);
                cuts[gen] = possibleCutPositions[cutPosition];
                possibleCutPositions.RemoveAt(cutPosition);
            }

            return cuts;
        }

        public static List<int[]> GenerateCutCombinations(int[] arrayA, int[] arrayB, int[] cuts)
        {
            int n = arrayA.Length;
            List<int[]> result = [];

            List<int> boundaries = [0, .. cuts.Order(), n];

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

        public static List<Applicant> CrossoverORDINAL(Applicant firstParent, Applicant secondParent)
        {
            int genesAmount = firstParent.Genes.Length;

            List<int[]> childrenGenes = GenerateCutCombinations(firstParent.Genes, secondParent.Genes, GenerateCuts(genesAmount));

            List<Applicant> children = [];

            for (int c = 0; c < childrenGenes.Count; c++)
            {
                int[] childGenes = childrenGenes[c];

                if (childGenes.SequenceEqual(firstParent.Genes) || childGenes.SequenceEqual(secondParent.Genes))
                    continue;

                children.Add(new Applicant(childGenes));
            }

            return children;
        }

        public static List<Applicant> OrgyORDINAL(List<Tuple<Applicant, Applicant>> parentPairs)
        {
            List<Applicant> children = [];

            foreach (var pair in parentPairs)
            {
                children = [.. children, .. CrossoverORDINAL(pair.Item1, pair.Item2)];
            }

            return children;
        }
    }
}
