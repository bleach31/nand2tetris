// This file is part of www.nand2tetris.org
// and the book "The Elements of Computing Systems"
// by Nisan and Schocken, MIT Press.
// File name: projects/04/Fill.asm

// Runs an infinite loop that listens to the keyboard input. 
// When a key is pressed (any key), the program blackens the screen,
// i.e. writes "black" in every pixel. When no key is pressed, the
// program clears the screen, i.e. writes "white" in every pixel.

// Put your code here.
(LOOP)
@KBD
D=M		//D=Mem[KBD]
@TOGGLE
D;JGT	//if D>0 goto TOGGLE
@LOOP	
0;JMP	//無限ループ

(TOGGLE)
@8192	//8192回ループ
D=A		//D=8192
(NURI)
	@SCREEN	//16384
	A=D+A	//A=D+SCREEN
	M=!M	//Rem[A]=!Rem[A]
	D=D-1
	@NURI
	D;JGT	//IF D>0 goto NURI
@LOOP	
0;JMP	//無限ループに戻る