using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Debug = UnityEngine.Debug;

namespace flyingSnow
{
    public class DualKeyDictionary<TK1, TK2, TV>:IEnumerable<DualKeyDictionary<TK1, TK2, TV>.KeyValuePair>
    {
        public struct KeyValuePair{
            public TK1 key1;
            public TK2 key2;
            public TV value;
        }

        private struct Entry
        {
            public int hashCode1;
            public int hashCode2;
            public int next1;
            public int next2;
            public TK1 key1;
            public TK2 key2;
            public TV value;
        }

        public static readonly int[] primes = {
            3, 7, 11, 17, 23, 29, 37, 47, 59, 71, 89, 107, 131, 163, 197, 239, 293, 353, 431, 521, 631, 761, 919,
            1103, 1327, 1597, 1931, 2333, 2801, 3371, 4049, 4861, 5839, 7013, 8419, 10103, 12143, 14591,
            17519, 21023, 25229, 30293, 36353, 43627, 52361, 62851, 75431, 90523, 108631, 130363, 156437,
            187751, 225307, 270371, 324449, 389357, 467237, 560689, 672827, 807403, 968897, 1162687, 1395263,
            1674319, 2009191, 2411033, 2893249, 3471899, 4166287, 4999559, 5999471, 7199369};

        private int[] buckets1;
        private int[] buckets2;
        private Entry[] entries;
        private int count;
        private int version;
        private int freeList;  // freeList用指针1连接
        private int freeCount;
        private IEqualityComparer<TK1> comparer1;
        private IEqualityComparer<TK2> comparer2;

        public ICollection<TK1> Key1s => throw new NotImplementedException();

        public ICollection<TK2> Key2s => throw new NotImplementedException();

        public ICollection<TV> Values => throw new NotImplementedException();

        public int Count => count - freeCount;

        public DualKeyDictionary(int capacity, IEqualityComparer<TK1> comparer1 = null, IEqualityComparer<TK2> comparer2 = null)
        {
            if (capacity < 0) throw new ArgumentOutOfRangeException("capacity", "capacity 不能小于 0");
            if (capacity > 0) Initialize(capacity);
            this.comparer1 = comparer1 ?? EqualityComparer<TK1>.Default;
            this.comparer2 = comparer2 ?? EqualityComparer<TK2>.Default;
        }
        private void Initialize(int capacity)
        {
            int size = HashHelpers.GetPrime(capacity);
            buckets1 = new int[size];
            buckets2 = new int[size];
            for (int i = 0; i < buckets1.Length; i++) buckets1[i] = -1;
            for (int i = 0; i < buckets2.Length; i++) buckets2[i] = -1;
            entries = new Entry[size];
            freeList = -1;
        }

        public bool Remove(TK1 key1, TK2 key2)
        {
            if (key1 == null || key2 == null)
            {
                ThrowArgumentException("键值不能为 null");
            }

            if (buckets1 != null && buckets2 != null)
            {
                int hashCode1 = comparer1.GetHashCode(key1) & 0x7FFFFFFF;
                int hashCode2 = comparer2.GetHashCode(key2) & 0x7FFFFFFF;
                int bucket1 = hashCode1 % buckets1.Length;
                int bucket2 = hashCode2 % buckets2.Length;

                int last1 = -1;
                for (int i = buckets1[bucket1]; i >= 0; last1 = i, i = entries[i].next1)
                {
                    if (entries[i].hashCode2 == hashCode2
                            && comparer1.Equals(entries[i].key1, key1)
                            && comparer2.Equals(entries[i].key2, key2)
                            )
                    {
                        if (last1 < 0)
                        {
                            buckets1[bucket1] = entries[i].next1;
                        }
                        else
                        {
                            entries[last1].next1 = entries[i].next1;
                        }
                        entries[i].hashCode1 = -1;
                        entries[i].next1 = freeList;
                        entries[i].key1 = default(TK1);
                        entries[i].value = default(TV);
                        freeList = i;
                        freeCount++;
                        version++;
                        return true;
                    }
                }
            }
            return false;
        }

