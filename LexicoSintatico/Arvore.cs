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
			public Nodo<U> Direita { get; set; }
			public Nodo<U> Esquerda { get; set; }
			public Nodo<U> Pai { get; set; }
			public Nodo(U dado, Nodo<U> pai = null)
			{
				Dado = dado;
				Esquerda = Direita = null;
				Pai = pai;
			}

			public void PrintPosOrdem(Nodo<U> nodo, StreamWriter sr)
			{
				if (nodo == null)
					return;
				PrintPosOrdem(nodo.Esquerda, sr);
				PrintPosOrdem(nodo.Direita, sr);
				sr.Write(nodo.Dado.ToString());
			}
		}

		public Arvore()
		{
			Raiz = null;
		}

		public void PrintPosOrdem(StreamWriter sr)
		{
			Raiz.PrintPosOrdem(Raiz, sr);
		}

	}
}
