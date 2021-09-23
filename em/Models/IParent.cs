using System;
using System.Collections.Generic;

namespace em.Models
{
    interface IParent<T>
    {
        IEnumerable<T> GetChildren();
    }
}
