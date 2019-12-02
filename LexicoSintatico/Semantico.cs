using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using FormaisECompiladores;
using static FormaisECompiladores.Sintatico;

namespace LexicoSintatico
{
	public class Semantico
	{
		private List<Contexto> Contextos;
		private Contexto CurrentContext;
		private Sintatico Sintatic;
		public Semantico()
		{
			Contextos = new List<Contexto>();
			CurrentContext = new Contexto(Token.Terminals.DOLLAR);
			Contextos.Add(CurrentContext);
			Sintatic = new Sintatico();
		}

		public bool WriteOutput(List<Token.Tok> lt, StreamWriter sr)
		{
			Stack<Simbolo> pilha = new Stack<Simbolo>();

			lt = Sintatic.CheckDollarSign(lt);
			NonTerminal last = NonTerminal.EMPTY;

			pilha.Push(new Simbolo { Nonterminal = NonTerminal.EMPTY, Terminal = Token.Terminals.DOLLAR });
			pilha.Push(new Simbolo { Nonterminal = NonTerminal.PROGRAM, Terminal = Token.Terminals.EMPTY });

			for (int i = 0; i < lt.Count; i++)
			{
				var token = lt[i];
				bool searchingTerminal = true;
				while (searchingTerminal)
				{
					List<Simbolo> newItems = new List<Simbolo>();
					newItems.Clear();

					if(token.t.Equals(pilha.Peek().Terminal))
					{
						searchingTerminal = false;
						bool success;
						if (CurrentContext.ChecaSeEhTerminal(token.t))
						{
							if (CurrentContext.FechaContextoDoPai)
								CurrentContext = CurrentContext.ContextoPai.ContextoPai;
							else
								CurrentContext = CurrentContext.ContextoPai;
						}
						switch (token.t)
						{
							case Token.Terminals.DEF:
							case Token.Terminals.INTEGER_T:
							case Token.Terminals.FLOAT_T:
							case Token.Terminals.STRING_T:
								success = CurrentContext.AddSimbolo(lt[i + 1].s, token.t);
								if (!success)
								{
									SetErrorMessage(last,token.s);
									return false;
								}
								break;
							case Token.Terminals.DOLLAR:
								return true;
							default:
								break;
						}
						pilha.Pop();
					}
					else
					{
						Contexto temp;
						NonTerminal nt = pilha.Pop().Nonterminal;
						switch (nt)
						{
							case NonTerminal.FUNCDEF:
								temp = CurrentContext.CriaContexto(Token.Terminals.CLOSEBRACE);
								CurrentContext = temp;
								break;
							case NonTerminal.STATEMENT:
								if (token.t.Equals(Token.Terminals.OPENBRACE))
									temp = CurrentContext.CriaContexto(Token.Terminals.CLOSEBRACE);
								else
									temp = CurrentContext.CriaContexto(Token.Terminals.SEPARATOR);
								CurrentContext = temp;
								break;
							case NonTerminal.IF2:
								if (!token.t.Equals(Token.Terminals.ELSE))
									break;
								temp = CurrentContext.CriaContexto(Token.Terminals.CLOSEBRACE);
								CurrentContext = temp;
								CurrentContext.FechaContextoDoPai = true;
								break;
							case NonTerminal.IFSTAT:
								temp = CurrentContext.CriaContexto(Token.Terminals.CLOSEBRACE);
								CurrentContext = temp;
								CurrentContext.FechaContextoDoPai = true;
								break;
							case NonTerminal.FORSTAT:
								temp = CurrentContext.CriaContexto(Token.Terminals.CLOSEBRACE, true);
								CurrentContext = temp;
								CurrentContext.FechaContextoDoPai = true;
								break;
							default:
								break;
						}
						if (!nt.Equals(NonTerminal.EMPTY))
							last = nt;
						Simbolo key = new Simbolo { Nonterminal = nt, Terminal = token.t };
						newItems = Sintatic.ReferenceTable[key];

						if (newItems[0].Terminal.Equals(Token.Terminals.EMPTY) &&
							newItems[0].Nonterminal.Equals(NonTerminal.EMPTY))
							newItems.Reverse();
						else
						{
							foreach (Simbolo p in newItems)
							{
								pilha.Push(p);
							}
							newItems.Reverse();
						}
					}
				}
			}

			return false;
		}

		public void SetErrorMessage(NonTerminal nt, string t)
		{
			foreach (var productions in Sintatic.Producoes.GetValueOrDefault(nt))
			{
				string forma_sentencial = "";
				foreach (Token.Terminals final_t in Sintatic.GetFirstFromProd(productions))
				{
					Sintatic.first_expected += final_t.ToString() + ",";
				}
				Sintatic.first_expected = Sintatic.first_expected.Replace("EMPTY", "ɛ");

				foreach (var symbols in productions)
				{
					forma_sentencial += (
						!symbols.Nonterminal.Equals(NonTerminal.EMPTY) ? symbols.Nonterminal.ToString() : (
							!symbols.Terminal.Equals(Token.Terminals.EMPTY) ? symbols.Terminal.ToString() : "ɛ")
					);
					forma_sentencial += " ";
				}

				Sintatic.sentence_hint += nt + " -> " + forma_sentencial + "\n";
			}
			Sintatic.message_error = "Erro Sintático: Esperando: '" + nt + "' - Achado: '" + t + "'";
		}

		private class Contexto
		{
			private Dictionary<string, (string, Token.Terminals)> TabelaDeSimbolos { get; set; }
			public List<Contexto> SubContextos { get; set; }
			public Contexto ContextoPai { get; set; }
			private bool IsLoop { get; set; }
			public List<Token.Terminals> TerminalDoContexto { get; set; }
			public bool FechaContextoDoPai { get; set; }

			public Contexto(Token.Terminals terminal, bool loop = false, Contexto pai = null)
			{
				TabelaDeSimbolos = new Dictionary<string, (string, Token.Terminals)>();
				SubContextos = new List<Contexto>();
				ContextoPai = pai;
				IsLoop = loop;
				TerminalDoContexto = new List<Token.Terminals>();
				TerminalDoContexto.Add(terminal);
				FechaContextoDoPai = false;
			}

			public bool ChecaSeTemSimbolo(string simbolo)
			{
				if (TabelaDeSimbolos.ContainsKey(simbolo))
					return true;

				if (ContextoPai != null & ContextoPai.ChecaSeTemSimbolo(simbolo))
					return true;

				return false;
			}

			public bool ChecaSePodeTerBreak()
			{
				return IsLoop;
			}

			public bool AddSimbolo(string simbolo, Token.Terminals tipo)
			{
				if (ChecaSeTemSimbolo(simbolo))
					return false;
				TabelaDeSimbolos.Add(simbolo, (simbolo, tipo));
				return true;
			}

			public bool ChecaSeSimboloETipoBatem(string simbolo, Token.Terminals tipo)
			{
				if (!ChecaSeTemSimbolo(simbolo))
				{
					return false;
				}

				if (TabelaDeSimbolos[simbolo].Item1 == simbolo && TabelaDeSimbolos[simbolo].Item2 == tipo)
				{
					return true;
				}
				return false;
			}

			public Contexto CriaContexto(Token.Terminals terminal, bool isloop = false)
			{
				bool t = isloop | IsLoop;
				var contexto = new Contexto(terminal, t, this);

				SubContextos.Add(contexto);

				return contexto;
			}

			public bool ChecaSeEhTerminal(Token.Terminals term)
			{
				return TerminalDoContexto.Contains(term);
			}
		}
	}


}
