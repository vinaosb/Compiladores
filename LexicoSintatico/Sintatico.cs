// ################################################
// Universidade Federal de Santa Catarina
// INE5426 - Construção de Compiladores
// Trabalho 1 - 2019/2
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

namespace FormaisECompiladores
{
	public class Sintatico
	{
        public string message_error = "";
        public string sentence_hint = "";
        public string first_expected = "";
        public List<string> listExpa;

        public enum NonTerminal
		{
			PROGRAM,
			FUNCLIST,
			FUNCLIST2,
			FUNCDEF,
			PARAMLIST,
			PARAMLIST2,
			STATEMENT,
			VARDECL,
			VAR2,
			FUNCCALL,
			PARAMLISTCALL,
			PARAMLISTCALL2,
			ATRIBSTAT,
			ATREXP,
			PRINTSTAT,
			READSTAT,
			RETURNSTAT,
			IFSTAT,
			IF2,
			FORSTAT,
			STATELIST,
			STATE2,
			ALLOCEXPRESSION,
			ALLOC2,
			ALLOC3,
			EXPRESSION,
			EXP2,
			NUMEXPRESSION,
			NUM2,
			TERM,
			TERM2,
			UNARYEXPR,
			FACTOR,
			LVALUE,
			EMPTY // Auxiliar pro sintatico
		};

		public struct Simbolo
		{
			public NonTerminal Nonterminal { get; set; }
			public Token.Terminals Terminal { get; set; }

			public override string ToString()
			{
				return Nonterminal.ToString() + "," + Terminal.ToString();
			}
		}

		public Dictionary<NonTerminal, List<List<Simbolo>>> Producoes { get; set; }
		public Dictionary<Simbolo, List<Simbolo>> ReferenceTable { get; set; }
		public Dictionary<NonTerminal, List<Token.Terminals>> first;
		public Dictionary<NonTerminal, HashSet<Token.Terminals>> Follows { get; set; }

		public Sintatico()
		{
			Producoes = new Dictionary<NonTerminal, List<List<Simbolo>>>();
			InitProd();
			first = new Dictionary<NonTerminal, List<Token.Terminals>>();
			Follows = new Dictionary<NonTerminal, HashSet<Token.Terminals>>();
			GenFollows();
			//printFirst();
			//printFollow();
			ReferenceTable = new Dictionary<Simbolo, List<Simbolo>>();
			InitRefTable();

		}

