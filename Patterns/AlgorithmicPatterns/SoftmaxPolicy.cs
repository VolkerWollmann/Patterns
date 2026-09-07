using System;
using System.Collections.Generic;
using System.Linq;

// Source : https://en.wikipedia.org/wiki/Multi-armed_bandit
//          https://en.wikipedia.org/wiki/Softmax_function
// A learning strategy has to solve the same dilemma every time: exploit what already
// looks best, or explore what might turn out better. A greedy policy always orders the
// current favourite and can spend a lifetime on a dish that just happened to be good
// the first time; choosing at random tastes everything and enjoys little.
// Softmax sits in between. It turns the value estimates into one probability per choice,
// P(a) = exp(Q(a)/tau) / sum exp(Q(b)/tau), so everything on the menu stays reachable
// while the better dishes are ordered far more often. The temperature tau decides how
// sharp that gets: a large tau spreads the probabilities out towards uniform, a small
// one concentrates them on the favourite. Cooling tau while learning tries a lot early
// and settles down later.

namespace Patterns.AlgorithmicPatterns
{
    // One dish on the menu, together with how often the kitchen actually gets it right.
    // That chance is the hidden truth: the guest only ever sees the plate in front of them.
    public class Dish
    {
        public Dish(string name, double chanceOfBeingGood)
        {
            if (chanceOfBeingGood < 0.0 || chanceOfBeingGood > 1.0)
                throw new ArgumentOutOfRangeException(nameof(chanceOfBeingGood), "A chance has to be in [0, 1].");

            Name = name;
            ChanceOfBeingGood = chanceOfBeingGood;
        }

        public string Name { get; }

        // How likely this dish is to be any good today.
        public double ChanceOfBeingGood { get; }

        public override string ToString()
        {
            return $"{Name} ({ChanceOfBeingGood:0.00})";
        }
    }

    // One lunch, together with what the guest believed at the time - so the shift from
    // trying things out to sticking with a favourite can be watched from outside.
    public class SoftmaxLearningStep
    {
        public SoftmaxLearningStep(
            int day,
            int choice,
            double reward,
            double temperature,
            IReadOnlyList<string> menu,
            IReadOnlyList<double> probabilities,
            IReadOnlyList<double> estimates)
        {
            Day = day;
            Choice = choice;
            Reward = reward;
            Temperature = temperature;
            Menu = menu;
            Probabilities = probabilities;
            Estimates = estimates;
        }

        public int Day { get; }

        // What the policy ordered.
        public int Choice { get; }

        // 1 for a good lunch, 0 for a disappointing one.
        public double Reward { get; }

        // The temperature the choice was made at.
        public double Temperature { get; }

        // What there was to choose between.
        public IReadOnlyList<string> Menu { get; }

        // The chance every dish had of being ordered.
        public IReadOnlyList<double> Probabilities { get; }

        // What the guest believes each dish is worth, after this lunch.
        public IReadOnlyList<double> Estimates { get; }

        public override string ToString()
        {
            string chances = string.Join(
                "  ", Probabilities.Select((chance, dish) => $"{Menu[dish]} {chance,4:P0}"));

            string verdict = Reward > 0.0 ? "good" : "disappointing";

            return $"day {Day,3}  tau {Temperature,5:0.000}  {chances}  ->  {Menu[Choice]}, {verdict}";
        }
    }

    // What the guest ended up believing, and how well they ate on the way there.
    public class SoftmaxLearningResult
    {
        public SoftmaxLearningResult(
            IReadOnlyList<string> menu,
            IReadOnlyList<double> estimates,
            IReadOnlyList<int> orders,
            double totalReward,
            int days)
        {
            Menu = menu;
            Estimates = estimates;
            Orders = orders;
            TotalReward = totalReward;
            Days = days;
        }

        public IReadOnlyList<string> Menu { get; }

        // The learned value of every dish.
        public IReadOnlyList<double> Estimates { get; }

        // How often each dish was ordered - this is where the policy shows itself.
        public IReadOnlyList<int> Orders { get; }

        // How many of the lunches were good ones.
        public double TotalReward { get; }

        public int Days { get; }

        // The dish the guest would settle on.
        public int Favourite => Estimates.ToList().IndexOf(Estimates.Max());

        public string FavouriteDish => Menu[Favourite];

        // What a lunch was worth on average - the number a policy is judged by.
        public double AverageReward => Days == 0 ? 0.0 : TotalReward / Days;

        public override string ToString()
        {
            string learned = string.Join(
                "   ", Estimates.Select((value, dish) => $"{Menu[dish]} {value:0.00} x{Orders[dish],4}"));

            return $"{learned}   average {AverageReward:0.000}";
        }
    }

    public class SoftmaxPolicyExample
    {
        // The lowest temperature the schedule may cool down to. At zero the exponent
        // would divide by zero, and this is close enough to greedy either way.
        public const double MinimumTemperature = 0.001;

        // Pizza is the one that disappoints most often, the salad is the kitchen's
        // quiet triumph - and nobody tells the guest any of that.
        public static IReadOnlyList<Dish> ExampleMenu()
        {
            return new[]
            {
                new Dish("Pizza", 0.25),
                new Dish("Pasta", 0.55),
                new Dish("Salat", 0.75)
            };
        }

