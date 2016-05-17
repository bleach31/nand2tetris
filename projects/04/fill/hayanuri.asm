// This file is part of www.nand2tetris.org
// and the book "The Elements of Computing Systems"
// by Nisan and Schocken, MIT Press.
// File name: projects/04/Fill.asm

// Runs an infinite loop that listens to the keyboard input. 
// When a key is pressed (any key), the program blackens the screen,
// i.e. writes "black" in every pixel. When no key is pressed, the
// program clears the screen, i.e. writes "white" in every pixel.

// Put your code here.
//なるべくはやく塗る。16バースト塗り。終了判定あり
@SCREEN	//16384
D=A		//D=16384
(LOOP)
A=D		//初回
M=!M	//Rem[A]=!Rem[A]
A=A+1	//
M=!M	//Rem[A]=!Rem[A]
A=A+1	//
M=!M	//Rem[A]=!Rem[A]
A=A+1	//
M=!M	//Rem[A]=!Rem[A]
A=A+1	//
M=!M	//Rem[A]=!Rem[A]
A=A+1	//
M=!M	//Rem[A]=!Rem[A]
A=A+1	//
M=!M	//Rem[A]=!Rem[A]
A=A+1	//
M=!M	//Rem[A]=!Rem[A]
A=A+1	//
M=!M	//Rem[A]=!Rem[A]
A=A+1	//
M=!M	//Rem[A]=!Rem[A]
A=A+1	//
M=!M	//Rem[A]=!Rem[A]
A=A+1	//
M=!M	//Rem[A]=!Rem[A]
A=A+1	//
M=!M	//Rem[A]=!Rem[A]
A=A+1	//
M=!M	//Rem[A]=!Rem[A]
A=A+1	//
M=!M	//Rem[A]=!Rem[A]
A=A+1	//
M=!M	//Rem[A]=!Rem[A]
@16		//
D=D+A	//D=D+16
@24576	//終了アドレス（16384+8192）が‐１になってたら分岐でもOK
D=D-A	//D=D-24576
@END	//
D;JGE	//if D >= 0 goto end 
@24576	//終了アドレスを引いた分を戻す
D=D+A	//D=D+24576
@LOOP
0;JMP	//ループ
(END)
@END
0;JMP	//無限ループ