// ################################################
// Universidade Federal de Santa Catarina
// INE5426 - Construção de Compiladores
// Trabalho 1 - 2019/2
// Alunos:
//		- Bruno George de Moraes (14100825)
//      - Renan Pinho Assi (12200656)
//      - Marcelo José Dias (15205398)
//      - Vinícius Schwinden Berkenbrock (16100751)
//#################################################
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Resources;
using System.Reflection;
using System.Globalization;

namespace FormaisECompiladores
{
	class Program
	{
		public static string resourceString = "LexicoSintatico.Resource1";
		public static ResourceManager rr;
		public static CultureInfo ci;
		enum Output
		{
			LexSin,
			Lex,
			Sin
		};

		enum ExitMode
		{
			File,
			Console
		};

		private static void PrintLexico(Token t, ExitMode mode)
		{
			StreamWriter sr;
			switch (mode)
			{
				case ExitMode.Console:
					Console.Out.Write("\n\n"+ rr.GetString("AnaLex", ci) +"\n\n");
					sr = new StreamWriter(Console.OpenStandardOutput())
					{
						AutoFlush = true
					};
					Console.SetOut(sr);
					Console.OutputEncoding = System.Text.Encoding.UTF8;

					t.PrintToken(sr);
					break;
				default:
					Console.Out.Write(rr.GetString("GerAnaLex", ci) + "\n");
					sr = new StreamWriter(@"AnaliseLexica.txt")
					{
						AutoFlush = true
					};

					t.PrintToken(sr);

					sr.Close();
					break;
			}

		}

		private static void PrintSintatico(Sintatico s, List<Token.Tok> lt, ExitMode mode)
		{
			StreamWriter sr;
			switch (mode)
			{
				case ExitMode.Console:
					Console.Out.Write("\n\n"+ rr.GetString("AnaSint", ci) +"\n\n");
					sr = new StreamWriter(Console.OpenStandardOutput())
					{
						AutoFlush = true
					};
					Console.SetOut(sr);
					Console.OutputEncoding = System.Text.Encoding.UTF8;

					s.WriteOutput(lt, sr);
					break;
				default:
					Console.Out.Write(rr.GetString("GerAnaSint", ci) + "\n\n");
					sr = new StreamWriter(@"AnaliseSintatica.txt");
					s.WriteOutput(lt, sr);
					sr.Flush();
					sr.Close();
					break;
			}

		}
        


            static void Main()
		{
			Console.OutputEncoding = System.Text.Encoding.UTF8;
			ci = new CultureInfo("en");
			Console.Out.WriteLine(DateTime.Now.ToString(ci.DateTimeFormat));
			rr = new ResourceManager(resourceString, Assembly.GetExecutingAssembly());

			Console.Out.WriteLine(rr.GetString("Menu0",ci));
			Console.Out.WriteLine(rr.GetString("Menu1", ci));
			Console.Out.WriteLine(rr.GetString("Menu2", ci));
			Console.Out.WriteLine(rr.GetString("MenuDef", ci));
			Output outputMode;
			try
			{
				outputMode = (Output)(int)char.GetNumericValue(Console.ReadLine()[0]);
			}
			catch (Exception)
			{
				outputMode = 0;
			}


			Console.Out.WriteLine(rr.GetString("Menu3", ci));
			Console.Out.WriteLine(rr.GetString("Menu4", ci));
			Console.Out.WriteLine(rr.GetString("MenuDef", ci));
			ExitMode exitMode;
			try
			{
				exitMode = (ExitMode)(int)char.GetNumericValue(Console.ReadLine()[0]);
			}
			catch (Exception)
			{
				exitMode = 0;
			}

			string name;
			List<Token.Tok> lt;
			Token t;
			do
			{
				string path = @"";
				Console.Out.WriteLine(rr.GetString("NomeArquivo1", ci));
				Console.Out.WriteLine(rr.GetString("NomeArquivo2", ci));
				name = Console.ReadLine();
				path += name;

				t = new Token(path);
				lt = t.ReadFile();
			} while (lt == null);
			Sintatico s = new Sintatico();

			switch (outputMode)
			{
				case Output.Lex:
					PrintLexico(t, exitMode);
					break;
				case Output.Sin:
					PrintSintatico(s, lt, exitMode);
					break;
				default:
					PrintLexico(t, exitMode);
					Console.Out.Close();
					StreamWriter sw = new StreamWriter(Console.OpenStandardOutput())
					{
						AutoFlush = true
					};
					if (exitMode.Equals(ExitMode.Console))
					{
						Console.SetOut(sw);
						Console.OutputEncoding = System.Text.Encoding.UTF8;
						Console.Out.WriteLine("\n\n"+ rr.GetString("Stop", ci) + "\n\n");
						Console.ReadKey();
					}
					PrintSintatico(s, lt, exitMode);
					break;
			}

			Console.Out.WriteLine("\n\n" + rr.GetString("End", ci));
			Console.ReadKey();
			Console.Out.Close();

		}
	}
}
