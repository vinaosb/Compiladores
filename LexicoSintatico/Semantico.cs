using System;
using System.Collections.Generic;
using System.Text;
using FormaisECompiladores;
using static FormaisECompiladores.Sintatico;

namespace LexicoSintatico
{
	public class Semantico
	{
		Arvore<Simbolo> ArvoreSimbolos;
		Sintatico SintaticoAux;
		Arvore<Token.Terminals> Contexto;
		public Semantico()
		{
			ArvoreSimbolos = new Arvore<Simbolo>();
			SintaticoAux = new Sintatico();
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
