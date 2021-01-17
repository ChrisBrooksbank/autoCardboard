using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace autoCardboard.Common
{
    [Serializable]
    public class GameState: IGameState
    {
        public object Clone()
        {
            using (var ms = new MemoryStream())
            {
                var formatter = new BinaryFormatter();
                formatter.Serialize(ms, this);
                ms.Position = 0;

                return (IGameState)formatter.Deserialize(ms);
            }
        }
    }
}
