using System;

namespace autoCardboard.Common.Domain
{
 
    public class Die
    {
        private static Random r = new Random();
        private int _sides;

        public Die(int sides = 6)
        {
            _sides = sides;
        }

        public int Throw()
        {
            return r.Next(1, _sides+1);
        }
    }
}
