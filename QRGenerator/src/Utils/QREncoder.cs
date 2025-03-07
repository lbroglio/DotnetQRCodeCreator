using System.Diagnostics;

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

                // Add each bit in the first four bits (starting at the LSB) to the array as its own byte type
                for(int j = 0; j < 4; j++){
                    short shiftedNum = (short)(threeDigitNumInt >> j);
                    byte LSB = (byte)(shiftedNum & 1);
                    encoded.Add(LSB);
                }

                
            }
            // If two numbers are left
            else if(toEncode.Length <= i + 2){
                // Convert to an int
                string num = toEncode.Substring(i, 2);
                short threeDigitNumInt = short.Parse(num);

                // Add each bit in the first four bits (starting at the LSB) to the array as its own byte type
                for(int j = 0; j < 8; j++){
                    short shiftedNum = (short)(threeDigitNumInt >> j);
                    byte LSB = (byte)(shiftedNum & 1);
                    encoded.Add(LSB);
                }
            }
            // If three+ numbers are left
            else{
                string threeDigitNum = toEncode.Substring(i, 3);
                Console.WriteLine(threeDigitNum);
                short threeDigitNumInt = short.Parse(threeDigitNum);

                // Add each bit in the first ten bits (starting at the LSB) to the array as its own byte type
                for(int j =0; j < 10; j++){
                    short shiftedNum = (short)(threeDigitNumInt >> j);
                    byte LSB = (byte)(shiftedNum & 1);
                    encoded.Add(LSB);
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
    private static Dictionary<char, int> ALPANUMERIC_TABLE {
        get {
            return new Dictionary<char, int>{
                {'0', 0},
                {'1', 1},
                {'2', 2},
                {'3', 3},
                {'4', 4},
                {'5', 5},
                {'6', 6},
                {'7', 7},
                {'8', 8},
                {'9', 9},
                {'A', 10},
                {'B', 11},
                {'C', 12},
                {'D', 13},
                {'E', 14},
                {'F', 15},
                {'G', 16},
                {'H', 17},
                {'I', 18},
                {'J', 19},
                {'K', 20},
                {'L', 21},
                {'M', 22},
                {'N', 23},
                {'O', 24},
                {'P', 25},
                {'Q', 26},
                {'R', 27},
                {'S', 28},
                {'T', 29},
                {'U', 30},
                {'V', 31},
                {'W', 32},
                {'X', 33},
                {'Y', 34},
                {'Z', 35},
                {' ', 36},
                {'$', 37},
                {'%', 38},
                {'*', 39},
                {'+', 40},
                {'-', 41},
                {'.', 42},
                {'/', 43},
                {':', 44}
            };
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

        for(int i = 0; i < toEncode.Length; i++){
            // If there is only one character left
            if(i + 1 >= toEncode.Length){

            }
            // If there are two+ characters left
            else{
                string twoCharString = toEncode.Substring(i, 2);

            }
        }


    }
}