using System;
using System.Collections.Generic;
using System.Linq;

// Source : https://en.wikipedia.org/wiki/Policy_gradient_method
//          https://en.wikipedia.org/wiki/Softmax_function
// The softmax policy learned one value per dish. That works as long as the menu never
// changes - but a dish it has never ordered is a blank to it. A linear policy does not
// learn dishes, it learns what makes a dish worth ordering. Every choice is described by
// a few numbers, its features phi(a) - here: is it warm, how filling is it, what does it
// cost - and its score is a weighted sum of them, w * phi(a). A softmax over the scores
// gives one probability per choice, exactly as before. Because only the weights are
// learned, the policy can judge a dish it has never seen, as long as it can describe it.
//
// The same features can be worth something different depending on the situation: soup
// is welcome on a cold day and hard to face on a hot one. So there is one set of weights
// per situation, and the situation picks which set is used.
//
// Learning is REINFORCE: after every lunch the weights move along
//     phi(chosen) - sum p(a) * phi(a)
// - the features of what was ordered minus what the policy would have expected to order -
// scaled by how much better the lunch was than usual. "Usual" is a running average of past
// lunches, the baseline. Without it every lunch that was merely okay would reinforce
// whatever had been ordered; with it only the surprise counts.
//
// This is the same model a Doppelkopf player can learn its card play with: there the
// choices are the legal cards, the features say whether a card is trump, whether it wins
// the trick, how many points it gives away, and the situation is who leads the trick.

namespace Patterns.AlgorithmicPatterns
{
    // The situation a lunch is chosen in. Each one gets its own set of weights.
    public enum Weather
    {
        Cold,
        Hot
    }

    // A dish as the policy sees it: nothing but its features. Unlike the softmax example
    // there is no hidden chance attached - what the dish is worth depends on the guest.
    public class Meal
    {
        public static readonly string[] FeatureNames = { "warm", "filling", "price" };

        public Meal(string name, bool warm, double filling, double euros)
        {
            if (filling < 0.0 || filling > 1.0)
                throw new ArgumentOutOfRangeException(nameof(filling), "Filling has to be in [0, 1].");

            Name = name;
            Warm = warm;
            Filling = filling;
            Euros = euros;
        }

        public string Name { get; }

        public bool Warm { get; }

        // 0 leaves you hungry, 1 keeps you going until the evening.
        public double Filling { get; }

        public double Euros { get; }

        // phi(a). All features are kept roughly in [0, 1], so no single one dominates the
        // score just by being measured in larger numbers - that is why the price is in tens.
        // There is no constant 1 among them: it would add the same to every score and
        // cancel out of the softmax.
        public double[] Features()
        {
            return new[] { Warm ? 1.0 : 0.0, Filling, Euros / 10.0 };
        }

        public override string ToString()
        {
            return Name;
        }
    }

    // The guest's taste - the hidden truth the policy has to find out by eating.
    public class Guest
    {
        // How much the guest likes each feature, per weather. The policy never sees this.
        private static readonly double[][] Taste =
        {
            new[] { 1.0, 0.6, -0.8 },   // cold: warm and filling, please, but not expensive
            new[] { -1.2, -0.3, -0.8 }  // hot: something cold and light, still not expensive
        };

        // How much one lunch can differ from another of the same dish.
        public const double Noise = 1.0;

        public static double Enjoyment(Meal meal, Weather weather)
        {
            double[] taste = Taste[(int)weather];
            double[] phi = meal.Features();
            double sum = 0.0;

            for (int j = 0; j < phi.Length; j++)
                sum += taste[j] * phi[j];

            return sum;
        }

        // What one lunch actually felt like: the taste plus a good or bad day in the kitchen.
        public static double Eat(Meal meal, Weather weather, Random random)
        {
            return Enjoyment(meal, weather) + Noise * (2.0 * random.NextDouble() - 1.0);
        }
    }

    public class LinearPolicyModel
    {
        // Large weights make the softmax sharp, small ones keep it curious - the weights
        // carry the temperature themselves. The limit keeps the policy from ever becoming
        // completely greedy and stops a run of lucky lunches from running away with it.
        public const double WeightLimit = 8.0;

