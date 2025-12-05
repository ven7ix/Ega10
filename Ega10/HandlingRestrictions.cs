namespace Ega10
{
    internal class HandlingRestrictions //2
    {
        public static List<IApplicant> KILLSAME(List<IApplicant> applicants)
        {
            return [.. applicants.Distinct()];
        }


        private static IApplicant ApplicantMODIFY(IApplicant applicant, Func<int[], IApplicant> applicantFactory)
        {
            int[] genes = applicant.Genes;
            List<int> uniqueGenes = [];
            List<int> newGenes = [];

            for (int gen = 0; gen < genes.Length; gen++)
            {
                if (!genes.Contains(gen))
                {
                    newGenes.Add(gen);
                }
            }

            for (int gen = 0, newGen = 0; gen < genes.Length; gen++)
            {
                if (uniqueGenes.Contains(genes[gen]))
                {
                    genes[gen] = newGenes[newGen++];
                }

                uniqueGenes.Add(genes[gen]);
            }

            var newApplicant = applicantFactory(genes);
            return newApplicant;
        }

        public static List<IApplicant> MODIFY(List<IApplicant> applicants, Func<int[], IApplicant> applicantFactory)
        {
            List<IApplicant> handledApplicants = [];

            foreach (IApplicant applicant in applicants)
            {
                handledApplicants.Add(ApplicantMODIFY(applicant, applicantFactory));
            }

            return KILLSAME(handledApplicants);
        }


        public static List<IApplicant> ELIMINATE(List<IApplicant> applicants)
        {
            List<IApplicant> handledApplicants = [];

            foreach (IApplicant applicant in applicants)
            {
                if (applicant.ValidGenes)
                {
                    handledApplicants.Add(applicant);
                }
            }

            return KILLSAME(handledApplicants);
        }


        public static List<IApplicant> DECODE(List<IApplicant> applicants)
        {
            List<IApplicant> handledApplicants = [];

            return KILLSAME(handledApplicants);
        }
    } 
}
