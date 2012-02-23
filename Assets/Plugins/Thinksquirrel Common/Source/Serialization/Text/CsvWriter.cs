// CSV Writer
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
using System.IO;
using System.Text;

namespace ThinksquirrelSoftware.Common.Serialization.Text {

  /// <summary>
  /// A tool class for writing Csv and other char-separated field files.
  /// </summary>
  /// <remarks>
  /// Original CsvReader code (c) Jouni Heikniemi. <http://www.heikniemi.fi/jhlib/>
  /// </remarks>
  public class CsvWriter : StreamWriter {

    #region Private variables

    private char separator;

    #endregion

    #region Constructors

    /// <summary>
    /// Creates a new Csv writer for the given filename (overwriting existing contents).
    /// </summary>
    /// <param name="filename">The name of the file being written to.</param>
    public CsvWriter(string filename) 
      : this(filename, ',', false) { }

    /// <summary>
    /// Creates a new Csv writer for the given filename.
    /// </summary>
    /// <param name="filename">The name of the file being written to.</param>
    /// <param name="append">True if the contents shall be appended to the
    /// end of the possibly existing file.</param>
    public CsvWriter(string filename, bool append) 
      : this(filename, ',', append) { }

    /// <summary>
    /// Creates a new Csv writer for the given filename and encoding.
    /// </summary>
    /// <param name="filename">The name of the file being written to.</param>
    /// <param name="enc">The encoding used.</param>
    /// <param name="append">True if the contents shall be appended to the
    /// end of the possibly existing file.</param>
    public CsvWriter(string filename, Encoding enc, bool append) 
      : this(filename, enc, ',', append) { }

    /// <summary>
    /// Creates a new writer for the given filename and separator.
    /// </summary>
    /// <param name="filename">The name of the file being written to.</param>
    /// <param name="separator">The field separator character used.</param>
    /// <param name="append">True if the contents shall be appended to the
    /// end of the possibly existing file.</param>
    public CsvWriter(string filename, char separator, bool append) 
      : base(filename, append) { this.separator = separator; }

    /// <summary>
    /// Creates a new writer for the given filename, separator and encoding.
    /// </summary>
    /// <param name="filename">The name of the file being written to.</param>
    /// <param name="enc">The encoding used.</param>
    /// <param name="separator">The field separator character used.</param>
    /// <param name="append">True if the contents shall be appended to the
    /// end of the possibly existing file.</param>
    public CsvWriter(string filename, Encoding enc, char separator, bool append) 
      : base(filename, append, enc) { this.separator = separator; }

    /// <summary>
    /// Creates a new Csv writer for the given stream.
    /// </summary>
    /// <param name="s">The stream to write the CSV to.</param>
    public CsvWriter(Stream s) 
      : this(s, ',') { }

    /// <summary>
    /// Creates a new writer for the given stream and separator character.
    /// </summary>
    /// <param name="s">The stream to write the CSV to.</param>
    /// <param name="separator">The field separator character used.</param>
    public CsvWriter(Stream s, char separator) 
      : base(s) { this.separator = separator; }

    /// <summary>
    /// Creates a new writer for the given stream, separator and encoding.
    /// </summary>
    /// <param name="s">The stream to write the CSV to.</param>
    /// <param name="enc">The encoding used.</param>
    /// <param name="separator">The field separator character used.</param>
    public CsvWriter(Stream s, Encoding enc, char separator) 
      : base(s, enc) { this.separator = separator; }

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


    public void WriteFields(params object[] content) {
      
      string s;

      for (int i = 0; i < content.Length; ++i) {
        s = (content[i] != null ? content[i].ToString() : "");
        if (s.IndexOfAny(new char[] { Separator, '"' }) >= 0)
          // We have to quote the string
          s = "\"" + s.Replace("\"", "\"\"") + "\"";
        Write(s);

        // Write the separator unless we're at the last position
        if (i < content.Length-1)
          Write(separator);
      }
      Write(NewLine);
    }

  }

}