using QRCreate.QREncoding;

namespace QRCreate;

/// <summary>
/// The different types of QR Code encodings. 
/// The different modes have different allowed character sets and a different number of bits 
/// to encode each character. <br/>
/// The possible encoding modes are <br/>
/// - Numeric: Only 0-9 are allowed. <br/>
/// - Alphanumeric: 0–9, A–Z (upper-case only), space, $, %, *, +, -, ., /, : are allowed. <br/>
/// - Byte: Any Latin 1 block Unicode character is allowed. (For more information see 
///       https://en.wikipedia.org/wiki/ISO/IEC_8859-1) <br/>
/// - Kanji: A character set with latin letters and Japanses Kanji characters as defined by JIS_X_0208. (For more information see 
///      https://en.wikipedia.org/wiki/JIS_X_0208)
/// </summary>
public enum EncodingMode
{
    NUMERIC = 0,
    ALPHA_NUMERIC = 1,
    BYTE = 2,
    KANJI = 3


}


/// <summary>
/// The level of error correction a QR code should have. A higher level will require a 
/// larger QR code to store the data. <br/>
/// The possible levels are: <br/>
/// - Level Low: Can restore 7% of data bytes <br/>
/// - Level Medium: Can restore 15% of data bytes <br/>
/// - Level Quartile: Can restore 25% of data bytes <br/>
/// - Level High: Can restore 30% of data bytes 
/// </summary>
public enum ErrorCorrectionLevel
{
    LOW = 0,
    MEDIUM = 1,
    QUARTILE = 2,
    HIGH = 3

}


public class QRCode {

    /// <summary>
    /// Create a QR code which contains the given data.
    /// The encoding mode will be the smallest mode which can encode the given data.
    /// The error correction level will be medium.
    /// </summary>
    /// <param name="dataToEncode">The string that will be encoded into the QR code. 
    /// The characters in this string must be allowed for the given encoding mode.</param>
    public QRCode(string dataToEncode){
        
    }


    /// <summary>
    /// Create a QR code which contains the given data using a specified encoding method.
    /// The error correction level will be medium.
    /// </summary>
    /// <param name="dataToEncode">The string that will be encoded into the QR code. 
    /// The characters in this string must be allowed for the given encoding mode.</param>
    /// <param name="encodingMode">The method of encoding to use for this QR code.</param>
    public QRCode(string dataToEncode, EncodingMode encodingMode) : this(dataToEncode, encodingMode, ErrorCorrectionLevel.MEDIUM){}

    /// <summary>
    /// Create a QR code which contains the given data using a specified encoding method.
    /// </summary>
    /// <param name="dataToEncode">The string that will be encoded into the QR code. 
    /// The characters in this string must be allowed for the given encoding mode.</param>
    /// <param name="encodingMode">The method of encoding to use for this QR code.</param>
    /// <param name="errorCorrectionLevel">The level of error correction this QR code should have.</param>
    public QRCode(string dataToEncode, EncodingMode encodingMode, ErrorCorrectionLevel errorCorrectionLevel){
        // Encode the data
        QREncoderBase encoder = encodingMode switch
        {
            EncodingMode.NUMERIC => new NumericQREncoder(),
            EncodingMode.ALPHA_NUMERIC => new AlphanumericQREncoder(),
            EncodingMode.BYTE => new ByteQREncoder(),
            EncodingMode.KANJI => new KanjiQREncoder(),
            _ => throw new ArgumentException("Unimplemented encoding mode provided"),
        };
        
        byte[] encoded = encoder.Encode(dataToEncode);
        
    } 

}
