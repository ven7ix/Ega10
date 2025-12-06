namespace Ega10
{
    internal class HandlingRestrictions //2
    {
        private static List<IApplicant> KILLSAME(in List<IApplicant> applicants)
        {
            return [.. applicants.Distinct()];
        }


        private static IApplicant ApplicantMODIFY(in IApplicant applicant, Func<int[], IApplicant> applicantFactory)
        {
            int genesCount = applicant.Genes.Length;

            int[] genes = new int[genesCount];
            Array.Copy(applicant.Genes, genes, genesCount);

            List<int> uniqueGenes = [];
            List<int> newGenes = [];

            for (int gen = 0; gen < genesCount; gen++)
            {
                if (!genes.Contains(gen))
                {
                    newGenes.Add(gen);
                }
            }

            for (int gen = 0, newGen = 0; gen < genesCount; gen++)
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

        /// <summary>
        /// Modifies all <paramref name="applicants"/> with invalid genes. Works like trash with <see cref="ApplicantOrdinal"/>
        /// </summary>
        /// <param name="applicants">Children</param>
        /// <param name="applicantFactory">How applicants will be created</param>
        /// <returns>List of applicants with modified genes, which are valid</returns>
        public static List<IApplicant> MODIFY(in List<IApplicant> applicants, Func<int[], IApplicant> applicantFactory)
        {
            int applicantsCount = applicants.Count;
            var handledApplicants = new List<IApplicant>(applicantsCount);

            for (int i = 0; i < applicantsCount; i++)
            {
                handledApplicants.Add(ApplicantMODIFY(applicants[i], applicantFactory));
            }

            return KILLSAME(handledApplicants);
        }


        /// <summary>
        /// Eliminates all <paramref name="applicants"/> with invalid genes
        /// </summary>
        /// <param name="applicants">Children</param>
        /// <returns>List of applicants, excluding those with invalid genes</returns>
        public static List<IApplicant> ELIMINATE(in List<IApplicant> applicants)
        {
            int applicantsCount = applicants.Count;
            var handledApplicants = new List<IApplicant>(applicantsCount);

            for (int i = 0; i < applicantsCount; i++)
            {
                if (applicants[i].ValidGenes)
                {
                    handledApplicants.Add(applicants[i]);
                }
            }

            return KILLSAME(handledApplicants);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="applicants"></param>
        /// <returns></returns>
        public static List<IApplicant> DECODE(in List<IApplicant> applicants, Func<int[], IApplicant> applicantFactory)
        {
            int applicantsCount = applicants.Count;
            var handledApplicants = new List<IApplicant>(applicantsCount);

            for (int i = 0; i < applicantsCount; i++)
            {
                if (applicants[i].ValidGenes)
                {
                    handledApplicants.Add(applicants[i]);
                    continue;
                }

                IApplicant applicant = applicantFactory(ApplicantOrdinal.Decode(applicants[i].Genes));

                if (applicant.ValidGenes)
                {
                    handledApplicants.Add(applicant);
                }
            }

            return KILLSAME(handledApplicants);
        }
    } 
}
