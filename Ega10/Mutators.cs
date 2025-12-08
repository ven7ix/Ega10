namespace Ega10
{
    internal interface IMutator
    {
        double ChromosomeMutationChance { get; }
        double GeneMutationChance { get; }

        List<IChromosome> Mutate(in List<IChromosome> chromosomes);
    }

    internal interface IComplementMutator : IMutator
    {
        List<IChromosome> IMutator.Mutate(in List<IChromosome> chromosomes)
        {
            for (int i = 0; i < chromosomes.Count; i++)
            {
                if (Tools.Random.NextDouble() >= ChromosomeMutationChance)
                    continue;

                int genesCount = chromosomes[i].Genes.Length;
                int[] complementGenes = GetComplementGenes(chromosomes[i].Genes);

                for (int gen = 0; gen < genesCount; gen++)
                {
                    if (Tools.Random.NextDouble() >= GeneMutationChance)
                        continue;

                    chromosomes[i].Genes[gen] = complementGenes[gen];
                }
            }

            return chromosomes;
        }

        protected int[] GetComplementGenes(in int[] Genes);
    }

    internal class RandomMutator(double chromosomeMutationChance, double geneMutationChance) : IMutator
    {
        public double ChromosomeMutationChance { get; } = chromosomeMutationChance;
        public double GeneMutationChance { get; } = geneMutationChance;

        public List<IChromosome> Mutate(in List<IChromosome> chromosomes)
        {
            for (int i = 0; i < chromosomes.Count; i++)
            {
                if (Tools.Random.NextDouble() >= ChromosomeMutationChance)
                    continue;

                int genesCount = chromosomes[i].Genes.Length;

                for (int gen = 0; gen < genesCount; gen++)
                {
                    if (Tools.Random.NextDouble() >= GeneMutationChance)
                        continue;

                    int mutatedGen = Tools.Random.Next(0, genesCount);
                    chromosomes[i].Genes[mutatedGen] = chromosomes[i].Genes[Tools.Random.Next(0, genesCount)];
                }
            }

            return chromosomes;
        }
    }

    internal class RandomControlledMutator(double chromosomeMutationChance, double geneMutationChance) : IMutator
    {
        public double ChromosomeMutationChance { get; } = chromosomeMutationChance;
        public double GeneMutationChance { get; } = geneMutationChance;

        public List<IChromosome> Mutate(in List<IChromosome> chromosomes)
        {
            for (int i = 0; i < chromosomes.Count; i++)
            {
                if (Tools.Random.NextDouble() >= ChromosomeMutationChance)
                    continue;

                int genesCount = chromosomes[i].Genes.Length;

                for (int gen = 0; gen < genesCount; gen++)
                {
                    if (Tools.Random.NextDouble() >= GeneMutationChance)
                        continue;

                    int gen1 = Tools.Random.Next(0, genesCount);
                    int gen2 = Tools.Random.Next(0, genesCount);

                    (chromosomes[i].Genes[gen1], chromosomes[i].Genes[gen2]) = (chromosomes[i].Genes[gen2], chromosomes[i].Genes[gen1]);
                }
            }

            return chromosomes;
        }
    }

    internal class CyclicComplementMutator(double chromosomeMutationChance, double geneMutationChance) : IComplementMutator
    {
        public double ChromosomeMutationChance { get; } = chromosomeMutationChance;
        public double GeneMutationChance { get; } = geneMutationChance;

        int[] IComplementMutator.GetComplementGenes(in int[] Genes)
        {
            int[] genes = new int[Genes.Length];

            for (int i = 0; i < Genes.Length; i++)
            {
                genes[i] = Genes.Length - 1 - Genes[i];
            }

            return genes;
        }
    }

    internal class OrdinalComplementMutator(double chromosomeMutationChance, double geneMutationChance) : IComplementMutator
    {
        public double ChromosomeMutationChance { get; } = chromosomeMutationChance;
        public double GeneMutationChance { get; } = geneMutationChance;

        int[] IComplementMutator.GetComplementGenes(in int[] Genes)
        {
            int[] genes = new int[Genes.Length];

            for (int i = 0; i < Genes.Length; i++)
            {
                genes[i] = Genes.Length - 1 - i - Genes[i];
            }

            return genes;
        }
    }
}
