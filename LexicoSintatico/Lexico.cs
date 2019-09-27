// ################################################
// Uiversidade Federal de Santa Catarina
// INE5426 - Construção de Compiladores
// Trabalho 1 - 2019/2
// Alunos:
//		- Bruno George Marques (14100825)
//      - Renan Pinho Assi (12200656)
//      - Marcelo José Dias (15205398)
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
					// Read the stream to a string, and write the string to the console.
					while (sr.Peek() >= 0)
					{
						String line = sr.ReadLine();

						LT.AddRange(Tokenize(line));
					}
				}
			}
			catch (Exception e)
			{
				Console.WriteLine("The file could not be read:");
				Console.WriteLine(e.Message);
			}

			return LT;
		}

		public List<Tok> Tokenize(String s)
		{
			List<Tok> tokens = new List<Tok>();
			char[] charSeparator = new char[] { ' ' };
			string[] result;

			result = s.Split(charSeparator, StringSplitOptions.RemoveEmptyEntries);

			foreach (var r in result)
			{
				Tok temp;
				temp.s = r;
				temp.t = getTerminal(r);
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
