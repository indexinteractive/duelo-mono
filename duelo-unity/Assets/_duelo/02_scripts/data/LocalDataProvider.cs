namespace Duelo.Data
{
    using System;
    using System.Collections;
    using System.Linq.Expressions;
    using Duelo.Common.Model;
    using UnityEngine;

    public class LocalDataProvider : IDataProvider
    {
        public IEnumerator Initialize(MatchDto match)
        {
            Debug.Log("[LocalDataProvider] Initializing");
            yield return null;


            Debug.Log("[LocalDataProvider] Initialized");
        }

        public void Set<TProp>(Expression<Func<MatchDto, TProp>> propertyExpression, TProp value)
        {
            // throw new NotImplementedException();
        }

        public void Subscribe<TProp>(Expression<Func<MatchDto, TProp>> propertyExpression, Action<TProp, TProp> callback)
        {
            // throw new NotImplementedException();
        }

        public void Unsubscribe<TProp>(Action<TProp, TProp> callback)
        {
            // throw new NotImplementedException();
        }
    }
}