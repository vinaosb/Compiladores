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
		Arvore<Simbolo> Arvore;
		Sintatico SintaticoAux;
		List<Tok> Lt;
		public Semantico(List<Tok> lt)
		{
			Arvore = new Arvore<Simbolo>();
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
			Simbolo t = new Simbolo();
			t.Terminal = Terminals.EMPTY;
			t.Nonterminal = NonTerminal.PROGRAM;
			Arvore.Raiz = new Nodo<Simbolo>(t);
			Nodo<Simbolo> atual = Arvore.Raiz;

			foreach (var token in Lt)
			{
				bool searchingTerminal = true;
				while (searchingTerminal)
				{
					List<Simbolo> newItems = new List<Simbolo>();
					newItems.Clear();
					if (token.t.Equals(atual.Dado.Terminal))
					{
						atual = atual.Pai;
						searchingTerminal = false;
					}
					else if (atual.Dado.Nonterminal.Equals(NonTerminal.EMPTY))
					{
						atual = atual.Pai;
					}
					else //NonTerminal para trocar
					{
						NonTerminal nt = atual.Dado.Nonterminal;
						Simbolo key = new Simbolo { Nonterminal = nt, Terminal = token.t };

						try
						{
							newItems = SintaticoAux.ReferenceTable[key];
						}
						catch (Exception)
						{
							atual = atual.Pai.PegaFilho();
							continue;
						}

						if (!(newItems[0].Terminal.Equals(Token.Terminals.EMPTY)
							&& newItems[0].Nonterminal.Equals(NonTerminal.EMPTY)))
						{
							atual.AddFilhos(newItems);
							atual = atual.PegaFilho();
						}
					}
					var sr = new StreamWriter(Console.OpenStandardOutput())
					{
						AutoFlush = true
					};
					Console.SetOut(sr);
					Console.OutputEncoding = System.Text.Encoding.UTF8;


					Arvore.PrintPosOrdem(sr);
				}
			}
		}

		public void WriteOutput(StreamWriter sr)
		{

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
