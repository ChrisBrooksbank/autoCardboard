using System.Collections.Generic;
using Xunit;

namespace autoCardboard.Common.Test
{
    public class DiceTests
    {
        [Fact]
        public void D6Throws1To6()
        {
            var die = new Die();
            Dictionary<int, int> countByThrow = new Dictionary<int, int>();

            for (var expectedThrowValue = 1; expectedThrowValue <= 6; expectedThrowValue++)
            {
                countByThrow[expectedThrowValue] = 0;
            }

            for (var diceThrow = 1; diceThrow < 100; diceThrow++)
            {
                var thrownNumber = die.Roll();
                countByThrow[thrownNumber]++;
            }

            var hasAllValues = countByThrow[1] > 0 && countByThrow[2] > 0 && countByThrow[3] > 0 &&
                               countByThrow[4] > 0 && countByThrow[5] > 0 && countByThrow[6] > 0;

            Assert.True(hasAllValues);
        }
    }
}
