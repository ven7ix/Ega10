using static Ega10.Tools;

namespace Ega10
{
    internal static class Mutation
    {
        public static List<Applicant> MutateDONT(List<Applicant> children)
        {
            return children;
        }

        public static List<Applicant> MutateRANDOM(List<Applicant> children)
        {
            List<Applicant> mutatedChildren = [];

            foreach (Applicant child in children)
            {
                int genMutations = Tools.Random.Next(0, child.Genes.Length);

                for (int i = 0; i < genMutations; i++)
                {
                    int gen1 = Tools.Random.Next(0, child.Genes.Length);
                    int gen2 = Tools.Random.Next(0, child.Genes.Length);

                    (child.Genes[gen1], child.Genes[gen2]) = (child.Genes[gen2], child.Genes[gen1]);
                }

                mutatedChildren.Add(child);
            }

            return mutatedChildren;
        }
    }
}
