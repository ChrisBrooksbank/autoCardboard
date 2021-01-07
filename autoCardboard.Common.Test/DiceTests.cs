using System.Collections.Generic;
using autoCardboard.Common.Domain.Dice;
using NUnit.Framework;

namespace autoCardboard.Common.Test
{
    public class DiceTests
    {
        private Die _die;

        [SetUp]
        public void Setup()
        {
            _die = new Die();
        }
        
        [Test]
        public void D6Throws1To6()
        {
            Dictionary<int, int> countByThrow = new Dictionary<int, int>();

            for (var expectedThrowValue = 1; expectedThrowValue <= 6; expectedThrowValue++)
            {
                countByThrow[expectedThrowValue] = 0;
            }

            for (var diceThrow = 1; diceThrow < 100; diceThrow++)
            {
                var thrownNumber = _die.Roll();
                countByThrow[thrownNumber]++;
            }

            var hasAllValues = countByThrow[1] > 0 && countByThrow[2] > 0 && countByThrow[3] > 0 &&
                               countByThrow[4] > 0 && countByThrow[5] > 0 && countByThrow[6] > 0;

            Assert.IsTrue(hasAllValues);
        }
    }
}
