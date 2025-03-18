namespace Duelo.Data
{
    using System;
    using System.Collections;
    using System.Linq.Expressions;
    using Duelo.Common.Model;

    public interface IDataProvider
    {
        public IEnumerator Initialize(MatchDto match);
        public void Subscribe<TProp>(Expression<Func<MatchDto, TProp>> propertyExpression, Action<TProp, TProp> callback);
        public void Unsubscribe<TProp>(Action<TProp, TProp> callback);
        public void Set<TProp>(Expression<Func<MatchDto, TProp>> propertyExpression, TProp value);
    }
}