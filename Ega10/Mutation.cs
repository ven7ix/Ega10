namespace Ega10
{
    internal static class Mutation //3
    {
        /// <summary>
        /// Does not alter <paramref name="children"/>
        /// </summary>
        /// <param name="children">Children</param>
        /// <returns><paramref name="children"/></returns>
        public static List<IApplicant> MutateDONT(in List<IApplicant> children)
        {
            return children;
        }

        /// <summary>
        /// Mutates <paramref name="children"/> randomly, but in a controlled manner. Cannot generate invalid permutations
        /// </summary>
        /// <param name="children">Children</param>
        /// <returns>Mutated children</returns>
        public static List<IApplicant> MutateRANDOMCONTOL(in List<IApplicant> children, int childMutationChance, int genMutationChance)
        {
            int childrenCount = children.Count;
            var mutatedChildren = new List<IApplicant>(childrenCount);

            for (int i = 0; i < childrenCount; i++)
            {
                if (Tools.Random.Next(0, childMutationChance) != 0)
                    continue;

                int genesCount = children[i].Genes.Length;

                for (int gen = 0; gen < genesCount; gen++)
                {
                    if (Tools.Random.Next(0, genMutationChance) != 0)
                        continue;

                    int gen1 = Tools.Random.Next(0, genesCount);
                    int gen2 = Tools.Random.Next(0, genesCount);

                    (children[i].Genes[gen1], children[i].Genes[gen2]) = (children[i].Genes[gen2], children[i].Genes[gen1]);
                }

                mutatedChildren.Add(children[i]);
            }

            return mutatedChildren;
        }

        /// <summary>
        /// Completely randomly mutates <paramref name="children"/>
        /// </summary>
        /// <param name="children">Children</param>
        /// <returns>Completely randomly mutated <paramref name="children"/></returns>
        public static List<IApplicant> MutateRANDOM(in List<IApplicant> children, int childMutationChance, int genMutationChance)
        {
            int childrenCount = children.Count;
            var mutatedChildren = new List<IApplicant>(childrenCount);

            for (int i = 0; i < childrenCount; i++)
            {
                if (Tools.Random.Next(0, childMutationChance) != 0)
                    continue;

                int genesCount = children[i].Genes.Length;

                for (int gen = 0; gen < genesCount; gen++)
                {
                    if (Tools.Random.Next(0, genMutationChance) != 0)
                        continue;

                    int mutatedGen = Tools.Random.Next(0, genesCount);
                    children[i].Genes[mutatedGen] = children[i].Genes[Tools.Random.Next(0, genesCount)];
                }

                mutatedChildren.Add(children[i]);
            }

            return mutatedChildren;
        }

        /// <summary>
        /// Mutates <paramref name="children"/> genes by changing their value to complement
        /// </summary>
        /// <param name="children">Children</param>
        /// <returns>Mutated by complementary <paramref name="children"/></returns>
        public static List<IApplicant> MutateCOMPLEMENT(in List<IApplicant> children, int childMutationChance, int genMutationChance)
        {
            int childrenCount = children.Count;
            var mutatedChildren = new List<IApplicant>(childrenCount);

            for (int i = 0; i < childrenCount; i++)
            {
                if (Tools.Random.Next(0, childMutationChance) != 0)
                {
                    continue;
                }

                int genesCount = children[i].Genes.Length;

                for (int gen = 0; gen < genesCount; gen++)
                {
                    if (Tools.Random.Next(0, genMutationChance) != 0)
                    {
                        continue;
                    }

                    children[i].Genes[gen] = genesCount - 1 - children[i].Genes[gen];
                }

                mutatedChildren.Add(children[i]);
            }

            return mutatedChildren;
        }
    }
}
