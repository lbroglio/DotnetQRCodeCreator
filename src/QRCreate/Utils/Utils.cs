namespace QRCreate.Utils;

internal static class UtilFunctions
{
    /// <summary>
    /// Returns the Bit at the bitPosition in containingValue.
    /// </summary>
    /// <param name="containingValue"></param>
    /// <param name="bitPosition"></param>
    /// <returns></returns>
    internal static byte IsolateBit(int containingValue, int bitPosition)
    {
        int shiftedValue = containingValue >> bitPosition;
        return (byte)(shiftedValue & 1);
    }
}