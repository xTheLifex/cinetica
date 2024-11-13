using Cinetica.Utility;

namespace Cinetica.Gameplay
{
    public class RoundManager : Singleton<RoundManager>
    {
        public enum Turn { Player, Enemy }
        public Turn turn;
        
    }
}