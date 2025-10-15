using QRCreate.Utils;

namespace QRCreateUnitTest;

public class PackedBitListTests
{
    [SetUp]
    public void Setup()
    {
    }

    [Test]
    public void TestBitSetToOne()
    {
        Bit b = new();
        b.SetToOne();

        Assert.That(b.Value, Is.EqualTo(1));

    }

    [Test]
    public void TestBitSetToZero()
    {
        Bit b = new();
        b.SetToZero();

        Assert.That(b.Value, Is.EqualTo(0));

    }

    [Test]
    public void TestNewBitStartsZero()
    {
        Bit b = new();

        Assert.That(b.Value, Is.EqualTo(0));
    }

    [Test]
    public void TestBitSetToValueOne()
    {
        Bit b = new();
        b.SetTo(1);

        Assert.That(b.Value, Is.EqualTo(1));
    }

    [Test]
    public void TestBitSetToValueZero()
    {
        Bit b = new();
        b.SetToZero();
        b.SetTo(0);

        Assert.That(b.Value, Is.EqualTo(0));
    }

    [Test]
    public void TestBitSetToValueUnderZeroThrows()
    {
        Bit b = new();

        Assert.Throws<ArgumentException>(() => b.SetTo(-1));
    }

    [Test]
    public void TestBitSetToValueOverOneThrows()
    {
        Bit b = new();

        Assert.Throws<ArgumentException>(() => b.SetTo(2));
    }

    [Test]
    public void TestBitEqualsOperator()
    {
        Bit b1 = new();
        b1.SetToOne();
        Bit b2 = new();
        b2.SetToOne();

        Assert.That(b1 == b2, Is.True);
    }

    [Test]
    public void TestBitEqualsOperatorNotEqual()
    {
        Bit b1 = new();
        b1.SetToOne();
        Bit b2 = new();
        b2.SetToZero();

        Assert.That(b1 == b2, Is.False);
    }

    [Test]
    public void TestBitNotEqualsOperator()
    {
        Bit b1 = new();
        b1.SetToOne();
        Bit b2 = new();
        b2.SetToZero();

        Assert.That(b1 != b2, Is.True);

    }

    [Test]
    public void TestBitNotEqualsOperatorEqual()
    {
        Bit b1 = new();
        b1.SetToOne();
        Bit b2 = new();
        b2.SetToOne();

        Assert.That(b1 != b2, Is.False);

    }


    [Test]
    public void TestBitEquals()
    {
        Bit b1 = new();
        b1.SetToOne();
        Bit b2 = new();
        b2.SetToOne();

        Assert.That(b1.Equals(b2), Is.True);

    }

    [Test]
    public void TestBitEqualsNotEquls()
    {
        Bit b1 = new();
        b1.SetToOne();
        Bit b2 = new();
        b2.SetToZero();

        Assert.That(b1.Equals(b2), Is.False);

    }

    [Test]
    public void TestBitHashcodeMatchesBackingHash()
    {
        Bit b = new();
        b.SetToOne();

        Assert.That(b.GetHashCode(), Is.EqualTo(1.GetHashCode()));
    }

    [Test]
    public void TestBitHashcodeEquivalentForEqual()
    {
        Bit b1 = new();
        b1.SetToZero();
        Bit b2 = new();
        b2.SetToZero(); ;

        Assert.That(b1.GetHashCode, Is.EqualTo(b2.GetHashCode()));
    }

    [Test]
    public void TestBitStaticFactoryOne()
    {
        Bit b = Bit.ONE;

        Assert.That(b.Value, Is.EqualTo(1));
    }

    [Test]
    public void TestBitStaticFactoryZero()
    {
        Bit b = Bit.ZERO;

        Assert.That(b.Value, Is.EqualTo(0));
    }
    


    
}