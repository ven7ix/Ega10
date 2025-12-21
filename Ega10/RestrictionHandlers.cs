namespace Ega10
{
    internal interface IRestriction
    {
        List<IChromosome> Apply(in List<IChromosome> chromosome);
    }

    internal interface IModifyRestriction : IRestriction
    {
        List<IChromosome> IRestriction.Apply(in List<IChromosome> chromosome)
        {
            int chromosomesCount = chromosome.Count;
            var handledChromosomes = new List<IChromosome>(chromosomesCount);

            for (int i = 0; i < chromosomesCount; i++)
            {
                if (chromosome[i].IsValid)
                {
                    handledChromosomes.Add(chromosome[i]);
                }
                else
                {
                    handledChromosomes.Add(ModifyChromosome(chromosome[i]));
                }
            }

            return [.. handledChromosomes.Distinct()];
        }

        protected IChromosome ModifyChromosome(in IChromosome chromosome);
    }

    internal class DistinctRestriction : IRestriction
    {
        public List<IChromosome> Apply(in List<IChromosome> chromosomes)
        {
            return [.. chromosomes.Distinct()];
        }
    }

    internal class EliminateRestriction : IRestriction
    {
        public List<IChromosome> Apply(in List<IChromosome> chromosomes)
        {
            return [.. chromosomes.Where(genes => genes.IsValid).Distinct()];
        }
    }

    internal class CyclicModifyRestriction(IChromosomeFactory chromosomeFactory) : IModifyRestriction
    {
        IChromosome IModifyRestriction.ModifyChromosome(in IChromosome chromosome)
        {
            if (chromosome is not CyclicChromosome)
            {
                throw new Exception();
            }

            int genesCount = chromosome.Genes.Length;

            int[] genes = new int[genesCount];
            Array.Copy(chromosome.Genes, genes, genesCount);

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

            var newApplicant = chromosomeFactory.Create(genes);
            return newApplicant;
        }
    }
}
