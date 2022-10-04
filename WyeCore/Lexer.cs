using System;
using System.Diagnostics;
using System.Collections.Generic;


namespace WyeCore {
  public class Lexer {

    /// <summary>
    /// Removes the next token in the buffer and returns it. If the end of the source is reached, returns null
    /// </summary>
    /// <param name="source">soure to extract a token from</param>
    /// <returns>the next token or null if end of source</returns>
    public Token? lexOne(LexBuffer source) {
      while(!source.atEnd()) {
        source.consumeWhitespace();

        if (source.atEnd())
          return null;

        CodeLocation startLocation = source.GetLocation();

        if (lexLineComment(source) || lexBlockComment(source))
          continue;

        string stringLiteral = lexStringLiteral(source);
        if (stringLiteral != null)
          return new Token(TokenType.STRING, stringLiteral, startLocation, source.GetLocation());

        string name = lexName(source);
        if (name != null)
          return new Token(TokenType.NAME, name, startLocation, source.GetLocation());

        string number = lexNumber(source);
        if (number != null)
          return new Token(TokenType.NUMBER, number, startLocation, source.GetLocation());

        return new Token(source.readChar(), startLocation, source.GetLocation());
      }
      return null;
    }

    /// <summary>
    /// Extracts all tokens from the provided buffer and returns a list of tokens.
    /// </summary>
    /// <param name="source"></param>
    /// <returns></returns>
    public List<Token> lexAll(LexBuffer source) {
      List<Token> tokens = new List<Token>();

      Token? next;
      while((next = lexOne(source)).HasValue) {
        tokens.Add(next.Value);
      }

      return tokens;
    }

    private static bool lexLineComment(LexBuffer source) {
      if (!source.tryRead("//"))
        return false;
      source.readTillLineEnd();
      return true;
    }

    private static bool lexBlockComment(LexBuffer source) {
      if (!source.tryRead("/*"))
        return false;
      do {
        source.readTillOneOf(new string[] {"/*", "*/"});
        lexBlockComment(source);
      } while (!source.tryRead("*/"));
      return true;
    }

    private static string lexStringLiteral(LexBuffer source) {
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

    private static string lexNumber(LexBuffer source) {
      char first = source.nextChar();
      if (
        !isDigit(first) &&
        first != '.'
      )
        return null;

      return source.readTill(c =>
        !isDigit(c) &&
        !isLetter(c) &&
        c != '.');
    }

    private static string lexName(LexBuffer source) {
      char first = source.nextChar();
      if (!isLetter(first) && first != '_')
        return null;

      return source.readTill(c =>
        !isLetter(c) &&
        !isDigit(c) &&
        c != '_');
    }

    private static bool isDigit(char c) {
      return c >= '0' && c <= '9';
    }

    private static bool isLetter(char c) {
      return isLowerLetter(c) || isUpperLetter(c);
    }

    private static bool isUpperLetter(char c) {
      return c >= 'A' && c <= 'Z';
    }

    private static bool isLowerLetter(char c) {
      return c >= 'a' && c <= 'z';
    }

    public enum TokenType {
      STRING,
      NUMBER,
      NAME,
      SYMBOL
    }

    public struct Token {
      public TokenType type;
      public string value;
      public char symbol;
      CodeLocation startLocation, endLocation;

      public Token(TokenType type, string value, CodeLocation start, CodeLocation end) {
        this.type = type;
        this.value = value;
        this.startLocation = start;
        this.endLocation = end;
        this.symbol = char.MinValue;
      }

      public Token(char symbol, CodeLocation start, CodeLocation end) {
        this.type = TokenType.SYMBOL;
        this.value = null;
        this.symbol = symbol;
        this.startLocation = start;
        this.endLocation = end;
      }

      public override string ToString() {
        return type == TokenType.SYMBOL ? 
          $"({type}: \"{symbol}\", {startLocation}-{endLocation})" :
          $"({type}: \"{value}\", {startLocation}-{endLocation})";
      }
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
