using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

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

			public void PrintPosOrdem(Nodo<U> nodo, StreamWriter sr)
			{
				if (nodo == null)
					return;
				foreach (var filho in Filhos)
				{
					PrintPosOrdem(filho, sr);
				}
				sr.Write(nodo.Dado.ToString() + " ");
			}

			public bool ContemDado(Nodo<U> nodo, U dado)
			{
				if (nodo == null)
					return false;
				if (dado.Equals(Dado))
					return true;
				foreach (var filho in Filhos)
					if(ContemDado(filho,dado))
						return true;
				return false;
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
			Raiz.PrintPosOrdem(Raiz, sr);
		}

		public bool ContemDado(T dado)
		{
			return Raiz.ContemDado(Raiz, dado);
		}

	}
}
