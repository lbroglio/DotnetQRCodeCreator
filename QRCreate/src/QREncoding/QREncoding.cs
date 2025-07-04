using System.Diagnostics;
using System.Text;
using QRCreate.IO;

namespace QRCreate.QREncoding;


/// <summary>
///  Interface of an object which encodes a character string into an array of bits depending on the 
/// encoding mode used for the QR code <br/>
/// The possible encoding modes are <br/>
/// - Numeric: Only 0-9 are allowed. <br/>
/// - Alphanumeric: 0–9, A–Z (upper-case only), space, $, %, *, +, -, ., /, : are allowed. <br/>
/// - Byte: Any Latin 1 block Unicode character is allowed. (For more information see 
///       https://en.wikipedia.org/wiki/ISO/IEC_8859-1) <br/>
/// - Kanji: A character set with latin letters and Japanses Kanji characters as defined by JIS_X_0208. (For more information see 
///      https://en.wikipedia.org/wiki/JIS_X_0208)
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
/// Encodes characters for a QR code in numeric encoding mode. <br/>
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

        // Encode string
        List<byte> encoded = new List<byte>();
        for(int i = 0; i < toEncode.Length; i+=3){
            // If only one number is left
            if(toEncode.Length <= i+1){
                // Convert to an int
                string num = toEncode.Substring(i, 1);
                short threeDigitNumInt = short.Parse(num);

                // Add each bit in the first four bits (starting at the MSB) to the array as its own byte type
                for(int j = 3; j >= 0; j-=1){
                    short shiftedNum = (short)(threeDigitNumInt >> j);
                    byte MSB = (byte)(shiftedNum & 1);
                    encoded.Add(MSB);
                }

                
            }
            // If two numbers are left
            else if(toEncode.Length <= i + 2){
                // Convert to an int
                string num = toEncode.Substring(i, 2);
                short threeDigitNumInt = short.Parse(num);

                // Add each bit in the first eight bits (starting at the MSB) to the array as its own byte type
                for(int j = 7; j >= 0; j-=1){
                    short shiftedNum = (short)(threeDigitNumInt >> j);
                    byte MSB = (byte)(shiftedNum & 1);
                    encoded.Add(MSB);
                }
            }
            // If three+ numbers are left
            else{
                string threeDigitNum = toEncode.Substring(i, 3);
                short threeDigitNumInt = short.Parse(threeDigitNum);

                // Add each bit in the first ten bits (starting at the MSB) to the array as its own byte type
                for(int j = 9; j >= 0; j-=1){
                    short shiftedNum = (short)(threeDigitNumInt >> j);
                    byte MSB = (byte)(shiftedNum & 1);
                    encoded.Add(MSB);
                }
            }

        } 

       return encoded.ToArray();
    }
}

/// <summary>
/// Encodes characters for a QR code in alphanumeric encoding mode. <br/>
/// Allowed characters are 0-9, A-Z (uppercase only), ' ' (space), $, %, *, +, -, ., /, :
/// </summary> 
internal class AlphanumericQREncoder : QREncoderBase
{

    /// <summary>
    /// Location of the embedded resource with the table which maps letters to their encodings for this type of QR code.
    /// </summary> 
    private static readonly string ENCODING_TABLE_LOCATION = "QRGenerator.Resources.Tables.QRAlphanumericEncodingTable.txt";

    private static Dictionary<char, int>? _encodingTable = null;
    /// <summary>
    /// Dictionary where the keys are the allowed chars for this QR code type and the values are their numeric encodings.
    /// </summary> 
    private static Dictionary<char, int> EncodingTable
    {
        get {
            _encodingTable ??= ResourceIO.ReadInEncodingTable(ENCODING_TABLE_LOCATION);
            return _encodingTable;
        }
    }

    protected override HashSet<char> ALLOWED_CHARS {
        get  { 
            return [
                '0', '1', '2', '3', '4', '5', '6', '7', '8', '9',
                'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M', 'N', 'O', 'P', 'Q', 'R', 'S', 'T', 'U', 'V', 'W', 'X', 'Y', 'Z',
                ' ', '$', '%', '*', '+', '-', '.', '/', ':'
            ];
            
        }
    }

    public override byte[] Encode(string toEncode)
    {
        // Check that toEncode is legal for the mode of this encoder
        if(!ValidateChars(toEncode)){
            throw new ArgumentException("toEncode contains characters not allowed in alphanumeric encoding");
        }

        List<byte> encoded = new List<byte>();
        for(int i = 0; i < toEncode.Length; i+=2){
            // If there is only one character left
            if(i + 1 >= toEncode.Length){
                short encodedNum = (short) EncodingTable[toEncode[i]];

                // Add each bit in the first eleven bits (starting at the MSB) to the array as its own byte type
                for(int j = 5; j >= 0; j-=1){
                    short shiftedNum = (short)(encodedNum >> j);
                    byte MSB = (byte)(shiftedNum & 1);
                    encoded.Add(MSB);
                }

            }
            // If there are two+ characters left
            else{

                // Encode each character 
                int encode1 = EncodingTable[toEncode[i]];
                int encode2 = EncodingTable[toEncode[i + 1]];

                // Combine the two and add to byte array
                short encodedNum = (short) ((encode1 * 45) + encode2);

                // Add each bit in the first eleven bits (starting at the MSB) to the array as its own byte type
                for(int j = 10; j >= 0; j-=1){
                    short shiftedNum = (short)(encodedNum >> j);
                    byte MSB = (byte)(shiftedNum & 1);
                    encoded.Add(MSB);
                }
            }
        }

        return encoded.ToArray();


    }
    
}

