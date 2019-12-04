using FormaisECompiladores;
using System;
using System.Collections.Generic;
using System.Text;
using static FormaisECompiladores.Sintatico;

namespace LexicoSintatico
{
	public class GeradorCodigo
	{
		public List<string[]> LinhasDeCodigo { get; set; }
		public int ContLinhas { get; set; }
		public int ContTemporarios { get; set; }
		public Sintatico Sintatic { get; private set; }

		public GeradorCodigo()
		{
			LinhasDeCodigo = new List<string[]>();
			ContLinhas = 0;
			ContTemporarios = 0;
		}

		//Goto
		public string[] GeraLinha(string label)
		{
			string[] ret = new string[3];
			//label
			ret[0] = ContLinhas.ToString();
			//dest
			ret[1] = "goto";
			//op
			ret[2] = label;
			return ret;
		}

		// L C R G L2
		public string[] GeraLinha(string comp, string result, string label)
		{
			return GeraLinha("goto", comp, result, label);
		}

		// L D V1 OP V2
		public string[] GeraLinha(string op, string dest, string var1, string var2)
		{
			string[] ret = new string[5];
			//label
			ret[0] = ContLinhas.ToString();
			//dest
			if (dest == "")
			{
				dest = "T" + ContTemporarios.ToString();
				ContTemporarios++;
			}
			ret[1] = dest;
			//op
			ret[3] = op;
			//var1
			ret[2] = var1;
			//var2
			ret[4] = var2;
			return ret;
		}

		public void Parser(List<Token.Tok> lt)
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

					if (token.t.Equals(pilha.Peek().Terminal))
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
								success = CurrentContext.AddSimbolo(lt[i + 1].s, token.t);
								if (!success)
								{
									SetErrorMessage(last, token.s);
									return false;
								}
								break;
							case Token.Terminals.BREAK:
								if (!CurrentContext.ChecaSePodeTerBreak())
								{
									SetErrorMessage(last, token.s);
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
								for (int j = i + 1; j < lt.Count; j++)
								{
									var tipo2 = CurrentContext.PegaTipoDoSimbolo(lt[j].s);
									if (lt[j].a.Equals(Token.Attributes.ASSERT) |
										lt[j].a.Equals(Token.Attributes.ARITMETHIC) |
										lt[j].a.Equals(Token.Attributes.COMPARISON) |
										lt[j].t.Equals(tipo) |
										tipo.Equals(tipo2))
										continue;
									if (lt[j].a.Equals(Token.Attributes.SEPARATOR))
										break;
									SetErrorMessage(nt, lt[j].s);
									return false;
								}
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
		}
		private class Contexto
		{
			public string LabelPai { get; set; }
			public string LabelFim { get; set; }
			public List<Contexto> SubContextos { get; set; }
			public Contexto ContextoPai { get; set; }
			public bool IsLoop { get; set; }

			public Contexto(Contexto pai = null, string labelpai = "", bool isloop = false)
			{
				LabelPai = labelpai;
				SubContextos = new List<Contexto>();
				ContextoPai = pai;
				IsLoop = isloop;
			}
		}
	}
}
