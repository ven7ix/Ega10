namespace Ega10
{
    internal interface IEvaluator
    {
        List<EvaluatedChromosome> EvaluateChromosomes(in List<IChromosome> chromosomes, in ProblemConditions problemConditions)
        {
            var evaluatedChromosomes = new List<EvaluatedChromosome>(chromosomes.Count);

            for (int i = 0; i < chromosomes.Count; i++)
            {
                evaluatedChromosomes.Add(EvaluateChromosome(chromosomes[i], problemConditions));
            }

            return evaluatedChromosomes;
        }

        EvaluatedChromosome EvaluateChromosome(in IChromosome chromosome, in ProblemConditions problemConditions);
    }

    internal class EvaluatorCyclic : IEvaluator
    {
        public EvaluatedChromosome EvaluateChromosome(in IChromosome chromosome, in ProblemConditions problemConditions)
        {
            if (chromosome is not CyclicChromosome)
            {
                throw new Exception();
            }

            int[] machineTimes = new int[problemConditions.Machines];
            int totalPenalty = 0;

            for (int a = 0; a < problemConditions.Applications; a++)
            {
                int minDueTimeIndex = chromosome.Genes[a];

                for (int m = 0; m < problemConditions.Machines; m++)
                {
                    int taskExecutionTime = problemConditions.ExecutionTimes[m][minDueTimeIndex];

                    if (m > 0)
                    {
                        if (machineTimes[m] < machineTimes[m - 1])
                        {
                            machineTimes[m] += machineTimes[m - 1] - machineTimes[m];
                        }
                    }

                    machineTimes[m] += taskExecutionTime;
                }

                totalPenalty += problemConditions.PenaltyMultiplyers[a] * Math.Max(0, machineTimes[^1] - problemConditions.DueTimes[minDueTimeIndex]);
            }

            return new EvaluatedChromosome(chromosome.Genes, totalPenalty);
        }
    }

    internal class EvaluatorOrdinal : IEvaluator
    {
        public EvaluatedChromosome EvaluateChromosome(in IChromosome chromosome, in ProblemConditions problemConditions)
        {
            if (chromosome is not OrdinalChromosome)
            {
                throw new Exception();
            }

            int[] decodedGenes = ChromosomeOperations.Decode(chromosome.Genes);

            int[] machineTimes = new int[problemConditions.Machines];
            int totalPenalty = 0;

            for (int a = 0; a < problemConditions.Applications; a++)
            {
                int minDueTimeIndex = decodedGenes[a];

                for (int m = 0; m < problemConditions.Machines; m++)
                {
                    int taskExecutionTime = problemConditions.ExecutionTimes[m][minDueTimeIndex];

                    if (m > 0)
                    {
                        if (machineTimes[m] < machineTimes[m - 1])
                        {
                            machineTimes[m] += machineTimes[m - 1] - machineTimes[m];
                        }
                    }

                    machineTimes[m] += taskExecutionTime;
                }

                totalPenalty += problemConditions.PenaltyMultiplyers[a] * Math.Max(0, machineTimes[^1] - problemConditions.DueTimes[minDueTimeIndex]);
            }

            return new EvaluatedChromosome(decodedGenes, totalPenalty);
        }
    }
}
