using System;
using System.Collections.Generic;
using Cinetica.Utility;
using UnityEngine;
using static Cinetica.Utility.Utils; 

namespace Cinetica
{
    public partial class GameManager : Singleton<GameManager>
    {
        private static readonly List<LevelEntry> _levelEntries = new List<LevelEntry>()
        {
            new LevelEntry("2Fort", "Fácil - 2 Fortes"),
            //new LevelEntry("Unimatrix", "Fácil - Unimatrix", new string[] {"2Fort"}),
            //new LevelEntry("Under", "Médio - Em Desvantagem", new string[] {"Unimatrix"}),
            //new LevelEntry("UnimatrixChallege", "Difícil - Unimatrix II", new string[] {"Under"}),
            new LevelEntry("Testing", "Extra - Mapa Original", new string[] {"2Fort"}),
        };
        public static List<LevelEntry> GetLevelEntries() => _levelEntries;
    }
    
    public class LevelEntry
    {
        public string displayName = "Fase";
        public string levelName = "Testing";
        public string[] levelRequirements = Array.Empty<string>();

        public LevelEntry(string levelName, string displayName, string[] levelRequirements = null)
        {
            this.displayName = displayName;
            this.levelName = levelName;
            this.levelRequirements = levelRequirements ?? Array.Empty<string>();
        }

        public bool RequirementsMet()
        {
            if (levelRequirements == null || levelRequirements.Length == 0) return true;
            bool allCompleted = true;

            if (!SceneExists(levelName))
                return false;
            
            foreach (var level in levelRequirements)
            {
                if (!GameManager.playerData.IsLevelComplete(level))
                {
                    allCompleted = false;
                }
            }

            return allCompleted;
        }
    }
}