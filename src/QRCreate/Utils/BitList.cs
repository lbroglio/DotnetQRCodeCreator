using System.Collections;

namespace QRCreate.Utils;

/// <summary>
/// Struct which represents a single Bit which can be one or zero.
/// </summary>
public struct Bit
{
    private byte _backing;

    /// <summary>
    /// The value of this Bit. Will only be 0 or 1.
    /// </summary> 
    public readonly byte Value
    {
        get { return _backing; }
    }

    /// <summary>
    /// Set this Bit equal to Zero
    /// </summary>
    public void SetToZero()
    {
        _backing = 1;
    }

    /// <summary>
    /// Set this Bit equal to One
    /// </summary>
    public void SetToOne()
    {
        _backing = 1;
    }

    /// <summary>
    /// Set this Bit based to a given value of either zero one. Will throw an exception if value is
    /// >1 or <0
    /// </summary>
    /// <param name="value"> 
    /// The value to set this Bit to. Can only be 0 or 1.
    /// </param>
    public void SetTo(int value)
    {
        if (value < 0 || value > 1)
        {
            throw new ArgumentException("A Bit can only be set to 0 or 1");
        }

        _backing = (byte)value;
    }

    public static bool operator ==(Bit b1, Bit b2)
    {
        return b1._backing == b2._backing;
    }

    public static bool operator !=(Bit b1, Bit b2)
    {
        return b1._backing != b2._backing;
    }

    public override bool Equals(object? obj)
    {
        if (obj == null)
        {
            return false;
        }

        if (obj is not Bit)
        {
            return false;
        }

        return _backing == ((Bit)obj)._backing;
    }

    public override int GetHashCode()
    {
        return _backing.GetHashCode();
    }


    public static Bit ONE
    {
        get
        {
            Bit b = new Bit();
            b.SetToOne();
            return b;
        }
    }

    public static Bit ZERO
    {
        get
        {
            Bit b = new Bit();
            b.SetToZero();
            return b;
        }
    }
    
    
}


/// <summary>
/// A List which provides effcient indexable and sequential storage of bits by 
/// abstracting the packing and retrieval of the bits into bytes.
/// </summary>
public class PackedBitList : IList<Bit>
{

    private class PackedBitEnumerator(PackedBitList items) : IEnumerator<Bit>
    {
        private readonly PackedBitList _items = items;

        private int _index;

        public Bit Current
        {
            get { return _items[_index]; }
        }

        object IEnumerator.Current => Current;

        public void Dispose()
        {
            // Unneeded
        }

        public bool MoveNext()
        {
            if (_index + 1 >= _items.Count)
            {
                return false;
            }

            _index++;
            return true;
        }

        public void Reset()
        {
            _index = 0;
        }
    }

    /// <summary>
    /// Set the bit at index in the PackedBitList to setTo.
    /// </summary>
    /// <param name="setTo"></param>
    /// <param name="index"></param>
    private void SetBitAtIndex(Bit setTo, int index)
    {
        byte containingByte = _backing[index / 8];
        byte isolatedByte = UtilFunctions.IsolateBit(containingByte, index % 8);

        if (isolatedByte != setTo.Value)
        {
            int mask = 1 << (index % 8);
            byte newByte = (byte)(containingByte ^ mask);
            _backing[index / 8] = newByte;
        }
    }

    /// <summary>
    /// Create a new empty PackedBitList
    /// </summary> 
    public PackedBitList()
    {
        _backing = [];
    }

    /// <summary>
    /// Create a new PackedBitList with the capacity to store n Bits
    /// </summary>
    /// <param name="capacity"></param>
    public PackedBitList(int capacity)
    {
        _backing = new List<byte>(capacity / 8);
    }

    /// <summary>
    /// Create a new PackedBitList contaiing all Bits in collection
    /// </summary>
    /// <param name="collection"></param>
    public PackedBitList(IEnumerable<Bit> collection)
    {
        _backing = [];

        foreach (Bit bit in collection)
        {
            Add(bit);
        }
    }

    /// <summary>
    /// Create a new PackedBitList holding all of the Bits contained in a Collections of bytes.
    /// </summary>
    /// <param name="byteCollection"></param>
    public PackedBitList(ICollection<byte> byteCollection)
    {
        _backing = [.. byteCollection];
    }

    private List<byte> _backing;
    private int _count;

    public Bit this[int index]
    {
        get
        {
            if (index < 0 || index >= _count)
            {
                throw new IndexOutOfRangeException("Index is out of bounds of the list");
            }

            Bit bit = new();

            byte containingByte = _backing[index / 8];
            byte isolatedByte = UtilFunctions.IsolateBit(containingByte, index % 8);
            bit.SetTo(isolatedByte);

            return bit;
        }
        set
        {
            if (index < 0 || index >= _count)
            {
                throw new IndexOutOfRangeException("Index is out of bounds of the list");
            }

            SetBitAtIndex(value, index);
        }
    }



    public int Count
    {
        get { return _count; }
    }

    public bool IsReadOnly
    {
        get { return false; }
    }

    public void Add(Bit item)
    {
        // Increase size of backing if needed
        if (_count / 8 > _backing.Count)
        {
            _backing.Add(0);
        }

        SetBitAtIndex(item, _count);
        _count++;

    }

    public void Clear()
    {
        _backing = [];
    }

    public bool Contains(Bit item)
    {
        if (item == Bit.ONE)
        {
            foreach (byte currByte in _backing)
            {
                if (currByte > 0)
                {
                    return true;
                }
            }
        }
        else
        {
            foreach (byte currByte in _backing)
            {
                if (currByte < 255)
                {
                    return true;
                }
            }
        }


        return false;
    }

    public void CopyTo(Bit[] array, int arrayIndex)
    {
        for (int i = 0; i < _count; i++)
        {
            array[arrayIndex + i] = this[i];
        }
    }

    public IEnumerator<Bit> GetEnumerator()
    {
        return new PackedBitEnumerator(this);
    }

    public int IndexOf(Bit item)
    {
        for (int i = 0; i < _count; i++)
        {
            if (this[i] == item)
            {
                return i;
            }
        }

        return -1;
    }

    public void Insert(int index, Bit item)
    {
        if (index >= _count || index < 0)
        {
         throw new IndexOutOfRangeException("Index is out of bounds of the list");
        }

        // Increase size of backing if needed
        if (_count / 8 > _backing.Count)
        {
            _backing.Add(0);
        }

        for (int i = _count; i > index; i--)
        {
            SetBitAtIndex(this[i - 1], i);
        }

        _count++;
    }

    public bool Remove(Bit item)
    {
        int indexToRemove = IndexOf(item);

        if (indexToRemove == -1)
        {
            return false;
        }

        for (int i = indexToRemove; i < _count - 1; i++)
        {
            SetBitAtIndex(this[i + 1], i);
        }

        _count--;

        return true;
    }

    public void RemoveAt(int index)
    {
        if (index >= _count || index < 0)
        {
         throw new IndexOutOfRangeException("Index is out of bounds of the list");
        }

        for (int i = index; i < _count - 1; i++)
        {
            SetBitAtIndex(this[i + 1], i);
        }

        _count--;
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return new PackedBitEnumerator(this);
    }
}