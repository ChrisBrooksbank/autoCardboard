using System.Linq;

namespace autoCardboard.Common.Test
{
    public class DiceCupTests
    {
        private DiceCup _diceCup;

        public void Setup()
        {
            _diceCup = new DiceCup();
        }

        public void Roll2D63D8()
        {
            var dice = _diceCup.Roll(6,6,8,8,8);
            // Assert.IsTrue(dice.Count() == 5);
        }
    }
}
