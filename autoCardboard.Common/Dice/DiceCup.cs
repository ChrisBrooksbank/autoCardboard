using System;
using System.Collections.Generic;

namespace autoCardboard.Common
{
    public class DiceCup
    {
        private static Random r = new Random();

        public IEnumerable<int> Roll(params int[] dieSides)
        {
            var rolledDice = new List<int>();

            foreach (var sides in dieSides)
            {
                rolledDice.Add(r.Next(1, sides + 1));
            }

            return rolledDice;
        }
    }
}