        public void Set(TK1 key1, TK2 key2, TV value)
        {
            if (key1 == null || key2 == null)
            {
                ThrowArgumentException("键值不能为 null");
            }

            if (buckets1 == null) Initialize(0);
            int hashCode1 = comparer1.GetHashCode(key1) & 0x7FFFFFFF;
            int hashCode2 = comparer2.GetHashCode(key2) & 0x7FFFFFFF;
            int targetBucket1 = hashCode1 % buckets1.Length;
            int targetBucket2 = hashCode2 % buckets2.Length;

            for (int i = buckets1[targetBucket1]; i >= 0; i = entries[i].next1)
            {
                // 顺着链1去找
                if (entries[i].hashCode2 == hashCode2
                        && comparer1.Equals(entries[i].key1, key1)
                        && comparer2.Equals(entries[i].key2, key2)
                        )
                {
                    entries[i].value = value;
                    version++;
                    return;
                }
            }

            // 没找到对应的 key
            int index;
            if (freeCount > 0)
            {
                index = freeList;
                freeList = entries[index].next1;
                freeCount--;
            }
            else
            {
                if (count == entries.Length)
                {
                    Resize();
                    targetBucket1 = hashCode1 % buckets1.Length;
                    targetBucket2 = hashCode2 % buckets2.Length;
                }
                index = count;
                count++;
            }

            entries[index].hashCode1 = hashCode1;
            entries[index].hashCode2 = hashCode2;
            entries[index].next1 = buckets1[targetBucket1];
            entries[index].next2 = buckets2[targetBucket2];
            entries[index].key1 = key1;
            entries[index].key2 = key2;
            entries[index].value = value;
            buckets1[targetBucket1] = index;
            buckets2[targetBucket2] = index;
            version++;
        }

        public void Add(TK1 key1, TK2 key2, TV value)
        {
            Set(key1, key2, value);
        }

        private void Resize()
        {
            Resize(HashHelpers.ExpandPrime(count), false);
        }

        private void Resize(int newSize, bool forceNewHashCodes)
        {
            if (newSize < entries.Length) ThrowArgumentException($"扩容出错:{entries.Length}->{newSize}");
            int[] newBuckets1 = new int[newSize];
            int[] newBuckets2 = new int[newSize];
            for (int i = 0; i < newBuckets1.Length; i++) newBuckets1[i] = -1;
            for (int i = 0; i < newBuckets2.Length; i++) newBuckets2[i] = -1;
            Entry[] newEntries = new Entry[newSize];
            Array.Copy(entries, 0, newEntries, 0, count);
            if (forceNewHashCodes)
            {
                for (int i = 0; i < count; i++)
                {
                    if (newEntries[i].hashCode1 != -1)
                    {
                        newEntries[i].hashCode1 = comparer1.GetHashCode(newEntries[i].key1) & 0x7FFFFFFF;
                    }

                    if (newEntries[i].hashCode2 != -1)
                    {
                        newEntries[i].hashCode2 = comparer2.GetHashCode(newEntries[i].key2) & 0x7FFFFFFF;
                    }
                }
            }
            for (int i = 0; i < count; i++)
            {
                if (newEntries[i].hashCode1 >= 0)
                {
                    int bucket = newEntries[i].hashCode1 % newSize;
                    newEntries[i].next1 = newBuckets1[bucket];
                    newBuckets1[bucket] = i;
                }
                if (newEntries[i].hashCode2 >= 0)
                {
                    int bucket = newEntries[i].hashCode2 % newSize;
                    newEntries[i].next2 = newBuckets2[bucket];
                    newBuckets2[bucket] = i;
                }
            }
            buckets1 = newBuckets1;
            buckets2 = newBuckets2;
            entries = newEntries;
        }

