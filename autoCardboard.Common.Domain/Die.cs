using System;

namespace autoCardboard.Common.Domain
{
 
    /// <summary>
    /// Throw a die, with any number of sides
    /// https://commons.wikimedia.org/wiki/Dice_by_number_of_sides#D1
    /// </summary>
    public class Die
    {
        private static Random r = new Random();
        private int _sides;

        public Die(int sides = 6)
        {
            _sides = sides;
        }

        public int Roll()
        {
            return r.Next(1, _sides+1);
        }
    }
}
