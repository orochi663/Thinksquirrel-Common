// CSV Reader
// CsvReader.cs
// Thinksquirrel Software Common Libraries
//  
// Authors:
//       Josh Montoute <josh@thinksquirrel.com>
// 
// Copyright (c) 2011-2012, Thinksquirrel Software, LLC
// All rights reserved.
//
// Redistribution and use in source and binary forms, with or without modification, 
// are permitted provided that the following conditions are met:
//
// Redistributions of source code must retain the above copyright notice,
// this list of conditions and the following disclaimer.
//
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND 
// ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES 
// OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT 
// SHALL THE COPYRIGHT HOLDER OR CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, 
// SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT 
// OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) 
// HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, 
// OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, 
// EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
//
// This file is available at https://github.com/Thinksquirrel-Software/Thinksquirrel-Common
//
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;

/// <summary>
/// Text serialization classes.
/// </summary>
namespace ThinksquirrelSoftware.Common.Serialization.Text
{
  /// <summary>
  /// A data-reader style interface for reading Csv (and otherwise-char-separated) files.
  /// </summary>
  /// <remarks>
  /// Original CsvReader code (c) Jouni Heikniemi. <http://www.heikniemi.fi/jhlib/>
  /// </remarks>
  public class CsvReader : IDisposable {

    #region Private variables

    private Stream stream;
    private StreamReader reader;
    private char separator;

    #endregion

    #region Constructors

    /// <summary>
    /// Creates a new Csv reader for the given stream.
    /// </summary>
    /// <param name="s">The stream to read the CSV from.</param>
    public CsvReader(Stream s) : this(s, null, ',') { }

    /// <summary>
    /// Creates a new reader for the given stream and separator.
    /// </summary>
    /// <param name="s">The stream to read the separator from.</param>
    /// <param name="separator">The field separator character</param>
    public CsvReader(Stream s, char separator) : this(s, null, separator) { }

    /// <summary>
    /// Creates a new Csv reader for the given stream and encoding.
    /// </summary>
    /// <param name="s">The stream to read the CSV from.</param>
    /// <param name="enc">The encoding used.</param>
    public CsvReader(Stream s, Encoding enc) : this(s, enc, ',') { }

    /// <summary>
    /// Creates a new reader for the given stream, encoding and separator character.
    /// </summary>
    /// <param name="s">The stream to read the data from.</param>
    /// <param name="enc">The encoding used.</param>
    /// <param name="separator">The separator character between the fields</param>
    public CsvReader(Stream s, Encoding enc, char separator) {

      this.separator = separator;
      this.stream = s;
      if (!s.CanRead) {
        throw new CsvReaderException("Could not read the given data stream!");
      }
      reader = (enc != null) ? new StreamReader(s, enc) : new StreamReader(s);
    }

    /// <summary>
    /// Creates a new Csv reader for the given text file path.
    /// </summary>
    /// <param name="filename">The name of the file to be read.</param>
    public CsvReader(string filename) : this(filename, null, ',') { }

    /// <summary>
    /// Creates a new reader for the given text file path and separator character.
    /// </summary>
    /// <param name="filename">The name of the file to be read.</param>
    /// <param name="separator">The field separator character</param>
    public CsvReader(string filename, char separator) : this(filename, null, separator) { }

    /// <summary>
    /// Creates a new Csv reader for the given text file path and encoding.
    /// </summary>
    /// <param name="filename">The name of the file to be read.</param>
    /// <param name="enc">The encoding used.</param>
    public CsvReader(string filename, Encoding enc) 
      : this(filename, enc, ',') { }

    /// <summary>
    /// Creates a new reader for the given text file path, encoding and field separator.
    /// </summary>
    /// <param name="filename">The name of the file to be read.</param>
    /// <param name="enc">The encoding used.</param>
    /// <param name="separator">The field separator character.</param>
    public CsvReader(string filename, Encoding enc, char separator) 
      : this(new FileStream(filename, FileMode.Open), enc, separator) { }

    #endregion

    #region Properties

