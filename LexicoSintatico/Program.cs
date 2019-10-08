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

		private static void PrintLexico(Token t, ExitMode mode)
		{
			StreamWriter sr;
			switch (mode)
			{
				case ExitMode.Console:
					Console.Out.Write("\n\n######Analise Lexica######\n\n");
					sr = new StreamWriter(Console.OpenStandardOutput())
					{
						AutoFlush = true
					};
					Console.SetOut(sr);
					Console.OutputEncoding = System.Text.Encoding.UTF8;

					t.PrintToken(sr);
					break;
				default:
					Console.Out.Write("Gerando arquivo AnaliseLexica.txt\n");
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
					Console.Out.Write("\n\n######Analise Sintatica######\n\n");
					sr = new StreamWriter(Console.OpenStandardOutput())
					{
						AutoFlush = true
					};
					Console.SetOut(sr);
					Console.OutputEncoding = System.Text.Encoding.UTF8;

					s.WriteOutput(lt, sr);
					break;
				default:
					Console.Out.Write("Escrevendo AnaliseSintatica.txt\n\n");
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

			Console.Out.WriteLine("Digite 0 para ver a Análise Léxica e Sintática");
			Console.Out.WriteLine("Digite 1 para ver apenas a Análise Léxica");
			Console.Out.WriteLine("Digite 2 para ver apenas a Análise Sintática");
			Console.Out.WriteLine("Default = 0");
			Output outputMode;
			try
			{
				outputMode = (Output)(int)char.GetNumericValue(Console.ReadLine()[0]);
			}
			catch (Exception)
			{
				outputMode = 0;
			}


			Console.Out.WriteLine("Digite 0 para criar um arquivo de output");
			Console.Out.WriteLine("Digite 1 para mostrar a saída no console");
			Console.Out.WriteLine("Default = 0");
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
				Console.Out.WriteLine("Escreva o nome do arquivo de leitura");
				Console.Out.WriteLine("Esse arquivo deve estar na pasta do executável");
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
						Console.Out.WriteLine("\n\nPressione uma tecla para continuar para o Sintatico\n\n");
						Console.ReadKey();
					}
					PrintSintatico(s, lt, exitMode);
					break;
			}

			Console.Out.WriteLine("\n\n###Fim de processo###");
			Console.ReadKey();
			Console.Out.Close();

		}
	}
}
