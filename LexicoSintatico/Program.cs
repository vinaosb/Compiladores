// ################################################
// Uiversidade Federal de Santa Catarina
// INE5421 - Linguagens Formais e Compiladores
// Trabalho 1 - 2018/2
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
		static void Main(string[] args)
		{
			Console.OutputEncoding = System.Text.Encoding.Unicode;
			Console.WriteLine("===### Analise Sintatica ###===");
			string name;
			string path = @"";
			Console.WriteLine("Escreva o nome do arquivo");
			name = Console.ReadLine();

			path += name;

			Token t = new Token(path);
			List<Token.Tok> lt = t.ReadFile();
			//Console.WriteLine("Analise Lexica");
			//foreach (var l in lt)
			//{
			//    Console.WriteLine("<{0},{1}>", l.a, l.s);
			//}
			Sintatico s = new Sintatico();
			// ##### PRINT PARSING TABLE ####
			Console.WriteLine("");
			Console.WriteLine("Parsing Table:");
			Console.WriteLine("...");
			Console.WriteLine("Output in (f)file or (c)console? Default = file");

			char output = Console.ReadLine()[0];

			if (output == 'c')
			{
				string prod = "";
				foreach (var sy in s.ReferenceTable)
				{
					prod = "";
					foreach (var pr in sy.Value)
					{
						if (pr.nonterminal.Equals(Sintatico.NonTerminal.EMPTY))
							prod += pr.terminal.ToString() + " ";
						else
							prod += pr.nonterminal.ToString() + " ";
					}
					prod = prod.Replace("EMPTY", "ɛ");
					Console.WriteLine("{0},{1}->{2}", sy.Key.nonterminal, sy.Key.terminal, prod);
				}
				// ##### END PRINT PARSING TABLE ####

				if (s.predictiveParser(lt, false))
					Console.WriteLine("Entrada Aceita");
				else
					Console.WriteLine("Entrada Nao Aceita");
			}
			else
			{
				Console.Write("\n\nWriting output.txt");
				s.WriteOutput(lt);
			}

			// Console.WriteLine('\u025B');
			Console.Read();

		}
	}
}
