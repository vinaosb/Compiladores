﻿using System;
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
		private string Error;
		public Semantico()
		{
			Contextos = new List<Contexto>();
			CurrentContext = new Contexto(Token.Terminals.DOLLAR);
			Contextos.Add(CurrentContext);
			Sintatic = new Sintatico();
		}

		public void WriteOutput(List<Token.Tok> lt, StreamWriter sr)
		{
			if (Parser(lt))
			{
				foreach (var c in Contextos)
				{
					c.Print(sr);
				}
				sr.WriteLine("Expressões Aritmeticas Válidas");
				sr.WriteLine("Declaração de Variáveis por Escopo OK");
				sr.WriteLine("'breaks' Válidos");
			}
			else
			{
				sr.WriteLine("Entrada Nao Aceita");

				sr.WriteLine(Error);
			}

		}

		public bool Parser(List<Token.Tok> lt)
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
						if (CurrentContext.ChecaSeEhTerminalDoContexto(token.t))
						{
							if (CurrentContext.FechaContextoDoPai)
								CurrentContext = CurrentContext.ContextoPai.ContextoPai;
							else
								CurrentContext = CurrentContext.ContextoPai;
						}
						switch (token.t)
						{
							case Token.Terminals.DEF: // Adicao de Simbolo
							case Token.Terminals.INTEGER_T: // Adicao de Simbolo
							case Token.Terminals.FLOAT_T: // Adicao de Simbolo
							case Token.Terminals.STRING_T: // Adicao de Simbolo
								if (i == 0)
								{
									success = CurrentContext.AddSimbolo(lt[i + 1].s, token.t);
									if (!success)
									{
										SetErrorMessage(last,token.s, "Erro de Contexto: ");
										return false;
									}
									break;
								}
								if (!lt[i - 1].t.Equals(Token.Terminals.NEW))
								{
									success = CurrentContext.AddSimbolo(lt[i + 1].s, token.t);
									if (!success)
									{
										SetErrorMessage(last, token.s, "Erro de Contexto: ");
										return false;
									}
								}
								break;
							case Token.Terminals.BREAK:
								if (!CurrentContext.ChecaSePodeTerBreak())
								{
									SetErrorMessage(last,token.s, "break na posição inválida");
									return false;
								}
								break;
							case Token.Terminals.DOLLAR: // Fim de Passada
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
							case NonTerminal.FUNCDEF: // Criacao de Contexto
								temp = CurrentContext.CriaContexto(Token.Terminals.CLOSEBRACE);
								CurrentContext = temp;
								break;
							case NonTerminal.STATEMENT: // Criacao de Contexto
								if (token.t.Equals(Token.Terminals.OPENBRACE))
									temp = CurrentContext.CriaContexto(Token.Terminals.CLOSEBRACE);
								else
									temp = CurrentContext.CriaContexto(Token.Terminals.SEPARATOR);
								CurrentContext = temp;
								break;
							case NonTerminal.IF2: // Criacao de Contexto
								if (!token.t.Equals(Token.Terminals.ELSE))
									break;
								temp = CurrentContext.CriaContexto(Token.Terminals.CLOSEBRACE);
								CurrentContext = temp;
								CurrentContext.FechaContextoDoPai = true;
								break;
							case NonTerminal.IFSTAT: // Criacao de Contexto
								temp = CurrentContext.CriaContexto(Token.Terminals.CLOSEBRACE);
								CurrentContext = temp;
								CurrentContext.FechaContextoDoPai = true;
								break;
							case NonTerminal.FORSTAT: // Criacao de Contexto
								temp = CurrentContext.CriaContexto(Token.Terminals.CLOSEBRACE, true);
								CurrentContext = temp;
								CurrentContext.FechaContextoDoPai = true;
								break;
							case NonTerminal.NUMEXPRESSION: // Verificacao de Tipos
								var tipo = CurrentContext.PegaTipoDoSimbolo(token.s);
								for (int j = i+1; j < lt.Count; j++)
								{
									var tipo2 = CurrentContext.PegaTipoDoSimbolo(lt[j].s);
									if (lt[j].a.Equals(Token.Attributes.ASSERT) |
										lt[j].a.Equals(Token.Attributes.ARITMETHIC) |
										lt[j].a.Equals(Token.Attributes.COMPARISON) |
										lt[j].a.Equals(Token.Attributes.BRKTPARE) |
										lt[j].t.Equals(tipo) |
										tipo.Equals(tipo2))
										continue;
									if (lt[j].a.Equals(Token.Attributes.SEPARATOR))
										break;
									SetErrorMessage(nt, lt[j].s, "Operação com atributos inválidos: ");
									return false;
								}
								break;
							default:
								break;
						}
						if (!nt.Equals(NonTerminal.EMPTY))
							last = nt;
						Simbolo key = new Simbolo { Nonterminal = nt, Terminal = token.t };
						if (!nt.Equals(NonTerminal.ATREXP) | !token.t.Equals(Token.Terminals.IDENT))
							newItems = Sintatic.ReferenceTable[key];
						else
						{
							if (lt[i + 1].t.Equals(Token.Terminals.OPENPARENT))
								newItems.Add(new Simbolo { Nonterminal = NonTerminal.FUNCCALL, Terminal = Token.Terminals.EMPTY });
							else
								newItems = Sintatic.ReferenceTable[key];
						}

						if (newItems[0].Terminal.Equals(Token.Terminals.EMPTY) &&
							newItems[0].Nonterminal.Equals(NonTerminal.EMPTY))
							newItems.Reverse();
						else
						{
							newItems.Reverse();
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

		public void SetErrorMessage(NonTerminal nt, string t, string message)
		{
			Error = message + "No não terminal: " + nt.ToString() + " com a entrada: " + t;
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
				if (pai != null)
					IsLoop = loop | pai.ChecaSePodeTerBreak();
				else
					IsLoop = loop;
				TerminalDoContexto = new List<Token.Terminals>();
				TerminalDoContexto.Add(terminal);
				FechaContextoDoPai = false;
			}

			public bool ChecaSeTemSimbolo(string simbolo)
			{
				if (TabelaDeSimbolos.ContainsKey(simbolo))
					return true;

				if (ContextoPai != null)
					if(ContextoPai.ChecaSeTemSimbolo(simbolo))
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

			public Token.Terminals PegaTipoDoSimbolo(string simbolo)
			{
				if (this.TabelaDeSimbolos.ContainsKey(simbolo))
					return this.TabelaDeSimbolos[simbolo].Item2;
				else if (this.ContextoPai != null)
					return this.ContextoPai.PegaTipoDoSimbolo(simbolo); 
				return Token.Terminals.ERROR;
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

			public bool ChecaSeEhTerminalDoContexto(Token.Terminals term)
			{
				return TerminalDoContexto.Contains(term);
			}

			public void Print(StreamWriter sr)
			{
				if (this.TabelaDeSimbolos.Count > 0)
					sr.WriteLine("Tabela de Símbolos ");
				foreach (var tab in this.TabelaDeSimbolos)
				{
					sr.WriteLine("Id = " + tab.Value.Item1.ToString() + " Tipo = " + tab.Value.Item2.ToString());
				}
				foreach (var filho in this.SubContextos)
					filho.Print(sr);
			}
		}
	}


}
