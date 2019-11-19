using System;
using System.Collections.Generic;
using System.Text;
using FormaisECompiladores;

namespace LexicoSintatico
{
	public class Semantico
	{
		public class Contexto
		{
			private Dictionary<string, (string, Token.Attributes)> TabelaDeSimbolos { get; set; }
			public List<Contexto> SubContextos { get; set; }
			public Contexto ContextoPai { get; set; }
			private bool IsLoop { get; set; }

			public Contexto(bool loop = false, Contexto pai = null)
			{
				TabelaDeSimbolos = new Dictionary<string, (string, Token.Attributes)>();
				SubContextos = new List<Contexto>();
				ContextoPai = pai;
				IsLoop = loop;
			}

			public bool ChecaSeTemSimbolo(string simbolo)
			{
				if (TabelaDeSimbolos.ContainsKey(simbolo))
					return true;
				foreach (var sub in SubContextos)
				{
					if (sub.ChecaSeTemSimbolo(simbolo))
						return true;
				}
				return false;
			}

			public bool ChecaSeTemBreak()
			{
				return TabelaDeSimbolos.ContainsKey("break") && IsLoop;
			}

			public bool AddSimbolo(string simbolo, Token.Attributes tipo)
			{
				if (ChecaSeTemSimbolo(simbolo))
					return false;
				TabelaDeSimbolos.Add(simbolo, (simbolo, tipo));
				return true;
			}

			public bool ChecaSeSimboloETipoBatem(string simbolo, Token.Attributes tipo)
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

			public Contexto CriaContexto(bool isLoop = false)
			{
				var contexto = new Contexto(isLoop, this);

				SubContextos.Add(contexto);

				return contexto;
			}
		}
	}
}
