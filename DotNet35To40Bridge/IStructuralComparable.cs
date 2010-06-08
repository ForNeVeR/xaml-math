using System.Collections;
namespace System
{
    public interface IStructuralComparable
    {
        int CompareTo(object other, IComparer comparer);
    }
}
