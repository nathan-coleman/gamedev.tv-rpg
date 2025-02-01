using UnityEngine;

namespace RPG.Control
{
    public interface IController
    {
        public void Die();
        public void Hit(GameObject attacker);
    }
}