    /// <summary>
    /// The separator character for the fields. Comma for normal CSV.
    /// </summary>
    public char Separator {
      get { return separator; }
      set { separator = value; }
    }

    #endregion

    #region Parsing

    /// <summary>
    /// Returns the fields for the next row of data (or null if at eof)
    /// </summary>
    /// <returns>A string array of fields or null if at the end of file.</returns>
    public string[] GetCsvLine() {

      string data = reader.ReadLine();
      if (data == null) return null;
      if (data.Length == 0) return new string[0];
      
      List<string> result = new List<string>();

      ParseCsvFields(result, data);
      
      return (string[])result.ToArray();
    }

    // Parses the fields and pushes the fields into the result list
    private void ParseCsvFields(List<string> result, string data) {

      int pos = -1;
      while (pos < data.Length)
        result.Add(ParseCsvField(data, ref pos));
    }

    // Parses the field at the given position of the data, modified pos to match
    // the first unparsed position and returns the parsed field
    private string ParseCsvField(string data, ref int startSeparatorPosition) {

      if (startSeparatorPosition == data.Length-1) {
        startSeparatorPosition++;
        // The last field is empty
        return "";
      }

      int fromPos = startSeparatorPosition + 1;

      // Determine if this is a quoted field
      if (data[fromPos] == '"') {
        // If we're at the end of the string, let's consider this a field that
        // only contains the quote
        if (fromPos == data.Length-1) {
          fromPos++;
          return "\"";
        }

        // Otherwise, return a string of appropriate length with double quotes collapsed
        // Note that FSQ returns data.Length if no single quote was found
        int nextSingleQuote = FindSingleQuote(data, fromPos+1);
        startSeparatorPosition = nextSingleQuote+1;
        return data.Substring(fromPos+1, nextSingleQuote-fromPos-1).Replace("\"\"", "\"");
      }

      // The field ends in the next separator or EOL
      int nextSeparator = data.IndexOf(separator, fromPos);
      if (nextSeparator == -1) {
        startSeparatorPosition = data.Length;
        return data.Substring(fromPos);
      }
      else {
        startSeparatorPosition = nextSeparator;
        return data.Substring(fromPos, nextSeparator - fromPos);
      }
    }

    // Returns the index of the next single quote mark in the string 
    // (starting from startFrom)
    private static int FindSingleQuote(string data, int startFrom) {

      int i = startFrom-1;
      while (++i < data.Length)
        if (data[i] == '"') {
          // If this is a double quote, bypass the chars
          if (i < data.Length-1 && data[i+1] == '"') {
            i++;
            continue;
          }
          else
            return i;
        }
      // If no quote found, return the end value of i (data.Length)
      return i;
    }

    #endregion


    /// <summary>
    /// Disposes the reader. The underlying stream is closed.
    /// </summary>
    public void Dispose() {
      // Closing the reader closes the underlying stream, too
      if (reader != null) reader.Close();
      else if (stream != null)
        stream.Close(); // In case we failed before the reader was constructed
      GC.SuppressFinalize(this);
    }
  }

  /// <summary>
  /// Exception class for CsvReader exceptions.
  /// </summary>
  [Serializable]
  public class CsvReaderException : System.Exception { 

    /// <summary>
    /// Constructs a new CsvReaderException.
    /// </summary>
    public CsvReaderException() : this("The CSV Reader encountered an error.") { }

    /// <summary>
    /// Constructs a new exception with the given message.
    /// </summary>
    /// <param name="message">The exception message.</param>
    public CsvReaderException(string message) : base(message) { }

    /// <summary>
    /// Constructs a new exception with the given message and the inner exception.
    /// </summary>
    /// <param name="message">The exception message.</param>
    /// <param name="inner">Inner exception that caused this issue.</param>
    public CsvReaderException(string message, Exception inner) : base(message, inner) { }

    /// <summary>
    /// Constructs a new exception with the given serialization information.
    /// </summary>
    /// <param name="info"></param>
    /// <param name="context"></param>
    protected CsvReaderException(System.Runtime.Serialization.SerializationInfo info, 
                                 System.Runtime.Serialization.StreamingContext context) 
      : base(info, context) { }
   
  }

}
