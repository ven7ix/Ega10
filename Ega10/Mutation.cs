using static Ega10.Tools;

namespace Ega10
{
    internal static class Mutation //3
    {
        public static List<IApplicant> MutateDONT(List<IApplicant> children)
        {
            return children;
        }

        public static List<IApplicant> MutateRANDOMCONTOL(List<IApplicant> children)
        {
            List<IApplicant> mutatedChildren = [];

            foreach (IApplicant child in children)
            {
                int genMutations = Tools.Random.Next(0, child.Genes.Length);

                for (int m = 0; m < genMutations; m++)
                {
                    int gen1 = Tools.Random.Next(0, child.Genes.Length);
                    int gen2 = Tools.Random.Next(0, child.Genes.Length);

                    (child.Genes[gen1], child.Genes[gen2]) = (child.Genes[gen2], child.Genes[gen1]);
                }

                mutatedChildren.Add(child);
            }

            return mutatedChildren;
        }

        public static List<IApplicant> MutateRANDOM(List<IApplicant> children)
        {
            List<IApplicant> mutatedChildren = [];

            foreach (IApplicant child in children)
            {
                int genMutations = Tools.Random.Next(0, child.Genes.Length);

                for (int m = 0; m < genMutations; m++)
                {
                    int mutatedGen = Tools.Random.Next(0, child.Genes.Length);

                    child.Genes[mutatedGen] = child.Genes[Tools.Random.Next(0, child.Genes.Length)];
                }

                mutatedChildren.Add(child);
            }

            return mutatedChildren;
        }

        public static List<IApplicant> MutateCOMPLEMENT(List<IApplicant> children)
        {
            List<IApplicant> mutatedChildren = [];
            
            foreach (IApplicant child in children)
            {
                for (int gen = 0; gen < child.Genes.Length; gen++)
                {
                    if (Tools.Random.Next(0, 2) == 0)
                        child.Genes[gen] = child.Genes.Length - 1 - child.Genes[gen];

                    mutatedChildren.Add(child);
                }
            }

            return mutatedChildren;
        }
    }
}