        private readonly double[][] _weights;
        private double _baseline;

        public LinearPolicyModel()
        {
            int situations = Enum.GetValues<Weather>().Length;
            _weights = new double[situations][];

            // All weights zero: every dish scores the same and the first choices are uniform.
            for (int situation = 0; situation < situations; situation++)
                _weights[situation] = new double[Meal.FeatureNames.Length];
        }

        // How far one lunch moves the weights.
        public double LearnRate { get; set; } = 0.05;

        // How quickly the baseline follows the lunches actually eaten.
        public double BaselineRate { get; set; } = 0.01;

        public double Baseline => _baseline;

        public IReadOnlyList<double> Weights(Weather weather)
        {
            return _weights[(int)weather];
        }

        // score(a) = w * phi(a), and the softmax turns the scores into chances.
        public IReadOnlyList<double> Probabilities(IReadOnlyList<Meal> menu, Weather weather)
        {
            double[] weights = _weights[(int)weather];

            double[] scores = menu
                .Select(meal => meal.Features().Zip(weights, (feature, weight) => feature * weight).Sum())
                .ToArray();

            return SoftmaxPolicyExample.Probabilities(scores, 1.0);
        }

        public int Choose(IReadOnlyList<Meal> menu, Weather weather, Random random)
        {
            return SoftmaxPolicyExample.Choose(Probabilities(menu, weather), random);
        }

        // One REINFORCE step. probabilities are the ones the choice was drawn from.
        public void Learn(
            IReadOnlyList<Meal> menu,
            Weather weather,
            IReadOnlyList<double> probabilities,
            int choice,
            double reward)
        {
            double[] weights = _weights[(int)weather];
            double[] chosen = menu[choice].Features();
            double advantage = reward - _baseline;

            for (int j = 0; j < weights.Length; j++)
            {
                // What the policy would have expected this feature to be ...
                double expected = 0.0;
                for (int i = 0; i < menu.Count; i++)
                    expected += probabilities[i] * menu[i].Features()[j];

                // ... and the gradient of log p(chosen): how much more of it was ordered.
                double gradient = chosen[j] - expected;

                weights[j] = Math.Clamp(weights[j] + LearnRate * advantage * gradient, -WeightLimit, WeightLimit);
            }

            _baseline += BaselineRate * (reward - _baseline);
        }
    }

    public class LinearPolicyExample
    {
        // What the canteen serves while the guest is learning.
        public static IReadOnlyList<Meal> Canteen()
        {
            return new[]
            {
                new Meal("Pizza", true, 0.8, 9.0),
                new Meal("Pasta", true, 0.7, 8.0),
                new Meal("Linsensuppe", true, 0.6, 5.0),
                new Meal("Gulasch", true, 1.0, 12.0),
                new Meal("Salat", false, 0.2, 7.0),
                new Meal("Gazpacho", false, 0.3, 6.0),
                new Meal("Wrap", false, 0.6, 7.0),
                new Meal("Sushi", false, 0.5, 14.0)
            };
        }

        // A new canteen. Not one of these dishes was ever on the menu while learning.
        public static IReadOnlyList<Meal> NewCanteen()
        {
            return new[]
            {
                new Meal("Chili", true, 0.9, 8.0),
                new Meal("Kartoffelsuppe", true, 0.6, 5.0),
                new Meal("Gurkensalat", false, 0.1, 4.0),
                new Meal("Bowl", false, 0.7, 11.0)
            };
        }

        // Every day the weather is drawn, and the canteen puts three of its dishes on the menu.
        public static void Train(LinearPolicyModel policy, IReadOnlyList<Meal> canteen, int days, Random random)
        {
            for (int day = 0; day < days; day++)
            {
                Weather weather = random.Next(2) == 0 ? Weather.Cold : Weather.Hot;
                Meal[] menu = canteen.OrderBy(_ => random.Next()).Take(3).ToArray();

                IReadOnlyList<double> probabilities = policy.Probabilities(menu, weather);
                int choice = SoftmaxPolicyExample.Choose(probabilities, random);
                double reward = Guest.Eat(menu[choice], weather, random);

                policy.Learn(menu, weather, probabilities, choice, reward);
            }
        }

