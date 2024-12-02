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
                            Debug.LogWarning($"[MovementFactory] Invalid ID for movement {type.Name}. You forgot to set the ActionId property");
                        }
                        else if (RegisteredDescriptors.ContainsKey(id))
                        {
                            Debug.LogWarning($"[MovementFactory] Movement {type.Name}[{id}] is already registered");
                        }
                        else
                        {
                            Debug.Log($"[MovementFactory] Movement {type.Name} available with ID {id}");
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
            var type = RegisteredDescriptors[actionId];
            return (ActionDescriptor)Activator.CreateInstance(type, args);
        }
        #endregion
    }
}