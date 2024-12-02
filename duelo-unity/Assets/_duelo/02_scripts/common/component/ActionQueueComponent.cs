namespace Duelo.Common.Component
{
    using System;
    using System.Collections.Generic;
    using UnityEngine;

    public partial class ActionQueueComponent : MonoBehaviour
    {
        #region Action Handling
        private Queue<ActionComponent> _actions;
        private ActionComponent _currentAction;

        public bool HasActions => _currentAction != null || _actions.Count > 0;
        public Action ActionsQueueChanged;

        public bool Run;
        #endregion

        #region Public Properties
        public IEnumerable<string> ActionsList
        {
            get
            {
                if (_currentAction != null)
                {
                    yield return $"{_currentAction.GetType().Name} (current)";
                }

                foreach (var action in _actions)
                {
                    yield return action.GetType().Name;
                }
            }
        }
        #endregion

        #region Unity Lifecycle
        private void Start()
        {
            _actions = new Queue<ActionComponent>();
            _currentAction = null;
            Run = false;
        }

        private void Update()
        {
            if (Run)
            {
                if (_currentAction != null)
                {
                    if (_currentAction.IsFinished)
                    {
                        _currentAction.OnActionRemoved();
                        AllowProcessing(_currentAction, false);
                        Destroy(_currentAction);
                        _currentAction = null;

                        ActionsQueueChanged?.Invoke();
                    }
                }
                else
                {
                    if (_actions.Count > 0)
                    {
                        _currentAction = _actions.Dequeue();

                        // TODO: Watch this, maybe _currentAction needs to be instantiated or something
                        // _currentAction.transform.SetParent(transform);
                        AllowProcessing(_currentAction, true);
                        ActionsQueueChanged?.Invoke();
                    }
                }
            }
        }
        #endregion

        #region Action Handling
        public void QueueAction(ActionComponent action)
        {
            Debug.Log($"[ActionQueue] Enqueuing action: {action.GetType().Name}");

            _actions.Enqueue(action);
            AllowProcessing(action, false);

            ActionsQueueChanged?.Invoke();
        }
        #endregion

        #region Private Helpers
        private void AllowProcessing(ActionComponent node, bool isAllowed)
        {
            node.enabled = isAllowed;
        }
        #endregion
    }
}