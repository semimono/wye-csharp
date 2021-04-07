using System;
using System.Diagnostics;
using System.IO;
using WyeCore;

namespace Wye {
  class Program {

    static void Main(string[] args) {
      if (args.Length == 0)
        throw new System.ArgumentException("Missing argument: <source file>");
      string sourceFile = args[0];
      
      Stopwatch watch = new Stopwatch();
      double nanosecPerTick = (1000L * 1000L * 1000L) / Stopwatch.Frequency;

      using(StreamReader source = File.OpenText(sourceFile)) {
        Lexer lexer = new Lexer();
        watch.Start();
        var tokens = lexer.lexAll(new LexBuffer(source));
        long managedMemoryUsage = GC.GetTotalMemory(true);
        watch.Stop();
        Console.WriteLine($"Tokens: [{ String.Join(", ", tokens) }]");
        Console.WriteLine("lexing took: " + (watch.ElapsedTicks * nanosecPerTick) /1000000d + " ms");
        Console.WriteLine("heap was: " + (managedMemoryUsage) + " B");
      }
    }
  }
}
