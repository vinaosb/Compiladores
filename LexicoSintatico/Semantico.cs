using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using FormaisECompiladores;
using static FormaisECompiladores.Sintatico;
using static FormaisECompiladores.Token;
using static LexicoSintatico.Arvore<FormaisECompiladores.Sintatico.Simbolo>;

namespace LexicoSintatico
{

	public class Semantico
	{
		Arvore<Terminals> Arvore;
		Sintatico SintaticoAux;
		List<Tok> Lt;
		public Semantico(List<Tok> lt)
		{
			Arvore = new Arvore<Terminals>();
			SintaticoAux = new Sintatico();
			Lt = lt;
			MontarArvore();
		}

		public bool PodeTerBreak(Nodo<Simbolo> nodo)
		{
			Simbolo t = new Simbolo();
			t.Terminal = Terminals.EMPTY;
			t.Nonterminal = NonTerminal.FORSTAT;
			return nodo.PaiContemDado(t);
		}

		public void MontarArvore()
		{
			Stack<Simbolo> pilha = new Stack<Simbolo>();
			Arvore.Raiz = new Arvore<Terminals>.Nodo<Terminals>(Terminals.EMPTY);
			var atual = Arvore.Raiz;

			Lt = SintaticoAux.CheckDollarSign(Lt);
			pilha.Push(new Simbolo { Nonterminal = NonTerminal.EMPTY, Terminal = Token.Terminals.DOLLAR });
			pilha.Push(new Simbolo { Nonterminal = NonTerminal.PROGRAM, Terminal = Token.Terminals.EMPTY });
			foreach (var token in Lt)
			{
				bool searchingTerminal = true;
				while (searchingTerminal)
				{
					List<Simbolo> newItems = new List<Simbolo>();
					newItems.Clear();
					if (token.t.Equals(pilha.Peek().Terminal))
					{
						pilha.Pop();
						searchingTerminal = false;
						atual.AddFilho(token.t);
						
						if (token.s == "$")
							return;
					}
					else //NonTerminal para trocar
					{
						NonTerminal nt = pilha.Pop().Nonterminal;
						Simbolo key = new Simbolo { Nonterminal = nt, Terminal = token.t };
						newItems = SintaticoAux.ReferenceTable[key];
						if (newItems[0].Terminal.Equals(Token.Terminals.EMPTY)
							&& newItems[0].Nonterminal.Equals(NonTerminal.EMPTY))
							newItems.Reverse();
						else
						{
							newItems.Reverse();
							foreach (Simbolo p in newItems)
							{
								atual.AddFilho(Terminals.EMPTY);
								pilha.Push(p);
							}
							atual = atual.PegaFilho();
							newItems.Reverse();//obrigatorio
						}
					}
				}
			}
		}

		public void WriteOutput(StreamWriter sr)
		{
			Arvore.PrintPosOrdem(sr);
		}
	}

	public class Contexto
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
			IsLoop = loop | pai.ChecaSePodeTerBreak();
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

		public Token.Terminals PegaTipoDoSimbolo(string simbolo)
		{
			return TabelaDeSimbolos[simbolo].Item2;
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
	}
}