		private void InitProd()
		{
			List<Simbolo> lp;
			List<List<Simbolo>> llp;

			foreach (NonTerminal nt in Enum.GetValues(typeof(NonTerminal)))
			{
				llp = new List<List<Simbolo>>();
				switch (nt)
				{
					// PROGRAM -> STATEMENT | FUNCLIST | &
					case NonTerminal.PROGRAM:
						lp = new List<Simbolo>();
						lp.Clear();
						lp.Add(new Simbolo { Nonterminal = NonTerminal.STATEMENT, Terminal = Token.Terminals.EMPTY });
						llp.Add(lp);
						lp = new List<Simbolo>();
						lp.Clear();
						lp.Add(new Simbolo { Nonterminal = NonTerminal.FUNCLIST, Terminal = Token.Terminals.EMPTY });
						llp.Add(lp);
						lp = new List<Simbolo>();
						lp.Clear();
						lp.Add(new Simbolo { Nonterminal = NonTerminal.EMPTY, Terminal = Token.Terminals.EMPTY });
						llp.Add(lp);
						break;
					// FUNCLIST -> FUNCDEF FUNCLIST2
					case NonTerminal.FUNCLIST:
						lp = new List<Simbolo>();
						lp.Clear();
						lp.Add(new Simbolo { Nonterminal = NonTerminal.FUNCDEF, Terminal = Token.Terminals.EMPTY });
						lp.Add(new Simbolo { Nonterminal = NonTerminal.FUNCLIST2, Terminal = Token.Terminals.EMPTY });
						llp.Add(lp);
						break;
					// FUNCLIST2 -> FUNCLIST | &
					case NonTerminal.FUNCLIST2:
						lp = new List<Simbolo>();
						lp.Clear();
						lp.Add(new Simbolo { Nonterminal = NonTerminal.FUNCLIST, Terminal = Token.Terminals.EMPTY });
						llp.Add(lp);
						lp = new List<Simbolo>();
						lp.Clear();
						lp.Add(new Simbolo { Nonterminal = NonTerminal.EMPTY, Terminal = Token.Terminals.EMPTY });
						llp.Add(lp);
						break;
					// FUNCDEF -> def ident ( PARAMLIST ) { STATELIST }
					case NonTerminal.FUNCDEF:
						lp = new List<Simbolo>();
						lp.Clear();
						lp.Add(new Simbolo { Nonterminal = NonTerminal.EMPTY, Terminal = Token.Terminals.DEF });
						lp.Add(new Simbolo { Nonterminal = NonTerminal.EMPTY, Terminal = Token.Terminals.IDENT });
						lp.Add(new Simbolo { Nonterminal = NonTerminal.EMPTY, Terminal = Token.Terminals.OPENPARENT });
						lp.Add(new Simbolo { Nonterminal = NonTerminal.PARAMLIST, Terminal = Token.Terminals.EMPTY });
						lp.Add(new Simbolo { Nonterminal = NonTerminal.EMPTY, Terminal = Token.Terminals.CLOSEPARENT });
						lp.Add(new Simbolo { Nonterminal = NonTerminal.EMPTY, Terminal = Token.Terminals.OPENBRKT });
						lp.Add(new Simbolo { Nonterminal = NonTerminal.STATELIST, Terminal = Token.Terminals.EMPTY });
						lp.Add(new Simbolo { Nonterminal = NonTerminal.EMPTY, Terminal = Token.Terminals.CLOSEBRKT });
						llp.Add(lp);
						break;
					// PARAMLIST -> int ident PARAMLIST2 | float ident PARAMLIST2 | string ident PARAMLIST2 | &
					case NonTerminal.PARAMLIST:
						lp = new List<Simbolo>();
						lp.Clear();
						lp.Add(new Simbolo { Nonterminal = NonTerminal.EMPTY, Terminal = Token.Terminals.EMPTY });
						llp.Add(lp);
						lp = new List<Simbolo>();
						lp.Clear();
						lp.Add(new Simbolo { Nonterminal = NonTerminal.EMPTY, Terminal = Token.Terminals.INTEGER_T });
						lp.Add(new Simbolo { Nonterminal = NonTerminal.EMPTY, Terminal = Token.Terminals.IDENT });
						lp.Add(new Simbolo { Nonterminal = NonTerminal.PARAMLIST2, Terminal = Token.Terminals.EMPTY });
						llp.Add(lp);
						lp = new List<Simbolo>();
						lp.Clear();
						lp.Add(new Simbolo { Nonterminal = NonTerminal.EMPTY, Terminal = Token.Terminals.FLOAT_T });
						lp.Add(new Simbolo { Nonterminal = NonTerminal.EMPTY, Terminal = Token.Terminals.IDENT });
						lp.Add(new Simbolo { Nonterminal = NonTerminal.PARAMLIST2, Terminal = Token.Terminals.EMPTY });
						llp.Add(lp);
						lp = new List<Simbolo>();
						lp.Clear();
						lp.Add(new Simbolo { Nonterminal = NonTerminal.EMPTY, Terminal = Token.Terminals.STRING_T });
						lp.Add(new Simbolo { Nonterminal = NonTerminal.EMPTY, Terminal = Token.Terminals.IDENT });
						lp.Add(new Simbolo { Nonterminal = NonTerminal.PARAMLIST2, Terminal = Token.Terminals.EMPTY });
						llp.Add(lp);
						break;
					// PARAMLIST2 -> , PARAMLIST | &
					case NonTerminal.PARAMLIST2:
						lp = new List<Simbolo>();
						lp.Clear();
						lp.Add(new Simbolo { Nonterminal = NonTerminal.EMPTY, Terminal = Token.Terminals.EMPTY });
						llp.Add(lp);
						lp = new List<Simbolo>();
						lp.Clear();
						lp.Add(new Simbolo { Nonterminal = NonTerminal.EMPTY, Terminal = Token.Terminals.COMMA });
						lp.Add(new Simbolo { Nonterminal = NonTerminal.PARAMLIST, Terminal = Token.Terminals.EMPTY });
						llp.Add(lp);
						break;
					// STATEMENT -> VARDECL; | ATRIBSTAT; | PRINTSTAT; | READSTAT; | RETURNSTAT; | IFSTAT; | FORSTAT; | {STATELIST}| break ; | ;
					case NonTerminal.STATEMENT:
						lp = new List<Simbolo>();
						lp.Clear();
						lp.Add(new Simbolo { Nonterminal = NonTerminal.EMPTY, Terminal = Token.Terminals.OPENBRACE });
						lp.Add(new Simbolo { Nonterminal = NonTerminal.STATELIST, Terminal = Token.Terminals.EMPTY });
						lp.Add(new Simbolo { Nonterminal = NonTerminal.EMPTY, Terminal = Token.Terminals.CLOSEBRACE });
						llp.Add(lp);
						lp = new List<Simbolo>();
						lp.Clear();
						lp.Add(new Simbolo { Nonterminal = NonTerminal.EMPTY, Terminal = Token.Terminals.BREAK });
						lp.Add(new Simbolo { Nonterminal = NonTerminal.EMPTY, Terminal = Token.Terminals.SEPARATOR });
						llp.Add(lp);
						lp = new List<Simbolo>();
						lp.Clear();
						lp.Add(new Simbolo { Nonterminal = NonTerminal.VARDECL, Terminal = Token.Terminals.EMPTY });
						lp.Add(new Simbolo { Nonterminal = NonTerminal.EMPTY, Terminal = Token.Terminals.SEPARATOR });
						llp.Add(lp);
						lp = new List<Simbolo>();
						lp.Clear();
						lp.Add(new Simbolo { Nonterminal = NonTerminal.ATRIBSTAT, Terminal = Token.Terminals.EMPTY });
						lp.Add(new Simbolo { Nonterminal = NonTerminal.EMPTY, Terminal = Token.Terminals.SEPARATOR });
						llp.Add(lp);
						lp = new List<Simbolo>();
						lp.Clear();
						lp.Add(new Simbolo { Nonterminal = NonTerminal.PRINTSTAT, Terminal = Token.Terminals.EMPTY });
						lp.Add(new Simbolo { Nonterminal = NonTerminal.EMPTY, Terminal = Token.Terminals.SEPARATOR });
						llp.Add(lp);
						lp = new List<Simbolo>();
						lp.Clear();
						lp.Add(new Simbolo { Nonterminal = NonTerminal.READSTAT, Terminal = Token.Terminals.EMPTY });
						lp.Add(new Simbolo { Nonterminal = NonTerminal.EMPTY, Terminal = Token.Terminals.SEPARATOR });
						llp.Add(lp);
						lp = new List<Simbolo>();
						lp.Clear();
						lp.Add(new Simbolo { Nonterminal = NonTerminal.RETURNSTAT, Terminal = Token.Terminals.EMPTY });
						lp.Add(new Simbolo { Nonterminal = NonTerminal.EMPTY, Terminal = Token.Terminals.SEPARATOR });
						llp.Add(lp);
						lp = new List<Simbolo>();
						lp.Clear();
						lp.Add(new Simbolo { Nonterminal = NonTerminal.IFSTAT, Terminal = Token.Terminals.EMPTY });
						llp.Add(lp);
						lp = new List<Simbolo>();
						lp.Clear();
						lp.Add(new Simbolo { Nonterminal = NonTerminal.FORSTAT, Terminal = Token.Terminals.EMPTY });
						llp.Add(lp);
						lp = new List<Simbolo>();
						lp.Clear();
						lp.Add(new Simbolo { Nonterminal = NonTerminal.EMPTY, Terminal = Token.Terminals.SEPARATOR });
						llp.Add(lp);
						break;
					// VARDECL -> int ident VAR2 | float ident VAR2 | string ident VAR2
					case NonTerminal.VARDECL:
						lp = new List<Simbolo>();
						lp.Clear();
						lp.Add(new Simbolo { Nonterminal = NonTerminal.EMPTY, Terminal = Token.Terminals.INTEGER_T });
						lp.Add(new Simbolo { Nonterminal = NonTerminal.EMPTY, Terminal = Token.Terminals.IDENT });
						lp.Add(new Simbolo { Nonterminal = NonTerminal.VAR2, Terminal = Token.Terminals.EMPTY });
						llp.Add(lp);
						lp = new List<Simbolo>();
						lp.Clear();
						lp.Add(new Simbolo { Nonterminal = NonTerminal.EMPTY, Terminal = Token.Terminals.FLOAT_T });
						lp.Add(new Simbolo { Nonterminal = NonTerminal.EMPTY, Terminal = Token.Terminals.IDENT });
						lp.Add(new Simbolo { Nonterminal = NonTerminal.VAR2, Terminal = Token.Terminals.EMPTY });
						llp.Add(lp);
						lp = new List<Simbolo>();
						lp.Clear();
						lp.Add(new Simbolo { Nonterminal = NonTerminal.EMPTY, Terminal = Token.Terminals.STRING_T });
						lp.Add(new Simbolo { Nonterminal = NonTerminal.EMPTY, Terminal = Token.Terminals.IDENT });
						lp.Add(new Simbolo { Nonterminal = NonTerminal.VAR2, Terminal = Token.Terminals.EMPTY });
						llp.Add(lp);
						break;
					// VAR2 -> [int_constant] VAR2 | &
					case NonTerminal.VAR2:
						lp = new List<Simbolo>();
						lp.Clear();
						lp.Add(new Simbolo { Nonterminal = NonTerminal.EMPTY, Terminal = Token.Terminals.OPENBRKT });
						lp.Add(new Simbolo { Nonterminal = NonTerminal.EMPTY, Terminal = Token.Terminals.INT });
						lp.Add(new Simbolo { Nonterminal = NonTerminal.EMPTY, Terminal = Token.Terminals.CLOSEBRKT });
						lp.Add(new Simbolo { Nonterminal = NonTerminal.VAR2, Terminal = Token.Terminals.EMPTY });
						llp.Add(lp);
						lp = new List<Simbolo>();
						lp.Clear();
						lp.Add(new Simbolo { Nonterminal = NonTerminal.EMPTY, Terminal = Token.Terminals.EMPTY });
						llp.Add(lp);
						break;
					// ATRIBSTAT -> LVALUE = ATREXP
					case NonTerminal.ATRIBSTAT:
						lp = new List<Simbolo>();
						lp.Clear();
						lp.Add(new Simbolo { Nonterminal = NonTerminal.LVALUE, Terminal = Token.Terminals.EMPTY });
						lp.Add(new Simbolo { Nonterminal = NonTerminal.EMPTY, Terminal = Token.Terminals.ASSERT });
						lp.Add(new Simbolo { Nonterminal = NonTerminal.ATREXP, Terminal = Token.Terminals.EMPTY });
						llp.Add(lp);
						break;
					// ATREXP -> EXPRESSION | ALLOCEXPRESSION | FUNCCALL
					case NonTerminal.ATREXP:
						lp = new List<Simbolo>();
						lp.Clear();
						lp.Add(new Simbolo { Nonterminal = NonTerminal.EXPRESSION, Terminal = Token.Terminals.EMPTY });
						llp.Add(lp);
						lp = new List<Simbolo>();
						lp.Clear();
						lp.Add(new Simbolo { Nonterminal = NonTerminal.ALLOCEXPRESSION, Terminal = Token.Terminals.EMPTY });
						llp.Add(lp);
						lp = new List<Simbolo>();
						lp.Clear();
						lp.Add(new Simbolo { Nonterminal = NonTerminal.FUNCCALL, Terminal = Token.Terminals.EMPTY });
						llp.Add(lp);
						break;
					// FUNCCALL -> ident ( PARAMLISTCALL )
					case NonTerminal.FUNCCALL:
						lp = new List<Simbolo>();
						lp.Clear();
						lp.Add(new Simbolo { Nonterminal = NonTerminal.EMPTY, Terminal = Token.Terminals.IDENT });
						lp.Add(new Simbolo { Nonterminal = NonTerminal.EMPTY, Terminal = Token.Terminals.OPENPARENT });
						lp.Add(new Simbolo { Nonterminal = NonTerminal.PARAMLISTCALL, Terminal = Token.Terminals.EMPTY });
						lp.Add(new Simbolo { Nonterminal = NonTerminal.EMPTY, Terminal = Token.Terminals.CLOSEPARENT });
						llp.Add(lp);
						break;
					// PARAMLISTCALL -> ident PARAMLISTCALL2 | &
					case NonTerminal.PARAMLISTCALL:
						lp = new List<Simbolo>();
						lp.Clear();
						lp.Add(new Simbolo { Nonterminal = NonTerminal.EMPTY, Terminal = Token.Terminals.IDENT });
						lp.Add(new Simbolo { Nonterminal = NonTerminal.PARAMLISTCALL2, Terminal = Token.Terminals.EMPTY });
						llp.Add(lp);
						lp = new List<Simbolo>();
						lp.Clear();
						lp.Add(new Simbolo { Nonterminal = NonTerminal.EMPTY, Terminal = Token.Terminals.EMPTY });
						llp.Add(lp);
						break;
					// PARAMLISTCALL2 -> , PARAMLISTCALL | &
					case NonTerminal.PARAMLISTCALL2:
						lp = new List<Simbolo>();
						lp.Clear();
						lp.Add(new Simbolo { Nonterminal = NonTerminal.EMPTY, Terminal = Token.Terminals.COMMA });
						lp.Add(new Simbolo { Nonterminal = NonTerminal.PARAMLISTCALL, Terminal = Token.Terminals.EMPTY });
						llp.Add(lp);
						lp = new List<Simbolo>();
						lp.Clear();
						lp.Add(new Simbolo { Nonterminal = NonTerminal.EMPTY, Terminal = Token.Terminals.EMPTY });
						llp.Add(lp);
						break;
					// PRINTSTAT -> print EXPRESSION
					case NonTerminal.PRINTSTAT:
						lp = new List<Simbolo>();
						lp.Clear();
						lp.Add(new Simbolo { Nonterminal = NonTerminal.EMPTY, Terminal = Token.Terminals.PRINT });
						lp.Add(new Simbolo { Nonterminal = NonTerminal.EXPRESSION, Terminal = Token.Terminals.EMPTY });
						llp.Add(lp);
						break;
					// READSTAT -> read LVALUE
					case NonTerminal.READSTAT:
						lp = new List<Simbolo>();
						lp.Clear();
						lp.Add(new Simbolo { Nonterminal = NonTerminal.EMPTY, Terminal = Token.Terminals.READ });
						lp.Add(new Simbolo { Nonterminal = NonTerminal.LVALUE, Terminal = Token.Terminals.EMPTY });
						llp.Add(lp);
						break;
					// RETURNSTAT -> return
					case NonTerminal.RETURNSTAT:
						lp = new List<Simbolo>();
						lp.Clear();
						lp.Add(new Simbolo { Nonterminal = NonTerminal.EMPTY, Terminal = Token.Terminals.RETURN });
						llp.Add(lp);
						break;
					// IFSTAT -> if( EXPRESSION ) STATEMENT IF2
					case NonTerminal.IFSTAT:
						lp = new List<Simbolo>();
						lp.Clear();
						lp.Add(new Simbolo { Nonterminal = NonTerminal.EMPTY, Terminal = Token.Terminals.IF });
						lp.Add(new Simbolo { Nonterminal = NonTerminal.EMPTY, Terminal = Token.Terminals.OPENPARENT });
						lp.Add(new Simbolo { Nonterminal = NonTerminal.EXPRESSION, Terminal = Token.Terminals.EMPTY });
						lp.Add(new Simbolo { Nonterminal = NonTerminal.EMPTY, Terminal = Token.Terminals.CLOSEPARENT });
						lp.Add(new Simbolo { Nonterminal = NonTerminal.STATEMENT, Terminal = Token.Terminals.EMPTY });
						lp.Add(new Simbolo { Nonterminal = NonTerminal.IF2, Terminal = Token.Terminals.EMPTY });
						llp.Add(lp);
						break;
					// IF2 -> else STATEMENT | e
					case NonTerminal.IF2:
						lp = new List<Simbolo>();
						lp.Clear();
						lp.Add(new Simbolo { Nonterminal = NonTerminal.EMPTY, Terminal = Token.Terminals.ELSE });
						lp.Add(new Simbolo { Nonterminal = NonTerminal.STATEMENT, Terminal = Token.Terminals.EMPTY });
						llp.Add(lp);
						lp = new List<Simbolo>();
						lp.Clear();
						lp.Add(new Simbolo { Nonterminal = NonTerminal.EMPTY, Terminal = Token.Terminals.EMPTY });
						llp.Add(lp);
						break;
					// FORSTAT -> for( ATRIBSTAT; NUMEXPRESSION; ATRIBSTAT) STATEMENT
					case NonTerminal.FORSTAT:
						lp = new List<Simbolo>();
						lp.Clear();
						lp.Add(new Simbolo { Nonterminal = NonTerminal.EMPTY, Terminal = Token.Terminals.FOR });
						lp.Add(new Simbolo { Nonterminal = NonTerminal.EMPTY, Terminal = Token.Terminals.OPENPARENT });
						lp.Add(new Simbolo { Nonterminal = NonTerminal.ATRIBSTAT, Terminal = Token.Terminals.EMPTY });
						lp.Add(new Simbolo { Nonterminal = NonTerminal.EMPTY, Terminal = Token.Terminals.SEPARATOR });
						lp.Add(new Simbolo { Nonterminal = NonTerminal.EXPRESSION, Terminal = Token.Terminals.EMPTY });
						lp.Add(new Simbolo { Nonterminal = NonTerminal.EMPTY, Terminal = Token.Terminals.SEPARATOR });
						lp.Add(new Simbolo { Nonterminal = NonTerminal.ATRIBSTAT, Terminal = Token.Terminals.EMPTY });
						lp.Add(new Simbolo { Nonterminal = NonTerminal.EMPTY, Terminal = Token.Terminals.CLOSEPARENT });
						lp.Add(new Simbolo { Nonterminal = NonTerminal.STATEMENT, Terminal = Token.Terminals.EMPTY });
						llp.Add(lp);
						break;
					// STATELIST -> STATEMENT STATE2
					case NonTerminal.STATELIST:
						lp = new List<Simbolo>();
						lp.Clear();
						lp.Add(new Simbolo { Nonterminal = NonTerminal.STATEMENT, Terminal = Token.Terminals.EMPTY });
						lp.Add(new Simbolo { Nonterminal = NonTerminal.STATE2, Terminal = Token.Terminals.EMPTY });
						llp.Add(lp);
						break;
					// STATE2 -> STATELIST | &
					case NonTerminal.STATE2:
						lp = new List<Simbolo>();
						lp.Clear();
						lp.Add(new Simbolo { Nonterminal = NonTerminal.STATELIST, Terminal = Token.Terminals.EMPTY });
						llp.Add(lp);
						lp = new List<Simbolo>();
						lp.Clear();
						lp.Add(new Simbolo { Nonterminal = NonTerminal.EMPTY, Terminal = Token.Terminals.EMPTY });
						llp.Add(lp);
						break;
					// ALLOCEXPRESSION -> new ALLOC2
					case NonTerminal.ALLOCEXPRESSION:
						lp = new List<Simbolo>();
						lp.Clear();
						lp.Add(new Simbolo { Nonterminal = NonTerminal.EMPTY, Terminal = Token.Terminals.NEW });
						lp.Add(new Simbolo { Nonterminal = NonTerminal.ALLOC2, Terminal = Token.Terminals.EMPTY });
						llp.Add(lp);
						break;
					// ALLOC2 -> int [ NUMEXPRESSION ] ALLOC3 | float [ NUMEXPRESSION ] ALLOC3 | string [ NUMEXPRESSION ] ALLOC3
					case NonTerminal.ALLOC2:
						lp = new List<Simbolo>();
						lp.Clear();
						lp.Add(new Simbolo { Nonterminal = NonTerminal.EMPTY, Terminal = Token.Terminals.INTEGER_T });
						lp.Add(new Simbolo { Nonterminal = NonTerminal.EMPTY, Terminal = Token.Terminals.OPENBRKT });
						lp.Add(new Simbolo { Nonterminal = NonTerminal.NUMEXPRESSION, Terminal = Token.Terminals.EMPTY });
						lp.Add(new Simbolo { Nonterminal = NonTerminal.EMPTY, Terminal = Token.Terminals.CLOSEBRKT });
						lp.Add(new Simbolo { Nonterminal = NonTerminal.ALLOC3, Terminal = Token.Terminals.EMPTY });
						llp.Add(lp);
						lp = new List<Simbolo>();
						lp.Clear();
						lp.Add(new Simbolo { Nonterminal = NonTerminal.EMPTY, Terminal = Token.Terminals.FLOAT_T });
						lp.Add(new Simbolo { Nonterminal = NonTerminal.EMPTY, Terminal = Token.Terminals.OPENBRKT });
						lp.Add(new Simbolo { Nonterminal = NonTerminal.NUMEXPRESSION, Terminal = Token.Terminals.EMPTY });
						lp.Add(new Simbolo { Nonterminal = NonTerminal.EMPTY, Terminal = Token.Terminals.CLOSEBRKT });
						lp.Add(new Simbolo { Nonterminal = NonTerminal.ALLOC3, Terminal = Token.Terminals.EMPTY });
						llp.Add(lp);
						lp = new List<Simbolo>();
						lp.Clear();
						lp.Add(new Simbolo { Nonterminal = NonTerminal.EMPTY, Terminal = Token.Terminals.STRING_T });
						lp.Add(new Simbolo { Nonterminal = NonTerminal.EMPTY, Terminal = Token.Terminals.OPENBRKT });
						lp.Add(new Simbolo { Nonterminal = NonTerminal.NUMEXPRESSION, Terminal = Token.Terminals.EMPTY });
						lp.Add(new Simbolo { Nonterminal = NonTerminal.EMPTY, Terminal = Token.Terminals.CLOSEBRKT });
						lp.Add(new Simbolo { Nonterminal = NonTerminal.ALLOC3, Terminal = Token.Terminals.EMPTY });
						llp.Add(lp);
						break;
					// ALLOC3 -> [ NUMEXPRESSION ] ALLOC3 | e
					case NonTerminal.ALLOC3:
						lp = new List<Simbolo>();
						lp.Clear();
						lp.Add(new Simbolo { Nonterminal = NonTerminal.EMPTY, Terminal = Token.Terminals.OPENBRKT });
						lp.Add(new Simbolo { Nonterminal = NonTerminal.NUMEXPRESSION, Terminal = Token.Terminals.EMPTY });
						lp.Add(new Simbolo { Nonterminal = NonTerminal.EMPTY, Terminal = Token.Terminals.CLOSEBRKT });
						lp.Add(new Simbolo { Nonterminal = NonTerminal.ALLOC3, Terminal = Token.Terminals.EMPTY });
						llp.Add(lp);
						lp = new List<Simbolo>();
						lp.Clear();
						lp.Add(new Simbolo { Nonterminal = NonTerminal.EMPTY, Terminal = Token.Terminals.EMPTY });
						llp.Add(lp);
						break;
					// EXPRESSION -> NUMEXPRESSION EXP2
					case NonTerminal.EXPRESSION:
						lp = new List<Simbolo>();
						lp.Clear();
						lp.Add(new Simbolo { Nonterminal = NonTerminal.NUMEXPRESSION, Terminal = Token.Terminals.EMPTY });
						lp.Add(new Simbolo { Nonterminal = NonTerminal.EXP2, Terminal = Token.Terminals.EMPTY });
						llp.Add(lp);
						break;
					// EXP2 -> < NUMEXPRESSION | > NUMEXPRESSION | <= NUMEXPRESSION | >= NUMEXPRESSION | == NUMEXPRESSION | ! = NUMEXPRESSION | e
					case NonTerminal.EXP2:
						lp = new List<Simbolo>();
						lp.Clear();
						lp.Add(new Simbolo { Nonterminal = NonTerminal.EMPTY, Terminal = Token.Terminals.LT });
						lp.Add(new Simbolo { Nonterminal = NonTerminal.NUMEXPRESSION, Terminal = Token.Terminals.EMPTY });
						llp.Add(lp);
						lp = new List<Simbolo>();
						lp.Clear();
						lp.Add(new Simbolo { Nonterminal = NonTerminal.EMPTY, Terminal = Token.Terminals.GT });
						lp.Add(new Simbolo { Nonterminal = NonTerminal.NUMEXPRESSION, Terminal = Token.Terminals.EMPTY });
						llp.Add(lp);
						lp = new List<Simbolo>();
						lp.Clear();
						lp.Add(new Simbolo { Nonterminal = NonTerminal.EMPTY, Terminal = Token.Terminals.LE });
						lp.Add(new Simbolo { Nonterminal = NonTerminal.NUMEXPRESSION, Terminal = Token.Terminals.EMPTY });
						llp.Add(lp);
						lp = new List<Simbolo>();
						lp.Clear();
						lp.Add(new Simbolo { Nonterminal = NonTerminal.EMPTY, Terminal = Token.Terminals.GE });
						lp.Add(new Simbolo { Nonterminal = NonTerminal.NUMEXPRESSION, Terminal = Token.Terminals.EMPTY });
						llp.Add(lp);
						lp = new List<Simbolo>();
						lp.Clear();
						lp.Add(new Simbolo { Nonterminal = NonTerminal.EMPTY, Terminal = Token.Terminals.EQ });
						lp.Add(new Simbolo { Nonterminal = NonTerminal.NUMEXPRESSION, Terminal = Token.Terminals.EMPTY });
						llp.Add(lp);
						lp = new List<Simbolo>();
						lp.Clear();
						lp.Add(new Simbolo { Nonterminal = NonTerminal.EMPTY, Terminal = Token.Terminals.NE });
						lp.Add(new Simbolo { Nonterminal = NonTerminal.NUMEXPRESSION, Terminal = Token.Terminals.EMPTY });
						llp.Add(lp);
						lp = new List<Simbolo>();
						lp.Clear();
						lp.Add(new Simbolo { Nonterminal = NonTerminal.EMPTY, Terminal = Token.Terminals.EMPTY });
						llp.Add(lp);
						break;
					// NUMEXPRESSION -> TERM NUM2
					case NonTerminal.NUMEXPRESSION:
						lp = new List<Simbolo>();
						lp.Clear();
						lp.Add(new Simbolo { Nonterminal = NonTerminal.TERM, Terminal = Token.Terminals.EMPTY });
						lp.Add(new Simbolo { Nonterminal = NonTerminal.NUM2, Terminal = Token.Terminals.EMPTY });
						llp.Add(lp);
						break;
					// NUM2 -> + TERM NUM2| - TERM NUM2| e
					case NonTerminal.NUM2:
						lp = new List<Simbolo>();
						lp.Clear();
						lp.Add(new Simbolo { Nonterminal = NonTerminal.EMPTY, Terminal = Token.Terminals.ADD });
						lp.Add(new Simbolo { Nonterminal = NonTerminal.TERM, Terminal = Token.Terminals.EMPTY });
						lp.Add(new Simbolo { Nonterminal = NonTerminal.NUM2, Terminal = Token.Terminals.EMPTY });
						llp.Add(lp);
						lp = new List<Simbolo>();
						lp.Clear();
						lp.Add(new Simbolo { Nonterminal = NonTerminal.EMPTY, Terminal = Token.Terminals.MINUS });
						lp.Add(new Simbolo { Nonterminal = NonTerminal.TERM, Terminal = Token.Terminals.EMPTY });
						lp.Add(new Simbolo { Nonterminal = NonTerminal.NUM2, Terminal = Token.Terminals.EMPTY });
						llp.Add(lp);
						lp = new List<Simbolo>();
						lp.Clear();
						lp.Add(new Simbolo { Nonterminal = NonTerminal.EMPTY, Terminal = Token.Terminals.EMPTY });
						llp.Add(lp);
						break;
					// TERM -> UNARYEXPR TERM2
					case NonTerminal.TERM:
						lp = new List<Simbolo>();
						lp.Clear();
						lp.Add(new Simbolo { Nonterminal = NonTerminal.UNARYEXPR, Terminal = Token.Terminals.EMPTY });
						lp.Add(new Simbolo { Nonterminal = NonTerminal.TERM2, Terminal = Token.Terminals.EMPTY });
						llp.Add(lp);
						break;
					// TERM2 -> ∗ UNARYEXPR TERM2| \ UNARYEXPR TERM2 | % UNARYEXPR TERM2 | e
					case NonTerminal.TERM2:
						lp = new List<Simbolo>();
						lp.Clear();
						lp.Add(new Simbolo { Nonterminal = NonTerminal.EMPTY, Terminal = Token.Terminals.MULTIPLY });
						lp.Add(new Simbolo { Nonterminal = NonTerminal.UNARYEXPR, Terminal = Token.Terminals.EMPTY });
						lp.Add(new Simbolo { Nonterminal = NonTerminal.TERM2, Terminal = Token.Terminals.EMPTY });
						llp.Add(lp);
						lp = new List<Simbolo>();
						lp.Clear();
						lp.Add(new Simbolo { Nonterminal = NonTerminal.EMPTY, Terminal = Token.Terminals.DIVIDE });
						lp.Add(new Simbolo { Nonterminal = NonTerminal.UNARYEXPR, Terminal = Token.Terminals.EMPTY });
						lp.Add(new Simbolo { Nonterminal = NonTerminal.TERM2, Terminal = Token.Terminals.EMPTY });
						llp.Add(lp);
						lp = new List<Simbolo>();
						lp.Clear();
						lp.Add(new Simbolo { Nonterminal = NonTerminal.EMPTY, Terminal = Token.Terminals.MODULUS });
						lp.Add(new Simbolo { Nonterminal = NonTerminal.UNARYEXPR, Terminal = Token.Terminals.EMPTY });
						lp.Add(new Simbolo { Nonterminal = NonTerminal.TERM2, Terminal = Token.Terminals.EMPTY });
						llp.Add(lp);
						lp = new List<Simbolo>();
						lp.Clear();
						lp.Add(new Simbolo { Nonterminal = NonTerminal.EMPTY, Terminal = Token.Terminals.EMPTY });
						llp.Add(lp);
						break;
					// UNARYEXPR -> + FACTOR | - FACTOR | FACTOR
					case NonTerminal.UNARYEXPR:
						lp = new List<Simbolo>();
						lp.Clear();
						lp.Add(new Simbolo { Nonterminal = NonTerminal.EMPTY, Terminal = Token.Terminals.ADD });
						lp.Add(new Simbolo { Nonterminal = NonTerminal.FACTOR, Terminal = Token.Terminals.EMPTY });
						llp.Add(lp);
						lp = new List<Simbolo>();
						lp.Clear();
						lp.Add(new Simbolo { Nonterminal = NonTerminal.EMPTY, Terminal = Token.Terminals.MINUS });
						lp.Add(new Simbolo { Nonterminal = NonTerminal.FACTOR, Terminal = Token.Terminals.EMPTY });
						llp.Add(lp);
						lp = new List<Simbolo>();
						lp.Clear();
						lp.Add(new Simbolo { Nonterminal = NonTerminal.FACTOR, Terminal = Token.Terminals.EMPTY });
						llp.Add(lp);
						break;
					// FACTOR -> int_constant | float_constant | string_constant | null | LVALUE |( EXPRESSION )
					case NonTerminal.FACTOR:
						lp = new List<Simbolo>();
						lp.Clear();
						lp.Add(new Simbolo { Nonterminal = NonTerminal.EMPTY, Terminal = Token.Terminals.INT });
						llp.Add(lp);
						lp = new List<Simbolo>();
						lp.Clear();
						lp.Add(new Simbolo { Nonterminal = NonTerminal.EMPTY, Terminal = Token.Terminals.FLT });
						llp.Add(lp);
						lp = new List<Simbolo>();
						lp.Clear();
						lp.Add(new Simbolo { Nonterminal = NonTerminal.EMPTY, Terminal = Token.Terminals.STR });
						llp.Add(lp);
						lp = new List<Simbolo>();
						lp.Clear();
						lp.Add(new Simbolo { Nonterminal = NonTerminal.EMPTY, Terminal = Token.Terminals.NULL });
						llp.Add(lp);
						lp = new List<Simbolo>();
						lp.Clear();
						lp.Add(new Simbolo { Nonterminal = NonTerminal.LVALUE, Terminal = Token.Terminals.EMPTY });
						llp.Add(lp);
						lp = new List<Simbolo>();
						lp.Clear();
						lp.Add(new Simbolo { Nonterminal = NonTerminal.EMPTY, Terminal = Token.Terminals.OPENPARENT });
						lp.Add(new Simbolo { Nonterminal = NonTerminal.NUMEXPRESSION, Terminal = Token.Terminals.EMPTY });
						lp.Add(new Simbolo { Nonterminal = NonTerminal.EMPTY, Terminal = Token.Terminals.CLOSEPARENT });
						llp.Add(lp);
						break;
					// LVALUE -> ident ALLOC3
					case NonTerminal.LVALUE:
						lp = new List<Simbolo>();
						lp.Clear();
						lp.Add(new Simbolo { Nonterminal = NonTerminal.EMPTY, Terminal = Token.Terminals.IDENT });
						lp.Add(new Simbolo { Nonterminal = NonTerminal.ALLOC3, Terminal = Token.Terminals.EMPTY });
						llp.Add(lp);
						break;
					default:
						break;
				}

				Producoes.Add(nt, llp);
			}
		}
		private void InitRefTable()
		{
			List<List<Simbolo>> llp;
			//Token.Terminals t;
			foreach (NonTerminal nt in Enum.GetValues(typeof(NonTerminal)))
			{
				bool hasEpson = false;
				if (nt.Equals(NonTerminal.EMPTY))//pois nao queremos calcular o First do Empty
					continue;
				llp = Producoes[nt];
				//Cada lp eh uma producao NT -> X
				foreach (List<Simbolo> lp in llp)
				{
					List<Token.Terminals> lt = GetFirstFromProd(lp);
					foreach (Token.Terminals t in lt)
					{
						if (t.Equals(Token.Terminals.EMPTY))
						{
							hasEpson = true;
							continue;//pois epson nao entra na tabela de analise
						}

						if (!ReferenceTable.ContainsKey(new Simbolo { Nonterminal = nt, Terminal = t }))//so por enquanto
							ReferenceTable.Add(new Simbolo { Nonterminal = nt, Terminal = t }, lp);

					}
					if (hasEpson)
					{
						foreach (Token.Terminals t in Follows[nt]) //sao os terminais do Follow
						{
							List<Simbolo> lpEpson = new List<Simbolo>
							{
								new Simbolo { Nonterminal = NonTerminal.EMPTY, Terminal = Token.Terminals.EMPTY }
							};
							ReferenceTable.TryAdd(new Simbolo { Nonterminal = nt, Terminal = t }, lpEpson);
						}
					}

				}
			}
		}

