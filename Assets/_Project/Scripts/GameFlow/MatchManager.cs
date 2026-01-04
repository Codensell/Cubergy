using Cubergy.Combat;
using Cubergy.Energy;
using Cubergy.Player;
using UnityEngine;

namespace Cubergy.GameFlow
{
    public sealed class MatchManager : MonoBehaviour
    {
        [SerializeField] private int _pointsToWin = 5;

        [SerializeField] private TargetWaveSpawner _spawner;

        [SerializeField] private PlayerFormController _forms;
        [SerializeField] private EnergyWallet _wallet;
        [SerializeField] private PlayerHealth _health;

        [SerializeField] private Transform _playerSpawnPoint;
        [SerializeField] private Transform _playerRoot;

        public int PlayerPoints { get; private set; }
        public int EnemyPoints { get; private set; }

        private bool _matchEnded;

        private void OnEnable()
        {
            if (_health != null)
                _health.Died += OnPlayerDied;

            StartRound();
        }

        private void OnDisable()
        {
            if (_health != null)
                _health.Died -= OnPlayerDied;
        }

        private void Update()
        {
            if (_matchEnded)
                return;

            if (_spawner != null && _spawner.AliveCount == 0)
                OnRoundWonByPlayer();
        }

        private void OnRoundWonByPlayer()
        {
            PlayerPoints++;

            if (PlayerPoints >= _pointsToWin)
            {
                _matchEnded = true;
                return;
            }

            StartRound();
        }

        private void OnPlayerDied()
        {
            if (_matchEnded)
                return;

            EnemyPoints++;

            if (EnemyPoints >= _pointsToWin)
            {
                _matchEnded = true;
                return;
            }

            StartRound();
        }

        private void StartRound()
        {
            if (_playerRoot != null && _playerSpawnPoint != null)
                _playerRoot.position = _playerSpawnPoint.position;

            if (_forms != null)
                _forms.ForceForm(PlayerForm.Form0, true);
            else if (_wallet != null)
                _wallet.ResetToZero();

            if (_spawner != null)
                _spawner.SpawnWave();
        }
    }
}
