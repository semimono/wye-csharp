using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.IO;


namespace WyeCore {
  public class Lexer {

    public Nugget lex(LexBuffer source) {
      while(!source.atEnd()) {
        source.consumeWhitespace();
        if (lexLineComment(source) || lexBlockComment(source))
          continue;
        
        string stringLiteral = lexStringLiteral(source);
        if (stringLiteral != null) {
          // return new Sequence<char>(stringLiteral.ToCharArray());
        }
      }
      return new Dictionary();
    }

    public static bool lexLineComment(LexBuffer source) {
      if (!source.tryRead("//"))
        return false;
      source.readTillLineEnd();
      return true;
    }

    public static bool lexBlockComment(LexBuffer source) {
      if (!source.tryRead("/*"))
        return false;
      do {
        source.readTillOneOf(new string[] {"/*", "*/"});
        lexBlockComment(source);
      } while (!source.tryRead("*/"));
      return true;
    }

    public static string lexStringLiteral(LexBuffer source) {
      if (!source.tryRead("\""))
        return null;
      string stringContents = "";
      while(true) {
        stringContents += source.readTillOneOf(new string[] {"\\", "\""});
        if (source.tryRead("\"")) {
          break;
        } else {
          source.consume("\\");
          // TODO: handle other backslash character combos (like \n)
          stringContents += source.readChar();
        }
      }
      return stringContents;
    }

  }

  public class LexBuffer {
    public const int READER_BUFFER_SIZE = 1024;

    private TextReader source;
    private char[] buffer = new char[READER_BUFFER_SIZE];
    private char[] futureBuffer = new char[READER_BUFFER_SIZE];
    private int position = 0;
    private int length = 0;
    private bool readerEmpty = false;
    private System.Text.StringBuilder builder = new System.Text.StringBuilder(READER_BUFFER_SIZE * 2);

    public LexBuffer(TextReader source) {
      this.source = source;
      
      // prime the buffers
      length = source.Read(buffer, 0, buffer.Length);
      length += source.Read(futureBuffer, 0, futureBuffer.Length);
      updateReaderEmpty();
    }

    public bool atEnd() {
      return position >= length && readerEmpty;
    }

    public void consumeWhitespace() {
      while(true) {
        if (atEnd())
          break;
        if (bufferEmpty())
          readChunk();
        if (!char.IsWhiteSpace(buffer[position]))
          break;
        ++position;
      }
    }

    /// <summary>
    /// Returns true if 'value' is immediately next in the buffer and consumes it.
    /// </summary>
    /// <param name="value">The string to match exactly</param>
    /// <returns>true if 'value' is immediately next in the buffer</returns>
    public bool tryRead(string value) {
      if (isNext(value)) {
        position += value.Length;
        return true;
      }
      return false;
    }

    public void consume(string value) {
      if (!tryRead(value))
        throw new Exception(); // TODO: use appropriate exception/params
    }

    public string readTillLineEnd() {
      guaranteeData();
      int offset = 0;
      builder.Clear();
      while(true) {
        if (position + offset >= length)
          throw new Exception(); // TODO: use appropriate exception/params
        if (position + offset >= buffer.Length) {
          if (offset != 0)
            builder.Append(buffer, position, offset);
          position = buffer.Length;
          readChunk();
          offset = 0;
          continue;
        }
        if (buffer[position + offset] == '\n') {
          if (offset != 0)
            builder.Append(buffer, position, offset);
          position += offset;
          break;
        }
        ++offset;
      }
      return builder.ToString();
    }

    public string readTillOneOf(string[] values) {
      builder.Clear();
      int lastReadPos = position;
      while (!atEnd()) {
        foreach (string value in values) {
          if (isNext(value)) {
            if (position > lastReadPos)
              builder.Append(buffer, lastReadPos, position - lastReadPos);
            return builder.ToString();
          }
        }
        ++position;
        if (bufferEmpty()) {
          if (position > lastReadPos)
            builder.Append(buffer, lastReadPos, position - lastReadPos);
          readChunk();
          lastReadPos = 0;
        }
      }
      throw new Exception(); // TODO: use appropriate exception/params
    }

    public char readChar() {
      guaranteeData();
      return buffer[position++];
    }

    private bool isNext(string value) {
      if (value.Length > READER_BUFFER_SIZE)
        throw new SystemException("Trying to read too large a value from buffer: " + value);
      if (value.Length > length - position)
        return false;

      guaranteeData();

      for (int i=0; i<value.Length; ++i) {
        if (i + position < buffer.Length) {
          if (buffer[i + position] != value[i])
            return false;
        } else {
          if (futureBuffer[i + position - buffer.Length] != value[i])
            return false;
        }
      }
      return true;
    }

    private char getAt(int index) {
      if (index >= READER_BUFFER_SIZE)
        throw new SystemException("Trying to read too far an index from buffer: " + index);
      int pos = position + index;
      if (pos >= length)
        throw new IndexOutOfRangeException("trying to read character at index beyond length of buffer/input."); // TODO: use appropriate exception/params
      if (pos >= buffer.Length) {
        return futureBuffer[pos - buffer.Length];
      } else {
        return buffer[pos];
      }
    }

    private void guaranteeData() {
      if (atEnd())
        throw new Exception(); // TODO: fill out exception type/params
      if (bufferEmpty())
        readChunk();
    }

    private bool bufferEmpty() {
      return position >= buffer.Length;
    }

    private void updateReaderEmpty() {
      readerEmpty = source.Peek() == -1;
    }

    private void readChunk() {
      if (position < buffer.Length)
        throw new SystemException("Position was set incorrectly when reading new chunk");
      if (length <= buffer.Length)
        throw new SystemException("Attempting to read next buffer without any more buffers");
      
      var tempBuffer = buffer;
      buffer = futureBuffer;
      futureBuffer = tempBuffer;

      length -= buffer.Length;
      position -= buffer.Length;

      length += source.Read(futureBuffer, 0, futureBuffer.Length);
      updateReaderEmpty();
    }
  }

  public class LexError: Exception {

    public CodeLocation location;

    public LexError(CodeLocation location, string message) : base(message) {
      this.location = location;
    }

    public override string ToString() {
      return String.Format("Syntax Error at ({0},{1}): {2}", location.line, location.column, Message);
    }
  }
}
