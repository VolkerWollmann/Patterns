using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

// Source : https://en.wikipedia.org/wiki/Genetic_algorithm
// Genetic search is a metaheuristic inspired by natural selection. A population of
// candidate solutions is scored by a fitness function; the fittest individuals are
// selected, recombined (crossover) and randomly altered (mutation) to breed the next
// generation. Repeating that cycle drives the population towards an optimum without
// the search ever knowing how to construct the solution directly.

namespace Patterns.AlgorithmicPatterns
{
    // One individual of the population together with the score it achieved.
    public class GeneticCandidate
    {
        public GeneticCandidate(string chromosome, int fitness)
        {
            Chromosome = chromosome;
            Fitness = fitness;
        }

        // The candidate solution itself - one gene per position.
        public string Chromosome { get; }

        // How many genes already sit at the right position.
        public int Fitness { get; }

        public override string ToString()
        {
            return $"{Chromosome} ({Fitness})";
        }
    }

    // A snapshot of a search: the fittest candidates of a generation. Watching more
    // than the single best one shows how the whole population moves towards the
    // target - and how much diversity is left to breed from.
    public class GeneticSearchResult
    {
        public GeneticSearchResult(IReadOnlyList<GeneticCandidate> top, int generation, bool solved)
        {
            Top = top;
            Generation = generation;
            Solved = solved;
        }

        // The fittest candidates, strongest first.
        public IReadOnlyList<GeneticCandidate> Top { get; }

        // The generation this snapshot was taken in.
        public int Generation { get; }

        // True once the fittest candidate matches the target exactly.
        public bool Solved { get; }

        public GeneticCandidate Fittest => Top[0];

        public string Best => Top[0].Chromosome;

        public int Fitness => Top[0].Fitness;

        public override string ToString()
        {
            return $"Generation {Generation,4}: {string.Join(" | ", Top)}";
        }
    }

    public class GeneticSearchExample
    {
        // The alphabet a candidate's genes are drawn from.
        public const string Genes = "ABCDEFGHIJKLMNOPQRSTUVWXYZ ";

        // Evolves a population of random strings towards the target.
        // Passing a seeded Random makes a run reproducible; onImprovement reports
        // every generation that beats the best fitness seen so far. topCount decides
        // how many of the fittest candidates a snapshot carries - raise it to
        // populationSize to watch the entire population.
        public static GeneticSearchResult Search(
            string target,
            int populationSize = 200,
            double mutationRate = 0.02,
            int maxGenerations = 1000,
            int topCount = 3,
            Random? random = null,
            Action<GeneticSearchResult>? onImprovement = null)
        {
            if (string.IsNullOrEmpty(target))
                throw new ArgumentException("A target is required.", nameof(target));

            random ??= new Random();
            topCount = Math.Clamp(topCount, 1, populationSize);

            // Generation 0 is nothing but noise - the search has to find its own way.
            List<string> population = Enumerable.Range(0, populationSize)
                .Select(_ => RandomCandidate(target.Length, random))
                .ToList();

            int bestFitnessSoFar = -1;

            for (int generation = 1; generation <= maxGenerations; generation++)
            {
                // Rank the population so the fittest individuals sit at the front.
                List<GeneticCandidate> ranked = Rank(population, target);
                GeneticCandidate best = ranked[0];
                bool solved = best.Fitness == target.Length;

                if (best.Fitness > bestFitnessSoFar)
                {
                    bestFitnessSoFar = best.Fitness;
                    onImprovement?.Invoke(Snapshot(ranked, topCount, generation, solved));
                }

                if (solved)
                    return Snapshot(ranked, topCount, generation, true);

                // Elitism: the strongest individuals survive unchanged, so a good
                // solution can never be lost again.
                int eliteCount = Math.Max(1, populationSize / 10);
                List<string> nextGeneration = ranked
                    .Take(eliteCount)
                    .Select(candidate => candidate.Chromosome)
                    .ToList();

                // The rest of the next generation is bred from parents drawn out of
                // the fitter half of the current one.
                int matingPoolSize = Math.Max(2, populationSize / 2);
                while (nextGeneration.Count < populationSize)
                {
                    string parentA = ranked[random.Next(matingPoolSize)].Chromosome;
                    string parentB = ranked[random.Next(matingPoolSize)].Chromosome;

                    nextGeneration.Add(Mutate(Crossover(parentA, parentB, random), mutationRate, random));
                }

                population = nextGeneration;
            }

            // Out of generations - report how close the search got.
            return Snapshot(Rank(population, target), topCount, maxGenerations, false);
        }

        // The fitness function: how many genes already sit at the right position.
        public static int Fitness(string candidate, string target)
        {
            int score = 0;

            for (int i = 0; i < target.Length; i++)
            {
                if (candidate[i] == target[i])
                    score++;
            }

            return score;
        }

        // Scores the whole population and sorts it, fittest first.
        private static List<GeneticCandidate> Rank(List<string> population, string target)
        {
            return population
                .Select(candidate => new GeneticCandidate(candidate, Fitness(candidate, target)))
                .OrderByDescending(candidate => candidate.Fitness)
                .ToList();
        }

        private static GeneticSearchResult Snapshot(
            List<GeneticCandidate> ranked, int topCount, int generation, bool solved)
        {
            return new GeneticSearchResult(ranked.Take(topCount).ToList(), generation, solved);
        }

        // A candidate of the right length, assembled from random genes.
        private static string RandomCandidate(int length, Random random)
        {
            StringBuilder candidate = new StringBuilder(length);

            for (int i = 0; i < length; i++)
            {
                candidate.Append(Genes[random.Next(Genes.Length)]);
            }

            return candidate.ToString();
        }

        // Single point crossover: the head of one parent, the tail of the other.
        private static string Crossover(string parentA, string parentB, Random random)
        {
            int cut = random.Next(1, parentA.Length);

            return parentA.Substring(0, cut) + parentB.Substring(cut);
        }

        // Mutation keeps fresh genes in the pool, so the search cannot get stuck on a
        // population that has become too uniform to improve.
        private static string Mutate(string candidate, double mutationRate, Random random)
        {
            char[] genes = candidate.ToCharArray();

            for (int i = 0; i < genes.Length; i++)
            {
                if (random.NextDouble() < mutationRate)
                    genes[i] = Genes[random.Next(Genes.Length)];
            }

            return new string(genes);
        }

        public static void GeneticSearch()
        {
            const string target = "CAROLIN NOTHEIS";

            Console.WriteLine($"Searching for \"{target}\" - starting from random noise.");
            Console.WriteLine("Each line shows the three fittest candidates, matching genes in brackets.");

            GeneticSearchResult result = Search(target, onImprovement: snapshot => Console.WriteLine(snapshot));

            if (result.Solved)
                Console.WriteLine($"Found \"{result.Best}\" after {result.Generation} generations.");
            else
                Console.WriteLine($"Gave up after {result.Generation} generations, best was \"{result.Best}\".");
        }
    }
}