		public void WriteOutput(List<Token.Tok> lt, StreamWriter sr)
		{

			// Read the stream to a string, and write the string to the console.
			foreach (var sy in ReferenceTable)
			{
				string prod = "";
				foreach (var pr in sy.Value)
				{
					if (pr.Nonterminal.Equals(NonTerminal.EMPTY))
						prod += pr.Terminal.ToString() + " ";
					else
						prod += pr.Nonterminal.ToString() + " ";
				}
				prod = prod.Replace("EMPTY", "ɛ");
				//sr.WriteLine("{0},{1}->{2}", sy.Key.Nonterminal, sy.Key.Terminal, prod);
			}

			if (PredictiveParser(lt, sr))
				sr.WriteLine("Entrada Aceita");
			else
			{
				sr.WriteLine("Entrada Nao Aceita");

				sr.WriteLine(message_error);
				sr.WriteLine("\nProduções possíveis:");
				sr.WriteLine(sentence_hint);
				sr.WriteLine("\nSímbolos esperados:");
				sr.WriteLine(first_expected);
			}


        }
       

        private List<Token.Terminals> GetFirstFromProd(List<Simbolo> lp)
		{
			List<Token.Terminals> lt = new List<Token.Terminals>();

			//Esse metodo usa o metodo First para pegar o First da producao a direita
			//ex: S -> ABc
			//vai retornar o first(A), se no first(A) tiver epson, inclui o first(B)
			if (lp[0].Nonterminal.Equals(NonTerminal.EMPTY))
			{
				lt.Add(lp[0].Terminal);
				return lt;
			}
			//else First
			foreach (Simbolo p in lp)
			{
				if (p.Nonterminal.Equals(NonTerminal.EMPTY))
				{
					lt.Add(p.Terminal);
					return lt;
				}
				//lt = fixedFirst(p.nonterminal);
				lt.AddRange(new List<Token.Terminals>(First(p.Nonterminal)).FindAll((x) => !lt.Contains(x)));
				// lt.AddRange(fixedFirst(p.nonterminal).FindAll((x) => !lt.Contains(x)));
				if (!First(p.Nonterminal).Contains(Token.Terminals.EMPTY))
					return lt;

			}

			//lt.Add(Token.Terminals.BASIC);//pra testar so.
			return lt;
		}

