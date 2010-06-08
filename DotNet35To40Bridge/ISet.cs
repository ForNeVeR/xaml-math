using System.Diagnostics;
using System.Security.Permissions;
using System.Runtime.Serialization;
using System.Security;
using System.Runtime.InteropServices;

namespace System.Collections.Generic
{
    public interface ISet<T> : ICollection<T>, IEnumerable<T>, IEnumerable
    {
        // Methods
        new bool Add(T item);
        void ExceptWith(IEnumerable<T> other);
        void IntersectWith(IEnumerable<T> other);
        bool IsProperSubsetOf(IEnumerable<T> other);
        bool IsProperSupersetOf(IEnumerable<T> other);
        bool IsSubsetOf(IEnumerable<T> other);
        bool IsSupersetOf(IEnumerable<T> other);
        bool Overlaps(IEnumerable<T> other);
        bool SetEquals(IEnumerable<T> other);
        void SymmetricExceptWith(IEnumerable<T> other);
        void UnionWith(IEnumerable<T> other);
    }
}





