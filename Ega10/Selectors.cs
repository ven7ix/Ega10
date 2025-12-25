namespace Ega10
{
    internal interface ISelector 
    {
        List<EvaluatedChromosome> Select(in List<EvaluatedChromosome> evaluatedChromosomes, int populationSize);
    }

    internal class SelectorDefault : ISelector
    {
        public List<EvaluatedChromosome> Select(in List<EvaluatedChromosome> evaluatedChromosomes, int populationSize)
        {
            var selectedCromosomes = new List<EvaluatedChromosome>(evaluatedChromosomes);

            selectedCromosomes.Sort();

            return selectedCromosomes.GetRange(0, Math.Min(populationSize, selectedCromosomes.Count));
        }
    }

    internal class SelectorRandom(double selectionChance) : ISelector
    {
        public List<EvaluatedChromosome> Select(in List<EvaluatedChromosome> evaluatedChromosomes, int populationSize)
        {
            var selectedChromosomes = new List<EvaluatedChromosome>(populationSize);

            for (int i = 0; i < evaluatedChromosomes.Count; i++)
            {
                if (Tools.Random.NextDouble() >= selectionChance)
                    continue;

                selectedChromosomes.Add(evaluatedChromosomes[i]);
            }

            selectedChromosomes.Sort();

            return selectedChromosomes.GetRange(0, Math.Min(populationSize, selectedChromosomes.Count));
        }
    }

    internal class SelectorBetaTournament(int tournamentSpaces) : ISelector
    {
        public List<EvaluatedChromosome> Select(in List<EvaluatedChromosome> evaluatedChromosomes, int populationSize)
        {
            var selectedChromosomes = new List<EvaluatedChromosome>(evaluatedChromosomes.Count);
            var competingChromosomes = new List<EvaluatedChromosome>(tournamentSpaces);

            List<int> availableIndices = [.. Enumerable.Range(0, evaluatedChromosomes.Count)];

            for (int i = 0; i < evaluatedChromosomes.Count; i++)
            {
                int index = Tools.Random.Next(0, availableIndices.Count);
                int applicantIndex = availableIndices[index];
                availableIndices.RemoveAt(index);

                competingChromosomes.Add(evaluatedChromosomes[applicantIndex]);

                if (competingChromosomes.Count >= tournamentSpaces)
                {
                    selectedChromosomes.Add(competingChromosomes.Max());
                    competingChromosomes.Clear();
                }
            }

            selectedChromosomes.Sort();

            return selectedChromosomes.GetRange(0, Math.Min(populationSize, selectedChromosomes.Count));
        }
    }
}
