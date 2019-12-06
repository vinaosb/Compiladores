using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using FormaisECompiladores;
using static FormaisECompiladores.Sintatico;

namespace LexicoSintatico
{
	public class GeradorCodigo
	{
		private List<ArvoreCodigo> Contextos;
		private ArvoreCodigo CurrentContext;
		private Sintatico Sintatic;
		private string Error;
		public GeradorCodigo()
		{
			Contextos = new List<ArvoreCodigo>();
			CurrentContext = new ArvoreCodigo();
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

					if (token.t.Equals(pilha.Peek().Terminal))
					{
						searchingTerminal = false;
						bool success;
						switch (token.t)
						{
							case Token.Terminals.IDENT:
								break;
							case Token.Terminals.DEF:
								break;
							case Token.Terminals.OPENBRACE:
								break;
							case Token.Terminals.CLOSEBRACE:
								break;
							case Token.Terminals.OPENBRKT:
								break;
							case Token.Terminals.CLOSEBRKT:
								break;
							case Token.Terminals.OPENPARENT:
								break;
							case Token.Terminals.CLOSEPARENT:
								break;
							case Token.Terminals.INT:
								break;
							case Token.Terminals.STR:
								break;
							case Token.Terminals.FLT:
								break;
							case Token.Terminals.INTEGER_T:
								break;
							case Token.Terminals.FLOAT_T:
								break;
							case Token.Terminals.STRING_T:
								break;
							case Token.Terminals.NULL:
								break;
							case Token.Terminals.PRINT:
								break;
							case Token.Terminals.RETURN:
								break;
							case Token.Terminals.READ:
								break;
							case Token.Terminals.IF:
								break;
							case Token.Terminals.ELSE:
								break;
							case Token.Terminals.FOR:
								break;
							case Token.Terminals.BREAK:
								break;
							case Token.Terminals.NEW:
								break;
							case Token.Terminals.ASSERT:
								break;
							case Token.Terminals.LT:
								break;
							case Token.Terminals.LE:
								break;
							case Token.Terminals.EQ:
								break;
							case Token.Terminals.GT:
								break;
							case Token.Terminals.GE:
								break;
							case Token.Terminals.NE:
								break;
							case Token.Terminals.ADD:
								break;
							case Token.Terminals.MINUS:
								break;
							case Token.Terminals.MULTIPLY:
								break;
							case Token.Terminals.DIVIDE:
								break;
							case Token.Terminals.MODULUS:
								break;
							case Token.Terminals.SEPARATOR:
								break;
							case Token.Terminals.COMMA:
								break;
							case Token.Terminals.ERROR:
								break;
							case Token.Terminals.EMPTY:
								break;
							case Token.Terminals.DOLLAR:
								break;
							default:
								break;
						}
						pilha.Pop();
					}
					else
					{
						ArvoreCodigo temp;
						NonTerminal nt = pilha.Pop().Nonterminal;
						switch (nt)
						{
							case NonTerminal.PROGRAM:
								break;
							case NonTerminal.FUNCLIST:
								break;
							case NonTerminal.FUNCLIST2:
								break;
							case NonTerminal.FUNCDEF:
								break;
							case NonTerminal.PARAMLIST:
								break;
							case NonTerminal.PARAMLIST2:
								break;
							case NonTerminal.STATEMENT:
								break;
							case NonTerminal.VARDECL:
								break;
							case NonTerminal.VAR2:
								break;
							case NonTerminal.FUNCCALL:
								break;
							case NonTerminal.PARAMLISTCALL:
								break;
							case NonTerminal.PARAMLISTCALL2:
								break;
							case NonTerminal.ATRIBSTAT:
								break;
							case NonTerminal.ATREXP:
								break;
							case NonTerminal.PRINTSTAT:
								break;
							case NonTerminal.READSTAT:
								break;
							case NonTerminal.RETURNSTAT:
								break;
							case NonTerminal.IFSTAT:
								break;
							case NonTerminal.IF2:
								break;
							case NonTerminal.FORSTAT:
								break;
							case NonTerminal.STATELIST:
								break;
							case NonTerminal.STATE2:
								break;
							case NonTerminal.ALLOCEXPRESSION:
								break;
							case NonTerminal.ALLOC2:
								break;
							case NonTerminal.ALLOC3:
								break;
							case NonTerminal.EXPRESSION:
								break;
							case NonTerminal.EXP2:
								break;
							case NonTerminal.NUMEXPRESSION:
								break;
							case NonTerminal.NUM2:
								break;
							case NonTerminal.TERM:
								break;
							case NonTerminal.TERM2:
								break;
							case NonTerminal.UNARYEXPR:
								break;
							case NonTerminal.FACTOR:
								break;
							case NonTerminal.LVALUE:
								break;
							case NonTerminal.EMPTY:
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

		private class ArvoreCodigo
		{
			public string Codigo { get; set; }
			public List<ArvoreCodigo> SubNodos { get; set; }
			public ArvoreCodigo NodoPai { get; set; }

			public ArvoreCodigo(string codigo = "", ArvoreCodigo pai = null)
			{
				Codigo = codigo;
				SubNodos = new List<ArvoreCodigo>();
				NodoPai = pai;
			}

			public ArvoreCodigo CriaNodoFilho(string codigo)
			{
				var contexto = new ArvoreCodigo(codigo, this);

				SubNodos.Add(contexto);

				return contexto;
			}

			public void Print(StreamWriter sr)
			{
				if (this.SubNodos.Count == 0)
				{
					sr.Write(this.Codigo + " ");
				}
				else
					foreach (var s in SubNodos)
					{
						s.Print(sr);
					}
			}
		}
	}


}
