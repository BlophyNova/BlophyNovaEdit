using System;
using System.Collections.Generic;

namespace Data.Interface
{
    public interface IRefresh
    {
        public void Refresh();
        public static bool TypesHasThis(List<Type> types,IRefresh refresh)
        {
            if(types==null)return false;
            foreach (var item in types)
            {
                if (refresh.GetType() == item) return true;
            }
            return false;
        }
    }
}