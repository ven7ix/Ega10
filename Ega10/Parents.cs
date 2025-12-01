using static Ega10.Tools;

namespace Ega10
{
    internal static class Parents
    {
        private const int MaxDistance = 40;
        private const int MinDistance = 40;

        [Obsolete]
        public static List<Applicant> PickRANDOM_OLD(List<Applicant> population)
        {
            int parentPairsAmount = population.Count / 2;
            List<Applicant> children = new(parentPairsAmount);

            for (int i = 0; i < parentPairsAmount; i++)
            {
                int firstParentID = Tools.Random.Next(0, population.Count);
                Applicant firstParent = new(EncodePermutation(population[firstParentID].Genes));
                population.RemoveAt(firstParentID);

                int secondParentID = Tools.Random.Next(0, population.Count);
                Applicant secondParent = new(EncodePermutation(population[secondParentID].Genes));
                population.RemoveAt(secondParentID);

                children = [.. children, .. Crossover.CrossoverORDINAL(firstParent, secondParent)];
            }

            return children;
        }

        public static List<Tuple<Applicant, Applicant>> PickRANDOM(List<Applicant> population)
        {
            int parentPairsAmount = population.Count / 2;
            List<Tuple<Applicant, Applicant>> parentPairs = new(parentPairsAmount);

            for (int i = 0; i < parentPairsAmount; i++)
            {
                int firstParentID = Tools.Random.Next(0, population.Count);
                Applicant firstParent = new(EncodePermutation(population[firstParentID].Genes));
                population.RemoveAt(firstParentID);

                int secondParentID = Tools.Random.Next(0, population.Count);
                Applicant secondParent = new(EncodePermutation(population[secondParentID].Genes));
                population.RemoveAt(secondParentID);

                parentPairs.Add(new Tuple<Applicant, Applicant>(firstParent, secondParent));
            }

            return parentPairs;
        }


        private static Applicant TryGetPartnerINBREEDING(List<Applicant> population, Applicant firstParent, int maxDistance)
        {
            Applicant partner;

            for (int p = 0; p < population.Count; p++)
            {
                partner = new(EncodePermutation(population[p].Genes));

                if (DistanceBetweenPermutations(firstParent.Genes, partner.Genes) < maxDistance)
                {
                    population.RemoveAt(p);

                    return partner;
                }
            }

            int partnerID = Tools.Random.Next(0, population.Count);
            partner = new(EncodePermutation(population[partnerID].Genes));
            population.RemoveAt(partnerID);

            return partner;
        }

        public static List<Tuple<Applicant, Applicant>> PickINBREEDING(List<Applicant> population, int maxDistance = MaxDistance)
        {
            List<Tuple<Applicant, Applicant>> parentPairs = [];

            for (int i = 0; i < population.Count; i += 2)
            {
                int firstParentID = Tools.Random.Next(0, population.Count);
                Applicant firstParent = new(EncodePermutation(population[firstParentID].Genes));
                population.RemoveAt(firstParentID);

                parentPairs.Add(new Tuple<Applicant, Applicant>(firstParent, TryGetPartnerINBREEDING(population, firstParent, maxDistance)));
            }

            return parentPairs;
        }


        private static Applicant TryGetPartnerOUTBREEDING(List<Applicant> population, Applicant firstParent, int minDistance)
        {
            Applicant partner;

            for (int p = 0; p < population.Count; p++)
            {
                partner = new(EncodePermutation(population[p].Genes));

                if (DistanceBetweenPermutations(firstParent.Genes, partner.Genes) > minDistance)
                {
                    population.RemoveAt(p);

                    return partner;
                }
            }

            int partnerID = Tools.Random.Next(0, population.Count);
            partner = new(EncodePermutation(population[partnerID].Genes));
            population.RemoveAt(partnerID);

            return partner;
        }

        public static List<Tuple<Applicant, Applicant>> PickOUTBREEDING(List<Applicant> population, int minDistance = MinDistance)
        {
            List<Tuple<Applicant, Applicant>> parentPairs = [];

            for (int i = 0; i < population.Count; i += 2)
            {
                int firstParentID = Tools.Random.Next(0, population.Count);
                Applicant firstParent = new(EncodePermutation(population[firstParentID].Genes));
                population.RemoveAt(firstParentID);

                parentPairs.Add(new Tuple<Applicant, Applicant>(firstParent, TryGetPartnerOUTBREEDING(population, firstParent, minDistance)));
            }

            return parentPairs;
        }
    }
}
