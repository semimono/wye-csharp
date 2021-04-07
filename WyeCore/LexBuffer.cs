using System;
using System.IO;

namespace WyeCore {
  public class LexBuffer {
    // public const int READER_BUFFER_SIZE = 1024;

    private TextReader source;
    private int bufferSize;
    // the current internal buffer
    private char[] buffer;
    // the next internal buffer
    private char[] futureBuffer;
    // the position within the internal buffer where reading will continue next
    private int position = 0;
    // the length of the current buffer
    private int length = 0;
    private bool readerEmpty = false;
    private System.Text.StringBuilder builder;

    public LexBuffer(TextReader source, int bufferSize=1024) {
      this.source = source;
      this.bufferSize = bufferSize;
      this.buffer = new char[bufferSize];
      this.futureBuffer = new char[bufferSize];
      this.builder = new System.Text.StringBuilder(bufferSize * 2);
      
      // prime the buffers
      length = source.Read(buffer, 0, buffer.Length);
      length += source.Read(futureBuffer, 0, futureBuffer.Length);
      updateReaderEmpty();
    }

    public bool atEnd() {
      return position >= length && readerEmpty;
    }

    public void consumeWhitespace() {
      while(!atEnd()) {
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
        throw new Exception("attempted to consume value that wasn't immediately next: " + value); // TODO: use appropriate exception/params
    }

    public string readTillLineEnd() {
      return readTill('\n');
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
      throw new Exception("read till end of content without encountering expected text"); // TODO: use appropriate exception/params
    }

    public string readTill(char value) {
      builder.Clear();
      int lastReadPos = position;
      while (!atEnd()) {
        if (isNext(value)) {
          if (position > lastReadPos)
            builder.Append(buffer, lastReadPos, position - lastReadPos);
          return builder.ToString();
        }
        ++position;
        if (bufferEmpty()) {
          if (position > lastReadPos)
            builder.Append(buffer, lastReadPos, position - lastReadPos);
          readChunk();
          lastReadPos = 0;
        }
      }
      throw new Exception("read till end of content without encountering expected character: '" + value + "'"); // TODO: use appropriate exception/params
    }

    public string readTill(Predicate<char> condition) {
      if (atEnd())
        return "";
      guaranteeData();
      builder.Clear();
      int lastReadPos = position;
      while (!atEnd() && !condition(buffer[position])) {
        ++position;
        if (bufferEmpty()) {
          if (position > lastReadPos)
            builder.Append(buffer, lastReadPos, position - lastReadPos);
          readChunk();
          lastReadPos = 0;
        }
      }
      if (position > lastReadPos)
        builder.Append(buffer, lastReadPos, position - lastReadPos);
      return builder.ToString();
    }

    public char readChar() {
      guaranteeData();
      return buffer[position++];
    }

    public char nextChar() {
      guaranteeData();
      return buffer[position];
    }

    public bool isNext(char value) {
      if (atEnd())
        return false;

      guaranteeData();

      return buffer[position] == value;
    }

    public bool isNextInRange(char min, char max) {
      if (atEnd())
        return false;

      guaranteeData();

      char next = buffer[position];
      return next >= min && next <= max;
    }

    private bool isNext(string value) {
      if (value.Length > bufferSize)
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
      if (index >= bufferSize)
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

    /// <summary>
    /// Loads at least a certain number of characters into the buffer
    /// </summary>
    /// <param name="count"></param>
    /// <returns>true if the data count or more is now in the buffer. False if it cannot put that many characters into the buffer.</returns>
    private bool guaranteeDataCount(int count=1) {
      if (count > bufferSize)
        throw new SystemException("Tried to read " + count + " bytes. Cannot read more than the buffer size at once.");

      guaranteeData();

      return count <= length - position;
    }

    private void guaranteeData() {
      if (!bufferEmpty())
        return;
      if (atEnd())
        throw new Exception("Reached end of file without completing block."); // TODO: fill out exception type/params
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
}