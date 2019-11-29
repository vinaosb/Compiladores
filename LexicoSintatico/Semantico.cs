using System;
using System.Collections.Generic;
using System.Text;
using FormaisECompiladores;

namespace LexicoSintatico
{
	public class Semantico
	{
		
	}

	public class SDT
	{
		public class Node<T>
		{
			public string Operacao { get; set; }
			public Node<T> Herdado { get; set; }
			public Node<T> Direita { get; set; }
			public Node<T> Sintetizado { get; set; }
			public T Valor { get; set; }
			public Node(string op = "", Node<T> esq = null, Node<T> dir = null, T val = default)
			{
				Operacao = op;
				Herdado = esq;
				Direita = dir;
				Valor = val;
			}

			public void Sintetizar()
			{
				switch (Operacao)
				{
					case "+":
						Sintetizado = Direita.Sintetizado;
						Herdado.Valor = Soma(Sintetizado.Valor, Direita.Valor);
						break;
					case "-":
						Sintetizado = Direita.Sintetizado;
						Herdado.Valor = Diferenca(Sintetizado.Valor, Direita.Valor);
						break;
					case "*":
						Sintetizado = Direita.Sintetizado;
						Herdado.Valor = Multiplica(Sintetizado.Valor, Direita.Valor);
						break;
					case "/":
						Sintetizado = Direita.Sintetizado;
						Herdado.Valor = Divide(Sintetizado.Valor, Direita.Valor);
						break;
					case "%":
						Sintetizado = Direita.Sintetizado;
						Herdado.Valor = Mod(Sintetizado.Valor, Direita.Valor);
						break;
					default:
						Sintetizado = Herdado;
						break;
				}
			}

			private T Soma(T i, T j)
			{
				dynamic a = i;
				dynamic b = j;
				if (b == null)
					return i;
				return (T) a + b;
			}
			private T Diferenca(T i, T j)
			{
				dynamic a = i;
				dynamic b = j;
				if (b == null)
					return -a;
				return (T) a - b;
			}
			private T Multiplica(T i, T j)
			{
				dynamic a = i;
				dynamic b = j;
				return (T) a * b;
			}
			private T Divide(T i, T j)
			{
				dynamic a = i;
				dynamic b = j;
				return (T) a / b;
			}
			private T Mod(T i, T j)
			{
				dynamic a = i;
				dynamic b = j;
				return (T) a % b;
			}
		}
	}

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
