namespace Duelo.Common.Component
{
    using System;
    using Duelo.Common.Core;
    using Duelo.Common.Model;
    using ObservableCollections;
    using R3;
    using UnityEngine;

    /// <summary>
    /// Any behaviour that inherits from this class will be removed
    /// or disabled after <see cref="ActiveRounds"/> number of rounds.
    ///
    /// Destroying / Disabling is controlled by the result returned
    /// from <see cref="OnDecomission"/>, which is called before the
    /// object is disposed.
    /// </summary>
    public class RoundFiniteObject : MonoBehaviour
    {
        #region Public Properties
        /// <summary>
        /// Number of rounds in which this object stays active
        /// </summary>
        public int ActiveRounds;
        #endregion

        #region Private Fields
        /// <summary>
        /// Round in which this object should be destroyed
        /// </summary>
        private int _endRound = -1;

        private IDisposable _roundChangedSub;
        #endregion

        #region Unity Lifecycle
        protected virtual void OnEnable()
        {
            _roundChangedSub = GlobalState.Match.Rounds
                .ObserveAdd()
                .Select(x => x.Value)
                .Subscribe(OnRoundChanged);
        }

        protected virtual void OnDisable()
        {
            _roundChangedSub?.Dispose();
        }

        protected virtual void OnDestroy()
        {
            _roundChangedSub?.Dispose();
        }
        #endregion

        #region Data Events
        private void OnRoundChanged(MatchRound currentRound)
        {
            if (_endRound == -1)
            {
                _endRound = currentRound.RoundNumber + ActiveRounds;
            }
            else if (currentRound.RoundNumber > _endRound)
            {
                Debug.Log($"[RoundFiniteObject] Round {currentRound} > {_endRound}: Decomissioning object {gameObject.name}");

                bool shouldDestroy = OnDecomission();
                if (shouldDestroy)
                {
                    Destroy(gameObject);
                }
                else
                {
                    gameObject.SetActive(false);
                }
            }
        }
        #endregion

        #region Inherited Methods
        /// <summary>
        /// This method should return `true` if the object should be destroyed,
        /// `false` if it should be disabled
        /// </summary>
        protected virtual bool OnDecomission() => true;
        #endregion
    }
}