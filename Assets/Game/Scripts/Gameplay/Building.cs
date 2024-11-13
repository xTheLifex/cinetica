using UnityEngine;

namespace Cinetica.Gameplay
{
    [RequireComponent(typeof(Damageable))]
    public class Building : MonoBehaviour
    {
        public Side side = Side.Player;
    }
}