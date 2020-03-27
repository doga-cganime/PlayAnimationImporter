using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Antlr4.Runtime;
using Antlr4.Runtime.Tree;

namespace TotekanReaderLib
{
	public class TotekanParse
	{
		public static DoGAE1PParser.FrameContext parseE1P(string nameFile)
		{
			DoGAE1PParser.FrameContext context = null;
			using (var fileSteam = new FileStream(nameFile, FileMode.Open)) {
				var reader = new StreamReader(fileSteam, Encoding.GetEncoding(932));
				var inputStream = new AntlrInputStream(reader);
				var lexer = new DoGAE1PLexer(inputStream);
				var stream = new CommonTokenStream(lexer);
				var parser = new DoGAE1PParser(stream);
				
				context = parser.frame() as DoGAE1PParser.FrameContext;
				var tree = context as IParseTree;
				Console.WriteLine("Tree: " + tree.ToStringTree(parser));

			}
			return context;
		}

		public static DoGASufParser.ObjsContext parseSuf(string nameFile)
		{
			DoGASufParser.ObjsContext context = null;
			using (var fileSteam = new FileStream(nameFile, FileMode.Open)) {
				var reader = new StreamReader(fileSteam, Encoding.GetEncoding(932));
				var inputStream = new AntlrInputStream(reader);
				var lexer = new DoGASufLexer(inputStream);
				var stream = new CommonTokenStream(lexer);
				var parser = new DoGASufParser(stream);

				context = parser.objs() as DoGASufParser.ObjsContext;
				var tree = context as IParseTree;
				Console.WriteLine("Tree: " + tree.ToStringTree(parser));

			}
			return context;
		}

		public static DoGAAtrParser.AtrsContext parseAtr(string nameFile)
		{
			DoGAAtrParser.AtrsContext context = null;
			using (var fileSteam = new FileStream(nameFile, FileMode.Open)) {
				var reader = new StreamReader(fileSteam, Encoding.GetEncoding(932));
				var inputStream = new AntlrInputStream(reader);
				var lexer = new DoGAAtrLexer(inputStream);
				var stream = new CommonTokenStream(lexer);
				var parser = new DoGAAtrParser(stream);

				context = parser.atrs() as DoGAAtrParser.AtrsContext;
				var tree = context as IParseTree;
				Console.WriteLine("Tree: " + tree.ToStringTree(parser));
			}
			return context;
		}
	}
}
