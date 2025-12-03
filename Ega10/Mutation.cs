using static Ega10.Tools;

namespace Ega10
{
    internal static class Mutation //2
    {
        public static List<Applicant> MutateDONT(List<Applicant> children)
        {
            return children;
        }

        public static List<Applicant> MutateRANDOMCONTOL(List<Applicant> children)
        {
            List<Applicant> mutatedChildren = [];

            foreach (Applicant child in children)
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

        public static List<Applicant> MutateRANDOM(List<Applicant> children)
        {
            List<Applicant> mutatedChildren = [];

            foreach (Applicant child in children)
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

        public static List<Applicant> MutateCOMPLEMENT(List<Applicant> children)
        {
            List<Applicant> mutatedChildren = [];

            return mutatedChildren;
        }
    }
}
