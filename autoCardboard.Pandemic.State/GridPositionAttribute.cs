using System;

namespace autoCardboard.Pandemic.State
{
    public class GridPositionAttribute: Attribute
    {
        public int Row { get; set; }
        public int Column { get; set; }

        public GridPositionAttribute(int row, int column)
        {
            Row = row;
            Column = column;
        }
    }
}
