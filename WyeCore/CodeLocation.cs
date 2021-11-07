

namespace WyeCore {

  public struct CodeLocation {
    public int absolutePosition;
    public int line, column;

    public CodeLocation(int absolutePosition=0, int line=0, int column=0) {
      this.absolutePosition = absolutePosition;
      this.line = line;
      this.column = column;
    }

    public void advanceBy(char c) {
      ++absolutePosition;
      if (c == '\n') {
        ++line;
        column = 0;
      } else {
        ++column;
      }
    }

    public void advanceBy(string s) {
      int lastLine = -1;
      for (int i = 0; i < s.Length; ++i) {
        if ('\n' == s[i]) {
          ++line;
          lastLine = i;
        }
      }
      absolutePosition += s.Length;
      column = lastLine == -1 ?
        column + s.Length :
        s.Length - 1 - lastLine;
    }

    public override string ToString() {
      return $"({line + 1}, {column + 1})";
    }
  }
}
