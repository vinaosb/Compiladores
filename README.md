# Compiladores

Universidade Federal de Santa Catarina

INE5426 - Construcao de Compiladores

Trabalho 1 - 2019/2

Alunos:
	- Bruno George de Moraes (14100825)
	- Marcelo Jose Dias (15205398)
	- Renan Pinho Assi (12200656)
	- Vinicius Schwinden Berkenbrock (16100751)


Esse Trabalho foi realizado utilizando a linguagem C#
A maquina virtual escolhida foi a .Net Core

O Sistema Operacional de destino escolhido foi o Ubuntu 18.04.03

Foi considerado que a maquina alvo possui os comandos snap e make.
Esses comandos podem ser instalados usando apt get snap e apt get build-essentials respectivamente.

Para realizar a instalacao do SDK da .Net Core apenas realize o comando "make install"

Para realizar o run do zero pode ser feito o comando "make run-clean"
Ou a sequencia de comandos "make" e "make run"

Para realizar o run depois basta realizar o comando "make run"

O programa irá criar uma subpasta -> bin/Debug/netcoreapp2.1/
Nessa pasta deverá estar localizado os arquivos .ccc para teste.

Quando o programa for aberto ele irá perguntar se o usuário quer ver apenas a Analise Lexica, a Analise Sintatica, a Análise Semantica ou uma combinacao acima.
Depois perguntara se quer que a saida seja feita em um arquivo ou no proprio console.
Caso for feita em arquivo, ele ira gerar um arquivo para cada analise no formato .txt.