		private HashSet<Token.Terminals> First(NonTerminal nt)
		{
			HashSet<Token.Terminals> ret = new HashSet<Token.Terminals>();

			List<List<Simbolo>> llp = Producoes.GetValueOrDefault(nt);

			foreach (var lp in llp)
			{
				int i = -1;
				do
				{
					i++;
					if (lp[i].Terminal != Token.Terminals.EMPTY)
					{
						ret.Add(lp[i].Terminal);
						continue;
					}

					ret.UnionWith(First(lp[i].Nonterminal));

				} while (NextHasEmpty(lp[i].Nonterminal));

			}

			if (ret.Count == 0)
				ret.Add(Token.Terminals.EMPTY);

			return ret;
		}

		private bool NextHasEmpty(NonTerminal nt)
		{
			List<List<Simbolo>> llp = Producoes.GetValueOrDefault(nt);

			foreach (var lp in llp)
				if (lp.Contains(new Simbolo { Terminal = Token.Terminals.EMPTY, Nonterminal = NonTerminal.EMPTY }))
					return true;
			return false;
		}

		private void GenFollows()
		{
			Follows.Clear();
			foreach (NonTerminal nt in Enum.GetValues(typeof(NonTerminal)))
			{
				HashSet<Token.Terminals> lt = new HashSet<Token.Terminals>();
				if (nt.Equals(NonTerminal.PROGRAM))
					lt.Add(Token.Terminals.DOLLAR); // $ no primeiro elemento - Regra 1
				Follows.Add(nt, lt);
			}
			bool houveMudancas;

			do
			{
				houveMudancas = false;

				foreach (NonTerminal nt in Enum.GetValues(typeof(NonTerminal)))
				{
					foreach (var lp in Producoes.GetValueOrDefault(nt))
					{
						for (int i = 0; i < lp.Count; i++)
						{
							if (lp[i].Nonterminal != NonTerminal.EMPTY && i == lp.Count - 1) // Regra 3.1
							{
								if (!Follows.GetValueOrDefault(lp[i].Nonterminal).IsSupersetOf(Follows.GetValueOrDefault(nt)))
									houveMudancas = true;
								Follows.GetValueOrDefault(lp[i].Nonterminal).UnionWith(Follows.GetValueOrDefault(nt));
								break;
							}
							else if (lp[i].Nonterminal != NonTerminal.EMPTY && lp[i + 1].Nonterminal != NonTerminal.EMPTY)
							{
								HashSet<Token.Terminals> temp = First(lp[i + 1].Nonterminal);
								temp.Remove(Token.Terminals.EMPTY);

								if (!Follows.GetValueOrDefault(lp[i].Nonterminal).IsSupersetOf(temp))
									houveMudancas = true;
								Follows.GetValueOrDefault(lp[i].Nonterminal).UnionWith(temp); // Regra 2.2


								if (!Follows.GetValueOrDefault(lp[i].Nonterminal).IsSupersetOf(Follows.GetValueOrDefault(lp[i + 1].Nonterminal))) // Regra 3.2
									houveMudancas = true;
								Follows.GetValueOrDefault(lp[i].Nonterminal).UnionWith(Follows.GetValueOrDefault(lp[i + 1].Nonterminal));
							}
							else if (lp[i].Nonterminal != NonTerminal.EMPTY && lp[i + 1].Terminal != Token.Terminals.EMPTY) // Regra 2.1
							{
								if (Follows.GetValueOrDefault(lp[i].Nonterminal).Add(lp[i + 1].Terminal))
									houveMudancas = true;
							}

						}

					}
				}
			} while (houveMudancas);
		}