        private int FindEntry(TK1 key1, TK2 key2)
        {
            if (buckets1 != null)
            {
                int hashCode1 = comparer1.GetHashCode(key1) & 0x7FFFFFFF;
                int hashCode2 = comparer2.GetHashCode(key2) & 0x7FFFFFFF;
                for (int i = buckets1[hashCode1 % buckets1.Length]; i >= 0; i = entries[i].next1)
                {
                    if (entries[i].hashCode1 == hashCode1
                        && entries[i].hashCode2 == hashCode2
                        && comparer1.Equals(entries[i].key1, key1)
                        && comparer2.Equals(entries[i].key2, key2)
                        )
                    {
                        return i;
                    }
                }
            }
            return -1;
        }

        public TV Get(TK1 key1, TK2 key2)
        {
            int i = FindEntry(key1, key2);
            if (i >= 0)
            {
                return entries[i].value;
            }
            return default(TV);
        }

        private void ThrowArgumentException(string msg)
        {
            throw new ArgumentNullException(msg);
        }

        public bool TryGetValue(TK1 key1, TK2 key2, out TV value)
        {
            int i = FindEntry(key1, key2);
            if (i >= 0)
            {
                value = entries[i].value;
                return true;
            }
            value = default(TV);
            return false;
        }

        public void Clear()
        {
            if (count > 0)
            {
                for (int i = 0; i < buckets1.Length; i++) buckets1[i] = -1;
                for (int i = 0; i < buckets2.Length; i++) buckets2[i] = -1;
                Array.Clear(entries, 0, count);
                freeList = -1;
                count = 0;
                freeCount = 0;
                version++;
            }
        }

        public bool Contains(TK1 key1, TK2 key2)
        {
            return FindEntry(key1, key2) >= 0;
        }

        public void Print() {
            for (int i = 0; i < entries.Length; i++)
            {
                var entry = entries[i];
                Debug.Log($"[{i}] ({entry.key1}, {entry.key2})={entry.value} h({entry.hashCode1}, {entry.hashCode2}) n({entry.next1} {entry.next2})");
            }

            for (int i = 0; i < buckets1.Length; i++)
            {
                Debug.Log($"start {i} {buckets1[i]}");
                int pointer = buckets1[i];

                while (pointer >= 0)
                {
                    Debug.Log($"{entries[pointer].hashCode1},{entries[pointer].hashCode2}: {entries[pointer].value}");
                    pointer = entries[pointer].next1;
                }
            }


            for (int i = 0; i < buckets2.Length; i++)
            {
                Debug.Log($"start {i} {buckets2[i]}");
                int pointer = buckets2[i];

                while (pointer >= 0)
                {
                    Debug.Log($"{entries[pointer].value}");
                    pointer = entries[pointer].next2;
                }
            }
        }

        public IEnumerator<KeyValuePair> GetEnumerator()
        {
            return new Enumerator(this);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return new Enumerator(this);
        }       

        public PartialEnumerator PartialEnumeratorKey1(TK1 key1)
        {
            return new PartialEnumerator(this, 
                ()=>{                    
                    int hashCode1 = comparer1.GetHashCode(key1) & 0x7FFFFFFF;
                    int targetBucket1 = hashCode1 % buckets1.Length;
                    return buckets1[targetBucket1];
                },
                (index)=>{
                    return entries[index].next1;
                } 
            );
        }       

        public PartialEnumerator PartialEnumeratorKey2(TK2 key2)
        {
            return new PartialEnumerator(this, 
                ()=>{                    
                    int hashCode2 = comparer2.GetHashCode(key2) & 0x7FFFFFFF;
                    int targetBucket2 = hashCode2 % buckets2.Length;
                    return buckets2[targetBucket2];
                },
                (index)=>{
                    return entries[index].next2;
                } 
            );
        }

        public struct Enumerator : IEnumerator<KeyValuePair>
        {
            private DualKeyDictionary<TK1, TK2, TV> dictionary;
            private int version;
            private int index;
            private KeyValuePair current;

