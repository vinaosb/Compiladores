// ################################################
// Universidade Federal de Santa Catarina
// INE5426 - Construção de Compiladores
// Trabalho 2 - 2019/2
// Alunos:
//		- Bruno George de Moraes (14100825)
//      - Renan Pinho Assi (12200656)
//      - Marcelo José Dias (15205398)
//      - Vinícius Schwinden Berkenbrock (16100751)
//#################################################

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Collections;
using System.Linq;
using System.Threading.Tasks;

namespace LexicoSintatico
{
	public class ArvoreSintatica
    {
        public Nodo raiz = null;
        public int qtNodo = 0;
        public String result;
        public int indicador;
        public Stack<Nodo> NodosInternos = new Stack<Nodo>();
        public Stack<Nodo> NodosPosicaoSeguinte = new Stack<Nodo>();
        public Hashtable posicao_seguinte = new Hashtable();
        public Hashtable folhas = new Hashtable();
        public HashSet<string> simbolos = new HashSet<string>();
        public int indicadorFim;

        public class Nodo
        {
            public Nodo nodoPai = null;
            public Nodo nodoEsquerda = null;
            public Nodo nodoDireita = null;
            public string valor = "";
            public HashSet<int> primeira_posicao = new HashSet<int>();
            public HashSet<int> ultima_posicao = new HashSet<int>();
            internal bool anulavel;
        }
        //Identifica se Nodo é Folha, ou seja indicador.
        public bool isLeaf(Nodo nodo)
        {
            return (nodo.nodoEsquerda == null && nodo.nodoDireita == null);
        }
        //Cria novo Nodo na Arvore
        public Nodo createLeaf(Nodo nodo)
        {
            Nodo n = new Nodo();
            n.nodoPai = nodo;
            if (raiz == null)
                raiz = nodo;
            return n;
        }
        //Recebe em que posicao da regex fazer o split e para cada novo termo chama o parse novamente
        //Ex: (a|b).c, pos=5, separa em dois termos: (a|b) e c, e para cada chama o parse
        //Na segunda chamada o (a|b) será dividido novamente a patir do simbolo |
        public void splitTerm(Nodo nodoOrigem, String termo, int pos)
        {
            Nodo folhaEsquerda = new Nodo();
            Nodo folhaDireita = new Nodo();
            folhaEsquerda = createLeaf(nodoOrigem);
            folhaEsquerda.valor = termo.Substring(0, pos);
            nodoOrigem.nodoEsquerda = folhaEsquerda;
            parseRegex(folhaEsquerda);
            //if (nodoOrigem.valor != "*")
            //{
                folhaDireita = createLeaf(nodoOrigem);
                folhaDireita.valor = termo.Substring(pos + 1);
                nodoOrigem.nodoDireita = folhaDireita;
                parseRegex(folhaDireita);
            //}
        }
        //Identifica simbolo de maior relevancia, e separa tudo à esquerda e à direita para ser os filhos.
        public void parseRegex(Nodo nodoOrigem)
        {
            int indexBar = 0, indexMult = 0, indexAdd = 0, indexMinus = 0, indexPct = 0;
            //Nodo newNodo = new Nodo();

            String termo = nodoOrigem.valor;
            termo = removeExternalParenthesis(termo);
            String novoTermo = ignoreBetweenParenthesis(termo);

            //int indexParenthesis = termo.IndexOf("(");
            indexMult = novoTermo.IndexOf("*");
            indexBar = novoTermo.IndexOf("/");
            indexAdd = novoTermo.IndexOf("+");
            indexMinus = novoTermo.IndexOf("-");
            indexPct = novoTermo.IndexOf("%");

            if (indexAdd > 0)
            {
                nodoOrigem.valor = "+";
                splitTerm(nodoOrigem, termo, indexAdd);
            }
            else if (indexMinus > 0)
            {
                nodoOrigem.valor = "-";
                splitTerm(nodoOrigem, termo, indexMinus);
            }
            else if (indexBar > 0)
            {
                nodoOrigem.valor = "/";
                splitTerm(nodoOrigem, termo, indexBar);
            }
            else if (indexPct > 0)
            {
                nodoOrigem.valor = "%";
                splitTerm(nodoOrigem, termo, indexPct);
            }
            else if (novoTermo.Contains("*"))
            {
                nodoOrigem.valor = "*";
                splitTerm(nodoOrigem, termo, novoTermo.IndexOf("*"));
            }


        }
        //Utiliza esse metodo para saber qual será o primeiro Novo na Arvore
        private string ignoreBetweenParenthesis(string termo)
        {
            String novoTermo = "";
            int openParenthesis = 0;
            foreach (var t in termo)
            {
                if (t == '(')
                {
                    openParenthesis++;
                    novoTermo = novoTermo + "@";
                    continue;
                }
                if (openParenthesis > 0)
                {
                    if (t == ')')
                        openParenthesis--;
                    novoTermo = novoTermo + "@";
                    continue;
                }

                novoTermo = novoTermo + t;
            }
            return novoTermo;
        }
        //Após a divisão do metodo acima, ele remove os parentesis externos para continuar o parse.
        //Ex: Continuando o exemplo, o filho esquerdo é (a.b.c), transforma em a.b.c
        public String removeExternalParenthesis(String text)
        {
            if (text.First() == '(' && text.Last() == ')')
                return text.Substring(1, text.Length - 2);
            return text;
        }
        //Metodo percorre a Arvore Sintatica toda, fazendo as seguintes funções:
        //1-Identifica os indicadores(nodos folhas)
        //2-Seta se os indicadores são anulaveis ou não
        //3-Identifica a primeira_posicao e ultima_posicao para os indicadores
        //4-Identifica quais os simbolos para o Automato
        //5-Insere Nodos não folha em uma pilha.
        public void readNodo(Nodo nodo)
        {
            if (isLeaf(nodo))
            {
                result = result + "," + nodo.valor;
                //result = result + "{" + indicador + "}" + nodo.valor;
                //if (nodo.valor == "&")
                //    nodo.anulavel = true;
                //else
                //    nodo.anulavel = false;
                //if (nodo.valor == "#")
                //    indicadorFim = indicador;
                //nodo.primeira_posicao.Add(indicador);
                //nodo.ultima_posicao.Add(indicador);
                //folhas.Add(indicador, nodo.valor);
                //simbolos.Add(nodo.valor);
                //indicador++;
                return;
            }
            //NodosInternos.Push(nodo);
            if(result=="")
                result = result + nodo.valor;
            else
                result = result +","+ nodo.valor;
            readNodo(nodo.nodoEsquerda);
            
            //if (nodo.valor != "*")
                readNodo(nodo.nodoDireita);
        }
        //Chama o primeiro Nodo da Arvore, e percorre toda ela
        public String listTree()
        {
            result = "";
            indicador = 1;
            readNodo(raiz);
            //cleanText(result);
            result = result.Replace(")", "");
            result = result.Replace("(", "");
            result = result.Replace("}", "");
            result = result.Replace("{", "");
            return result;
        }
        //public void cleanText(String str)
        //{
        //    result = result.Replace(")", "");
        //    result = result.Replace("(", "");
        //    result = result.Replace("}", "");
        //    result = result.Replace("{", "");
        //}
        //Inclui os Nodos Internos em uma pilha para serem percorridos na forma depth-first.
        //public void copyStack()
        //{
        //    NodosPosicaoSeguinte = new Stack<Nodo>(NodosInternos.Reverse());
        //}
        //O Primeiro Novo será e expressão inteira
        public Nodo initialNodo(String regex)
        {
            Nodo nodo = new Nodo();
            nodo.valor = regex;
            return nodo;
        }
        
    }
}
