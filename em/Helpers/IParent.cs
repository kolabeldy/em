using System;
using System.Collections.Generic;

namespace em.Helpers
{
    interface IParent<T>
    {
        IEnumerable<T> GetChildren();
    }
}
