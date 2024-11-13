namespace Cinetica
{
    public enum Turn { Player, Enemy }
    public enum Side { Player, Enemy }
    
    
    public enum RoundState { Playing, Victory, Defeat }
    public enum TurnState { PreTurn, SelectWeapon, SelectTarget, WaitForResult }
}