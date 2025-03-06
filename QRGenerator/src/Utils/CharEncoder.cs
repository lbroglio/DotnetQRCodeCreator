namespace QRGenerator.Utils;

/// <summary>
///  Interface of an object which encodes a character into an array of bits depending on the 
/// encoding mode used for the QR code <br/>
/// The possible encoding modes are <br/>
/// - Numeric: Only 0-9 are allowed. <br/>
/// - Alphanumeric: 0–9, A–Z (upper-case only), space, $, %, *, +, -, ., /, : are allowed. <br/>
/// - Byte: Any Latin 1 block Unicode character is allowed. (For more information see 
///       https://en.wikipedia.org/wiki/ISO/IEC_8859-1) <br/>
/// - Kanji: Japanses Kanji characters as defined by https://en.wikipedia.org/wiki/JIS_X_0208
/// </summary>
internal interface IQREncoder{
    /// <summary>
    /// Encode a string using the encoding mode specific to this encoder.
    /// </summary>
    /// <param name="toEncode">The string of characters to encode. Should be limited to the 
    /// allowed characters for the mode of this encoder. </param>
    /// <returns>The encoded string as an array of one byte unsigned ints. The value of each byte in the array is the value of that bit 
    /// in the sequence. <br/>
    ///Ex. if the first bit in the encoded string is a 1 the first byte in the array will equal 1.
    /// If the second equals zero the second byte in the array will = 0 (and so on).
    /// </returns> 
    byte[] Encode(string toEncode);
}

internal abstract class QREncoderBase : IQREncoder
{
    /// <summary>
    /// Set containing the characters allowed to be used for the mode this Encoder encodes strings into.
    /// </summary> 
    protected abstract HashSet<char> ALLOWED_CHARS{ get; }

    public abstract byte[] Encode(string toEncode);

    /// <summary>
    /// Validate that the given string only contains characters in the set ALLOWED_CHARS.
    /// </summary>
    /// <param name="toValidate">A string containing characters</param>
    /// <returns>
    /// - True: If all characters in toValidate are in ALLOWED_CHARS <br/>
    /// - False: If any characters in toValidate are not in  ALLOWED_CHARS
    ///  </returns>
    protected bool ValidateChars(string toValidate){
        foreach(char c in toValidate){
            if(!ALLOWED_CHARS.Contains(c)){
                return false;
            }
        }

        return true;
    }
   
}

/// <summary>
/// Encodes characters for a QR code in numeric coding mode. <br/>
/// Allowed characters are 0-9.
/// </summary> 
internal class NumericQREncoder : QREncoderBase
{
    protected override HashSet<char> ALLOWED_CHARS  {
        get { return ['0', '1', '2', '3', '4', '5', '6', '7', '8', '9'];}
    }

    public override byte[] Encode(string toEncode)
    {
        // Check that toEncode is legal for the mode of this encoder
        if(!ValidateChars(toEncode)){
            throw new ArgumentException("toEncode contains characters not allowed in numeric encoding");
        }

        throw new NotImplementedException();
    }
}