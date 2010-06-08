using System.Diagnostics;
using System.Security.Permissions;
using System.Runtime.Serialization;
using System.Security;
using System.Runtime.InteropServices;

namespace System.Collections.Generic
{
    [Serializable, DebuggerDisplay("Count = {Count}"), HostProtection(SecurityAction.LinkDemand, MayLeakOnAbort = true)]
    public class HashSet<T> : ICollection<T>, IEnumerable<T>, IEnumerable, ISerializable, IDeserializationCallback
    {
        private const string CapacityName = "Capacity";
        private const string ComparerName = "Comparer";
        private const string ElementsName = "Elements";
        private const int GrowthFactor = 2;
        private const int Lower31BitMask = 0x7fffffff;
        private int[] m_buckets;
        private IEqualityComparer<T> m_comparer;
        private int m_count;
        private int m_freeList;
        private int m_lastIndex;
        private SerializationInfo m_siInfo;
        private Slot<T>[] m_slots;
        private int m_version;
        private const int ShrinkThreshold = 3;
        private const int StackAllocThreshold = 100;
        private const string VersionName = "Version";

        public HashSet()
            : this(EqualityComparer<T>.Default)
        {
        }

        public HashSet(IEnumerable<T> collection)
            : this(collection, EqualityComparer<T>.Default)
        {
        }

        public HashSet(IEqualityComparer<T> comparer)
        {
            if (comparer == null)
            {
                comparer = EqualityComparer<T>.Default;
            }
            this.m_comparer = comparer;
            this.m_lastIndex = 0;
            this.m_count = 0;
            this.m_freeList = -1;
            this.m_version = 0;
        }

        public HashSet(IEnumerable<T> collection, IEqualityComparer<T> comparer)
            : this(comparer)
        {
            if (collection == null)
            {
                throw new ArgumentNullException("collection");
            }
            int capacity = 0;
            ICollection<T> is2 = collection as ICollection<T>;
            if (is2 != null)
            {
                capacity = is2.Count;
            }
            this.Initialize(capacity);
            this.UnionWith(collection);
            if (((this.m_count == 0) && (this.m_slots.Length > HashHelpers.GetMinPrime())) || ((this.m_count > 0) && ((this.m_slots.Length / this.m_count) > 3)))
            {
                this.TrimExcess();
            }
        }

        protected HashSet(SerializationInfo info, StreamingContext context)
        {
            this.m_siInfo = info;
        }

        public bool Add(T item)
        {
            return this.AddIfNotPresent(item);
        }

        private bool AddIfNotPresent(T value)
        {
            int freeList;
            if (this.m_buckets == null)
            {
                this.Initialize(0);
            }
            int hashCode = this.InternalGetHashCode(value);
            int index = hashCode % this.m_buckets.Length;
            for (int i = this.m_buckets[hashCode % this.m_buckets.Length] - 1; i >= 0; i = this.m_slots[i].next)
            {
                if ((this.m_slots[i].hashCode == hashCode) && this.m_comparer.Equals(this.m_slots[i].value, value))
                {
                    return false;
                }
            }
            if (this.m_freeList >= 0)
            {
                freeList = this.m_freeList;
                this.m_freeList = this.m_slots[freeList].next;
            }
            else
            {
                if (this.m_lastIndex == this.m_slots.Length)
                {
                    this.IncreaseCapacity();
                    index = hashCode % this.m_buckets.Length;
                }
                freeList = this.m_lastIndex;
                this.m_lastIndex++;
            }
            this.m_slots[freeList].hashCode = hashCode;
            this.m_slots[freeList].value = value;
            this.m_slots[freeList].next = this.m_buckets[index] - 1;
            this.m_buckets[index] = freeList + 1;
            this.m_count++;
            this.m_version++;
            return true;
        }

