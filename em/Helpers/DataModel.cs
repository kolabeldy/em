using System;
using System.Collections.Generic;
using System.Windows;

namespace em.Helpers
{
    public class Family : DependencyObject, IParent<object>
    {
        public string Name { get; set; }
        public int Id { get; set; }
        public List<Person> Members { get; set; }

        IEnumerable<object> IParent<object>.GetChildren()
        {
            return Members;
        }
    }

    public class Person : DependencyObject
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }
}