namespace Ega10
{
    internal interface IInitialPopalionGenerator
    {
        List<IChromosome> Generate(in IChromosomeFactory chromosomeFactory, int populationSize, int genesCount);
    }

    internal class RandomInitialPopalionGenerator(IRestriction restriction) : IInitialPopalionGenerator
    {
        private static int[] GenerateGenesRandom(int genesCount)
        {
            int[] genes = new int[genesCount];

            for (int gen = 0; gen < genesCount; gen++)
            {
                genes[gen] = Tools.Random.Next(0, genesCount);
            }

            return genes;
        }

        public List<IChromosome> Generate(in IChromosomeFactory chromosomeFactory, int populationSize, int genesCount)
        {
            List<IChromosome> initialPopulation = [];

            for (int i = 0; i < populationSize; i++)
            {
                IChromosome applicant = chromosomeFactory.Create(GenerateGenesRandom(genesCount));
                initialPopulation.Add(applicant);
            }

            return restriction.Apply(initialPopulation);
        }
    }

    internal class RandomControlledInitialPopalionGenerator : IInitialPopalionGenerator
    {
        public List<IChromosome> Generate(in IChromosomeFactory chromosomeFactory, int populationSize, int genesCount)
        {
            List<IChromosome> initialPopulation = [];

            for (int i = 0; i < populationSize; i++)
            {
                List<int> genesIndices = [.. Enumerable.Range(0, genesCount)];
                int[] genes = new int[genesCount];

                for (int a = 0; a < genesCount; a++)
                {
                    int genIndex = genesIndices[Tools.Random.Next(0, genesIndices.Count)];
                    _ = genesIndices.Remove(genIndex);

                    genes[a] = genIndex;
                }

                var applicant = chromosomeFactory.Create(genes);
                initialPopulation.Add(applicant);
            }

            return initialPopulation;
        }
    }

    internal class HueristicInitialPopalionGenerator(ProblemConditions problemConditions) : IInitialPopalionGenerator
    {
        private void GenerateOrdering(in IndexValuePair[] ordering, ref double orderingSum)
        {
            for (int a = 0; a < problemConditions.Applications; a++)
            {
                int value = 0;

                for (int m = 0; m < problemConditions.Machines; m++)
                {
                    value += problemConditions.ExecutionTimes[m][a];
                }

                ordering[a] = new(a, (value - problemConditions.DueTimes[a]) * problemConditions.PenaltyMultiplyers[a]);
            }

            double orderingMin = ordering.Min().Value;
            for (int a = 0; a < problemConditions.Applications; a++)
            {
                ordering[a].Value += Math.Abs(orderingMin) + 1;
                orderingSum += ordering[a].Value;
            }
        }

        private IndexValuePair PopApplication(in IndexValuePair[] ordering, ref double orderingSum)
        {
            double xi = Tools.Random.Next(1, (int)orderingSum + 1);
            double sum = 0;
            for (int a = 0; a < problemConditions.Applications; a++)
            {
                sum += ordering[a].Value;

                if (xi <= sum)
                {
                    IndexValuePair currentApplication = ordering[a];
                    orderingSum -= currentApplication.Value;
                    ordering[a] = new(currentApplication.Index, 0); //set value to 0 to skip it
                    return currentApplication;
                }
            }

            return new(-1, int.MinValue);
        }

        private IChromosome GenerateApplicant(in IChromosomeFactory chromosomeFactory, in IndexValuePair[] ordering, double orderingSum)
        {
            var applicantOrdering = new IndexValuePair[ordering.Length];
            Array.Copy(ordering, applicantOrdering, ordering.Length);

            int[] applicationsPermutation = new int[problemConditions.Applications];
            Array.Fill(applicationsPermutation, -1);

            int totalPenalty = 0;

            int[] machineTimes = new int[problemConditions.Machines];

            Array.Sort(applicantOrdering);

            var orderingByProbability = new IndexValuePair[problemConditions.Applications];
            for (int a = 0; a < problemConditions.Applications; a++)
            {
                orderingByProbability[a] = new(applicantOrdering[a].Index, applicantOrdering[^(a + 1)].Value);
            }

            for (int a = 0; a < problemConditions.Applications; a++)
            {
                IndexValuePair currentApplication = PopApplication(orderingByProbability, ref orderingSum);

                applicationsPermutation[a] = currentApplication.Index;

                for (int m = 0; m < problemConditions.Machines; m++)
                {
                    int taskExecutionTime = problemConditions.ExecutionTimes[m][currentApplication.Index];

                    if (m > 0)
                    {
                        if (machineTimes[m] < machineTimes[m - 1])
                        {
                            machineTimes[m] += machineTimes[m - 1] - machineTimes[m];
                        }
                    }

                    machineTimes[m] += taskExecutionTime;
                }

                totalPenalty += problemConditions.PenaltyMultiplyers[a] * Math.Max(0, machineTimes[problemConditions.Machines - 1] - problemConditions.DueTimes[currentApplication.Index]);
            }

            return chromosomeFactory.Create(applicationsPermutation);
        }

        public List<IChromosome> Generate(in IChromosomeFactory chromosomeFactory, int populationSize, int genesCount)
        {
            List<IChromosome> initialPopulation = [];

            var ordering = new IndexValuePair[problemConditions.Applications];
            double orderingSum = 0;

            GenerateOrdering(ordering, ref orderingSum);

            for (int i = 0; i < populationSize; i++)
            {
                initialPopulation.Add(GenerateApplicant(chromosomeFactory, ordering, orderingSum));
            }

            return initialPopulation;
        }
    }
}
