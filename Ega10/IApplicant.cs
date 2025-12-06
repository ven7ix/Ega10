using System.Collections;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace Ega10
{
    internal interface IChromosome
    {
        int[] Genes { get; }
        bool ValidGenes { get; }
    }

    internal interface ICrossoverable<T> where T : IChromosome
    {
        List<T> CrossoverWith(T partner);
    }

    internal interface IEvaluatable
    {
        EvaluatedApplicant Evaluate(int applications, int machines, int[][] executionTimes, int[] dueTimes, int[] penaltyMultiplyers);
    }

    internal interface IMutable
    {
        IChromosome ModifyGenes(Func<int[], IChromosome> chromosomeFactory);
    }

    internal abstract class Chromosome : IChromosome
    {
        public int[] Genes { get; protected set; }
        public virtual bool ValidGenes
        {
            get
            {
                int genesLength = Genes.Length;

                bool[] found = new bool[genesLength];

                for (int i = 0; i < genesLength; i++)
                {
                    int gene = Genes[i];

                    if (gene < 0 || gene >= genesLength || found[gene])
                        return false;

                    found[gene] = true;
                }

                return true;
            }
        }

        public override string ToString()
        {
            string arrayString = string.Empty;

            for (int i = 0; i < Genes.Length; i++)
            {
                arrayString += $"{(Genes[i] < 10 ? ' ' : string.Empty)}{Genes[i]} ";
            }

            return arrayString;
        }

        public override bool Equals(object? obj)
        {
            if (obj is Chromosome chromosome)
            {
                return Genes.SequenceEqual(chromosome.Genes);
            }

            throw new ArgumentException("Inccorect type comparason");
        }

        public override int GetHashCode()
        {
            int hash = 17;

            for (int i = 0; i < Genes.Length; i++)
            {
                hash = hash * 31 + Genes[i];
            }

            return hash;
        }
    }

    internal interface IApplicant
    {
        int[] Genes { get; }
        bool ValidGenes 
        {
            get
            {
                for (int gen = 0; gen < Genes.Length; gen++)
                {
                    if (!Genes.Contains(gen))
                        return false;
                }

                return true;
            }
        }

        /// <summary>
        /// Determines the distance between two applicants by finding the difference in their corresponding genes
        /// </summary>
        /// <param name="applicant">Other applicant</param>
        /// <returns>Distance between two applicants</returns>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="Exception"></exception>
        int DistanceToApplicant(IApplicant applicant)
        {
            if (GetType() != applicant.GetType())
            {
                throw new ArgumentException("Inccorect applicant type");
            }
            else if (Genes.Length != applicant.Genes.Length)
            {
                throw new Exception("Cant find distance due to different Genes lengths");
            }

            int distance = 0;

            for (int i = 0; i < Genes.Length; i++)
            {
                distance += Math.Abs(Genes[i] - applicant.Genes[i]);
            }

            return distance;
        }

        /// <summary>
        /// Makes crossover between <see cref="this"/> applicant and a <paramref name="partner"/>. Each struct implements this in its own way
        /// </summary>
        /// <param name="partner">Partner to cross with</param>
        /// <returns>List of children who do not equal their parents</returns>
        List<IApplicant> CrossoverWith(IApplicant partner);

        int[] GetComplementGenes();

        IApplicant ModifyGenes(in IApplicant applicant, Func<int[], IApplicant> applicantFactory)
        {
            int genesCount = applicant.Genes.Length;

            int[] genes = new int[genesCount];
            Array.Copy(applicant.Genes, genes, genesCount);

            List<int> uniqueGenes = [];
            List<int> newGenes = [];

            for (int gen = 0; gen < genesCount; gen++)
            {
                if (!genes.Contains(gen))
                {
                    newGenes.Add(gen);
                }
            }

            for (int gen = 0, newGen = 0; gen < genesCount; gen++)
            {
                if (uniqueGenes.Contains(genes[gen]))
                {
                    genes[gen] = newGenes[newGen++];
                }

                uniqueGenes.Add(genes[gen]);
            }

            var newApplicant = applicantFactory(genes);
            return newApplicant;
        }

        EvaluatedApplicant Evaluate(int applications, int machines, int[][] executionTimes, int[] dueTimes, int[] penaltyMultiplyers)
        {
            int[] machineTimes = new int[machines];
            int totalPenalty = 0;

            for (int a = 0; a < applications; a++)
            {
                int minDueTimeIndex = Genes[a];

                for (int m = 0; m < machines; m++)
                {
                    int taskExecutionTime = executionTimes[m][minDueTimeIndex];

                    if (m > 0)
                    {
                        if (machineTimes[m] < machineTimes[m - 1])
                        {
                            machineTimes[m] += machineTimes[m - 1] - machineTimes[m];
                        }
                    }

                    machineTimes[m] += taskExecutionTime;
                }

                totalPenalty += penaltyMultiplyers[a] * Math.Max(0, machineTimes[^1] - dueTimes[minDueTimeIndex]);
            }

            return new EvaluatedApplicant(Genes, totalPenalty);
        }
    }

    internal struct ApplicantCyclic(int[] genes) : IApplicant
    {
        public int[] Genes { get; private set; } = genes;

        public override readonly string ToString()
        {
            string arrayString = string.Empty;

            for (int i = 0; i < Genes.Length; i++)
            {
                arrayString += $"{(Genes[i] < 10 ? ' ' : string.Empty)}{Genes[i]} ";
            }

            return arrayString;
        }

        public override readonly bool Equals([NotNullWhen(true)] object? obj)
        {
            if (obj is ApplicantCyclic applicant)
            {
                return Genes.SequenceEqual(applicant.Genes);
            }

            throw new ArgumentException("Inccorect type comparason");
        }

        public override readonly int GetHashCode()
        {
            return ((IStructuralEquatable)Genes).GetHashCode(EqualityComparer<int>.Default);
        }

        private static List<int[]> ListAllCycles(int[] permutationIndices, int[] permutationValues)
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

        public readonly List<IApplicant> CrossoverWith(IApplicant partner)
        {
            List<int[]> permutationCycles = ListAllCycles(Genes, partner.Genes);

            int childrenCount = 1 << permutationCycles.Count;
            List<IApplicant> children = new(childrenCount);

            for (int c = 0; c < childrenCount; c++)
            {
                int[] childGenes = new int[Genes.Length];

                for (int p = 0; p < permutationCycles.Count; p++)
                {
                    int[] parentGenes = ((c >> p) & 1) == 1 ? Genes : partner.Genes;

                    for (int gen = 0; gen < parentGenes.Length; gen++)
                    {
                        childGenes[gen] += parentGenes[gen] * permutationCycles[p][gen];
                    }
                }

                if (childGenes.SequenceEqual(Genes) || childGenes.SequenceEqual(partner.Genes))
                    continue;

                children.Add(new ApplicantCyclic(childGenes));
            }

            return children;
        }

        public readonly int[] GetComplementGenes()
        {
            int[] genes = new int[Genes.Length];

            for (int i = 0; i < Genes.Length; i++)
            {
                genes[i] = Genes.Length - 1 - Genes[i];
            }

            return genes;
        }
    }

    internal struct ApplicantOrdinal(int[] genes, bool encodeGenes = true) : IApplicant
    {
        public int[] Genes { get; private set; } = encodeGenes ? Encode(genes) : genes;
        public readonly bool ValidGenes
        {
            get
            {
                for (int gen = 0; gen < Genes.Length; gen++)
                {
                    if (Genes[gen] < 0 || Genes[gen] > Genes.Length - 1 - gen)
                        return false;
                }

                return true;
            }
        }

        public override readonly string ToString()
        {
            string arrayString = string.Empty;

            for (int i = 0; i < Genes.Length; i++)
            {
                arrayString += $"{(Genes[i] < 10 ? ' ' : string.Empty)}{Genes[i]} ";
            }

            return arrayString;
        }

        public override readonly bool Equals([NotNullWhen(true)] object? obj)
        {
            if (obj is ApplicantOrdinal applicant)
            {
                return Genes.SequenceEqual(applicant.Genes);
            }

            throw new ArgumentException("Inccorect type comparason");
        }

        public override readonly int GetHashCode()
        {
            return ((IStructuralEquatable)Genes).GetHashCode(EqualityComparer<int>.Default);
        }

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

        public readonly List<IApplicant> CrossoverWith(IApplicant partner)
        {
            int genesAmount = Genes.Length;

            List<int[]> childrenGenes = GenerateCutCombinations(Genes, partner.Genes, GenerateCuts(genesAmount));

            List<IApplicant> children = [];

            for (int c = 0; c < childrenGenes.Count; c++)
            {
                int[] childGenes = childrenGenes[c];

                if (childGenes.SequenceEqual(Genes) || childGenes.SequenceEqual(partner.Genes))
                    continue;

                children.Add(new ApplicantOrdinal(childGenes, encodeGenes: false));
            }

            return children;
        }

        public readonly int[] GetComplementGenes()
        {
            int[] genes = new int[Genes.Length];

            for (int i = 0; i < Genes.Length; i++)
            {
                genes[i] = Genes.Length - 1 - i - Genes[i];
            }

            return genes;
        }

        public readonly IApplicant ModifyGenes(in IApplicant applicant, Func<int[], IApplicant> applicantFactory)
        {
            int genesCount = applicant.Genes.Length;

            int[] genes = new int[genesCount];
            Array.Copy(Decode(applicant.Genes), genes, genesCount);

            List<int> uniqueGenes = [];
            List<int> newGenes = [];

            for (int gen = 0; gen < genesCount; gen++)
            {
                if (!genes.Contains(gen))
                {
                    newGenes.Add(gen);
                }
            }

            for (int gen = 0, newGen = 0; gen < genesCount; gen++)
            {
                if (uniqueGenes.Contains(genes[gen]))
                {
                    genes[gen] = newGenes[newGen++];
                }

                uniqueGenes.Add(genes[gen]);
            }

            var newApplicant = applicantFactory(Encode(genes));
            return newApplicant;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="genes"></param>
        /// <returns></returns>
        public static int[] Encode(in int[] genes)
        {
            int genesCount = genes.Length;

            var genesCopy = new Dictionary<int, int>(genesCount);
            int[] encodedGenes = new int[genesCount];

            var basePermutation = new Dictionary<int, int>(genesCount); //0 through n - 1 mapped to 0 through n - 1

            for (int i = 0; i < genesCount; i++)
            {
                genesCopy[i] = genes[i];
                basePermutation[i] = i;
            }

            for (int i = 0; i < genesCount; i++)
            {
                encodedGenes[i] = basePermutation[genesCopy[i]];
                basePermutation[genesCopy[i]] = -1;

                for (int j = 0, k = 0; j < genesCount; j++)
                {
                    if (basePermutation[j] != basePermutation[genesCopy[i]])
                    {
                        basePermutation[j] = k++;
                    }
                }
            }

            return encodedGenes;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="encodedGenes"></param>
        /// <returns></returns>
        public static int[] Decode(in int[] encodedGenes)
        {
            int genesCount = encodedGenes.Length;

            int[] encodedGenesCopy = new int[genesCount];
            Array.Copy(encodedGenes, encodedGenesCopy, genesCount);
            int[] genes = new int[genesCount];

            var basePermutation = new Dictionary<int, int>(genesCount); //0 through n - 1 mapped to 0 through n - 1

            for (int i = 0; i < genesCount; i++)
            {
                basePermutation[i] = i;
            }

            for (int i = 0; i < genesCount; i++)
            {
                int key = basePermutation.FirstOrDefault(pair => pair.Value == encodedGenesCopy[i]).Key;

                genes[i] = key;
                basePermutation[key] = -1;

                for (int j = 0, k = 0; j < genesCount; j++)
                {
                    if (basePermutation[j] != basePermutation[key])
                    {
                        basePermutation[j] = k++;
                    }
                }
            }

            return genes;
        }

        public readonly EvaluatedApplicant Evaluate(int applications, int machines, int[][] executionTimes, int[] dueTimes, int[] penaltyMultiplyers)
        {
            int[] decodedGenes = Decode(Genes);

            int[] machineTimes = new int[machines];
            int totalPenalty = 0;

            for (int a = 0; a < applications; a++)
            {
                int minDueTimeIndex = decodedGenes[a];

                for (int m = 0; m < machines; m++)
                {
                    int taskExecutionTime = executionTimes[m][minDueTimeIndex];

                    if (m > 0)
                    {
                        if (machineTimes[m] < machineTimes[m - 1])
                        {
                            machineTimes[m] += machineTimes[m - 1] - machineTimes[m];
                        }
                    }

                    machineTimes[m] += taskExecutionTime;
                }

                totalPenalty += penaltyMultiplyers[a] * Math.Max(0, machineTimes[^1] - dueTimes[minDueTimeIndex]);
            }

            return new EvaluatedApplicant(decodedGenes, totalPenalty);
        }
    }

    internal struct EvaluatedApplicant(int[] genes, double value) : IApplicant, IComparable
    {
        public int[] Genes { get; private set; } = genes;
        public double Value { get; set; } = value;

        public readonly int CompareTo(object? obj)
        {
            if (obj is EvaluatedApplicant pair)
                return Value.CompareTo(pair.Value);

            throw new ArgumentException("Inccorect type comparason");
        }

        public override readonly string ToString()
        {
            string arrayString = string.Empty;

            for (int i = 0; i < Genes.Length; i++)
            {
                arrayString += $"{(Genes[i] < 10 ? ' ' : string.Empty)}{Genes[i]} ";
            }

            return $"{Value}: {arrayString}";
        }

        public List<IApplicant> CrossoverWith(IApplicant partner)
        {
            throw new NotImplementedException();
        }

        public readonly int[] GetComplementGenes()
        {
            throw new NotImplementedException();
        }
    }

    internal struct IndexValuePair(int index, double value) : IComparable
    {
        public int Index { get; private set; } = index;
        public double Value { get; set; } = value;

        public readonly int CompareTo(object? obj)
        {
            if (obj is IndexValuePair pair)
                return Value.CompareTo(pair.Value);

            throw new ArgumentException("Inccorect type comparason");
        }
    }
}
