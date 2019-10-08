// ################################################
// Universidade Federal de Santa Catarina
// INE5426 - Construção de Compiladores
// Trabalho 1 - 2019/2
// Alunos:
//		- Bruno George Marques (14100825)
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
		public enum NonTerminal
		{
			PROGRAM,
			STATEMENT,
			VARDECL,
			VAR2,
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

		public struct simbolo
		{
			public NonTerminal nonterminal { get; set; }
			public Token.Terminals terminal { get; set; }
		}

		public Dictionary<NonTerminal, List<List<simbolo>>> Producoes { get; set; }
		public Dictionary<simbolo, List<simbolo>> ReferenceTable { get; set; }
		public Dictionary<NonTerminal, List<Token.Terminals>> first;
		public Dictionary<NonTerminal, HashSet<Token.Terminals>> Follows { get; set; }
		StreamWriter sr;


		public Sintatico()
		{
			Producoes = new Dictionary<NonTerminal, List<List<simbolo>>>();
			initProd();
			first = new Dictionary<NonTerminal, List<Token.Terminals>>();
			Follows = new Dictionary<NonTerminal, HashSet<Token.Terminals>>();
			GenFollows();
			//printFirst();
			//printFollow();
			ReferenceTable = new Dictionary<simbolo, List<simbolo>>();
			initRefTable();

		}

		private void initProd()
		{
			List<simbolo> lp;
			List<List<simbolo>> llp;

			foreach (NonTerminal nt in Enum.GetValues(typeof(NonTerminal)))
			{
				llp = new List<List<simbolo>>();
				/*
				lp = new List<simbolo>();
				lp.Clear();
				lp.Add(new simbolo { nonterminal = NonTerminal.EMPTY, terminal = Token.Terminals.EMPTY });
				llp.Add(lp);
				*/
				switch (nt)
				{
					// PROGRAM -> STATEMENT | &
					case NonTerminal.PROGRAM:
						lp = new List<simbolo>();
						lp.Clear();
						lp.Add(new simbolo { nonterminal = NonTerminal.STATEMENT, terminal = Token.Terminals.EMPTY });
						llp.Add(lp);
						lp = new List<simbolo>();
						lp.Clear();
						lp.Add(new simbolo { nonterminal = NonTerminal.EMPTY, terminal = Token.Terminals.EMPTY });
						llp.Add(lp);
						break;
					// STATEMENT -> VARDECL; | ATRIBSTAT; | PRINTSTAT; | READSTAT; | RETURNSTAT; | IFSTAT; | FORSTAT; | {STATELIST}| break ; | ;
					case NonTerminal.STATEMENT:
						lp = new List<simbolo>();
						lp.Clear();
						lp.Add(new simbolo { nonterminal = NonTerminal.EMPTY, terminal = Token.Terminals.OPENBRACE });
						lp.Add(new simbolo { nonterminal = NonTerminal.STATELIST, terminal = Token.Terminals.EMPTY });
						lp.Add(new simbolo { nonterminal = NonTerminal.EMPTY, terminal = Token.Terminals.CLOSEBRACE });
						llp.Add(lp);
						lp = new List<simbolo>();
						lp.Clear();
						lp.Add(new simbolo { nonterminal = NonTerminal.EMPTY, terminal = Token.Terminals.BREAK });
						lp.Add(new simbolo { nonterminal = NonTerminal.EMPTY, terminal = Token.Terminals.SEPARATOR });
						llp.Add(lp);
						lp = new List<simbolo>();
						lp.Clear();
						lp.Add(new simbolo { nonterminal = NonTerminal.VARDECL, terminal = Token.Terminals.EMPTY });
						lp.Add(new simbolo { nonterminal = NonTerminal.EMPTY, terminal = Token.Terminals.SEPARATOR });
						llp.Add(lp);
						lp = new List<simbolo>();
						lp.Clear();
						lp.Add(new simbolo { nonterminal = NonTerminal.ATRIBSTAT, terminal = Token.Terminals.EMPTY });
						lp.Add(new simbolo { nonterminal = NonTerminal.EMPTY, terminal = Token.Terminals.SEPARATOR });
						llp.Add(lp);
						lp = new List<simbolo>();
						lp.Clear();
						lp.Add(new simbolo { nonterminal = NonTerminal.PRINTSTAT, terminal = Token.Terminals.EMPTY });
						lp.Add(new simbolo { nonterminal = NonTerminal.EMPTY, terminal = Token.Terminals.SEPARATOR });
						llp.Add(lp);
						lp = new List<simbolo>();
						lp.Clear();
						lp.Add(new simbolo { nonterminal = NonTerminal.READSTAT, terminal = Token.Terminals.EMPTY });
						lp.Add(new simbolo { nonterminal = NonTerminal.EMPTY, terminal = Token.Terminals.SEPARATOR });
						llp.Add(lp);
						lp = new List<simbolo>();
						lp.Clear();
						lp.Add(new simbolo { nonterminal = NonTerminal.RETURNSTAT, terminal = Token.Terminals.EMPTY });
						lp.Add(new simbolo { nonterminal = NonTerminal.EMPTY, terminal = Token.Terminals.SEPARATOR });
						llp.Add(lp);
						lp = new List<simbolo>();
						lp.Clear();
						lp.Add(new simbolo { nonterminal = NonTerminal.IFSTAT, terminal = Token.Terminals.EMPTY });
						lp.Add(new simbolo { nonterminal = NonTerminal.EMPTY, terminal = Token.Terminals.SEPARATOR });
						llp.Add(lp);
						lp = new List<simbolo>();
						lp.Clear();
						lp.Add(new simbolo { nonterminal = NonTerminal.FORSTAT, terminal = Token.Terminals.EMPTY });
						lp.Add(new simbolo { nonterminal = NonTerminal.EMPTY, terminal = Token.Terminals.SEPARATOR });
						llp.Add(lp);
						lp = new List<simbolo>();
						lp.Clear();
						lp.Add(new simbolo { nonterminal = NonTerminal.EMPTY, terminal = Token.Terminals.SEPARATOR });
						llp.Add(lp);
						break;
					// VARDECL -> int ident VAR2 | float ident VAR2 | string ident VAR2
					case NonTerminal.VARDECL:
						lp = new List<simbolo>();
						lp.Clear();
						lp.Add(new simbolo { nonterminal = NonTerminal.EMPTY, terminal = Token.Terminals.INTEGER_T });
						lp.Add(new simbolo { nonterminal = NonTerminal.EMPTY, terminal = Token.Terminals.IDENT });
						lp.Add(new simbolo { nonterminal = NonTerminal.VAR2, terminal = Token.Terminals.EMPTY });
						llp.Add(lp);
						lp = new List<simbolo>();
						lp.Clear();
						lp.Add(new simbolo { nonterminal = NonTerminal.EMPTY, terminal = Token.Terminals.FLOAT_T });
						lp.Add(new simbolo { nonterminal = NonTerminal.EMPTY, terminal = Token.Terminals.IDENT });
						lp.Add(new simbolo { nonterminal = NonTerminal.VAR2, terminal = Token.Terminals.EMPTY });
						llp.Add(lp);
						lp = new List<simbolo>();
						lp.Clear();
						lp.Add(new simbolo { nonterminal = NonTerminal.EMPTY, terminal = Token.Terminals.STRING_T });
						lp.Add(new simbolo { nonterminal = NonTerminal.EMPTY, terminal = Token.Terminals.IDENT });
						lp.Add(new simbolo { nonterminal = NonTerminal.VAR2, terminal = Token.Terminals.EMPTY });
						llp.Add(lp);
						break;
					// VAR2 -> [int_constant] VAR2 | &
					case NonTerminal.VAR2:
						lp = new List<simbolo>();
						lp.Clear();
						lp.Add(new simbolo { nonterminal = NonTerminal.EMPTY, terminal = Token.Terminals.OPENBRKT });
						lp.Add(new simbolo { nonterminal = NonTerminal.EMPTY, terminal = Token.Terminals.INT });
						lp.Add(new simbolo { nonterminal = NonTerminal.EMPTY, terminal = Token.Terminals.CLOSEBRKT });
						lp.Add(new simbolo { nonterminal = NonTerminal.VAR2, terminal = Token.Terminals.EMPTY });
						llp.Add(lp);
						lp = new List<simbolo>();
						lp.Clear();
						lp.Add(new simbolo { nonterminal = NonTerminal.EMPTY, terminal = Token.Terminals.EMPTY });
						llp.Add(lp);
						break;
					// ATRIBSTAT -> LVALUE = ATREXP
					case NonTerminal.ATRIBSTAT:
						lp = new List<simbolo>();
						lp.Clear();
						lp.Add(new simbolo { nonterminal = NonTerminal.LVALUE, terminal = Token.Terminals.EMPTY });
						lp.Add(new simbolo { nonterminal = NonTerminal.EMPTY, terminal = Token.Terminals.ASSERT });
						lp.Add(new simbolo { nonterminal = NonTerminal.ATREXP, terminal = Token.Terminals.EMPTY });
						llp.Add(lp);
						break;
					// ATREXP -> EXPRESSION | ALLOCEXPRESSION
					case NonTerminal.ATREXP:
						lp = new List<simbolo>();
						lp.Clear();
						lp.Add(new simbolo { nonterminal = NonTerminal.EXPRESSION, terminal = Token.Terminals.EMPTY });
						llp.Add(lp);
						lp = new List<simbolo>();
						lp.Clear();
						lp.Add(new simbolo { nonterminal = NonTerminal.ALLOCEXPRESSION, terminal = Token.Terminals.EMPTY });
						llp.Add(lp);
						break;
					// PRINTSTAT -> print EXPRESSION
					case NonTerminal.PRINTSTAT:
						lp = new List<simbolo>();
						lp.Clear();
						lp.Add(new simbolo { nonterminal = NonTerminal.EMPTY, terminal = Token.Terminals.PRINT });
						lp.Add(new simbolo { nonterminal = NonTerminal.EXPRESSION, terminal = Token.Terminals.EMPTY });
						llp.Add(lp);
						break;
					// READSTAT -> read LVALUE
					case NonTerminal.READSTAT:
						lp = new List<simbolo>();
						lp.Clear();
						lp.Add(new simbolo { nonterminal = NonTerminal.EMPTY, terminal = Token.Terminals.READ });
						lp.Add(new simbolo { nonterminal = NonTerminal.LVALUE, terminal = Token.Terminals.EMPTY });
						llp.Add(lp);
						break;
					// RETURNSTAT -> return
					case NonTerminal.RETURNSTAT:
						lp = new List<simbolo>();
						lp.Clear();
						lp.Add(new simbolo { nonterminal = NonTerminal.EMPTY, terminal = Token.Terminals.RETURN });
						llp.Add(lp);
						break;
					// IFSTAT -> if( EXPRESSION ) STATEMENT IF2
					case NonTerminal.IFSTAT:
						lp = new List<simbolo>();
						lp.Clear();
						lp.Add(new simbolo { nonterminal = NonTerminal.EMPTY, terminal = Token.Terminals.IF });
						lp.Add(new simbolo { nonterminal = NonTerminal.EMPTY, terminal = Token.Terminals.OPENPARENT });
						lp.Add(new simbolo { nonterminal = NonTerminal.EXPRESSION, terminal = Token.Terminals.EMPTY });
						lp.Add(new simbolo { nonterminal = NonTerminal.EMPTY, terminal = Token.Terminals.CLOSEPARENT });
						lp.Add(new simbolo { nonterminal = NonTerminal.STATEMENT, terminal = Token.Terminals.EMPTY });
						lp.Add(new simbolo { nonterminal = NonTerminal.IF2, terminal = Token.Terminals.EMPTY });
						llp.Add(lp);
						break;
					// IF2 -> else STATEMENT | e
					case NonTerminal.IF2:
						lp = new List<simbolo>();
						lp.Clear();
						lp.Add(new simbolo { nonterminal = NonTerminal.EMPTY, terminal = Token.Terminals.ELSE });
						lp.Add(new simbolo { nonterminal = NonTerminal.STATEMENT, terminal = Token.Terminals.EMPTY });
						llp.Add(lp);
						lp = new List<simbolo>();
						lp.Clear();
						lp.Add(new simbolo { nonterminal = NonTerminal.EMPTY, terminal = Token.Terminals.EMPTY });
						llp.Add(lp);
						break;
					// FORSTAT -> for( ATRIBSTAT; NUMEXPRESSION; ATRIBSTAT) STATEMENT
					case NonTerminal.FORSTAT:
						lp = new List<simbolo>();
						lp.Clear();
						lp.Add(new simbolo { nonterminal = NonTerminal.EMPTY, terminal = Token.Terminals.FOR });
						lp.Add(new simbolo { nonterminal = NonTerminal.EMPTY, terminal = Token.Terminals.OPENPARENT });
						lp.Add(new simbolo { nonterminal = NonTerminal.ATRIBSTAT, terminal = Token.Terminals.EMPTY });
						lp.Add(new simbolo { nonterminal = NonTerminal.EMPTY, terminal = Token.Terminals.SEPARATOR });
						lp.Add(new simbolo { nonterminal = NonTerminal.EXPRESSION, terminal = Token.Terminals.EMPTY });
						lp.Add(new simbolo { nonterminal = NonTerminal.EMPTY, terminal = Token.Terminals.SEPARATOR });
						lp.Add(new simbolo { nonterminal = NonTerminal.ATRIBSTAT, terminal = Token.Terminals.EMPTY });
						lp.Add(new simbolo { nonterminal = NonTerminal.EMPTY, terminal = Token.Terminals.CLOSEPARENT });
						lp.Add(new simbolo { nonterminal = NonTerminal.STATEMENT, terminal = Token.Terminals.EMPTY });
						llp.Add(lp);
						break;
					// STATELIST -> STATEMENT STATE2
					case NonTerminal.STATELIST:
						lp = new List<simbolo>();
						lp.Clear();
						lp.Add(new simbolo { nonterminal = NonTerminal.STATEMENT, terminal = Token.Terminals.EMPTY });
						lp.Add(new simbolo { nonterminal = NonTerminal.STATE2, terminal = Token.Terminals.EMPTY });
						llp.Add(lp);
						break;
					// STATE2 -> STATELIST | &
					case NonTerminal.STATE2:
						lp = new List<simbolo>();
						lp.Clear();
						lp.Add(new simbolo { nonterminal = NonTerminal.STATELIST, terminal = Token.Terminals.EMPTY });
						llp.Add(lp);
						lp = new List<simbolo>();
						lp.Clear();
						lp.Add(new simbolo { nonterminal = NonTerminal.EMPTY, terminal = Token.Terminals.EMPTY });
						llp.Add(lp);
						break;
					// ALLOCEXPRESSION -> new ALLOC2
					case NonTerminal.ALLOCEXPRESSION:
						lp = new List<simbolo>();
						lp.Clear();
						lp.Add(new simbolo { nonterminal = NonTerminal.EMPTY, terminal = Token.Terminals.NEW });
						lp.Add(new simbolo { nonterminal = NonTerminal.ALLOC2, terminal = Token.Terminals.EMPTY });
						llp.Add(lp);
						break;
					// ALLOC2 -> int [ EXPRESSION ] ALLOC3 | float [ EXPRESSION ] ALLOC3 | string [ EXPRESSION ] ALLOC3
					case NonTerminal.ALLOC2:
						lp = new List<simbolo>();
						lp.Clear();
						lp.Add(new simbolo { nonterminal = NonTerminal.EMPTY, terminal = Token.Terminals.INTEGER_T });
						lp.Add(new simbolo { nonterminal = NonTerminal.EMPTY, terminal = Token.Terminals.OPENBRKT });
						lp.Add(new simbolo { nonterminal = NonTerminal.EXPRESSION, terminal = Token.Terminals.EMPTY });
						lp.Add(new simbolo { nonterminal = NonTerminal.EMPTY, terminal = Token.Terminals.CLOSEBRKT });
						lp.Add(new simbolo { nonterminal = NonTerminal.ALLOC3, terminal = Token.Terminals.EMPTY });
						llp.Add(lp);
						lp = new List<simbolo>();
						lp.Clear();
						lp.Add(new simbolo { nonterminal = NonTerminal.EMPTY, terminal = Token.Terminals.FLOAT_T });
						lp.Add(new simbolo { nonterminal = NonTerminal.EMPTY, terminal = Token.Terminals.OPENBRKT });
						lp.Add(new simbolo { nonterminal = NonTerminal.EXPRESSION, terminal = Token.Terminals.EMPTY });
						lp.Add(new simbolo { nonterminal = NonTerminal.EMPTY, terminal = Token.Terminals.CLOSEBRKT });
						lp.Add(new simbolo { nonterminal = NonTerminal.ALLOC3, terminal = Token.Terminals.EMPTY });
						llp.Add(lp);
						lp = new List<simbolo>();
						lp.Clear();
						lp.Add(new simbolo { nonterminal = NonTerminal.EMPTY, terminal = Token.Terminals.STRING_T });
						lp.Add(new simbolo { nonterminal = NonTerminal.EMPTY, terminal = Token.Terminals.OPENBRKT });
						lp.Add(new simbolo { nonterminal = NonTerminal.EXPRESSION, terminal = Token.Terminals.EMPTY });
						lp.Add(new simbolo { nonterminal = NonTerminal.EMPTY, terminal = Token.Terminals.CLOSEBRKT });
						lp.Add(new simbolo { nonterminal = NonTerminal.ALLOC3, terminal = Token.Terminals.EMPTY });
						llp.Add(lp);
						break;
					// ALLOC3 -> [ EXPRESSION ] ALLOC3 | e
					case NonTerminal.ALLOC3:
						lp = new List<simbolo>();
						lp.Clear();
						lp.Add(new simbolo { nonterminal = NonTerminal.EMPTY, terminal = Token.Terminals.OPENBRKT });
						lp.Add(new simbolo { nonterminal = NonTerminal.EXPRESSION, terminal = Token.Terminals.EMPTY });
						lp.Add(new simbolo { nonterminal = NonTerminal.EMPTY, terminal = Token.Terminals.CLOSEBRKT });
						lp.Add(new simbolo { nonterminal = NonTerminal.ALLOC3, terminal = Token.Terminals.EMPTY });
						llp.Add(lp);
						lp = new List<simbolo>();
						lp.Clear();
						lp.Add(new simbolo { nonterminal = NonTerminal.EMPTY, terminal = Token.Terminals.EMPTY });
						llp.Add(lp);
						break;
					// EXPRESSION -> NUMEXPRESSION EXP2
					case NonTerminal.EXPRESSION:
						lp = new List<simbolo>();
						lp.Clear();
						lp.Add(new simbolo { nonterminal = NonTerminal.NUMEXPRESSION, terminal = Token.Terminals.EMPTY });
						lp.Add(new simbolo { nonterminal = NonTerminal.EXP2, terminal = Token.Terminals.EMPTY });
						llp.Add(lp);
						break;
					// EXP2 -> < NUMEXPRESSION | > NUMEXPRESSION | <= NUMEXPRESSION | >= NUMEXPRESSION | == NUMEXPRESSION | ! = NUMEXPRESSION | e
					case NonTerminal.EXP2:
						lp = new List<simbolo>();
						lp.Clear();
						lp.Add(new simbolo { nonterminal = NonTerminal.EMPTY, terminal = Token.Terminals.LT });
						lp.Add(new simbolo { nonterminal = NonTerminal.NUMEXPRESSION, terminal = Token.Terminals.EMPTY });
						llp.Add(lp);
						lp = new List<simbolo>();
						lp.Clear();
						lp.Add(new simbolo { nonterminal = NonTerminal.EMPTY, terminal = Token.Terminals.GT });
						lp.Add(new simbolo { nonterminal = NonTerminal.NUMEXPRESSION, terminal = Token.Terminals.EMPTY });
						llp.Add(lp);
						lp = new List<simbolo>();
						lp.Clear();
						lp.Add(new simbolo { nonterminal = NonTerminal.EMPTY, terminal = Token.Terminals.LE });
						lp.Add(new simbolo { nonterminal = NonTerminal.NUMEXPRESSION, terminal = Token.Terminals.EMPTY });
						llp.Add(lp);
						lp = new List<simbolo>();
						lp.Clear();
						lp.Add(new simbolo { nonterminal = NonTerminal.EMPTY, terminal = Token.Terminals.GE });
						lp.Add(new simbolo { nonterminal = NonTerminal.NUMEXPRESSION, terminal = Token.Terminals.EMPTY });
						llp.Add(lp);
						lp = new List<simbolo>();
						lp.Clear();
						lp.Add(new simbolo { nonterminal = NonTerminal.EMPTY, terminal = Token.Terminals.EQ });
						lp.Add(new simbolo { nonterminal = NonTerminal.NUMEXPRESSION, terminal = Token.Terminals.EMPTY });
						llp.Add(lp);
						lp = new List<simbolo>();
						lp.Clear();
						lp.Add(new simbolo { nonterminal = NonTerminal.EMPTY, terminal = Token.Terminals.NE });
						lp.Add(new simbolo { nonterminal = NonTerminal.NUMEXPRESSION, terminal = Token.Terminals.EMPTY });
						llp.Add(lp);
						lp = new List<simbolo>();
						lp.Clear();
						lp.Add(new simbolo { nonterminal = NonTerminal.EMPTY, terminal = Token.Terminals.EMPTY });
						llp.Add(lp);
						break;
					// NUMEXPRESSION -> TERM NUM2
					case NonTerminal.NUMEXPRESSION:
						lp = new List<simbolo>();
						lp.Clear();
						lp.Add(new simbolo { nonterminal = NonTerminal.TERM, terminal = Token.Terminals.EMPTY });
						lp.Add(new simbolo { nonterminal = NonTerminal.NUM2, terminal = Token.Terminals.EMPTY });
						llp.Add(lp);
						break;
					// NUM2 -> + TERM NUM2| - TERM NUM2| e
					case NonTerminal.NUM2:
						lp = new List<simbolo>();
						lp.Clear();
						lp.Add(new simbolo { nonterminal = NonTerminal.EMPTY, terminal = Token.Terminals.ADD });
						lp.Add(new simbolo { nonterminal = NonTerminal.TERM, terminal = Token.Terminals.EMPTY });
						lp.Add(new simbolo { nonterminal = NonTerminal.NUM2, terminal = Token.Terminals.EMPTY });
						llp.Add(lp);
						lp = new List<simbolo>();
						lp.Clear();
						lp.Add(new simbolo { nonterminal = NonTerminal.EMPTY, terminal = Token.Terminals.MINUS });
						lp.Add(new simbolo { nonterminal = NonTerminal.TERM, terminal = Token.Terminals.EMPTY });
						lp.Add(new simbolo { nonterminal = NonTerminal.NUM2, terminal = Token.Terminals.EMPTY });
						llp.Add(lp);
						lp = new List<simbolo>();
						lp.Clear();
						lp.Add(new simbolo { nonterminal = NonTerminal.EMPTY, terminal = Token.Terminals.EMPTY });
						llp.Add(lp);
						break;
					// TERM -> UNARYEXPR TERM2
					case NonTerminal.TERM:
						lp = new List<simbolo>();
						lp.Clear();
						lp.Add(new simbolo { nonterminal = NonTerminal.UNARYEXPR, terminal = Token.Terminals.EMPTY });
						lp.Add(new simbolo { nonterminal = NonTerminal.TERM2, terminal = Token.Terminals.EMPTY });
						llp.Add(lp);
						break;
					// TERM2 -> ∗ UNARYEXPR TERM2| \ UNARYEXPR TERM2 | % UNARYEXPR TERM2 | e
					case NonTerminal.TERM2:
						lp = new List<simbolo>();
						lp.Clear();
						lp.Add(new simbolo { nonterminal = NonTerminal.EMPTY, terminal = Token.Terminals.MULTIPLY });
						lp.Add(new simbolo { nonterminal = NonTerminal.UNARYEXPR, terminal = Token.Terminals.EMPTY });
						lp.Add(new simbolo { nonterminal = NonTerminal.TERM2, terminal = Token.Terminals.EMPTY });
						llp.Add(lp);
						lp = new List<simbolo>();
						lp.Clear();
						lp.Add(new simbolo { nonterminal = NonTerminal.EMPTY, terminal = Token.Terminals.DIVIDE });
						lp.Add(new simbolo { nonterminal = NonTerminal.UNARYEXPR, terminal = Token.Terminals.EMPTY });
						lp.Add(new simbolo { nonterminal = NonTerminal.TERM2, terminal = Token.Terminals.EMPTY });
						llp.Add(lp);
						lp = new List<simbolo>();
						lp.Clear();
						lp.Add(new simbolo { nonterminal = NonTerminal.EMPTY, terminal = Token.Terminals.MODULUS });
						lp.Add(new simbolo { nonterminal = NonTerminal.UNARYEXPR, terminal = Token.Terminals.EMPTY });
						lp.Add(new simbolo { nonterminal = NonTerminal.TERM2, terminal = Token.Terminals.EMPTY });
						llp.Add(lp);
						lp = new List<simbolo>();
						lp.Clear();
						lp.Add(new simbolo { nonterminal = NonTerminal.EMPTY, terminal = Token.Terminals.EMPTY });
						llp.Add(lp);
						break;
					// UNARYEXPR -> + FACTOR | - FACTOR | FACTOR
					case NonTerminal.UNARYEXPR:
						lp = new List<simbolo>();
						lp.Clear();
						lp.Add(new simbolo { nonterminal = NonTerminal.EMPTY, terminal = Token.Terminals.ADD });
						lp.Add(new simbolo { nonterminal = NonTerminal.FACTOR, terminal = Token.Terminals.EMPTY });
						llp.Add(lp);
						lp = new List<simbolo>();
						lp.Clear();
						lp.Add(new simbolo { nonterminal = NonTerminal.EMPTY, terminal = Token.Terminals.MINUS });
						lp.Add(new simbolo { nonterminal = NonTerminal.FACTOR, terminal = Token.Terminals.EMPTY });
						llp.Add(lp);
						lp = new List<simbolo>();
						lp.Clear();
						lp.Add(new simbolo { nonterminal = NonTerminal.FACTOR, terminal = Token.Terminals.EMPTY });
						llp.Add(lp);
						break;
					// FACTOR -> int_constant | float_constant | string_constant | null | LVALUE |( EXPRESSION )
					case NonTerminal.FACTOR:
						lp = new List<simbolo>();
						lp.Clear();
						lp.Add(new simbolo { nonterminal = NonTerminal.EMPTY, terminal = Token.Terminals.INT });
						llp.Add(lp);
						lp = new List<simbolo>();
						lp.Clear();
						lp.Add(new simbolo { nonterminal = NonTerminal.EMPTY, terminal = Token.Terminals.FLT });
						llp.Add(lp);
						lp = new List<simbolo>();
						lp.Clear();
						lp.Add(new simbolo { nonterminal = NonTerminal.EMPTY, terminal = Token.Terminals.STR });
						llp.Add(lp);
						lp = new List<simbolo>();
						lp.Clear();
						lp.Add(new simbolo { nonterminal = NonTerminal.EMPTY, terminal = Token.Terminals.NULL });
						llp.Add(lp);
						lp = new List<simbolo>();
						lp.Clear();
						lp.Add(new simbolo { nonterminal = NonTerminal.LVALUE, terminal = Token.Terminals.EMPTY });
						llp.Add(lp);
						lp = new List<simbolo>();
						lp.Clear();
						lp.Add(new simbolo { nonterminal = NonTerminal.EMPTY, terminal = Token.Terminals.OPENPARENT });
						lp.Add(new simbolo { nonterminal = NonTerminal.EXPRESSION, terminal = Token.Terminals.EMPTY });
						lp.Add(new simbolo { nonterminal = NonTerminal.EMPTY, terminal = Token.Terminals.CLOSEPARENT });
						llp.Add(lp);
						break;
					// LVALUE -> ident ALLOC3
					case NonTerminal.LVALUE:
						lp = new List<simbolo>();
						lp.Clear();
						lp.Add(new simbolo { nonterminal = NonTerminal.EMPTY, terminal = Token.Terminals.IDENT });
						lp.Add(new simbolo { nonterminal = NonTerminal.ALLOC3, terminal = Token.Terminals.EMPTY });
						llp.Add(lp);
						break;
					default:
						break;
				}

				Producoes.Add(nt, llp);
			}
		}
		private void initRefTable()
		{
			List<List<simbolo>> llp;
			//Token.Terminals t;
			foreach (NonTerminal nt in Enum.GetValues(typeof(NonTerminal)))
			{
				bool hasEpson = false;
				if (nt.Equals(NonTerminal.EMPTY))//pois nao queremos calcular o First do Empty
					continue;
				llp = Producoes[nt];
				//Cada lp eh uma producao NT -> X
				foreach (List<simbolo> lp in llp)
				{
					List<Token.Terminals> lt = getFirstFromProd(lp);
					foreach (Token.Terminals t in lt)
					{
						if (t.Equals(Token.Terminals.EMPTY))
						{
							hasEpson = true;
							continue;//pois epson nao entra na tabela de analise
						}

						if (!ReferenceTable.ContainsKey(new simbolo { nonterminal = nt, terminal = t }))//so por enquanto
							ReferenceTable.Add(new simbolo { nonterminal = nt, terminal = t }, lp);

					}
					if (hasEpson)
					{
						foreach (Token.Terminals t in Follows[nt]) //sao os terminais do Follow
						{
							List<simbolo> lpEpson = new List<simbolo>();
							lpEpson.Add(new simbolo { nonterminal = NonTerminal.EMPTY, terminal = Token.Terminals.EMPTY });
							ReferenceTable.Add(new simbolo { nonterminal = nt, terminal = t }, lpEpson);
						}
					}

				}
			}
		}

		public void WriteOutput(List<Token.Tok> lt, StreamWriter sr)
		{
			string prod = "";

			// Read the stream to a string, and write the string to the console.
			foreach (var sy in ReferenceTable)
			{
				prod = "";
				foreach (var pr in sy.Value)
				{
					if (pr.nonterminal.Equals(Sintatico.NonTerminal.EMPTY))
						prod += pr.terminal.ToString() + " ";
					else
						prod += pr.nonterminal.ToString() + " ";
				}
				prod = prod.Replace("EMPTY", "ɛ");
				sr.WriteLine("{0},{1}->{2}", sy.Key.nonterminal, sy.Key.terminal, prod);
			}

			if (predictiveParser(lt, sr))
				sr.WriteLine("Entrada Aceita");
			else
				sr.WriteLine("Entrada Nao Aceita");
		}

		private List<Token.Terminals> getFirstFromProd(List<simbolo> lp)
		{
			List<Token.Terminals> lt = new List<Token.Terminals>();

			//Esse metodo usa o metodo First para pegar o First da producao a direita
			//ex: S -> ABc
			//vai retornar o first(A), se no first(A) tiver epson, inclui o first(B)
			if (lp[0].nonterminal.Equals(NonTerminal.EMPTY))
			{
				lt.Add(lp[0].terminal);
				return lt;
			}
			//else First
			foreach (simbolo p in lp)
			{
				if (p.nonterminal.Equals(NonTerminal.EMPTY))
				{
					lt.Add(p.terminal);
					return lt;
				}
				//lt = fixedFirst(p.nonterminal);
				lt.AddRange(new List<Token.Terminals>(First(p.nonterminal)).FindAll((x) => !lt.Contains(x)));
				// lt.AddRange(fixedFirst(p.nonterminal).FindAll((x) => !lt.Contains(x)));
				if (!First(p.nonterminal).Contains(Token.Terminals.EMPTY))
					return lt;

			}

			//lt.Add(Token.Terminals.BASIC);//pra testar so.
			return lt;
		}
		public void calculaFirst()
		{
			List<NonTerminal> naoTerminais = new List<NonTerminal>();
			foreach (var t in Producoes)
			{
				List<Token.Terminals> terminais = new List<Token.Terminals>();
				procuraFirst(t.Key, terminais);
				first.Add(t.Key, terminais);
			}
		}

		public void procuraFirst(NonTerminal naoTerminalAtual, List<Token.Terminals> terminais)
		{
			List<List<simbolo>> p = Producoes[naoTerminalAtual];
			foreach (var t in p)
			{
				int i = 0;
				foreach (var h in t)
				{
					if (i == 0)
					{
						if (h.nonterminal != NonTerminal.EMPTY)
						{
							if (h.terminal == Token.Terminals.EMPTY)
							{
								procuraFirst(h.nonterminal, terminais);
							}
						}
						else
						{
							terminais.Add(h.terminal);
						}
					}
					i++;
				}
			}
		}

		private HashSet<Token.Terminals> First(NonTerminal nt)
		{
			HashSet<Token.Terminals> ret = new HashSet<Token.Terminals>();

			List<List<simbolo>> llp = Producoes.GetValueOrDefault(nt);

			foreach (var lp in llp)
			{
				int i = -1;
				do
				{
					i++;
					if (lp[i].terminal != Token.Terminals.EMPTY)
					{
						ret.Add(lp[i].terminal);
						continue;
					}

					ret.UnionWith(First(lp[i].nonterminal));

				} while (NextHasEmpty(lp[i].nonterminal));

			}

			if (ret.Count == 0)
				ret.Add(Token.Terminals.EMPTY);

			return ret;
		}

		private bool NextHasEmpty(NonTerminal nt)
		{
			List<List<simbolo>> llp = Producoes.GetValueOrDefault(nt);

			foreach (var lp in llp)
				if (lp.Contains(new simbolo { terminal = Token.Terminals.EMPTY, nonterminal = NonTerminal.EMPTY }))
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
							if (lp[i].nonterminal != NonTerminal.EMPTY && i == lp.Count - 1) // Regra 3.1
							{
								if (!Follows.GetValueOrDefault(lp[i].nonterminal).IsSupersetOf(Follows.GetValueOrDefault(nt)))
									houveMudancas = true;
								Follows.GetValueOrDefault(lp[i].nonterminal).UnionWith(Follows.GetValueOrDefault(nt));
								break;
							}
							else if (lp[i].nonterminal != NonTerminal.EMPTY && lp[i + 1].nonterminal != NonTerminal.EMPTY)
							{
								HashSet<Token.Terminals> temp = First(lp[i + 1].nonterminal);
								temp.Remove(Token.Terminals.EMPTY);

								if (!Follows.GetValueOrDefault(lp[i].nonterminal).IsSupersetOf(temp))
									houveMudancas = true;
								Follows.GetValueOrDefault(lp[i].nonterminal).UnionWith(temp); // Regra 2.2


								if (!Follows.GetValueOrDefault(lp[i].nonterminal).IsSupersetOf(Follows.GetValueOrDefault(lp[i + 1].nonterminal))) // Regra 3.2
									houveMudancas = true;
								Follows.GetValueOrDefault(lp[i].nonterminal).UnionWith(Follows.GetValueOrDefault(lp[i + 1].nonterminal));
							}
							else if (lp[i].nonterminal != NonTerminal.EMPTY && lp[i + 1].terminal != Token.Terminals.EMPTY) // Regra 2.1
							{
								if (Follows.GetValueOrDefault(lp[i].nonterminal).Add(lp[i + 1].terminal))
									houveMudancas = true;
							}

						}

					}
				}
			} while (houveMudancas);
		}

		public void printFirst()
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

		public void printFollow()
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
		public bool predictiveParser(List<Token.Tok> toks, StreamWriter sr)
		{
			string output = "";
			bool exit = true;
			Stack<simbolo> pilha = new Stack<simbolo>();
			List<simbolo> newItems = new List<simbolo>();


			sr.WriteLine("");
			sr.WriteLine("Parser: (Pilha)");
			sr.WriteLine(String.Format("|{0,-150}|{1,-150}|", "Stack", "Matched"));
			sr.WriteLine(String.Format("|{0,150}|{0,150}|", "PROGRAM $"));
			toks = checkDollarSign(toks);
			pilha.Push(new simbolo { nonterminal = NonTerminal.EMPTY, terminal = Token.Terminals.DOLLAR });
			pilha.Push(new simbolo { nonterminal = NonTerminal.PROGRAM, terminal = Token.Terminals.EMPTY });
			foreach (var token in toks)
			{
				bool searchingTerminal = true;
				while (searchingTerminal)
				{
					newItems = new List<simbolo>();
					newItems.Clear();
					if (token.t.Equals(pilha.Peek().terminal))
					{
						pilha.Pop();
						searchingTerminal = false;
						output += token.s + " ";
						if (token.s == "$")
							return true;
					}
					else if (pilha.Peek().nonterminal.Equals(NonTerminal.EMPTY))
					{
						//terminal diferente da entrada
						pilha.Pop();
						output += token.s + " ";
						exit = false;

					}
					else //NonTerminal para trocar
					{
						NonTerminal nt = pilha.Pop().nonterminal;
						simbolo key = new simbolo { nonterminal = nt, terminal = token.t };
						newItems = ReferenceTable[key];
						if (newItems[0].terminal.Equals(Token.Terminals.EMPTY)
							&& newItems[0].nonterminal.Equals(NonTerminal.EMPTY))
							newItems.Reverse();
						else
						{
							newItems.Reverse();
							foreach (simbolo p in newItems)
							{
								pilha.Push(p);
							}
							newItems.Reverse();//obrigatorio
						}
					}

					string st = "";
					foreach (var p in pilha)
					{
						if (p.nonterminal.Equals(NonTerminal.EMPTY))
						{
							if (p.terminal.Equals(Token.Terminals.IDENT))
							{
								st += p.terminal + " ";
								continue;
							}
							foreach (var t in toks)
							{
								if (t.t.Equals(p.terminal))
								{
									st += t.s + " ";
									break;
								}
							}
						}
						else
							st += p.nonterminal + " ";
					}
					//Console.WriteLine(st + "  | " + output);

					sr.WriteLine(String.Format("|{0,150}|{1,150}|", st, output));
					if (!exit)
						return false;
				}
			}

			return true;
		}

		private List<Token.Tok> checkDollarSign(List<Token.Tok> toks)
		{
			if (!toks[toks.Count - 1].t.Equals(Token.Terminals.DOLLAR))
			{
				Token.Tok token = new Token.Tok();
				token.s = "$";
				token.t = Token.Terminals.DOLLAR;
				toks.Add(token);
			}
			return toks;
		}
	}
}