        // How much the guest would enjoy their lunch on average, over every three-dish menu
        // the canteen could put together and both kinds of weather. No dice are rolled here:
        // each dish counts with the chance the policy gives it, and with its true taste.
        public static double ExpectedEnjoyment(LinearPolicyModel policy, IReadOnlyList<Meal> canteen)
        {
            return Situations(canteen).Average(situation =>
            {
                IReadOnlyList<double> probabilities = policy.Probabilities(situation.Menu, situation.Weather);
                return situation.Menu.Select((meal, i) => probabilities[i] * Guest.Enjoyment(meal, situation.Weather)).Sum();
            });
        }

        // The same, for a guest who always knows the best dish on the menu.
        public static double BestEnjoyment(IReadOnlyList<Meal> canteen)
        {
            return Situations(canteen).Average(situation =>
                situation.Menu.Max(meal => Guest.Enjoyment(meal, situation.Weather)));
        }

        private static IEnumerable<(Meal[] Menu, Weather Weather)> Situations(IReadOnlyList<Meal> canteen)
        {
            for (int a = 0; a < canteen.Count; a++)
            for (int b = a + 1; b < canteen.Count; b++)
            for (int c = b + 1; c < canteen.Count; c++)
            foreach (Weather weather in Enum.GetValues<Weather>())
                yield return (new[] { canteen[a], canteen[b], canteen[c] }, weather);
        }

        private static string Describe(LinearPolicyModel policy, IReadOnlyList<Meal> menu, Weather weather)
        {
            IReadOnlyList<double> probabilities = policy.Probabilities(menu, weather);
            return string.Join("  ", menu.Select((meal, i) => $"{meal.Name} {probabilities[i],4:P0}"));
        }

        public static void LinearPolicy()
        {
            IReadOnlyList<Meal> canteen = Canteen();
            IReadOnlyList<Meal> newCanteen = NewCanteen();
            const int days = 3000;

            var policy = new LinearPolicyModel();

            Console.WriteLine($"Every day a menu of three out of: {string.Join(", ", canteen)}.");
            Console.WriteLine($"The policy only sees the features ({string.Join(", ", Meal.FeatureNames)}) and the weather.");
            Console.WriteLine();
            Console.WriteLine($"Before learning, enjoyment {ExpectedEnjoyment(policy, canteen):0.000} per lunch - just picking at random.");

            Train(policy, canteen, days, new Random(18));

            Console.WriteLine($"After {days} lunches, enjoyment {ExpectedEnjoyment(policy, canteen):0.000} " +
                              $"(knowing the guest's taste: {BestEnjoyment(canteen):0.000}).");
            Console.WriteLine();
            Console.WriteLine("What the weights say:");

            foreach (Weather weather in Enum.GetValues<Weather>())
            {
                string weights = string.Join("  ", policy.Weights(weather)
                    .Select((weight, j) => $"{Meal.FeatureNames[j]} {weight,6:+0.00;-0.00}"));

                Console.WriteLine($"  {weather,-5} {weights}");
            }

            Console.WriteLine();
            Console.WriteLine($"A new canteen: {string.Join(", ", newCanteen)} - never seen any of it.");

            Meal[] menu = { newCanteen[0], newCanteen[1], newCanteen[2] };

            foreach (Weather weather in Enum.GetValues<Weather>())
                Console.WriteLine($"  {weather,-5} {Describe(policy, menu, weather)}");

            Console.WriteLine();
            Console.WriteLine($"Enjoyment there: random {ExpectedEnjoyment(new LinearPolicyModel(), newCanteen):0.000}, " +
                              $"learned {ExpectedEnjoyment(policy, newCanteen):0.000}, " +
                              $"best {BestEnjoyment(newCanteen):0.000}.");
        }
    }
}
