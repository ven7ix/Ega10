using static Ega10.Tools;

namespace Ega10
{
    internal class HandlingRestrictions //2
    {
        public static List<Applicant> KILLSAME(List<Applicant> applicants)
        {
            return [.. applicants.Distinct()];
        }


        public static Applicant ApplicantMODIFY(Applicant applicant)
        {
            int[] genes = DecodePermutation(applicant.Genes);
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

            return new Applicant(EncodePermutation(genes));
        }

        public static List<Applicant> MODIFY(List<Applicant> applicants)
        {
            List<Applicant> handledApplicants = [];

            foreach (Applicant applicant in applicants)
            {
                handledApplicants.Add(ApplicantMODIFY(applicant));
            }

            return KILLSAME(handledApplicants);
        }


        public static List<Applicant> ELIMINATE(List<Applicant> applicants)
        {
            List<Applicant> handledApplicants = [];

            foreach (Applicant applicant in applicants)
            {
                if (RightPemutation(DecodePermutation(applicant.Genes)))
                {
                    handledApplicants.Add(applicant);
                }
            }

            return KILLSAME(handledApplicants);
        }

        
        public static List<Applicant> DECODE(List<Applicant> applicants)
        {
            List<Applicant> handledApplicants = [];

            return KILLSAME(handledApplicants);
        }
    } 
}