/// <summary>
/// Encodes characters for a QR code in byte encoding mode. <br/>
/// Allowed characters are any characters in the ISO8859 character set.
/// </summary> 
internal class ByteQREncoder : QREncoderBase
{

    private static HashSet<char>? _charSet = null;
    protected override HashSet<char> ALLOWED_CHARS
    {
        get
        {
            _charSet ??= ResourceIO.ReadInCharset("QRGenerator.Resources.Charsets.ISO8859-1.txt");
            return _charSet;
        }
    }

    public override byte[] Encode(string toEncode)
    {
        // Check that toEncode is legal for the mode of this encoder
        if (!ValidateChars(toEncode))
        {
            throw new ArgumentException("toEncode contains characters not allowed in byte encoding. Allowed Chars are specified in ISO8859-1");
        }

        //Encode String
        List<byte> encoded = new List<byte>();
        byte[] stringLatin1 = Encoding.Latin1.GetBytes(toEncode);
        // Load every bit into the array
        foreach (byte b in stringLatin1)
        {
            for (int i = 7; i >= 0; i -= 1)
            {
                byte shifted = (byte)(b >> i);
                byte MSB = (byte)(shifted & 1);
                encoded.Add(MSB);
            }
        }

        return encoded.ToArray();
    }
}

/// <summary>
/// Encodes characters for a QR code in kanji encoding mode. <br/>
/// Allowed characters are any characters in the JIS-X-0208 character set.
/// </summary> 
internal class KanjiQREncoder : QREncoderBase
{
    private static HashSet<char>? _charSet = null;

    protected override HashSet<char> ALLOWED_CHARS
    {
        get
        {
            _charSet ??= ResourceIO.ReadInCharset("QRGenerator.Resources.Charsets.JIS-X-0208.txt");
            return _charSet;
        }
    }

    public override byte[] Encode(string toEncode)
    {
        // Register a provider so we can access the legacy shift_jis encoding method 
        Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

        // Check that toEncode is legal for the mode of this encoder
        if (!ValidateChars(toEncode))
        {
            throw new ArgumentException("toEncode contains characters not allowed in kanji encoding. Allowed Chars are all two byte characters in Shift JIS-x0208.");
        }

        // Encode string
        List<byte> encoded = new List<byte>();

        // Convert to encode to Shift JIS-X0208 bytes
        Encoding enc = Encoding.GetEncoding("shift_jis");
        byte[] shiftJisBytes = enc.GetBytes(toEncode);

        // Encode each character (characters are two bytes) 
        for (int i = 0; i < shiftJisBytes.Length; i += 2)
        {
            // Combine into a single value
            byte[] thisCharBytes = new byte[2];
            Array.Copy(shiftJisBytes, i, thisCharBytes, 0, 2);
            // Since shift_jis is treated as big endian we may need to reverse this array when getting int value
            if (BitConverter.IsLittleEndian)
            {
                Array.Reverse(shiftJisBytes);
            }
            ushort charIntRep = BitConverter.ToUInt16(shiftJisBytes, 0);

            int qrEncodedVal;
            // If this character is between 0x8140 and 0x9FFC encode using first method 
            if (charIntRep >= 0x8140 && charIntRep <= 0x9FFC)
            {
                // Encoded for the QR code as (MSB * 0xC0) + LSB
                qrEncodedVal = (shiftJisBytes[i] * 0xC0) + shiftJisBytes[i + 1];
            }
            // If this between 0xE040 and 0xEBBF (all character which reach this point are in this range)
            // enode using second method
            else
            {
                ushort interVal = (ushort)(charIntRep - 0xC140);

                // Split into MSB and LSB
                ushort ivMSB = (ushort)(interVal >> 8);
                ushort ivLSB = (ushort)(interVal & 0x00FF);

                // Encoded for the QR code as (MSB * 0xC0) + LSB
                qrEncodedVal = (ivMSB * 0xC0) + ivLSB;
            }

            // Write first 13 bits into string starting at the HSB
            for (int j = 12; j >= 0; j -= 1)
            {
                short shiftedNum = (short)(qrEncodedVal >> j);
                byte MSB = (byte)(shiftedNum & 1);
                encoded.Add(MSB);
            }
        }

        return encoded.ToArray();

    }
}