namespace Cinetica
{
    public enum Turn { Player, Enemy }
    public enum Side { Player, Enemy }
    
    
    public enum RoundState { Playing, Victory, Defeat }
    public enum TurnState { PreTurn, SelectBuilding, SelectTarget, WaitForResult }
    public enum Difficulty { Easy, Medium, Hard }
    
    public enum BuildingType { Dummy, Core, Turret, Railgun, ShieldGenerator }
}