        private bool AddOrGetLocation(T value, out int location)
        {
            int freeList;
            int hashCode = this.InternalGetHashCode(value);
            int index = hashCode % this.m_buckets.Length;
            for (int i = this.m_buckets[hashCode % this.m_buckets.Length] - 1; i >= 0; i = this.m_slots[i].next)
            {
                if ((this.m_slots[i].hashCode == hashCode) && this.m_comparer.Equals(this.m_slots[i].value, value))
                {
                    location = i;
                    return false;
                }
            }
            if (this.m_freeList >= 0)
            {
                freeList = this.m_freeList;
                this.m_freeList = this.m_slots[freeList].next;
            }
            else
            {
                if (this.m_lastIndex == this.m_slots.Length)
                {
                    this.IncreaseCapacity();
                    index = hashCode % this.m_buckets.Length;
                }
                freeList = this.m_lastIndex;
                this.m_lastIndex++;
            }
            this.m_slots[freeList].hashCode = hashCode;
            this.m_slots[freeList].value = value;
            this.m_slots[freeList].next = this.m_buckets[index] - 1;
            this.m_buckets[index] = freeList + 1;
            this.m_count++;
            this.m_version++;
            location = freeList;
            return true;
        }

        private static bool AreEqualityComparersEqual(HashSet<T> set1, HashSet<T> set2)
        {
            return set1.Comparer.Equals(set2.Comparer);
        }

        [SecurityCritical]
        private unsafe ElementCount<T> CheckUniqueAndUnfoundElements(IEnumerable<T> other, bool returnIfUnfound)
        {
            ElementCount<T> count;
            if (this.m_count != 0)
            {
                BitHelper helper;
                int length = BitHelper.ToIntArrayLength(this.m_lastIndex);
                if (length <= 100)
                {
                    int* bitArrayPtr = stackalloc int[length];
                    helper = new BitHelper(bitArrayPtr, length);
                }
                else
                {
                    int[] bitArray = new int[length];
                    helper = new BitHelper(bitArray, length);
                }
                int num4 = 0;
                int num5 = 0;
                foreach (T local in other)
                {
                    int bitPosition = this.InternalIndexOf(local);
                    if (bitPosition >= 0)
                    {
                        if (!helper.IsMarked(bitPosition))
                        {
                            helper.MarkBit(bitPosition);
                            num5++;
                        }
                    }
                    else
                    {
                        num4++;
                        if (returnIfUnfound)
                        {
                            break;
                        }
                    }
                }
                count.uniqueCount = num5;
                count.unfoundCount = num4;
                return count;
            }
            int num = 0;
            using (IEnumerator<T> enumerator = other.GetEnumerator())
            {
                while (enumerator.MoveNext())
                {
                    T current = enumerator.Current;
                    num++;
                    goto Label_0039;
                }
            }
        Label_0039:
            count.uniqueCount = 0;
            count.unfoundCount = num;
            return count;
        }

        public void Clear()
        {
            if (this.m_lastIndex > 0)
            {
                Array.Clear(this.m_slots, 0, this.m_lastIndex);
                Array.Clear(this.m_buckets, 0, this.m_buckets.Length);
                this.m_lastIndex = 0;
                this.m_count = 0;
                this.m_freeList = -1;
            }
            this.m_version++;
        }

