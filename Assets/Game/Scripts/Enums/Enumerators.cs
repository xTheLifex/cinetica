namespace Cinetica
{
    public enum Turn { Player, Enemy }
    public enum Side { Player, Enemy }
    
    
    public enum RoundState { Playing, Victory, Defeat }
    public enum TurnState { PreTurn, SelectBuilding, SelectTarget, InputParameters, WaitForResult }
    public enum Difficulty { Easy, Medium, Hard, Impossible }
    
    public enum BuildingType { Dummy, Core, Turret, Railgun, ShieldGenerator }
}