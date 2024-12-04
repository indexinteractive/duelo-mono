namespace Duelo.Common.Kernel
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;
    using Ind3x.Util;
    using UnityEngine;

    public class ActionFactory : Singleton<ActionFactory>
    {
        #region Private Fields
        public readonly Dictionary<int, Type> RegisteredDescriptors = new();
        #endregion

        #region Initialization
        public ActionFactory()
        {
            Debug.Log("[ActionFactory] Initializing");
            RegisterActions();
        }
        #endregion

        #region Public Methods
        private void RegisterActions()
        {
            var assembly = Assembly.GetExecutingAssembly();
            foreach (var type in assembly.GetTypes())
            {
                if (typeof(ActionDescriptor).IsAssignableFrom(type) && !type.IsAbstract)
                {
                    var idProp = type.GetProperty("ActionId");
                    if (idProp != null)
                    {
                        var emptyInstance = Activator.CreateInstance(type);
                        var id = (int)idProp.GetValue(emptyInstance);

                        if (id == -1)
                        {
                            Debug.LogWarning($"[ActionFactory] Invalid ID for movement {type.Name}. You forgot to set the ActionId property");
                        }
                        else if (RegisteredDescriptors.ContainsKey(id))
                        {
                            Debug.LogWarning($"[ActionFactory] Movement {type.Name}[{id}] is already registered");
                        }
                        else
                        {
                            Debug.Log($"[ActionFactory] Movement {type.Name} registered Id {id}");
                            RegisteredDescriptors.Add(id, type);
                        }
                    }
                }
            }
        }
        #endregion

        #region Action Handling
        public ActionDescriptor GetDescriptor(int actionId, params object[] args)
        {
            if (!RegisteredDescriptors.ContainsKey(actionId))
            {
                Debug.LogWarning($"[ActionFactory] Movement {actionId} not found");
                return null;
            }

            var type = RegisteredDescriptors[actionId];
            return (ActionDescriptor)Activator.CreateInstance(type, args);
        }
        #endregion
    }
}