namespace Ega10
{
    internal interface INewPopulationGenerator
    {
        List<IChromosome> Generate(in List<EvaluatedChromosome> evaluatedChromosomes, in IChromosomeFactory chromosomeFactory);
    }

    internal class NewPopulationGeneratorDefault : INewPopulationGenerator
    {
        public List<IChromosome> Generate(in List<EvaluatedChromosome> evaluatedChromosomes, in IChromosomeFactory chromosomeFactory)
        {
            var newPopulation = new List<IChromosome>(evaluatedChromosomes.Count);

            for (int i = 0; i < evaluatedChromosomes.Count; i++)
            {
                IChromosome chromosome = chromosomeFactory.Create(evaluatedChromosomes[i].Genes);
                newPopulation.Add(chromosome);
            }

            return newPopulation;
        }
    }
}