		public void PrintFirst()
		{
			foreach (NonTerminal nt in Enum.GetValues(typeof(NonTerminal)))
			{
				if (nt.Equals(NonTerminal.EMPTY))
					continue;
				HashSet<Token.Terminals> list = First(nt);
				string term = "";
				foreach (var t in list)
				{
					term += t.ToString() + ",";
				}
				term = term.Replace("EMPTY", "ɛ");
				Console.WriteLine("First({0}):{1}", nt.ToString(), term);
			}
		}

		public void PrintFollow()
		{
			Console.Write("\n\n\nFollows:\n");
			foreach (NonTerminal nt in Enum.GetValues(typeof(NonTerminal)))
			{
				if (nt.Equals(NonTerminal.EMPTY))
					continue;
				string term = "";
				foreach (var follows in Follows.GetValueOrDefault(nt))
				{
					term += follows.ToString() + ",";
				}
				term = term.Replace("EMPTY", "ɛ");
				Console.WriteLine("Follow({0}): {1}", nt.ToString(), term);
			}
		}

        public void setErrorMessage(NonTerminal nt, string t) {
            foreach (var productions in Producoes.GetValueOrDefault(nt))
            {

                string forma_sentencial = "";
                foreach (Token.Terminals final_t in GetFirstFromProd(productions))
                {
                    first_expected += final_t.ToString() + ",";
                }
                first_expected = first_expected.Replace("EMPTY", "ɛ");

                foreach (var symbols in productions)
                {
                    forma_sentencial += (
                        !symbols.Nonterminal.Equals(NonTerminal.EMPTY) ? symbols.Nonterminal.ToString() : (
                            !symbols.Terminal.Equals(Token.Terminals.EMPTY) ? symbols.Terminal.ToString() : "ɛ")
                    );
                    forma_sentencial += " ";
                }

                sentence_hint += nt + " -> " + forma_sentencial + "\n";
            }
            message_error = "Erro Sintático: Esperando: '" + nt + "' - Achado: '" + t + "'";
        }
		public bool PredictiveParser(List<Token.Tok> toks, StreamWriter sr)
		{
			string output = "";
			bool exit = true;
			Stack<Simbolo> pilha = new Stack<Simbolo>();
            listExpa = new List<string>();
            string signal = "", expa = "";
            bool recordExp = false;
            //sr.WriteLine("");
            //sr.WriteLine("Parser: (Pilha)");
            //sr.WriteLine(String.Format("|{0,-150}|{1,-150}|", "Stack", "Matched"));
            //sr.WriteLine(String.Format("|{0,150}|{0,150}|", "PROGRAM $"));
            toks = CheckDollarSign(toks);
			pilha.Push(new Simbolo { Nonterminal = NonTerminal.EMPTY, Terminal = Token.Terminals.DOLLAR });
			pilha.Push(new Simbolo { Nonterminal = NonTerminal.PROGRAM, Terminal = Token.Terminals.EMPTY });
			foreach (var token in toks)
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
                        //grava saida nalista
                        if (recordExp)
                        {
                            if (token.a.Equals(Token.Attributes.COMPARISON) || token.a.Equals(Token.Attributes.SEPARATOR)
                                || token.t.Equals(Token.Terminals.OPENBRACE) || token.t.Equals(Token.Terminals.ASSERT))
                            {
                                recordExp = false;
                                //insere na lista de exp
                                listExpa.Add(expa);
                                expa = "";//limpa para a prox expa
                            }
                            else
                            {
                                if (signal != "")
                                    expa = expa + signal;
                                else
                                    expa = expa + token.s;
                            }
                        }
                        output += token.s + " ";
						if (token.s == "$")
							return true;
					}
					else if (pilha.Peek().Nonterminal.Equals(NonTerminal.EMPTY))
                    {
                        NonTerminal nt = pilha.Pop().Nonterminal;
                        //terminal diferente da entrada
                        pilha.Pop();
						output += token.s + " ";
						exit = false;
                        setErrorMessage(nt, token.s);


                    }
					else //NonTerminal para trocar
					{
						NonTerminal nt = pilha.Pop().Nonterminal;
                        //if nt = NUMEXPRESSION start saving
                        if (nt.Equals(NonTerminal.NUMEXPRESSION))
                            recordExp = true;
                        //if nt = EXP2 stop saving
                        if (nt.Equals(NonTerminal.EXP2))
                        {
                            recordExp = false;
                            //insere na lista de exp
                            listExpa.Add(expa);
                            expa = "";//limpa para a prox expa
                        }

                        Simbolo key = new Simbolo { Nonterminal = nt, Terminal = token.t };
                        try{
                            newItems = ReferenceTable[key];
                        }catch (Exception)
                        {
                            setErrorMessage(nt, token.s);
                            return false;
                        }
						if (newItems[0].Terminal.Equals(Token.Terminals.EMPTY)
							&& newItems[0].Nonterminal.Equals(NonTerminal.EMPTY))
							newItems.Reverse();
						else
						{
							newItems.Reverse();
							foreach (Simbolo p in newItems)
							{
								pilha.Push(p);
							}
							newItems.Reverse();//obrigatorio
						}
					}
                    if (newItems.Count == 2)
                    {
                        if (newItems[1].Nonterminal.Equals(NonTerminal.FACTOR))
                        {
                            if (newItems[0].Terminal.Equals(Token.Terminals.MINUS))
                                signal = "NEG";
                            else if (newItems[0].Terminal.Equals(Token.Terminals.ADD))
                                signal = "POS";
                            else signal = "";
                        }
                        else
                            signal = "";
                    }
                    else
                        signal = "";

                    string st = "";
					foreach (var p in pilha)
					{
						if (p.Nonterminal.Equals(NonTerminal.EMPTY))
						{
							if (p.Terminal.Equals(Token.Terminals.IDENT))
							{
								st += p.Terminal + " ";
								continue;
							}
							foreach (var t in toks)
							{
								if (t.t.Equals(p.Terminal))
								{
									st += t.s + " ";
									break;
								}
							}
						}
						else
							st += p.Nonterminal + " ";
					}
                    //Console.WriteLine(st + "  | " + output);
                    //sr.WriteLine(String.Format("|{0,150}|{1,150}|", st, output));

                    if (!exit)
						return false;
				}
			}

            return true;
		}

		public List<Token.Tok> CheckDollarSign(List<Token.Tok> toks)
		{
			if (!toks[toks.Count - 1].t.Equals(Token.Terminals.DOLLAR))
			{
				Token.Tok token = new Token.Tok
				{
					s = "$",
					t = Token.Terminals.DOLLAR
				};
				toks.Add(token);
			}
			return toks;
		}
	}
}
