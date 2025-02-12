namespace Duelo.Common.Component
{
    using System;
    using Duelo.Common.Core;
    using Firebase.Database;
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
        private DatabaseReference _syncRef;

        /// <summary>
        /// Round in which this object should be destroyed
        /// </summary>
        private int _endRound = -1;
        #endregion

        #region Unity Lifecycle
        private void Awake()
        {
            _syncRef = GlobalState.MatchRef.Child("sync/round");
        }

        protected virtual void OnEnable()
        {
            if (_syncRef != null)
            {
                _syncRef.ValueChanged += OnRoundChanged;
            }
        }

        protected virtual void OnDisable()
        {
            if (_syncRef != null)
            {
                _syncRef.ValueChanged -= OnRoundChanged;
            }
        }

        protected virtual void OnDestroy()
        {
            if (_syncRef != null)
            {
                _syncRef.ValueChanged -= OnRoundChanged;
            }
        }
        #endregion

        #region Firebase
        private void OnRoundChanged(object sender, ValueChangedEventArgs e)
        {
            if (e.Snapshot.Exists)
            {
                int currentRound = Convert.ToInt32(e.Snapshot.Value);
                if (_endRound == -1)
                {
                    _endRound = currentRound + ActiveRounds;
                }
                else if (currentRound > _endRound)
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