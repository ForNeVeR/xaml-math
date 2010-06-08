using System.Text;
using System.Collections;
namespace System
{
    internal interface ITuple
    {
        int GetHashCode(IEqualityComparer comparer);
        string ToString(StringBuilder sb);
        int Size { get; }
    }
}
