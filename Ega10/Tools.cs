namespace Ega10
{
    internal static class Tools
    {
        public struct IndexValuePair(int index, double value) : IComparable
        {
            public int Index { get; private set; } = index;
            public double Value { get; set; } = value;

            public readonly int CompareTo(object? obj)
            {
                if (obj is IndexValuePair pair)
                    return Value.CompareTo(pair.Value);

                throw new ArgumentException("Inccorect type comparason");
            }
        }

        public struct EvaluatedApplicant(int[] genes, double value) : IComparable
        {
            public int[] Genes { get; private set; } = genes; //Applications permutation
            public double Value { get; set; } = value;

            public readonly int CompareTo(object? obj)
            {
                if (obj is EvaluatedApplicant pair)
                    return Value.CompareTo(pair.Value);

                throw new ArgumentException("Inccorect type comparason");
            }

            public override readonly string ToString()
            {
                return $"{Value}: {ArrayToString(Genes)}";
            }
        }

        public struct Applicant(int[] genes)
        {
            public int[] Genes { get; private set; } = genes; //Applications permutation

            public override readonly string ToString()
            {
                return $"{ArrayToString(Genes)}";
            }
        }


        public static Random Random { get; } = new(1);


        public static string GetLongBinar(int number, int bitsToRepresent)
        {
            string shortBase2 = Convert.ToString(number, toBase: 2);
            string longBase2 = string.Empty;

            int shortBase2Length = shortBase2.Length;

            if (shortBase2Length < bitsToRepresent)
            {
                for (int i = 0; i < bitsToRepresent - shortBase2Length; i++)
                    longBase2 += '0';
            }

            return longBase2 + shortBase2;
        }

        public static bool AlreadyHaveTheseGenes(List<Applicant> applicants, int[] genes)
        {
            foreach (Applicant applicant in applicants)
            {
                if (genes.SequenceEqual(applicant.Genes))
                    return true;
            }

            return false;
        }

        public static void PrintCycles(List<int[]> permutationCycles)
        {
            for (int i = 0; i < permutationCycles.Count; i++)
            {
                Console.WriteLine($"[{string.Join(", ", permutationCycles[i])}]");
            }
        }

        public static string ArrayToString(int[] array)
        {
            string arrayString = string.Empty;

            for (int i = 0; i < array.Length; i++)
            {
                arrayString += $"{(array[i] < 10 ? ' ' : string.Empty)}{array[i]} ";
            }

            return arrayString;
        }

        public static void PrintInitialConditions(int applications, int machines, int[][] executionTimes, int[] dueTimes, int[] penaltyMultiplyers)
        {
            Console.Clear();

            Console.WriteLine($"Applications: {applications}");
            Console.WriteLine($"Machines: {machines}");

            Console.WriteLine("Execution times:");
            for (int m = 0; m < executionTimes.Length; m++)
            {
                for (int a = 0; a < executionTimes[m].Length; a++)
                {
                    Console.Write($"{(executionTimes[m][a] < 10 ? ' ' : string.Empty)}{executionTimes[m][a]} ");
                }
                Console.WriteLine();
            }
            Console.WriteLine();

            Console.Write("Due times: ");
            for (int d = 0; d < dueTimes.Length; d++)
            {
                Console.Write($"{dueTimes[d]} ");
            }
            Console.WriteLine();

            Console.Write("Penalty multiplyers: ");
            for (int p = 0; p < penaltyMultiplyers.Length; p++)
            {
                Console.Write($"{penaltyMultiplyers[p]} ");
            }
            Console.WriteLine();

            Console.WriteLine("---------------------------------------------------------------------");
        }

        public static int[] EncodePermutation(int[] permutationValues)
        {
            int permutationLength = permutationValues.Length;

            Dictionary<int, int> permutation = new(permutationLength);

            Dictionary<int, int> basePermutation = new(permutationLength); //0 through n - 1 mapped to 0 through n - 1
            int[] encodedPermutation = new int[permutationLength];

            for (int i = 0; i < permutationLength; i++)
            {
                permutation[i] = permutationValues[i];
                basePermutation[i] = i;
            }

            for (int i = 0; i < permutationLength; i++)
            {
                encodedPermutation[i] = basePermutation[permutation[i]];
                basePermutation[permutation[i]] = -1;

                for (int j = 0, k = 0; j < permutationLength; j++)
                {
                    if (basePermutation[j] != basePermutation[permutation[i]])
                    {
                        basePermutation[j] = k++;
                    }
                }
            }

            return encodedPermutation;
        }

        //can return array insted of dictionary, because all indices in it are sorted (0 through n - 1)
        public static int[] DecodePermutation(int[] encodedPermutation)
        {
            int permutationLength = encodedPermutation.Length;

            int[] decodedPermutation = new int[permutationLength];

            Dictionary<int, int> basePermutation = new(permutationLength); //0 through n - 1 mapped to 0 through n - 1

            for (int i = 0; i < permutationLength; i++)
            {
                basePermutation[i] = i;
            }

            for (int i = 0; i < permutationLength; i++)
            {
                int key = basePermutation.FirstOrDefault(pair => pair.Value == encodedPermutation[i]).Key;

                decodedPermutation[i] = key;
                basePermutation[key] = -1;

                for (int j = 0, k = 0; j < permutationLength; j++)
                {
                    if (basePermutation[j] != basePermutation[key])
                    {
                        basePermutation[j] = k++;
                    }
                }
            }

            return decodedPermutation;
        }
    }
}