        public bool Contains(T item)
        {
            if (this.m_buckets != null)
            {
                int hashCode = this.InternalGetHashCode(item);
                for (int i = this.m_buckets[hashCode % this.m_buckets.Length] - 1; i >= 0; i = this.m_slots[i].next)
                {
                    if ((this.m_slots[i].hashCode == hashCode) && this.m_comparer.Equals(this.m_slots[i].value, item))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        private bool ContainsAllElements(IEnumerable<T> other)
        {
            foreach (T local in other)
            {
                if (!this.Contains(local))
                {
                    return false;
                }
            }
            return true;
        }

        public void CopyTo(T[] array)
        {
            this.CopyTo(array, 0, this.m_count);
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            this.CopyTo(array, arrayIndex, this.m_count);
        }

        public void CopyTo(T[] array, int arrayIndex, int count)
        {
            if (array == null)
            {
                throw new ArgumentNullException("array");
            }
            if (arrayIndex < 0)
            {
                throw new ArgumentOutOfRangeException("arrayIndex", SR.GetString("ArgumentOutOfRange_NeedNonNegNum"));
            }
            if (count < 0)
            {
                throw new ArgumentOutOfRangeException("count", SR.GetString("ArgumentOutOfRange_NeedNonNegNum"));
            }
            if ((arrayIndex > array.Length) || (count > (array.Length - arrayIndex)))
            {
                throw new ArgumentException(SR.GetString("Arg_ArrayPlusOffTooSmall"));
            }
            int num = 0;
            for (int i = 0; (i < this.m_lastIndex) && (num < count); i++)
            {
                if (this.m_slots[i].hashCode >= 0)
                {
                    array[arrayIndex + num] = this.m_slots[i].value;
                    num++;
                }
            }
        }

        public static IEqualityComparer<HashSet<T>> CreateSetComparer()
        {
            return new HashSetEqualityComparer<T>();
        }

        public void ExceptWith(IEnumerable<T> other)
        {
            if (other == null)
            {
                throw new ArgumentNullException("other");
            }
            if (this.m_count != 0)
            {
                if (other == this)
                {
                    this.Clear();
                }
                else
                {
                    foreach (T local in other)
                    {
                        this.Remove(local);
                    }
                }
            }
        }

        public Enumerator<T> GetEnumerator()
        {
            return new Enumerator<T>((HashSet<T>)this);
        }

        [SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.SerializationFormatter)]
        public virtual void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            if (info == null)
            {
                throw new ArgumentNullException("info");
            }
            info.AddValue("Version", this.m_version);
            info.AddValue("Comparer", this.m_comparer, typeof(IEqualityComparer<T>));
            info.AddValue("Capacity", (this.m_buckets == null) ? 0 : this.m_buckets.Length);
            if (this.m_buckets != null)
            {
                T[] array = new T[this.m_count];
                this.CopyTo(array);
                info.AddValue("Elements", array, typeof(T[]));
            }
        }

        internal static bool HashSetEquals(HashSet<T> set1, HashSet<T> set2, IEqualityComparer<T> comparer)
        {
            if (set1 == null)
            {
                return (set2 == null);
            }
            if (set2 == null)
            {
                return false;
            }
            if (HashSet<T>.AreEqualityComparersEqual(set1, set2))
            {
                if (set1.Count != set2.Count)
                {
                    return false;
                }
                foreach (T local in set2)
                {
                    if (!set1.Contains(local))
                    {
                        return false;
                    }
                }
                return true;
            }
            foreach (T local2 in set2)
            {
                bool flag = false;
                foreach (T local3 in set1)
                {
                    if (comparer.Equals(local2, local3))
                    {
                        flag = true;
                        break;
                    }
                }
                if (!flag)
                {
                    return false;
                }
            }
            return true;
        }

        private void IncreaseCapacity()
        {
            int min = this.m_count * 2;
            if (min < 0)
            {
                min = this.m_count;
            }
            int prime = HashHelpers.GetPrime(min);
            if (prime <= this.m_count)
            {
                throw new ArgumentException(SR.GetString("Arg_HSCapacityOverflow"));
            }
            Slot<T>[] destinationArray = new Slot<T>[prime];
            if (this.m_slots != null)
            {
                Array.Copy(this.m_slots, 0, destinationArray, 0, this.m_lastIndex);
            }
            int[] numArray = new int[prime];
            for (int i = 0; i < this.m_lastIndex; i++)
            {
                int index = destinationArray[i].hashCode % prime;
                destinationArray[i].next = numArray[index] - 1;
                numArray[index] = i + 1;
            }
            this.m_slots = destinationArray;
            this.m_buckets = numArray;
        }

        private void Initialize(int capacity)
        {
            int prime = HashHelpers.GetPrime(capacity);
            this.m_buckets = new int[prime];
            this.m_slots = new Slot<T>[prime];
        }

        private int InternalGetHashCode(T item)
        {
            if (item == null)
            {
                return 0;
            }
            return (this.m_comparer.GetHashCode(item) & 0x7fffffff);
        }

        private int InternalIndexOf(T item)
        {
            int hashCode = this.InternalGetHashCode(item);
            for (int i = this.m_buckets[hashCode % this.m_buckets.Length] - 1; i >= 0; i = this.m_slots[i].next)
            {
                if ((this.m_slots[i].hashCode == hashCode) && this.m_comparer.Equals(this.m_slots[i].value, item))
                {
                    return i;
                }
            }
            return -1;
        }

        [SecurityCritical]
        public void IntersectWith(IEnumerable<T> other)
        {
            if (other == null)
            {
                throw new ArgumentNullException("other");
            }
            if (this.m_count != 0)
            {
                ICollection<T> is2 = other as ICollection<T>;
                if (is2 != null)
                {
                    if (is2.Count == 0)
                    {
                        this.Clear();
                        return;
                    }
                    HashSet<T> set = other as HashSet<T>;
                    if ((set != null) && HashSet<T>.AreEqualityComparersEqual((HashSet<T>)this, set))
                    {
                        this.IntersectWithHashSetWithSameEC(set);
                        return;
                    }
                }
                this.IntersectWithEnumerable(other);
            }
        }

        [SecurityCritical]
        private unsafe void IntersectWithEnumerable(IEnumerable<T> other)
        {
            BitHelper helper;
            int lastIndex = this.m_lastIndex;
            int length = BitHelper.ToIntArrayLength(lastIndex);
            if (length <= 100)
            {
                int* bitArrayPtr = stackalloc int[length];
                helper = new BitHelper(bitArrayPtr, length);
            }
            else
            {
                int[] bitArray = new int[length];
                helper = new BitHelper(bitArray, length);
            }
            foreach (T local in other)
            {
                int bitPosition = this.InternalIndexOf(local);
                if (bitPosition >= 0)
                {
                    helper.MarkBit(bitPosition);
                }
            }
            for (int i = 0; i < lastIndex; i++)
            {
                if ((this.m_slots[i].hashCode >= 0) && !helper.IsMarked(i))
                {
                    this.Remove(this.m_slots[i].value);
                }
            }
        }

        private void IntersectWithHashSetWithSameEC(HashSet<T> other)
        {
            for (int i = 0; i < this.m_lastIndex; i++)
            {
                if (this.m_slots[i].hashCode >= 0)
                {
                    T item = this.m_slots[i].value;
                    if (!other.Contains(item))
                    {
                        this.Remove(item);
                    }
                }
            }
        }

        [SecurityCritical]
        public bool IsProperSubsetOf(IEnumerable<T> other)
        {
            if (other == null)
            {
                throw new ArgumentNullException("other");
            }
            ICollection<T> is2 = other as ICollection<T>;
            if (is2 != null)
            {
                if (this.m_count == 0)
                {
                    return (is2.Count > 0);
                }
                HashSet<T> set = other as HashSet<T>;
                if ((set != null) && HashSet<T>.AreEqualityComparersEqual((HashSet<T>)this, set))
                {
                    if (this.m_count >= set.Count)
                    {
                        return false;
                    }
                    return this.IsSubsetOfHashSetWithSameEC(set);
                }
            }
            ElementCount<T> count = this.CheckUniqueAndUnfoundElements(other, false);
            return ((count.uniqueCount == this.m_count) && (count.unfoundCount > 0));
        }

        [SecurityCritical]
        public bool IsProperSupersetOf(IEnumerable<T> other)
        {
            if (other == null)
            {
                throw new ArgumentNullException("other");
            }
            if (this.m_count == 0)
            {
                return false;
            }
            ICollection<T> is2 = other as ICollection<T>;
            if (is2 != null)
            {
                if (is2.Count == 0)
                {
                    return true;
                }
                HashSet<T> set = other as HashSet<T>;
                if ((set != null) && HashSet<T>.AreEqualityComparersEqual((HashSet<T>)this, set))
                {
                    if (set.Count >= this.m_count)
                    {
                        return false;
                    }
                    return this.ContainsAllElements(set);
                }
            }
            ElementCount<T> count = this.CheckUniqueAndUnfoundElements(other, true);
            return ((count.uniqueCount < this.m_count) && (count.unfoundCount == 0));
        }

        [SecurityCritical]
        public bool IsSubsetOf(IEnumerable<T> other)
        {
            if (other == null)
            {
                throw new ArgumentNullException("other");
            }
            if (this.m_count == 0)
            {
                return true;
            }
            HashSet<T> set = other as HashSet<T>;
            if ((set != null) && HashSet<T>.AreEqualityComparersEqual((HashSet<T>)this, set))
            {
                if (this.m_count > set.Count)
                {
                    return false;
                }
                return this.IsSubsetOfHashSetWithSameEC(set);
            }
            ElementCount<T> count = this.CheckUniqueAndUnfoundElements(other, false);
            return ((count.uniqueCount == this.m_count) && (count.unfoundCount >= 0));
        }

        private bool IsSubsetOfHashSetWithSameEC(HashSet<T> other)
        {
            foreach (T local in this)
            {
                if (!other.Contains(local))
                {
                    return false;
                }
            }
            return true;
        }

        public bool IsSupersetOf(IEnumerable<T> other)
        {
            if (other == null)
            {
                throw new ArgumentNullException("other");
            }
            ICollection<T> is2 = other as ICollection<T>;
            if (is2 != null)
            {
                if (is2.Count == 0)
                {
                    return true;
                }
                HashSet<T> set = other as HashSet<T>;
                if (((set != null) && HashSet<T>.AreEqualityComparersEqual((HashSet<T>)this, set)) && (set.Count > this.m_count))
                {
                    return false;
                }
            }
            return this.ContainsAllElements(other);
        }

        public virtual void OnDeserialization(object sender)
        {
            if (this.m_siInfo != null)
            {
                int num = this.m_siInfo.GetInt32("Capacity");
                this.m_comparer = (IEqualityComparer<T>)this.m_siInfo.GetValue("Comparer", typeof(IEqualityComparer<T>));
                this.m_freeList = -1;
                if (num != 0)
                {
                    this.m_buckets = new int[num];
                    this.m_slots = new Slot<T>[num];
                    T[] localArray = (T[])this.m_siInfo.GetValue("Elements", typeof(T[]));
                    if (localArray == null)
                    {
                        throw new SerializationException(SR.GetString("Serialization_MissingKeys"));
                    }
                    for (int i = 0; i < localArray.Length; i++)
                    {
                        this.AddIfNotPresent(localArray[i]);
                    }
                }
                else
                {
                    this.m_buckets = null;
                }
                this.m_version = this.m_siInfo.GetInt32("Version");
                this.m_siInfo = null;
            }
        }

        public bool Overlaps(IEnumerable<T> other)
        {
            if (other == null)
            {
                throw new ArgumentNullException("other");
            }
            if (this.m_count != 0)
            {
                foreach (T local in other)
                {
                    if (this.Contains(local))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        public bool Remove(T item)
        {
            if (this.m_buckets != null)
            {
                int hashCode = this.InternalGetHashCode(item);
                int index = hashCode % this.m_buckets.Length;
                int num3 = -1;
                for (int i = this.m_buckets[index] - 1; i >= 0; i = this.m_slots[i].next)
                {
                    if ((this.m_slots[i].hashCode == hashCode) && this.m_comparer.Equals(this.m_slots[i].value, item))
                    {
                        if (num3 < 0)
                        {
                            this.m_buckets[index] = this.m_slots[i].next + 1;
                        }
                        else
                        {
                            this.m_slots[num3].next = this.m_slots[i].next;
                        }
                        this.m_slots[i].hashCode = -1;
                        this.m_slots[i].value = default(T);
                        this.m_slots[i].next = this.m_freeList;
                        this.m_freeList = i;
                        this.m_count--;
                        this.m_version++;
                        return true;
                    }
                    num3 = i;
                }
            }
            return false;
        }

        public int RemoveWhere(Predicate<T> match)
        {
            if (match == null)
            {
                throw new ArgumentNullException("match");
            }
            int num = 0;
            for (int i = 0; i < this.m_lastIndex; i++)
            {
                if (this.m_slots[i].hashCode >= 0)
                {
                    T local = this.m_slots[i].value;
                    if (match(local) && this.Remove(local))
                    {
                        num++;
                    }
                }
            }
            return num;
        }

        [SecurityCritical]
        public bool SetEquals(IEnumerable<T> other)
        {
            if (other == null)
            {
                throw new ArgumentNullException("other");
            }
            HashSet<T> set = other as HashSet<T>;
            if ((set != null) && HashSet<T>.AreEqualityComparersEqual((HashSet<T>)this, set))
            {
                if (this.m_count != set.Count)
                {
                    return false;
                }
                return this.ContainsAllElements(set);
            }
            ICollection<T> is2 = other as ICollection<T>;
            if (((is2 != null) && (this.m_count == 0)) && (is2.Count > 0))
            {
                return false;
            }
            ElementCount<T> count = this.CheckUniqueAndUnfoundElements(other, true);
            return ((count.uniqueCount == this.m_count) && (count.unfoundCount == 0));
        }

        [SecurityCritical]
        public void SymmetricExceptWith(IEnumerable<T> other)
        {
            if (other == null)
            {
                throw new ArgumentNullException("other");
            }
            if (this.m_count == 0)
            {
                this.UnionWith(other);
            }
            else if (other == this)
            {
                this.Clear();
            }
            else
            {
                HashSet<T> set = other as HashSet<T>;
                if ((set != null) && HashSet<T>.AreEqualityComparersEqual((HashSet<T>)this, set))
                {
                    this.SymmetricExceptWithUniqueHashSet(set);
                }
                else
                {
                    this.SymmetricExceptWithEnumerable(other);
                }
            }
        }

        [SecurityCritical]
        private unsafe void SymmetricExceptWithEnumerable(IEnumerable<T> other)
        {
            BitHelper helper;
            BitHelper helper2;
            int lastIndex = this.m_lastIndex;
            int length = BitHelper.ToIntArrayLength(lastIndex);
            if (length <= 50)
            {
                int* bitArrayPtr = stackalloc int[length];
                helper = new BitHelper(bitArrayPtr, length);
                int* numPtr2 = stackalloc int[length];
                helper2 = new BitHelper(numPtr2, length);
            }
            else
            {
                int[] bitArray = new int[length];
                helper = new BitHelper(bitArray, length);
                int[] numArray2 = new int[length];
                helper2 = new BitHelper(numArray2, length);
            }
            foreach (T local in other)
            {
                int location = 0;
                if (this.AddOrGetLocation(local, out location))
                {
                    helper2.MarkBit(location);
                }
                else if ((location < lastIndex) && !helper2.IsMarked(location))
                {
                    helper.MarkBit(location);
                }
            }
            for (int i = 0; i < lastIndex; i++)
            {
                if (helper.IsMarked(i))
                {
                    this.Remove(this.m_slots[i].value);
                }
            }
        }

        private void SymmetricExceptWithUniqueHashSet(HashSet<T> other)
        {
            foreach (T local in other)
            {
                if (!this.Remove(local))
                {
                    this.AddIfNotPresent(local);
                }
            }
        }

        void ICollection<T>.Add(T item)
        {
            this.AddIfNotPresent(item);
        }

        IEnumerator<T> IEnumerable<T>.GetEnumerator()
        {
            return new Enumerator<T>((HashSet<T>)this);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return new Enumerator<T>((HashSet<T>)this);
        }

        internal T[] ToArray()
        {
            T[] array = new T[this.Count];
            this.CopyTo(array);
            return array;
        }

        public void TrimExcess()
        {
            if (this.m_count == 0)
            {
                this.m_buckets = null;
                this.m_slots = null;
                this.m_version++;
            }
            else
            {
                int prime = HashHelpers.GetPrime(this.m_count);
                Slot<T>[] slotArray = new Slot<T>[prime];
                int[] numArray = new int[prime];
                int index = 0;
                for (int i = 0; i < this.m_lastIndex; i++)
                {
                    if (this.m_slots[i].hashCode >= 0)
                    {
                        slotArray[index] = this.m_slots[i];
                        int num4 = slotArray[index].hashCode % prime;
                        slotArray[index].next = numArray[num4] - 1;
                        numArray[num4] = index + 1;
                        index++;
                    }
                }
                this.m_lastIndex = index;
                this.m_slots = slotArray;
                this.m_buckets = numArray;
                this.m_freeList = -1;
            }
        }

        public void UnionWith(IEnumerable<T> other)
        {
            if (other == null)
            {
                throw new ArgumentNullException("other");
            }
            foreach (T local in other)
            {
                this.AddIfNotPresent(local);
            }
        }

        // Properties
        public IEqualityComparer<T> Comparer
        {
            get
            {
                return this.m_comparer;
            }
        }

        public int Count
        {
            get
            {
                return this.m_count;
            }
        }

        bool ICollection<T>.IsReadOnly
        {
            get
            {
                return false;
            }
        }

        // Nested Types
        [StructLayout(LayoutKind.Sequential)]
        internal struct ElementCount
        {
            internal int uniqueCount;
            internal int unfoundCount;
        }

        [Serializable, StructLayout(LayoutKind.Sequential), HostProtection(SecurityAction.LinkDemand, MayLeakOnAbort = true)]
        public struct Enumerator : IEnumerator<T>, IDisposable, IEnumerator
        {
            private HashSet<T> set;
            private int index;
            private int version;
            private T current;
            internal Enumerator(HashSet<T> set)
            {
                this.set = set;
                this.index = 0;
                this.version = set.m_version;
                this.current = default(T);
            }

            public void Dispose()
            {
            }

            public bool MoveNext()
            {
                if (this.version != this.set.m_version)
                {
                    throw new InvalidOperationException(SR.GetString("InvalidOperation_EnumFailedVersion"));
                }
                while (this.index < this.set.m_lastIndex)
                {
                    if (this.set.m_slots[this.index].hashCode >= 0)
                    {
                        this.current = this.set.m_slots[this.index].value;
                        this.index++;
                        return true;
                    }
                    this.index++;
                }
                this.index = this.set.m_lastIndex + 1;
                this.current = default(T);
                return false;
            }

            public T Current
            {
                get
                {
                    return this.current;
                }
            }
            object IEnumerator.Current
            {
                get
                {
                    if ((this.index == 0) || (this.index == (this.set.m_lastIndex + 1)))
                    {
                        throw new InvalidOperationException(SR.GetString("InvalidOperation_EnumOpCantHappen"));
                    }
                    return this.Current;
                }
            }
            void IEnumerator.Reset()
            {
                if (this.version != this.set.m_version)
                {
                    throw new InvalidOperationException(SR.GetString("InvalidOperation_EnumFailedVersion"));
                }
                this.index = 0;
                this.current = default(T);
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        internal struct Slot
        {
            internal int hashCode;
            internal T value;
            internal int next;
        }
    }
}
