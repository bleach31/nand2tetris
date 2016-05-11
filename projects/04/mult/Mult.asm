// This file is part of www.nand2tetris.org
// and the book "The Elements of Computing Systems"
// by Nisan and Schocken, MIT Press.
// File name: projects/04/Mult.asm

// Multiplies R0 and R1 and stores the result in R2.
// (R0, R1, R2 refer to RAM[0], RAM[1], and RAM[2], respectively.)

// Put your code here.
@2		//計算用メモリの初期化
M=0		//Mem[2]=0
(LOOP)
	@0		//ループ回数のロード
	D=M		//D=Mem[0]
	@1		
	D=D-A	//D=D-1
	@END
	D;JLT	//IF D<0 goto 20
	@0
	M=D		//Mem[0]=D ループ回数の更新
	@1
	D=M		//D=Mem[1]
	@2
	M=M+D	//Mem[2]=Mem[2]+D
	@LOOP
	0;JMP	//
(END)
