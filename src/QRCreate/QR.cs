using QRCreate.QREncoding;
using QRCreate.Utils;

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
/// - Arbitrary: This is not an official encdoding mode from the QR code specification
///      and instead represents a QR code storing arbitrary data passed to the constructor 
///     already in binary form.
/// </summary>
public enum EncodingMode
{
    NUMERIC = 0,
    ALPHA_NUMERIC = 1,
    BYTE = 2,
    KANJI = 3,
    ARBITRARY =  4


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

public partial class QRCode
{
    /// <summary>
    /// The data this QR Code contains encoded with the <see cref="EncodingMode"/> of
    /// this QR Code.
    /// </summary>
    public PackedBitList EncodedData
    {
        get { return _encodedData; }
    }

    /// <summary>
    /// The data this QR Code contains encoded with the <see cref="EncodingMode"/> of
    /// this QR Code.
    /// </summary>
    private readonly PackedBitList _encodedData;

    private readonly PackedBitList _errorCorrectionBytes;

    /// <summary>
    /// The type of QRCode encoding this QR code uses. 
    /// The possible encodings are defined by <see cref="EncodingMode"/>.
    /// </summary> 
    public EncodingMode EncodingMode
    {
        get { return _encodingMode; }
    }

    /// <summary>
    /// The type of QRCode encoding this QR code uses. 
    /// The possible encodings are defined by <see cref="EncodingMode"/>.
    /// </summary> 
    private readonly EncodingMode _encodingMode;

    /// <summary>
    /// Create a QR code which contains the given data.
    /// The encoding mode will be the smallest mode which can encode the given data.
    /// The error correction level will be medium.
    /// </summary>
    /// <param name="dataToEncode">The string that will be encoded into the QR code. 
    /// The characters in this string must be allowed in Numeric, Alphanumeric, Byte, or Kanji Encoding mode.</param>
    public QRCode(string dataToEncode)
    {

        IQREncoder encoder;

        // Select the smallest EncodingMode possible for dataToEncode
        if (NumericQREncoder.ValidateChars(dataToEncode))
        {
            _encodingMode = EncodingMode.NUMERIC;
            encoder = new NumericQREncoder();
        }
        else if (AlphanumericQREncoder.ValidateChars(dataToEncode))
        {
            _encodingMode = EncodingMode.ALPHA_NUMERIC;
            encoder = new AlphanumericQREncoder();
        }
        else if (ByteQREncoder.ValidateChars(dataToEncode))
        {
            _encodingMode = EncodingMode.BYTE;
            encoder = new ByteQREncoder();
        }
        else if (KanjiQREncoder.ValidateChars(dataToEncode))
        {
            _encodingMode = EncodingMode.KANJI;
            encoder = new KanjiQREncoder();
        }
        else
        {
            throw new ArgumentException("dataToEncode can not be encoded into a QR code using any EncodingMode.");
        }

        _encodedData = encoder.Encode(dataToEncode);

        // TODO: Calculate error correction
    }


    /// <summary>
    /// Create a QR code which contains the given data using a specified encoding method.
    /// The error correction level will be medium.
    /// </summary>
    /// <param name="dataToEncode">The string that will be encoded into the QR code. 
    /// The characters in this string must be allowed for the given encoding mode.</param>
    /// <param name="encodingMode">The method of encoding to use for this QR code.</param>
    public QRCode(string dataToEncode, EncodingMode encodingMode) : this(dataToEncode, encodingMode, ErrorCorrectionLevel.MEDIUM) { }

    /// <summary>
    /// Create a QR code which contains the given data using a specified encoding method.
    /// </summary>
    /// <param name="dataToEncode">The string that will be encoded into the QR code. 
    /// The characters in this string must be allowed for the given encoding mode.</param>
    /// <param name="encodingMode">The method of encoding to use for this QR code.</param>
    /// <param name="errorCorrectionLevel">The level of error correction this QR code should have.</param>
    public QRCode(string dataToEncode, EncodingMode encodingMode, ErrorCorrectionLevel errorCorrectionLevel)
    {
        if (encodingMode == EncodingMode.ARBITRARY)
        {
            // TODO: Either warn or throw an error about arbitrary encoding. 
        }
        else
        {
            // TODO: Move out if arbitrary encoding is disallowed

            // Encode the data
            IQREncoder encoder = GetEncoder(_encodingMode);
            _encodedData = encoder.Encode(dataToEncode);
        }

        _encodingMode = encodingMode;



        // TODO: Calculate error correction

    }

    /// <summary>
    /// Create a QRCode which contains arbritrary binary instead of encoded text. <br/>
    /// <b>QRCodes created using this constructor will likely not work with most readers unless arbitaryQRData is already valid QR encoded data.</b> <br/>
    /// This constructor should only be used if you have a specific purpose for encoding arbitrary data or 
    /// the data you are passing is already valid QR encoded.
    /// </summary>
    /// <param name="arbitraryQRData">A List of bytes containing the arbitrary data to put in this QR code.
    /// </b> </param>
    /// <param name="errorCorrectionLevel"></param>
    public QRCode(List<byte> arbitraryQRData, ErrorCorrectionLevel errorCorrectionLevel)
    {
        _encodedData = new PackedBitList(arbitraryQRData);
        // TODO: Calculate error correction
    }

    /// <summary>
    /// Returns a <see cref="QREncoderBase"/> which encodes a string using the given EncodingMode
    /// </summary>
    /// <param name="encodingMode"></param>
    /// <returns></returns>
    private static IQREncoder GetEncoder(EncodingMode encodingMode)
    {
        return encodingMode switch
        {
            EncodingMode.NUMERIC => new NumericQREncoder(),
            EncodingMode.ALPHA_NUMERIC => new AlphanumericQREncoder(),
            EncodingMode.BYTE => new ByteQREncoder(),
            EncodingMode.KANJI => new KanjiQREncoder(),
            _ => throw new ArgumentException("Unimplemented encoding mode provided"),
        };
    }

}
