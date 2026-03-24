using System.Collections;

namespace QRCreate.Utils;

/// <summary>
/// Struct which represents a single Bit which can be one or zero.
/// </summary>
public struct Bit : IEquatable<Bit>
{
    /// <summary>
    /// Create a new Bit with the value of zero.
    /// </summary>
    public Bit()
    {
        Value = 0;
    }

    /// <summary>
    /// The value of this Bit. Will only be 0 or 1.
    /// </summary> 
    public byte Value
    {
        get;
        private set;
    }

    /// <summary>
    /// Set this Bit equal to Zero
    /// </summary>
    public void SetToZero()
    {
        Value = 0;
    }

    /// <summary>
    /// Set this Bit equal to One
    /// </summary>
    public void SetToOne()
    {
        Value = 1;
    }

    /// <summary>
    /// Set this Bit based to a given value of either zero one. Will throw an exception if value is
    /// not 1 or 0
    /// </summary>
    /// <param name="value"> 
    /// The value to set this Bit to. Can only be 0 or 1.
    /// </param>
    public void SetTo(int value)
    {
        if (value is not 0 && value is not 1)
        {
            throw new ArgumentException("A Bit can only be set to 0 or 1");
        }

        Value = (byte)value;
    }

    public static bool operator ==(Bit b1, Bit b2)
    {
        return b1.Value == b2.Value;
    }

    public static bool operator !=(Bit b1, Bit b2)
    {
        return b1.Value != b2.Value;
    }

    public override bool Equals(object? obj)
    {
        if (obj is Bit b2)
        {
            return Value == b2.Value;
        }
        
        return false;
    }
    
    public bool Equals(Bit b2)
    {
        return Value == b2.Value;
    }

    public override int GetHashCode()
    {
        return Value.GetHashCode();
    }


    public static Bit ONE
    {
        get
        {
            var b = new Bit();
            b.SetToOne();
            return b;
        }
    }

    public static Bit ZERO
    {
        get
        {
            var b = new Bit();
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

        public Bit Current => _items[_index];

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
        _backing = [0];
        _count = 0;
    }

    /// <summary>
    /// Create a new PackedBitList with the capacity to store n Bits
    /// </summary>
    /// <param name="capacity"></param>
    public PackedBitList(int capacity)
    {
        _backing = new List<byte>(capacity / 8){0};
        _count = 0;
    }

    /// <summary>
    /// Create a new PackedBitList contaiing all Bits in collection
    /// </summary>
    /// <param name="collection"></param>
    public PackedBitList(IEnumerable<Bit> collection)
    {   
        // Initialize backing with one byte as starting storage
        _backing = [0];

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
        // Initialize backing with one byte as starting storage
        _backing = [0];
        // Copy list into backing
        foreach (byte b in byteCollection)
        {
            _backing.Add(b);
            _count += 8;
        }
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
        _count = 0;
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