            internal Enumerator(DualKeyDictionary<TK1, TK2, TV> dictionary)
            {
                this.dictionary = dictionary;
                version = dictionary.version;
                index = 0;
                current = new KeyValuePair();
            }
            public KeyValuePair Current => current;

            object IEnumerator.Current
            {
                get { 
                    if(index == 0 || (index == dictionary.count + 1)) {
                        throw new InvalidOperationException("不能在遍历时修改字典");
                    }
                    return new KeyValuePair()
                    {
                        key1 = current.key1,
                        key2 = current.key2,
                        value = current.value
                    };
                }
            }

            public void Dispose()
            {
            }

            public bool MoveNext()
            {
                if (version != dictionary.version)
                {
                    throw new InvalidOperationException("不能在遍历时修改字典");
                }

                // Use unsigned comparison since we set index to dictionary.count+1 when the enumeration ends.
                // dictionary.count+1 could be negative if dictionary.count is Int32.MaxValue
                while ((uint)index < (uint)dictionary.count)
                {
                    if (dictionary.entries[index].hashCode1 >= 0)
                    {
                        current = new KeyValuePair() 
                        {
                            key1 = dictionary.entries[index].key1,
                            key2 = dictionary.entries[index].key2,
                            value = dictionary.entries[index].value
                        };
                        index++;
                        return true;
                    }
                    index++;
                }

                index = dictionary.count + 1;
                current = new KeyValuePair();
                return false;
            }

            public void Reset()
            {
                if (version != dictionary.version)
                {
                    throw new InvalidOperationException("不能在遍历时修改字典");
                }
                index = 0;
                current = new KeyValuePair();
            }
        }

        
        public struct PartialEnumerator : IEnumerator<KeyValuePair>, IEnumerable<KeyValuePair>
        {
            private DualKeyDictionary<TK1, TK2, TV> dictionary;
            private Func<int> firstEntryFunc;
            private Func<int, int> nextEntryFunc;
            private int version;
            private int entry;
            private KeyValuePair current;

            internal PartialEnumerator(
                DualKeyDictionary<TK1, TK2, TV> dictionary, 
                Func<int> firstEntryFunc,
                Func<int, int> nextEntryFunc
            )
            {
                this.dictionary = dictionary;
                this.firstEntryFunc = firstEntryFunc;
                this.nextEntryFunc = nextEntryFunc;
                version = dictionary.version;
                entry = firstEntryFunc();
                current = new KeyValuePair();
            }
            public KeyValuePair Current => current;

            object IEnumerator.Current
            {
                get { 
                    if(entry < 0) {
                        throw new InvalidOperationException("不能在遍历时修改字典");
                    }
                    return new KeyValuePair()
                    {
                        key1 = current.key1,
                        key2 = current.key2,
                        value = current.value
                    };
                }
            }

            public void Dispose(){}

            public bool MoveNext()
            {
                if (version != dictionary.version)
                {
                    throw new InvalidOperationException("不能在遍历时修改字典");
                }

                // Use unsigned comparison since we set index to dictionary.count+1 when the enumeration ends.
                // dictionary.count+1 could be negative if dictionary.count is Int32.MaxValue
                while ((uint)entry < (uint)dictionary.count)
                {
                    if (dictionary.entries[entry].hashCode1 >= 0)
                    {
                        current = new KeyValuePair() 
                        {
                            key1 = dictionary.entries[entry].key1,
                            key2 = dictionary.entries[entry].key2,
                            value = dictionary.entries[entry].value
                        };
                        entry = nextEntryFunc(entry);
                        return true;
                    }
                    entry = nextEntryFunc(entry);;
                }

                entry = -1;
                current = new KeyValuePair();
                return false;
            }

            public void Reset()
            {
                if (version != dictionary.version)
                {
                    throw new InvalidOperationException("不能在遍历时修改字典");
                }
                entry = firstEntryFunc();
                current = new KeyValuePair();
            }

            public IEnumerator<KeyValuePair> GetEnumerator()
            {
                return this;
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return this;
            }  
        }
    }
}
