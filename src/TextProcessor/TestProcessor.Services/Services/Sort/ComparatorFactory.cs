using System;
using System.Collections;
using System.Collections.Generic;
using TextProcessor.Base.Enums;

namespace TextProcessor.Services.Services.Sort
{
    #region Interface
    /// <summary>
    /// This gives the camparator for custom comparision
    /// </summary>
    public interface IComparatorFactory
    {
        IComparer GetComparator(SortBy sortBy);
    }

    #endregion

    #region implementation
    public class ComparatorFactory : IComparatorFactory
    {
        private readonly IDictionary<SortBy, Func<IComparer>> _factory;

        public ComparatorFactory(AlphanumComparator alphanumComparator) => _factory = new Dictionary<SortBy, Func<IComparer>>
        {
            {SortBy.Alphanumeric, () => alphanumComparator }
        };
   
        public IComparer GetComparator(SortBy sortBy)
        {
            if(_factory.ContainsKey(sortBy))
            {
                return _factory[sortBy]();
            }
            else
            {
                return null;
            }
        }
    }
    #endregion

}
