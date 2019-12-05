using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using static FormaisECompiladores.Token;

namespace LexicoSintatico
{
	public class Arvore<T>
	{
		public Nodo<T> Raiz { get; set; }
		public class Nodo<U>
		{
			public U Dado { get; set; }
			public LinkedList<Nodo<U>> Filhos { get; set; }
			public Nodo<U> Pai { get; set; }
			public Nodo(U dado, Nodo<U> pai = null)
			{
				Dado = dado;
				Filhos = new LinkedList<Nodo<U>>();
				Pai = pai;
			}

			public Nodo<U> AddFilho(U dado)
			{
				var nodo = new Nodo<U>(dado, this);
				Filhos.AddLast(nodo);
				return nodo;
			}

			public List<Nodo<U>> AddFilhos(List<U> dado)
			{
				List<Nodo<U>> list = new List<Nodo<U>>();
				foreach (var d in dado)
				{
					list.Add(new Nodo<U>(d, this));
				}
				foreach (var l in list)
					Filhos.AddLast(l);
				return list;
			}

			public void PrintPosOrdem(StreamWriter sr)
			{
				if (this == null)
					return;
				foreach (var filho in Filhos)
				{
					filho.PrintPosOrdem(sr);
				}
				if (this.Dado.ToString() != Terminals.EMPTY.ToString())
				sr.Write(this.Dado.ToString() + " ");
			}

			public bool ContemDado(U dado)
			{
				if (this == null)
					return false;
				if (dado.Equals(this.Dado))
					return true;
				foreach (var filho in Filhos)
					if(filho.ContemDado(dado))
						return true;
				return false;
			}

			public bool PaiContemDado(U dado)
			{
				if (this == null)
					return false;
				if (dado.Equals(this.Dado))
					return true;
				return this.Pai.PaiContemDado(dado);
			}

			public int PegaProfundidade()
			{
				if (this.Pai == null)
					return 0;
				return 1 + this.Pai.PegaProfundidade();
			}

			public Nodo<U> PegaFilho()
			{
				var ret = Filhos.First.Value;

				Filhos.AddLast(ret);
				Filhos.RemoveFirst();

				return ret;
			}
		}

		public Arvore()
		{
			Raiz = null;
		}

		public Arvore(Nodo<T> nodo)
		{
			Raiz = nodo;
		}

		public Arvore(T dado)
		{
			Raiz = new Nodo<T>(dado);
		}

		public void PrintPosOrdem(StreamWriter sr)
		{
			sr.WriteLine("\n\n#######\n\n");
			Raiz.PrintPosOrdem(sr);
			sr.WriteLine("\n");
		}

		public bool ContemDado(T dado)
		{
			return Raiz.ContemDado(dado);
		}
		public bool PaiContemDado(T dado)
		{
			return Raiz.PaiContemDado(dado);
		}

	}
}
