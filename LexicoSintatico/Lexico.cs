// ################################################
// Universidade Federal de Santa Catarina
// INE5426 - Construção de Compiladores
// Trabalho 1 - 2019/2
// Alunos:
//		- Bruno George Marques           (14100825)
//      - Marcelo José Dias              (15205398)
//      - Renan Pinho Assi               (12200656)
//      - Vinícius Schwinden Berkenbrock (16100751)
//#################################################
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace FormaisECompiladores
{
	public class Token
	{

		public enum Attributes
		{
			ID, // IDENT
			BRKTPARE, // {}[]()
			INT, // Inteiros
			FLOAT, // Floats
			STRING, // Strings
			TYPES, // tipos: int, float, string
			NULL,
			LOOP, // for, break
			ITE, // if else
			ASSERT, // =
			COMPARISON, // <= >= != ...
			ARITMETHIC, // + - * / 
			SEPARATOR, // ;
			ERROR,
			EMPTY, // Auxiliar pro sintatico
			DOLLAR
		};
		public enum Terminals
		{
			IDENT, // ID
			OPENBRACE, CLOSEBRACE, OPENBRKT, CLOSEBRKT, OPENPARENT, CLOSEPARENT, // {}[]()
			INT, // int_constant
			STR, // string_constant
			FLT, // float_constant
			INTEGER_T, FLOAT_T, STRING_T, NULL, // tipos: int, float, string, null
			PRINT, RETURN, READ, // print, return, read
			IF, ELSE, FOR, BREAK,
			ASSERT, // =
			LT, LE, EQ, GT, GE, NE, // < <= == > >= <>
			ADD, MINUS, MULTIPLY, DIVIDE, MODULUS, // + - * / %
			SEPARATOR, // ;
			ERROR,
			EMPTY, // Auxiliar pro sintatico
			DOLLAR
		};


		public string path { get; set; }
		public Dictionary<string, Terminals> TokenCorrelation;
		public Dictionary<Terminals, Attributes> AttrCorrelation;

		// Could be moved to a better place
		public Dictionary<string, string> mapString;
		public string prefix_identifier = "STRING_CONTENT_";
		public int counter = 0;

		public struct Tok
		{
			public string s;
			public Terminals t;
			public Attributes a;
		}

		public Token(string Path)
		{
			path = Path;
			TokenCorrelation = new Dictionary<string, Terminals>();
			AttrCorrelation = new Dictionary<Terminals, Attributes>();
			mapString = new Dictionary<string, string>();
			init();
		}

		public void init()
		{
			TokenCorrelation.Add("{", Terminals.OPENBRACE);
			TokenCorrelation.Add("}", Terminals.CLOSEBRACE);
			TokenCorrelation.Add("[", Terminals.OPENBRKT);
			TokenCorrelation.Add("]", Terminals.CLOSEBRKT);
			TokenCorrelation.Add("(", Terminals.OPENPARENT);
			TokenCorrelation.Add(")", Terminals.CLOSEPARENT);
			TokenCorrelation.Add("int", Terminals.INTEGER_T);
			TokenCorrelation.Add("float", Terminals.FLOAT_T);
			TokenCorrelation.Add("string", Terminals.STRING_T);
			TokenCorrelation.Add("null", Terminals.NULL);
			TokenCorrelation.Add("print", Terminals.PRINT);
			TokenCorrelation.Add("return", Terminals.RETURN);
			TokenCorrelation.Add("read", Terminals.READ);
			TokenCorrelation.Add("if", Terminals.IF);
			TokenCorrelation.Add("else", Terminals.ELSE);
			TokenCorrelation.Add("for", Terminals.FOR);
			TokenCorrelation.Add("break", Terminals.BREAK);
			TokenCorrelation.Add("=", Terminals.ASSERT);
			TokenCorrelation.Add("<", Terminals.LT);
			TokenCorrelation.Add("<=", Terminals.LE);
			TokenCorrelation.Add("==", Terminals.EQ);
			TokenCorrelation.Add(">=", Terminals.GT);
			TokenCorrelation.Add(">", Terminals.GE);
			TokenCorrelation.Add("!=", Terminals.NE);
			TokenCorrelation.Add("+", Terminals.ADD);
			TokenCorrelation.Add("-", Terminals.MINUS);
			TokenCorrelation.Add("*", Terminals.MULTIPLY);
			TokenCorrelation.Add("/", Terminals.DIVIDE);
			TokenCorrelation.Add("%", Terminals.MODULUS);
			TokenCorrelation.Add(";", Terminals.SEPARATOR);
			TokenCorrelation.Add("stringExtensa", Terminals.STR);

			AttrCorrelation.Add(Terminals.IDENT, Attributes.ID);
			AttrCorrelation.Add(Terminals.OPENBRACE, Attributes.BRKTPARE);
			AttrCorrelation.Add(Terminals.OPENBRKT, Attributes.BRKTPARE);
			AttrCorrelation.Add(Terminals.OPENPARENT, Attributes.BRKTPARE);
			AttrCorrelation.Add(Terminals.CLOSEBRACE, Attributes.BRKTPARE);
			AttrCorrelation.Add(Terminals.CLOSEBRKT, Attributes.BRKTPARE);
			AttrCorrelation.Add(Terminals.CLOSEPARENT, Attributes.BRKTPARE);
			AttrCorrelation.Add(Terminals.INT, Attributes.INT);
			AttrCorrelation.Add(Terminals.STR, Attributes.STRING);
			AttrCorrelation.Add(Terminals.FLT, Attributes.FLOAT);
			AttrCorrelation.Add(Terminals.IF, Attributes.ITE);
			AttrCorrelation.Add(Terminals.ELSE, Attributes.ITE);
			AttrCorrelation.Add(Terminals.FOR, Attributes.LOOP);
			AttrCorrelation.Add(Terminals.BREAK, Attributes.LOOP);
			AttrCorrelation.Add(Terminals.ASSERT, Attributes.ASSERT);
			AttrCorrelation.Add(Terminals.LT, Attributes.COMPARISON);
			AttrCorrelation.Add(Terminals.LE, Attributes.COMPARISON);
			AttrCorrelation.Add(Terminals.EQ, Attributes.COMPARISON);
			AttrCorrelation.Add(Terminals.GT, Attributes.COMPARISON);
			AttrCorrelation.Add(Terminals.GE, Attributes.COMPARISON);
			AttrCorrelation.Add(Terminals.NE, Attributes.COMPARISON);
			AttrCorrelation.Add(Terminals.ADD, Attributes.ARITMETHIC);
			AttrCorrelation.Add(Terminals.MINUS, Attributes.ARITMETHIC);
			AttrCorrelation.Add(Terminals.MULTIPLY, Attributes.ARITMETHIC);
			AttrCorrelation.Add(Terminals.DIVIDE, Attributes.ARITMETHIC);
			AttrCorrelation.Add(Terminals.MODULUS, Attributes.ARITMETHIC);
			AttrCorrelation.Add(Terminals.SEPARATOR, Attributes.SEPARATOR);
		}

		public List<Tok> ReadFile()
		{
			List<Tok> LT = new List<Tok>();

			try
			{   // Open the text file using a stream reader.
				using (StreamReader sr = new StreamReader(path))
				{
					// Raw Text
					String full_text = "";
					String uniline_text = "";
					String token_string_text = "";
					while (sr.Peek() >= 0)
					{
						// Add Each Line to full_text variable
						string line = sr.ReadLine();
						uniline_text += line.Trim();
						full_text += line;
						// Also, skip to the next line
						full_text += "\n";
					}

					char[] charSeparator = new char[] { ' ' };
					string[] register = uniline_text.Split(charSeparator, StringSplitOptions.RemoveEmptyEntries);
					uniline_text = string.Join(" ", uniline_text.Split(charSeparator, StringSplitOptions.RemoveEmptyEntries)).Trim();
					token_string_text = searchStrings(uniline_text);

					// Print Original Code
					Console.WriteLine("Original Code:\n");
					Console.WriteLine(full_text);
					Console.WriteLine("\n\n\n\n");

					Console.WriteLine("Minified Code:\n");
					Console.WriteLine(uniline_text);
					Console.WriteLine("\n\n\n\n");


					Console.WriteLine("String Hunted Code:\n");
					Console.WriteLine(token_string_text);
					Console.WriteLine("\n\n\n\n");


					/* 
                     * Passing Just one line with everything 
                     * If break lines are important we can mark it
                     */

					//  // Read the stream to a string, and write the string to the console.
					//  while (sr.Peek() >= 0) { 
					//      String line = sr.ReadLine();
					//      Console.WriteLine(line);

					LT.AddRange(Tokenize(token_string_text));
					//  }
				}
			}
			catch (Exception e)
			{
				Console.WriteLine("The file could not be read:");
				Console.WriteLine(e.Message);
			}

			return LT;
		}

		private String searchStrings(String text)
		{
			string result = String.Copy(text);
			string substring = "";
			Boolean open_string = false;

			foreach (char c in text)
			{
				Boolean isQuote = c.Equals('\"');

				if (open_string || isQuote)
				{
					substring += c;
				}

				if (isQuote)
				{
					if (open_string)
					{
						String mapped_string_identifier = (prefix_identifier + counter);
						open_string = false;
						result = result.Replace(substring, mapped_string_identifier);
						mapString.Add(mapped_string_identifier, substring);
						substring = "";
						counter++;
					}
					else
					{
						open_string = true;
					}
				}

			}

			return result;
		}


		private String addSpace(String line)
		{
			foreach (char c in line)
			{
				if (c.Equals('{')
					| c.Equals('}')
					| c.Equals('[')
					| c.Equals(']')
					| c.Equals('(')
					| c.Equals(')')
					| c.Equals('=')
					| c.Equals('<')
					| c.Equals('>')
					| c.Equals('!')
					| c.Equals('+')
					| c.Equals('-')
					| c.Equals('*')
					| c.Equals('/')
					| c.Equals('%')
					| c.Equals(';'))
					line = line.Replace(c.ToString(), " " + c.ToString() + " ");

				line = line.Replace("  ", " ");
			}
			line = line.Replace("= =", "==");
			line = line.Replace("< =", "<=");
			line = line.Replace("> =", ">=");
			line = line.Replace("! =", "!=");

			return line;
		}

		public List<Tok> Tokenize(String s)
		{
			List<Tok> tokens = new List<Tok>();
			char[] charSeparator = new char[] { ' ' };
			string[] result;

			s = addSpace(s);

			result = s.Split(charSeparator, StringSplitOptions.RemoveEmptyEntries);

			foreach (var r in result)
			{
				string real_r = r;
				if (mapString.ContainsKey(r))
				{
					real_r = mapString.GetValueOrDefault(r);
				}
				Tok temp;
				temp.s = real_r;
				temp.t = getTerminal(real_r);
				temp.a = AttrCorrelation.GetValueOrDefault(temp.t);
				tokens.Add(temp);
			}

			return tokens;
		}

		public Terminals getTerminal(string s)
		{
			if (Char.IsNumber(s[0]))
			{
				if (s.Contains("."))
					return Terminals.FLT;
				return Terminals.INT;
			}
			if (TokenCorrelation.ContainsKey(s))
				return TokenCorrelation.GetValueOrDefault(s);
			if (Char.IsLetter(s[0]))
				return Terminals.IDENT;
			if (Char.Equals(s[0], '"'))
				return Terminals.STR;
			Console.WriteLine("{0} é invalido", s);
			return Terminals.ERROR;
		}
	}
}
