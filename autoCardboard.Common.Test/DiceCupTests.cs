using System.Linq;
using Xunit;

namespace autoCardboard.Common.Test
{
    public class DiceCupTests
    {
        [Fact]
        public void Roll2D63D8()
        {
            var diceCup = new DiceCup();
            var dice = diceCup.Roll(6, 6, 8, 8, 8);
            Assert.True(dice.Count() == 5);
        }
    }
}
