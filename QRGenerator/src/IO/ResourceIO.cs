namespace QRGenerator.IO;

/// <summary>
/// Class holding methods to read in files included in the resources folder. Handles parsing the files into 
/// specific formats.
/// </summary>
internal static class ResourceIO {
    /// <summary>
    /// Reads in a text file containing a collection of chars and the integer values they encode into <br/>
    /// The format of these files should be <br/>
    /// {Char value} {int value} <br/>
    /// Ex. <br/>
    /// A 10 <br/>
    /// See Resources/QRAlphanumericEncodingTable.txt for an example
    /// </summary>
    /// <returns>A Dictionary with the chars read in as the key and the ints they encode to as the values</returns>
    internal static Dictionary<char, int> ReadInEncodingTable(){
        throw new NotImplementedException();
    }
}