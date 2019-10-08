// ################################################
// Universidade Federal de Santa Catarina
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

namespace FormaisECompiladores
{
	class Program
	{
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

		private static void PrintLexico(List<Token.Tok> lt, ExitMode mode)
		{
			StreamWriter sr;
			switch (mode)
			{
				case ExitMode.Console:
					Console.Out.Write("\n\n######Analise Lexica######\n\n");
					sr = new StreamWriter(Console.OpenStandardOutput());
					sr.AutoFlush = true;
					Console.SetOut(sr);
					break;
				default:
					Console.Out.Write("Gerando arquivo AnaliseLexica.txt\n");
					sr = new StreamWriter(@"AnaliseLexica.txt");
					break;
			}

			Console.Out.WriteLine("Analise Lexica\n\n");
			foreach (var l in lt)
			{
				Console.Out.WriteLine("<{0},{1}>", l.a, l.s);
			}

			sr.Flush();
		}

		private static void PrintSintatico(Sintatico s, List<Token.Tok> lt, ExitMode mode)
		{
			StreamWriter sr;
			switch (mode)
			{
				case ExitMode.Console:
					Console.Out.Write("\n\n######Analise Sintatica######\n\n");
					sr = new StreamWriter(Console.OpenStandardOutput());
					sr.AutoFlush = true;
					Console.SetOut(sr);
					Console.OutputEncoding = System.Text.Encoding.Unicode;

					break;
				default:
					Console.Out.Write("Escrevendo AnaliseSintatica.txt\n\n");
					sr = new StreamWriter(@"AnaliseSintatica.txt");
					break;
			}

			s.WriteOutput(lt, sr);
			sr.Flush();
		}

		static void Main(string[] args)
		{
			Console.OutputEncoding = System.Text.Encoding.Unicode;

			Console.Out.WriteLine("Digite 0 para ver a Análise Léxica e Sintática");
			Console.Out.WriteLine("Digite 1 para ver apenas a Análise Léxica");
			Console.Out.WriteLine("Digite 2 para ver apenas a Análise Sintática");
			Console.Out.WriteLine("Default = 0");
			Output outputMode = (Output)(int)char.GetNumericValue(Console.ReadLine()[0]);


			Console.Out.WriteLine("Digite 0 para criar um arquivo de output");
			Console.Out.WriteLine("Digite 1 para mostrar a saída no console");
			Console.Out.WriteLine("Default = 0");
			ExitMode exitMode = (ExitMode)(int)char.GetNumericValue(Console.ReadLine()[0]);

			string name;
			string path = @"";
			Console.Out.WriteLine("Escreva o nome do arquivo");
			Console.Out.WriteLine("Esse arquivo deve estar na pasta do executável");
			name = Console.ReadLine();
			path += name;

			Token t = new Token(path);
			List<Token.Tok> lt = t.ReadFile();
			Sintatico s = new Sintatico();

			switch (outputMode)
			{
				case Output.Lex:
					PrintLexico(lt, exitMode);
					break;
				case Output.Sin:
					PrintSintatico(s, lt, exitMode);
					break;
				default:
					PrintLexico(lt, exitMode);
					StreamWriter sw = new StreamWriter(Console.OpenStandardOutput());
					sw.AutoFlush = true;
					Console.SetOut(sw);
					Console.OutputEncoding = System.Text.Encoding.Unicode;
					Console.Out.WriteLine("\n\nPressione uma tecla para continuar\n\n");
					Console.ReadKey();
					PrintSintatico(s, lt, exitMode);
					break;
			}


			Console.ReadKey();

		}
	}
}
