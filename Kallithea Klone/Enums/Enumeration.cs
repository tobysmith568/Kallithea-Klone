using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace KallitheaKlone.Enums
{
    public abstract class Enumeration : IComparable
    {
        //  Properties
        //  ==========

        public string Value { get; private set; }
        public int Id { get; private set; }

        //  Constructors
        //  ============

        protected Enumeration(int id, string value)
        {
            Id = id;
            Value = value;
        }

        //  Methods
        //  =======

        public override string ToString() => Value ?? string.Empty;

        /// <exception cref="TargetException">Ignore.</exception>
        /// <exception cref="FieldAccessException">Ignore.</exception>
        public static IEnumerable<T> GetAll<T>() where T : Enumeration
        {
            var fields = typeof(T).GetFields(BindingFlags.Public |
                                             BindingFlags.Static |
                                             BindingFlags.DeclaredOnly);

            return fields.Select(f => f.GetValue(null)).Cast<T>();
        }

        public override bool Equals(object obj)
        {
            if (!(obj is Enumeration otherValue))
            {
                return false;
            }

            var typeMatches = GetType().Equals(obj.GetType());
            var valueMatches = Id.Equals(otherValue.Id);

            return typeMatches && valueMatches;
        }

        public int CompareTo(object other) => Id.CompareTo(((Enumeration)other).Id);
    }
}
