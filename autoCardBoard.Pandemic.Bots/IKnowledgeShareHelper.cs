using autoCardboard.Pandemic.State;

namespace autoCardBoard.Pandemic.Bots
{
    public interface IKnowledgeShareHelper
    {
        bool CanKnowledgeShare(int currentPlayerId, IPandemicState state);
        KnowledgeShare GetSuggestedKnowledgeShare(int currentPlayerId, IPandemicState state);
    }
}
