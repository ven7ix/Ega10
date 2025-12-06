namespace Ega10
{
    internal static class Tools
    {
        public static Random Random { get; } = new(0);
    }

    internal class Program
    {
        public static void Main()
        {
            Solution.SolveGeneticDiversity(genes => new ApplicantCyclic(genes), 0.55f, 10);
        }
    }
}
