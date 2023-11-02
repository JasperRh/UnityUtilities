using System;
using System.Collections.Generic;
using System.Linq;

namespace Crimsilk.Utilities.Extensions
{
    public static class EnumerableExtensions
    {
        private static Random random;

        static EnumerableExtensions()
        {
            random = new Random();
        }
        
        /// <summary>
        /// Returns a single random object from the enumerable. Returns null if enumerable is empty.
        /// </summary>
        public static T Random<T>(this IEnumerable<T> input)
        {
            if (input.Count() == 0)
            {
                return default(T);
            }

            return input.ElementAt(random.Next(input.Count()));
        }
        
        /// <summary>
        /// Returns multiple random objects from the enumerable.
        /// </summary>
        /// <param name="amount">The amount of random items to be selected</param>
        /// <param name="allowDuplicate">If false, the same item will not be selected twice.</param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        public static IEnumerable<T> Random<T>(this IEnumerable<T> input, int amount = 1, bool allowDuplicate = false)
        {
            if (amount <= 0)
            {
                throw new ArgumentException($"Random selection amount must be greater than 0");
            }

            IList<T> inputList = new List<T>(input);
            if (!allowDuplicate && amount > inputList.Count())
            {
                throw new ArgumentException(
                    $"When allowDuplicate is set to false, your amount must be lesser or equal to the size of the IEnumerable.");
            }

            IList<T> randomItems = new List<T>();
            for (int i = 0; i < amount; i++)
            {
                T selected = inputList.ElementAt(random.Next(inputList.Count()));
                randomItems.Add(selected);

                if (!allowDuplicate)
                {
                    // Remove item so it cannot be selected again
                    inputList.Remove(selected);
                }
            }
            
            return randomItems;
        }

        public static void ForEach<T>(this IEnumerable<T> input, Action<T> action)
        {
            foreach (T element in input)
            {
                action(element);
            }
        }
    }
}