﻿// ################################################
// Universidade Federal de Santa Catarina
// INE5426 - Construção de Compiladores
// Trabalho 1 - 2019/2
// Alunos:
//		- Bruno George de Moraes (14100825)
//      - Renan Pinho Assi (12200656)
//      - Marcelo José Dias (15205398)
//      - Vinícius Schwinden Berkenbrock (16100751)
//#################################################
using LexicoSintatico;
using System;
using System.Collections.Generic;
using System.IO;

namespace FormaisECompiladores
{
	class Program
	{
		enum Output
		{
			LexSinSem,
			LexSin,
			LexSem,
			SinSem,
			Lex,
			Sin,
			Sem
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

		private static void PrintArvoreSintatica(List<Token.Tok> lt, StreamWriter srX)
		{
			//######## PRINT DA ARVORE DE SINTAXE ##############

			//ArvoreSintatica arvore = new ArvoreSintatica();

			SintaticoX sX = new SintaticoX();
			sX.PredictiveParser(lt);
			srX.WriteLine("Imprimindo Arvores de Sintaxes no formato raiz-esquerda-direita");
			int count = 0;
			foreach (string exp in sX.listExpa)
			{
				if ((exp.Contains("*") || exp.Contains("+") || exp.Contains("-") || exp.Contains("/") || exp.Contains("%"))
					&& exp.Length > 2)
				{
					count++;
					ArvoreSintatica arvore = new ArvoreSintatica();
					arvore.parseRegex(arvore.initialNodo(exp));
					string result = arvore.listTree();
					srX.WriteLine("Arvore de Sintaxe " + count + ":");
					srX.WriteLine(result);
					srX.WriteLine();
				}
			}

			//######## FIM PRINT DA ARVORE DE SINTAXE ##############

		}

		private static void PrintSemantico(Semantico s, List<Token.Tok> lt, ExitMode mode)
		{
			StreamWriter sr;
			switch (mode)
			{
				case ExitMode.Console:
					Console.Out.Write("\n\n######Analise Semantica######\n\n");
					sr = new StreamWriter(Console.OpenStandardOutput())
					{
						AutoFlush = true
					};
					Console.SetOut(sr);
					Console.OutputEncoding = System.Text.Encoding.UTF8;

					PrintArvoreSintatica(lt, sr);
					s.WriteOutput(lt, sr);
					break;
				default:
					Console.Out.Write("Escrevendo AnaliseSemantica.txt\n\n");
					sr = new StreamWriter(@"AnaliseSemantica.txt");
					PrintArvoreSintatica(lt, sr);
					s.WriteOutput(lt, sr);
					sr.Flush();
					sr.Close();
					break;
			}

		}



		static void Main()
		{
			Console.OutputEncoding = System.Text.Encoding.UTF8;

			Console.Out.WriteLine("Digite 0 para ver a Análise Léxica, Sintática e Semântica");
			Console.Out.WriteLine("Digite 1 para ver apenas a Análise Léxica e Sintática");
			Console.Out.WriteLine("Digite 2 para ver apenas a Análise Léxica e Semântica");
			Console.Out.WriteLine("Digite 3 para ver apenas a Análise Sintática e Semântica");
			Console.Out.WriteLine("Digite 4 para ver apenas a Análise Léxica");
			Console.Out.WriteLine("Digite 5 para ver apenas a Análise Sintática");
			Console.Out.WriteLine("Digite 6 para ver apenas a Análise Semântica");
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
			Semantico sem = new Semantico();

			switch (outputMode)
			{
				case Output.LexSin:
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
				case Output.LexSem:
					PrintLexico(t, exitMode);
					Console.Out.Close();
					sw = new StreamWriter(Console.OpenStandardOutput())
					{
						AutoFlush = true
					};
					if (exitMode.Equals(ExitMode.Console))
					{
						Console.SetOut(sw);
						Console.OutputEncoding = System.Text.Encoding.UTF8;
						Console.Out.WriteLine("\n\nPressione uma tecla para continuar para o Semantico\n\n");
						Console.ReadKey();
					}
					PrintSemantico(sem, lt, exitMode);
					break;
				case Output.SinSem:
					PrintSintatico(s, lt, exitMode);
					Console.Out.Close();
					sw = new StreamWriter(Console.OpenStandardOutput())
					{
						AutoFlush = true
					};
					if (exitMode.Equals(ExitMode.Console))
					{
						Console.SetOut(sw);
						Console.OutputEncoding = System.Text.Encoding.UTF8;
						Console.Out.WriteLine("\n\nPressione uma tecla para continuar para o Semantico\n\n");
						Console.ReadKey();
					}
					PrintSemantico(sem, lt, exitMode);
					break;
				case Output.Lex:
					PrintLexico(t, exitMode);
					break;
				case Output.Sin:
					PrintSintatico(s, lt, exitMode);
					break;
				case Output.Sem:
					PrintSemantico(sem, lt, exitMode);
					break;
				default:
					PrintLexico(t, exitMode);
					Console.Out.Close();
					sw = new StreamWriter(Console.OpenStandardOutput())
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
					Console.Out.Close();
					sw = new StreamWriter(Console.OpenStandardOutput())
					{
						AutoFlush = true
					};
					if (exitMode.Equals(ExitMode.Console))
					{
						Console.SetOut(sw);
						Console.OutputEncoding = System.Text.Encoding.UTF8;
						Console.Out.WriteLine("\n\nPressione uma tecla para continuar para o Semantico\n\n");
						Console.ReadKey();
					}
					PrintSemantico(sem, lt, exitMode);
					break;
			}

			Console.Out.WriteLine("\n\n###Fim de processo###");
			Console.ReadKey();
			Console.Out.Close();

		}
	}
}
