namespace Ega10
{
    internal class Solution(ProblemConditions problemConditions, int populationSize, bool doElite = false, int printStep = 20)
    {
        private List<IChromosome> Population { get; set; } = new List<IChromosome>(populationSize);
        private EvaluatedChromosome Best { get; set; } = new EvaluatedChromosome([], int.MaxValue);

        private void PrintStep(int step, IEvaluator evaluator)
        {
            if (step % printStep != 0)
                return;

            Console.WriteLine($"STEP: {step}");

            Console.WriteLine($"BEST: {Best}");

            Console.WriteLine("POPULATION:");
            for (int i = 0; i < Population.Count; i++)
            {
                Console.WriteLine(evaluator.EvaluateChromosome(Population[i], problemConditions));
            }
            Console.WriteLine();
        }

        private void GenerateNewPopulation(
            in IChromosomeFactory chromosomeFactory,
            in IParentPairsSelector parentPairsSelector,
            in ICrossoverOperator crossoverOperator,
            in IMutator mutator,
            in IRestriction restriction,
            in IEvaluator evaluator,
            in ISelector selector,
            in INewPopulationGenerator newPopulationGenerator)
        {
            List<EvaluatedChromosome> children;

            if (doElite)
            {
                children =
                    selector.Select(
                        evaluator.EvaluateChromosomes([.. Population.Union(
                            restriction.Apply(
                                mutator.Mutate(
                                    crossoverOperator.CrossoverPairs(
                                        parentPairsSelector.Select(Population)))))], problemConditions), populationSize);
            }
            else
            {
                children =
                    selector.Select(
                        evaluator.EvaluateChromosomes(
                            restriction.Apply(
                                mutator.Mutate(
                                    crossoverOperator.CrossoverPairs(
                                        parentPairsSelector.Select(Population)))), problemConditions), populationSize);
            }

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
            in ISelector selector,
            in INewPopulationGenerator newPopulationGenerator,
            int maxIterations)
        {
            Population = initialPopalionGenerator.Generate(chromosomeFactory, populationSize, problemConditions.Applications);

            for (int i = 0; i < maxIterations && Population.Count > 0; i++)
            {
                EvaluatedChromosome currentApplicant = evaluator.EvaluateChromosome(Population[0], problemConditions);

                if (currentApplicant.Value < Best.Value)
                {
                    Best = currentApplicant;
                }

                PrintStep(i, evaluator);
                GenerateNewPopulation(chromosomeFactory, parentPairsSelector, crossoverOperator, mutator, restriction, evaluator, selector, newPopulationGenerator);
            }

            Console.WriteLine($"BEST: {Best}");
        }

        public void SolveSameBest(
            in IChromosomeFactory chromosomeFactory,
            in IInitialPopalionGenerator initialPopalionGenerator,
            in IParentPairsSelector parentPairsSelector,
            in ICrossoverOperator crossoverOperator,
            in IMutator mutator,
            in IRestriction restriction,
            in IEvaluator evaluator,
            in ISelector selector,
            in INewPopulationGenerator newPopulationGenerator,
            int maxIterations)
        {
            Population = initialPopalionGenerator.Generate(chromosomeFactory, populationSize, problemConditions.Applications);

            for (int i = 0; i < maxIterations && Population.Count > 0; i++)
            {
                EvaluatedChromosome currentApplicant = evaluator.EvaluateChromosome(Population[0], problemConditions);

                if (currentApplicant.Value < Best.Value)
                {
                    i = 0;
                    Best = currentApplicant;
                }

                PrintStep(i, evaluator);
                GenerateNewPopulation(chromosomeFactory, parentPairsSelector, crossoverOperator, mutator, restriction, evaluator, selector, newPopulationGenerator);
            }

            Console.WriteLine($"BEST: {Best}");
        }

        public void SolveGeneticDiversity(
            in IChromosomeFactory chromosomeFactory,
            in IInitialPopalionGenerator initialPopalionGenerator,
            in IParentPairsSelector parentPairsSelector,
            in ICrossoverOperator crossoverOperator,
            in IMutator mutator,
            in IRestriction restriction,
            in IEvaluator evaluator,
            in ISelector selector,
            in INewPopulationGenerator newPopulationGenerator,
            double geneticDiversity, int checkDelay)
        {
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

                PrintStep(i, evaluator);
                GenerateNewPopulation(chromosomeFactory, parentPairsSelector, crossoverOperator, mutator, restriction, evaluator, selector, newPopulationGenerator);
            }

            Console.WriteLine($"BEST: {Best}");
        }
    }
}
