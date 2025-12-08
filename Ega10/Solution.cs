namespace Ega10
{
    internal class Solution(ProblemConditions problemConditions, int populationSize)
    {
        private List<IChromosome> Population { get; set; } = new List<IChromosome>(populationSize);
        private EvaluatedChromosome Best { get; set; } = new EvaluatedChromosome([], int.MaxValue);

        private void PrintInitialConditions()
        {
            Console.Clear();

            Console.WriteLine($"Applications: {problemConditions.Applications}");
            Console.WriteLine($"Machines: {problemConditions.Machines}");

            Console.WriteLine("Execution times:");
            for (int m = 0; m < problemConditions.ExecutionTimes.Length; m++)
            {
                for (int a = 0; a < problemConditions.ExecutionTimes[m].Length; a++)
                {
                    Console.Write($"{(problemConditions.ExecutionTimes[m][a] < 10 ? ' ' : string.Empty)}{problemConditions.ExecutionTimes[m][a]} ");
                }
                Console.WriteLine();
            }
            Console.WriteLine();

            Console.Write("Due times: ");
            for (int d = 0; d < problemConditions.DueTimes.Length; d++)
            {
                Console.Write($"{problemConditions.DueTimes[d]} ");
            }
            Console.WriteLine();

            Console.Write("Penalty multiplyers: ");
            for (int p = 0; p < problemConditions.PenaltyMultiplyers.Length; p++)
            {
                Console.Write($"{problemConditions.PenaltyMultiplyers[p]} ");
            }
            Console.WriteLine();

            Console.WriteLine("---------------------------------------------------------------------");
        }

        private void GenerateNewPopulation(
            in IChromosomeFactory chromosomeFactory,
            in IParentPairsSelector parentPairsSelector,
            in ICrossoverOperator crossoverOperator,
            in IMutator mutator,
            in IRestriction restriction,
            in IEvaluator evaluator,
            in INewPopulationGenerator newPopulationGenerator)
        {
            var children =
                evaluator.EvaluateChromosomes(
                    restriction.Apply(
                        mutator.Mutate(
                            crossoverOperator.CrossoverPairs(
                                parentPairsSelector.Select(Population)))), populationSize, problemConditions);

            Population = newPopulationGenerator.Generate(children, chromosomeFactory);
        }


        public void SolveMaxIterarions(
            in IChromosomeFactory chromosomeFactory,
            in IInitialPopalionGenerator initialPopalionGenerator,
            in IParentPairsSelector parentPairsSelector,
            in ICrossoverOperator crossoverOperator,
            in IMutator mutator,
            in IRestriction restriction,
            in IEvaluator evaluator,
            in INewPopulationGenerator newPopulationGenerator,
            int maxIterations)
        {
            PrintInitialConditions();

            Population = initialPopalionGenerator.Generate(chromosomeFactory, populationSize, problemConditions.Applications);

            for (int i = 0; i < maxIterations && Population.Count > 0; i++)
            {
                EvaluatedChromosome currentApplicant = evaluator.EvaluateChromosome(Population[0], problemConditions);

                if (currentApplicant.Value < Best.Value)
                {
                    Best = currentApplicant;
                    Console.WriteLine(Best);
                }

                GenerateNewPopulation(chromosomeFactory, parentPairsSelector, crossoverOperator, mutator, restriction, evaluator, newPopulationGenerator);
            }
        }

        public void SolveSameBest(
            in IChromosomeFactory chromosomeFactory,
            in IInitialPopalionGenerator initialPopalionGenerator,
            in IParentPairsSelector parentPairsSelector,
            in ICrossoverOperator crossoverOperator,
            in IMutator mutator,
            in IRestriction restriction,
            in IEvaluator evaluator,
            in INewPopulationGenerator newPopulationGenerator,
            int maxIterations)
        {
            PrintInitialConditions();

            Population = initialPopalionGenerator.Generate(chromosomeFactory, populationSize, problemConditions.Applications);

            for (int i = 0; i < maxIterations && Population.Count > 0; i++)
            {
                EvaluatedChromosome currentApplicant = evaluator.EvaluateChromosome(Population[0], problemConditions);

                if (currentApplicant.Value < Best.Value)
                {
                    i = 0;
                    Best = currentApplicant;
                    Console.WriteLine(Best);
                }

                GenerateNewPopulation(chromosomeFactory, parentPairsSelector, crossoverOperator, mutator, restriction, evaluator, newPopulationGenerator);
            }
        }

        public void SolveGeneticDiversity(
            in IChromosomeFactory chromosomeFactory,
            in IInitialPopalionGenerator initialPopalionGenerator,
            in IParentPairsSelector parentPairsSelector,
            in ICrossoverOperator crossoverOperator,
            in IMutator mutator,
            in IRestriction restriction,
            in IEvaluator evaluator,
            in INewPopulationGenerator newPopulationGenerator,
            double geneticDiversity, int checkDelay)
        {
            PrintInitialConditions();

            var initialPopuationRandom = new RandomControlledInitialPopalionGenerator();
            double diversityRandom = ChromosomeOperations.ChromosomesDiversity(initialPopuationRandom.Generate(chromosomeFactory, populationSize, problemConditions.Applications));
            
            Population = initialPopalionGenerator.Generate(chromosomeFactory, populationSize, problemConditions.Applications);

            for (int i = 0; Population.Count > 0; i++)
            {
                if (i % checkDelay == 0)
                {
                    if (ChromosomeOperations.ChromosomesDiversity(Population) / diversityRandom < geneticDiversity)
                        break;
                }

                EvaluatedChromosome currentApplicant = evaluator.EvaluateChromosome(Population[0], problemConditions);

                if (currentApplicant.Value < Best.Value)
                {
                    Best = currentApplicant;
                    Console.WriteLine(Best);
                }

                GenerateNewPopulation(chromosomeFactory, parentPairsSelector, crossoverOperator, mutator, restriction, evaluator, newPopulationGenerator);
            }
        }
    }
}
