using QRCreate.Utils;

namespace QRCreateUnitTest.BitlistTests;

public class PackedBitListTests
{
    [SetUp]
    public void Setup()
    {
    }

    [Test]
    public void TestCreateBitlistFromEnumerable()
    {
        IEnumerable<Bit> bits = new List<Bit>()
        {
            Bit.ONE,
            Bit.ZERO,
            Bit.ONE
        };

        PackedBitList bl = new PackedBitList(bits);

        Assert.Multiple(() =>
        {
            Assert.That(bl, Has.Count.EqualTo(3));
            Assert.That(bl[0].Value, Is.EqualTo(1));
            Assert.That(bl[1].Value, Is.EqualTo(0));
            Assert.That(bl[2].Value, Is.EqualTo(1));
        });
    }

    [Test]
    public void TestCreateBitlistFromCollection()
    {
        ICollection<Bit> bits = new List<Bit>()
        {
            Bit.ONE,
            Bit.ZERO,
            Bit.ONE
        };

        PackedBitList bl = new PackedBitList(bits);

        Assert.Multiple(() =>
        {
            Assert.That(bl, Has.Count.EqualTo(3));
            Assert.That(bl[0].Value, Is.EqualTo(1));
            Assert.That(bl[1].Value, Is.EqualTo(0));
            Assert.That(bl[2].Value, Is.EqualTo(1));
        });
    }

    [Test]
    public void TestBitlistGetOperator()
    {
        PackedBitList bl = new PackedBitList();

        bl.Add(Bit.ONE);
        bl.Add(Bit.ZERO);

        Assert.Multiple(() =>
        {
            Assert.That(bl[0].Value, Is.EqualTo(1));
            Assert.That(bl[1].Value, Is.EqualTo(0));
        });
    }

    [Test]
    public void TestBitlistSetOperator()
    {
        PackedBitList bl = new PackedBitList();

        bl.Add(Bit.ONE);
        bl.Add(Bit.ONE);

        bl[0] = Bit.ZERO;
        bl[1] = Bit.ONE;

        Assert.Multiple(() =>
        {
            Assert.That(bl[0].Value, Is.EqualTo(0));
            Assert.That(bl[1].Value, Is.EqualTo(1));
        });
    }

    [Test]
    public void TestBitListAdd()
    {
        List<Bit> bits = new List<Bit>()
        {
            Bit.ONE,
            Bit.ZERO,
        };

        PackedBitList bl = new PackedBitList(bits);

        bl.Add(Bit.ONE);

        Assert.Multiple(() =>
        {
            Assert.That(bl, Has.Count.EqualTo(3));
            Assert.That(bl[2], Is.EqualTo(Bit.ONE));
            // Test other bits are unchanged
            Assert.That(bl[0], Is.EqualTo(Bit.ONE));
            Assert.That(bl[1], Is.EqualTo(Bit.ZERO));
        });

    }

    [Test]
    public void TestBitListAddMultiple()
    {
        List<Bit> bits = new List<Bit>()
        {
            Bit.ONE,
            Bit.ZERO,
        };

        PackedBitList bl = new PackedBitList(bits);

        bl.Add(Bit.ONE);
        bl.Add(Bit.ZERO);

        Assert.Multiple(() =>
        {
            Assert.That(bl, Has.Count.EqualTo(4));
            Assert.That(bl[2], Is.EqualTo(Bit.ONE));
            Assert.That(bl[3], Is.EqualTo(Bit.ZERO));
            // Test other bits are unchanged
            Assert.That(bl[0], Is.EqualTo(Bit.ONE));
            Assert.That(bl[1], Is.EqualTo(Bit.ZERO));
        });

    }

    [Test]
    public void TestBitListClear()
    {
        List<Bit> bits = new List<Bit>()
        {
            Bit.ONE,
            Bit.ZERO,
        };

        PackedBitList bl = new PackedBitList(bits);

        bl.Clear();

        Assert.That(bl, Has.Count.EqualTo(0));
    }
    





    
}