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
    


    
}