        // The softmax itself: value estimates in, one probability per dish out.
        // Subtracting the largest estimate first keeps exp() from overflowing - it
        // cancels out of the quotient, so the probabilities are unchanged.
        public static IReadOnlyList<double> Probabilities(IReadOnlyList<double> estimates, double temperature)
        {
            if (estimates == null || estimates.Count == 0)
                throw new ArgumentException("A policy needs something to choose between.", nameof(estimates));

            if (temperature <= 0.0)
                throw new ArgumentOutOfRangeException(nameof(temperature), "The temperature has to be positive.");

            double highest = estimates.Max();
            double[] weights = estimates.Select(value => Math.Exp((value - highest) / temperature)).ToArray();
            double total = weights.Sum();

            return weights.Select(weight => weight / total).ToArray();
        }

        // Roulette wheel: every dish owns a slice of [0, 1) the size of its chance.
        public static int Choose(IReadOnlyList<double> probabilities, Random random)
        {
            double drawn = random.NextDouble();
            double edge = 0.0;

            for (int dish = 0; dish < probabilities.Count - 1; dish++)
            {
                edge += probabilities[dish];

                if (drawn < edge)
                    return dish;
            }

            // The last dish takes the rest, so rounding can never fall through.
            return probabilities.Count - 1;
        }

        // Learns which dish is worth ordering, knowing nothing but the plates served.
        // temperature starts the schedule and cooling multiplies it after every lunch -
        // 1.0 keeps it fixed, anything below turns trying out into sticking with.
        // onLunch reports every single day.
        public static SoftmaxLearningResult Learn(
            IReadOnlyList<Dish> menu,
            int days = 1000,
            double temperature = 0.1,
            double cooling = 1.0,
            Random? random = null,
            Action<SoftmaxLearningStep>? onLunch = null)
        {
            if (menu == null || menu.Count == 0)
                throw new ArgumentException("A guest needs something to choose between.", nameof(menu));

            if (days < 0)
                throw new ArgumentOutOfRangeException(nameof(days), "Cannot learn over a negative number of days.");

            if (temperature <= 0.0)
                throw new ArgumentOutOfRangeException(nameof(temperature), "The temperature has to be positive.");

            if (cooling <= 0.0 || cooling > 1.0)
                throw new ArgumentOutOfRangeException(nameof(cooling), "Cooling has to be in (0, 1].");

            random ??= new Random();

            string[] names = menu.Select(dish => dish.Name).ToArray();

            // The guest starts out believing nothing, which makes the first choice a
            // uniform one: equal estimates give equal probabilities.
            double[] estimates = new double[menu.Count];
            int[] orders = new int[menu.Count];
            double totalReward = 0.0;

            for (int day = 1; day <= days; day++)
            {
                IReadOnlyList<double> probabilities = Probabilities(estimates, temperature);
                int choice = Choose(probabilities, random);

                // The kitchen answers - this plate is all the guest ever gets to see.
                double reward = random.NextDouble() < menu[choice].ChanceOfBeingGood ? 1.0 : 0.0;

                totalReward += reward;
                orders[choice]++;

                // Incremental average: the estimate moves towards today's lunch by one
                // n-th, which keeps it the mean of every plate of that dish so far.
                estimates[choice] += (reward - estimates[choice]) / orders[choice];

                onLunch?.Invoke(new SoftmaxLearningStep(
                    day, choice, reward, temperature, names, probabilities, (double[])estimates.Clone()));

                temperature = Math.Max(temperature * cooling, MinimumTemperature);
            }

            return new SoftmaxLearningResult(names, estimates, orders, totalReward, days);
        }

        public static void SoftmaxPolicy()
        {
            IReadOnlyList<Dish> menu = ExampleMenu();
            const int days = 1000;

            Console.WriteLine($"Lunch, every day, same three dishes: {string.Join(", ", menu)}.");
            Console.WriteLine("The number in brackets is how often the kitchen gets it right - the guest is never told.");
            Console.WriteLine();
            Console.WriteLine("The first days, starting hungry for anything and calming down:");

            Learn(
                menu,
                days: 12,
                temperature: 1.0,
                cooling: 0.99,
                random: new Random(18),
                onLunch: lunch => Console.WriteLine(lunch));

            Console.WriteLine();
            Console.WriteLine($"After {days} lunches - what the guest learned, and how often they ordered it:");

            // Hot: every dish keeps a sizeable chance, so the guest never settles.
            Console.WriteLine($"  tau 1.00 fixed     {Learn(menu, days, 1.0, random: new Random(18))}");

            // Cold: near greedy, and one good first plate can decide the whole year.
            Console.WriteLine($"  tau 0.02 fixed     {Learn(menu, days, 0.02, random: new Random(18))}");

            // Cooling: tries everything while it knows nothing, settles once it does.
            Console.WriteLine($"  tau 1.00 cooling   {Learn(menu, days, 1.0, 0.99, new Random(18))}");

            Dish best = menu.OrderByDescending(dish => dish.ChanceOfBeingGood).First();

            Console.WriteLine();
            Console.WriteLine($"Always ordering the {best.Name} would have been good {best.ChanceOfBeingGood:0.000} of the time.");
        }
    }
}
