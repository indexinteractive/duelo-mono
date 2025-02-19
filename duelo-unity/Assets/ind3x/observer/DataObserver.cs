namespace Ind3x.Observer
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Reflection;
    using Unity.VisualScripting;

    public class DataObserver<TData>
    {
        #region Private Fields
        private TData _data;

        /// <summary>
        /// Dictionary of subscribers for each property path.
        /// </summary>
        private readonly Dictionary<string, List<Action<object, object>>> _subscribers = new();

        /// <summary>
        /// Dictionary of callback references for each property path.
        /// Used to store actual callback references that can later be referenced in <see cref="Unsubscribe"/>.
        /// </summary>
        private readonly Dictionary<string, List<Delegate>> _callbackReferences = new();

        private readonly object _lock = new();
        #endregion

        #region Initialization
        public DataObserver(TData data = default)
        {
            _data = data ?? default;
        }
        #endregion

        public string GetPropertyPath<TProp>(Expression<Func<TData, TProp>> expression)
        {
            var bodyString = expression.Body.ToString();
            var firstDotIndex = bodyString.IndexOf('.') + 1;
            return bodyString.Substring(firstDotIndex);
        }

        #region Subscriptions
        public void Subscribe<TProp>(Expression<Func<TData, TProp>> propertyExpression, Action<TProp, TProp> callback)
        {
            string propertyPath = GetPropertyPath(propertyExpression);

            if (!_subscribers.ContainsKey(propertyPath))
            {
                _subscribers[propertyPath] = new List<Action<object, object>>();
                _callbackReferences[propertyPath] = new List<Delegate>();
            }

            Action<object, object> wrappedCallback = (newValue, oldValue) => callback((TProp)newValue, (TProp)oldValue);
            _subscribers[propertyPath].Add(wrappedCallback);
            _callbackReferences[propertyPath].Add(callback);
        }

        public void Unsubscribe<TProp>(Action<TProp, TProp> callback)
        {
            lock (_lock)
            {
                for (int i = _subscribers.Count - 1; i >= 0; i--)
                {
                    var entry = _subscribers.ElementAt(i);
                    int index = _callbackReferences[entry.Key].FindIndex(d => d == (Delegate)callback);
                    if (index >= 0)
                    {
                        _subscribers[entry.Key].RemoveAt(index);
                        _callbackReferences[entry.Key].RemoveAt(index);
                    }

                    if (_subscribers[entry.Key].Count == 0)
                    {
                        _subscribers.Remove(entry.Key);
                        _callbackReferences.Remove(entry.Key);
                    }
                }
            }
        }

        private void NotifySubscribers(string path, object newValue, object oldValue)
        {
            if (!Equals(newValue, oldValue))
            {
                foreach (var callback in _subscribers[path])
                {
                    callback(newValue, oldValue);
                }
            }
        }
        #endregion

        #region Data Methods
        public void ReplaceData(TData newData)
        {
            lock (_lock)
            {
                foreach (var propertyPath in _subscribers.Keys)
                {
                    object newValue = GetMemberValue(newData, propertyPath);
                    object oldValue = GetMemberValue(_data, propertyPath) ?? Activator.CreateInstance(newValue.GetType());

                    NotifySubscribers(propertyPath, newValue, oldValue);
                }

                _data = newData;
            }
        }

        public void Set<TProp>(Expression<Func<TData, TProp>> propertyExpression, TProp newValue)
        {
            string propertyPath = GetPropertyPath(propertyExpression);

            lock (_lock)
            {
                object oldValue = GetMemberValue(_data, propertyPath);

                if (!Equals(newValue, oldValue))
                {
                    SetPropertyValue(_data, propertyPath, newValue);

                    if (_subscribers.ContainsKey(propertyPath))
                    {
                        object safeNewValue = newValue ?? default(TProp);
                        object safeOldValue = oldValue ?? default(TProp);

                        NotifySubscribers(propertyPath, safeNewValue, safeOldValue);
                    }
                }
            }
        }
        #endregion

        #region Reflection
        private static object GetMemberValue(object obj, string propertyPath)
        {
            object GetMemberInfoValue(MemberInfo memberInfo, object fromObj) => memberInfo.MemberType switch
            {
                MemberTypes.Field => ((FieldInfo)memberInfo).GetValue(fromObj),
                MemberTypes.Property => ((PropertyInfo)memberInfo).GetValue(fromObj),
                _ => throw new NotImplementedException()
            };

            foreach (var part in propertyPath.Split('.'))
            {
                if (obj == null)
                {
                    return null;
                }

                Type type = obj.GetType();

                int leftBracketIndex = part.IndexOf('[');
                int rightBracketIndex = part.IndexOf(']');

                if (leftBracketIndex >= 0 && rightBracketIndex >= 0)
                {
                    string collectionName = part.Substring(0, leftBracketIndex);
                    int index = int.Parse(part.Substring(leftBracketIndex + 1, rightBracketIndex - leftBracketIndex - 1));

                    MemberInfo info = type.GetField(collectionName) as MemberInfo ?? type.GetProperty(collectionName) as MemberInfo;

                    var collection = GetMemberInfoValue(info, obj) as IEnumerable;

                    if (collection != null)
                    {
                        obj = collection.Cast<object>().ElementAtOrDefault(index);
                    }
                    else
                    {
                        obj = type.Default();
                    }
                }
                else
                {
                    MemberInfo info = type.GetField(part) as MemberInfo ?? type.GetProperty(part) as MemberInfo;
                    obj = GetMemberInfoValue(info, obj);
                }
            }

            return obj ?? Activator.CreateInstance(obj?.GetType() ?? typeof(object));
        }

        private void SetPropertyValue(object obj, string propertyPath, object value)
        {
            string[] parts = propertyPath.Split('.');
            for (int i = 0; i < parts.Length - 1; i++)
            {
                if (obj == null)
                {
                    return;
                }

                int leftBracketIndex = parts[i].IndexOf('[');
                int rightBracketIndex = parts[i].IndexOf(']');

                if (leftBracketIndex >= 0 && rightBracketIndex >= 0)
                {
                    string collectionName = parts[i].Substring(0, leftBracketIndex);
                    int index = int.Parse(parts[i].Substring(leftBracketIndex + 1, rightBracketIndex - leftBracketIndex - 1));

                    PropertyInfo prop = obj.GetType().GetProperty(collectionName);
                    var collection = prop?.GetValue(obj) as IList;

                    if (collection != null && index < collection.Count)
                    {
                        obj = collection[index];
                    }
                    else
                    {
                        return;
                    }
                }
                else
                {
                    PropertyInfo prop = obj.GetType().GetProperty(parts[i]);
                    obj = prop?.GetValue(obj);
                }
            }

            PropertyInfo finalProp = obj?.GetType().GetProperty(parts[^1]);
            finalProp?.SetValue(obj, value);
        }
        #endregion
    }

    public static class PropertyPathHelper
    {
        public static string PropPathToString<T, TProp>(Expression<Func<T, TProp>> expression)
        {
            return GetExpressionPath(expression.Body);
        }

        private static string GetExpressionPath(Expression expression)
        {
            if (expression is MemberExpression memberExpression)
            {
                return GetMemberPathWithIndexes(memberExpression);
            }
            else if (expression is MethodCallExpression methodCall)
            {
                return GetIndexedPathWithIndexes(methodCall);
            }
            throw new ArgumentException("Invalid property expression.");
        }

        private static string GetMemberPathWithIndexes(MemberExpression expression)
        {
            string path = expression.Member.Name;
            Expression current = expression.Expression;

            while (current is MemberExpression parent)
            {
                path = parent.Member.Name + "." + path;
                current = parent.Expression;
            }

            if (current is MethodCallExpression methodCall)
            {
                path = GetIndexedPathWithIndexes(methodCall) + "." + path;
            }

            return path;
        }

        private static string GetIndexedPathWithIndexes(MethodCallExpression methodCall)
        {
            if (methodCall.Object is MemberExpression member)
            {
                string path = GetMemberPathWithIndexes(member);
                var indexArgument = methodCall.Arguments[0];

                if (indexArgument is ConstantExpression constantExpression)
                {
                    path += $"[{constantExpression.Value}]";
                }
                else
                {
                    throw new ArgumentException("Index must be a constant.");
                }

                return path;
            }

            throw new ArgumentException("Invalid method call expression.");
        }
    }

}