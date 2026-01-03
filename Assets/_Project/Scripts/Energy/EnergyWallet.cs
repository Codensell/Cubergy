using System;
using UnityEngine;

namespace Cubergy.Energy
{
    public sealed class EnergyWallet : MonoBehaviour
    {
        public event Action<int> Changed;

        public int Current { get; private set; }

        public void Add(int amount)
        {
            if (amount <= 0)
                return;

            Current += amount;
            Changed?.Invoke(Current);
        }

        public void ResetToZero()
        {
            Current = 0;
            Changed?.Invoke(Current);
        }
    }
}