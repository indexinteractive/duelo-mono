namespace Duelo.Editor
{
    using UnityEngine;
    using UnityEditor;
    using System;
    using System.Linq;
    using System.Collections.Generic;
    using System.Reflection;
    using Duelo.Common.Model;

    [CustomPropertyDrawer(typeof(ActionIdAttribute))]
    public class ActionIdDrawer : PropertyDrawer
    {
        private string[] actionNames;
        private int[] actionValues;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (property.propertyType != SerializedPropertyType.Integer)
            {
                EditorGUI.LabelField(position, label.text, "Use [ActionId] with an int field.");
                return;
            }

            Dictionary<string, int> actions = GetAllActionIds();
            actionNames = actions.Keys.ToArray();
            actionValues = actions.Values.ToArray();

            int selectedIndex = Array.IndexOf(actionValues, property.intValue);
            if (selectedIndex < 0)
            {
                selectedIndex = 0;
            }

            selectedIndex = EditorGUI.Popup(position, label.text, selectedIndex, actionNames);
            property.intValue = actionValues[selectedIndex];
        }

        private Dictionary<string, int> GetAllActionIds()
        {
            Dictionary<string, int> actionDict = new Dictionary<string, int>
            {
                { "None", -1 }
            };

            Type baseType = typeof(ActionId);
            IEnumerable<Type> actionTypes = Assembly.GetAssembly(baseType)
                .GetTypes()
                .Where(t => t.IsClass && !t.IsAbstract && baseType.IsAssignableFrom(t));

            foreach (Type type in actionTypes)
            {
                FieldInfo[] fields = type.GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy);

                foreach (FieldInfo field in fields)
                {
                    if (field.FieldType == typeof(int))
                    {
                        string actionName = $"{type.Name.Split("ActionId")[0]} - {field.Name}";
                        int actionValue = (int)field.GetValue(null);

                        if (!actionDict.ContainsValue(actionValue))
                        {
                            actionDict[actionName] = actionValue;
                        }
                    }
                }
            }

            return actionDict;
        }
    }

}