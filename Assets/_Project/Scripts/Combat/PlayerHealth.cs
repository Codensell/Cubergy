using System;
using UnityEngine;

namespace Cubergy.Combat
{
    public sealed class PlayerHealth : MonoBehaviour
    {
        public event Action<int, int> Changed;
        public event Action Died;

        public int Current { get; private set; }
        public int Max { get; private set; }

        public void SetMax(int max, bool fillToMax)
        {
            if (max < 1)
                max = 1;

            Max = max;

            if (fillToMax || Current > Max)
                Current = Max;

            Changed?.Invoke(Current, Max);
        }

        public void Damage(int amount)
        {
            if (amount <= 0 || Current <= 0)
                return;

            Current -= amount;
            if (Current < 0)
                Current = 0;

            Changed?.Invoke(Current, Max);

            if (Current == 0)
                Died?.Invoke();
        }

        public void Heal(int amount)
        {
            if (amount <= 0 || Current <= 0)
                return;

            Current += amount;
            if (Current > Max)
                Current = Max;

            Changed?.Invoke(Current, Max);
        }
    }
}