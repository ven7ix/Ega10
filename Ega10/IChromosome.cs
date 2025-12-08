namespace Ega10
{
    internal static class ChromosomeOperations
    {
        public static int DistanceBetweenChromosomes(in IChromosome chromosome1, in IChromosome chromosome2)
        {
            if (chromosome1.GetType() != chromosome2.GetType())
            {
                throw new ArgumentException("Inccorect applicant type");
            }
            else if (chromosome1.Genes.Length != chromosome2.Genes.Length)
            {
                throw new Exception("Cant find distance due to different Genes lengths");
            }

            int distance = 0;

            for (int i = 0; i < chromosome1.Genes.Length; i++)
            {
                distance += Math.Abs(chromosome1.Genes[i] - chromosome2.Genes[i]);
            }

            return distance;
        }

        public static int ChromosomesDiversity(List<IChromosome> chromosomes)
        {
            int diversity = 0;

            for (int i = 0; i < chromosomes.Count; i++)
            {
                for (int j = 0; j < chromosomes.Count; j++)
                {
                    diversity += DistanceBetweenChromosomes(chromosomes[i], chromosomes[j]);
                }
            }

            return diversity;
        }

        public static int[] Encode(in int[] genes)
        {
            int genesLength = genes.Length;

            var genesCopy = new Dictionary<int, int>(genesLength);
            int[] encodedGenes = new int[genesLength];

            var basePermutation = new Dictionary<int, int>(genesLength); //0 through n - 1 mapped to 0 through n - 1

            for (int i = 0; i < genesLength; i++)
            {
                genesCopy[i] = genes[i];
                basePermutation[i] = i;
            }

            for (int i = 0; i < genesLength; i++)
            {
                encodedGenes[i] = basePermutation[genesCopy[i]];
                basePermutation[genesCopy[i]] = -1;

                for (int j = 0, k = 0; j < genesLength; j++)
                {
                    if (basePermutation[j] != basePermutation[genesCopy[i]])
                    {
                        basePermutation[j] = k++;
                    }
                }
            }

            return encodedGenes;
        }

        public static int[] Decode(in int[] encodedGenes)
        {
            int genesLength = encodedGenes.Length;

            int[] encodedGenesCopy = new int[genesLength];
            Array.Copy(encodedGenes, encodedGenesCopy, genesLength);
            int[] genes = new int[genesLength];

            var basePermutation = new Dictionary<int, int>(genesLength); //0 through n - 1 mapped to 0 through n - 1

            for (int i = 0; i < genesLength; i++)
            {
                basePermutation[i] = i;
            }

            for (int i = 0; i < genesLength; i++)
            {
                int key = basePermutation.FirstOrDefault(pair => pair.Value == encodedGenesCopy[i]).Key;

                genes[i] = key;
                basePermutation[key] = -1;

                for (int j = 0, k = 0; j < genesLength; j++)
                {
                    if (basePermutation[j] != basePermutation[key])
                    {
                        basePermutation[j] = k++;
                    }
                }
            }

            return genes;
        }
    }

    internal interface IChromosome : IEquatable<IChromosome>
    {
        int[] Genes { get; }
        bool IsValid { get; }
    }

    internal readonly struct CyclicChromosome(int[] genes) : IChromosome
    {
        public readonly int[] Genes { get; } = genes;

        public readonly bool IsValid
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

        public readonly bool Equals(IChromosome? other)
        {
            if (other == null)
                return false;

            return Genes.SequenceEqual(other.Genes);
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

    internal readonly struct OrdinalChromosome(int[] genes, bool encodeGenes = true) : IChromosome
    {
        public readonly int[] Genes { get; } = encodeGenes ? ChromosomeOperations.Encode(genes) : genes;

        public readonly bool IsValid
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

        public readonly bool Equals(IChromosome? other)
        {
            if (other == null)
                return false;

            return Genes.SequenceEqual(other.Genes);
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

    internal readonly struct EvaluatedChromosome(int[] genes, double value) : IChromosome, IComparable<EvaluatedChromosome>
    {
        public readonly int[] Genes { get; } = genes;

        public readonly bool IsValid => throw new NotImplementedException();

        public readonly double Value { get; } = value;

        public int CompareTo(EvaluatedChromosome other)
        {
            return Value.CompareTo(other.Value);
        }

        public readonly bool Equals(IChromosome? other)
        {
            if (other == null)
                return false;

            return Genes.SequenceEqual(other.Genes);
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

        public readonly override string ToString()
        {
            string arrayString = string.Empty;

            for (int i = 0; i < Genes.Length; i++)
            {
                arrayString += $"{(Genes[i] < 10 ? ' ' : string.Empty)}{Genes[i]} ";
            }

            return $"{Value}: {arrayString}";
        }
    }


    internal interface IChromosomeFactory
    {
        IChromosome Create(int[] genes);
    }

    internal class CyclicChromosomeFactory : IChromosomeFactory
    {
        public IChromosome Create(int[] genes)
        {
            return new CyclicChromosome(genes);
        }
    }

    internal class OrdinalChromosomeFactory(bool encode) : IChromosomeFactory
    {
        public IChromosome Create(int[] genes)
        {
            return new OrdinalChromosome(genes, encode);
        }
    }
}
