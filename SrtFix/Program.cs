// See https://aka.ms/new-console-template for more information


using SrtFix.Common;

var result = await Parser.ParseAsync("""
  s:\Shared Videos\Yellowstone\Yellowstone.2018.S05.1080p.x265-AMBER\Yellowstone.2018.S05E04.1080p.x265-AMBER.cs.srt
  """);

Console.WriteLine("Hello, World!");
