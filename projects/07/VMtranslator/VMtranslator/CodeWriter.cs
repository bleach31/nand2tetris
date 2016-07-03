using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace VMtranslator
{
	class CodeWriter
	{
		StreamWriter m_file_writer;
		string m_FunctionName;
		/// <summary>
		/// 出力ファイル/ストリームを開きパースを行う準備をする
		/// </summary>
		/// <param name="file">入力ファイルパス</param>
		public CodeWriter(String filepath)
		{
			callnum = 0;
			m_FunctionName = Path.GetFileNameWithoutExtension(filepath);
			m_file_writer = new StreamWriter(filepath);


			/// sys.vmがある場合だけ必要
			//WriteInit();

		}

		public void close() {
			m_file_writer.Close();
		}

		/// <summary>
		/// </summary>
		public void WriteInit()
		{
			myWriter("@256");
			myWriter("D=A");
			myWriter("@SP");
			myWriter("M=D");

			writeGoto("Sys.init");
		}


		//gtなどで用いる内部的なlabel名の生成に使う
		//HACK:コンパイル後のlabel名によっては最悪重複する。→コンパイルで生成されるlabelがかぶらないか確認する
		int callnum;

        /// <summary>
        /// 与えられた算術コマンドをアセンブリコードに変換し、それを書き込む
        /// </summary>
        /// <param name="command"></param>
        public void writeArithmetic(string command)
		{
			callnum++;
			//一つ目をR13に取り出す negとnot以外は2項演算なので
			if (!command.Equals("neg") && !command.Equals("not")) {
				myWriter("@SP");										//SP=258 M[258]=null.M[257]=y,M[256]=x
				myWriter("M=M-1");	//SPの更新　M[SP] = M[SP] - 1	//SP=257
				myWriter("A=M");		//A=M[SP]
				myWriter("D=M");		//D=M[M[SP]]  = y
				//xをR13へ退避
				myWriter("@R13");
				myWriter("M=D");		//M[R13]= y
			}
			//一つ目or二つ目をDに取り出す
			myWriter("@SP");											//SP=257
			//myWriter("M=M-1");	//どうせpushするのでSPを更新しない
			myWriter("A=M-1");	//A=M[SP]-1		
			myWriter("D=M");		//D=M[M[SP]-1]
			
			//データの場所が異なるので要注意
			//二項演算　M[R13]=y,D=x
			//単項演算　D=y
			switch (command)
			{
				case "add":
					myWriter("@R13");
					myWriter("D=D+M");	//D= x + y
					break;
				case "sub":
					myWriter("@R13");
					myWriter("D=D-M");	//D= x - y
					break;
				case "neg":
					myWriter("D=-D");	//D= !y
					break;
				case "eq":
					myWriter("@R13");
					myWriter("D=D-M");	//D= x - y
					myWriter("@" + m_FunctionName + "_EQ_TRUE_CALL"+callnum);	
					myWriter("D;JEQ");	//if D==0 jump
					//偽の場合
					myWriter("D=0");		//D= false 
					myWriter("@" + m_FunctionName + "_EQ_END_CALL" + callnum);	
					myWriter("0;JMP");	//無条件ジャンプ
					//真の場合
					myWriter("(" + m_FunctionName + "_EQ_TRUE_CALL" + callnum + ")");	
					myWriter("D=-1");	//D= true
					
					myWriter("(" + m_FunctionName + "_EQ_END_CALL" + callnum + ")");	
					break;
				case "gt":
					myWriter("@R13");
					myWriter("D=D-M");	//D= x - y
					myWriter("@" + m_FunctionName + "_GT_TRUE_CALL"+callnum);	
					myWriter("D;JGT");	//if D > 0 jump
					//偽の場合
					myWriter("D=0");		//D= false
					myWriter("@" + m_FunctionName + "_GT_END_CALL"+callnum);	
					myWriter("0;JMP");	//無条件ジャンプ
					//真の場合
					myWriter("(" + m_FunctionName + "_GT_TRUE_CALL" + callnum + ")");	
					myWriter("D=-1");	//D= true

					myWriter("(" + m_FunctionName + "_GT_END_CALL" + callnum + ")");	
					break;
				case "lt":
					myWriter("@R13");
					myWriter("D=D-M");	//D= x - y
					myWriter("@" + m_FunctionName + "_LT_TRUE_CALL"+callnum);	
					myWriter("D;JLT");	//if D < 0 jump
					//偽の場合
					myWriter("D=0");		//D= false
					myWriter("@" + m_FunctionName + "_LT_END_CALL"+callnum);	
					myWriter("0;JMP");	//無条件ジャンプ
					//真の場合
					myWriter("(" + m_FunctionName + "_LT_TRUE_CALL" + callnum + ")");	
					myWriter("D=-1");	//D= true

					myWriter("(" + m_FunctionName + "_LT_END_CALL" + callnum + ")");	
					break;
				case "and":
					myWriter("@R13");
					myWriter("D=D&M");	//D= x And y
					break;
				case "or":
					myWriter("@R13");
					myWriter("D=D|M");	//D= x + y
					break;
				case "not":
					myWriter("D=!D");	//D= !y
					break;

				default:
                        break;
			}
			//スタックへ戻す	SPはPOP時に-1してないのでそのままでOK
			myWriter("@SP");										//SP=257 
			myWriter("A=M-1");	//A=M[SP]-1						//A=256
			myWriter("M=D");		//M[M[SP]-1]=D					//M[258]=null.M[257]=null,M[256]=結果
		}

		/// <summary>
		/// C_PUSH または C_POP コマンドをアセンブリコードに変換し、それを書き込む
		/// R13,R14使用
		/// </summary>
		/// <param name="command"></param>
		/// <param name="segment"></param>
		/// <param name="index"></param>
		public void writePushPop(VMtranslator.Parser.CommandTypes command, string segment, int index)
		{
			if (command == VMtranslator.Parser.CommandTypes.C_PUSH)
			{
				//データをDにロードする
				List<string> memInAddressSegments = new List<string>(){"local","argument","this","that"};
                List<string> memMappedSegments = new List<string>() { "pointer", "temp" };
                if (memInAddressSegments.Contains(segment))
                {
                    //segmentの値がそのまま使えないので変換
                    if (segment.Equals("local")) segment = "LCL";
                    if (segment.Equals("argument")) segment = "ARG";
                    else segment = segment.ToUpper();

                    myWriter("@" + segment);
                    myWriter("D=M");     //D = M[segment] = ベースアドレス
                    myWriter("@" + index);
                    myWriter("A=D+A");   //A = ベースアドレス + インデックス
                    myWriter("D=M");     //D = M[base + index]
                }
				else if (memMappedSegments.Contains(segment))
				{
					int address = 0;
					if (segment.Equals("pointer")) address = 3 + index;
					if (segment.Equals("temp")) address = 5 + index;
					myWriter("@" + address);
                    myWriter("D=M");     //D = M[base + index]
                }
                else if (segment.Equals("constant"))
                {
                    myWriter("@" + index);
                    myWriter("D=A");     // D = index
                }
                else if (segment.Equals("static"))
                {
                    myWriter("@" + m_FunctionName + "." + index);    //Aにラベルのアドレスを入れる
                    myWriter("D=M");     //D = M[ラベルのアドレス]
                }

				//スタックにDをロードする
				myWriter("@SP");
				myWriter("A=M");	// A = M[SP]
				myWriter("M=D");	// M[M[SP]] = D
				
				//スタックポインタを１つ上げる
				myWriter("@SP");
				myWriter("M=M+1");	//M[SP] = M[SP] + 1

			}
			else if (command == VMtranslator.Parser.CommandTypes.C_POP)
			{
				//スタックポインタを１つ下げる
				myWriter("@SP");
				myWriter("M=M-1");	//M[SP] = M[SP] - 1

				//スタックからDに読みだす
				myWriter("@SP");
				myWriter("A=M");	// A = M[SP]
				myWriter("D=M"); // D = M[M[SP]]

                //データをDから所定の場所へロードする。　Dは使えないので注意
                List<string> memInAdressSegments = new List<string>() { "local", "argument", "this", "that" };
                List<string> memMappedSegments = new List<string>() { "pointer", "temp" };
                if (memInAdressSegments.Contains(segment))
                {
                    //segmentの値がそのままマップされてないので変換
                    if (segment.Equals("local")) segment = "LCL";
                    if (segment.Equals("argument")) segment = "ARG";
                    else segment = segment.ToUpper();

                    //すごく冗長になってしまった。D= M[segment]+index がしたいだけ。
		               //segment -> base addressのテーブルを持っておくとindexをこの場で加算するだけでいいので早い
                    //DをM[R13]に退避
                    myWriter("@R13");
                    myWriter("M=D"); //M[R13]=D

                    //書き込み先アドレスの作成
                    myWriter("@" + segment);
                    myWriter("D=M");     //D = M[segment]
                    myWriter("@" + index);
                    myWriter("D=D+A");   //D = ベースアドレス + インデックス

                    //書き込み先アドレスをR14に退避
                    myWriter("@R14");
                    myWriter("M=D");     //M[R14]= D = ベースアドレス + インデックス

                    //Dに書き込む値を入れる
                    myWriter("@R13");
                    myWriter("D=M");     //D=M[R13]

                    //Aにアドレスを入れる
                    myWriter("@R14");
                    myWriter("A=M");     //A=M[R14]
                    //書き込む
                    myWriter("M=D");     //M[base + index] = D

                } else if (memMappedSegments.Contains(segment))
                {
					int address = 0;
					if (segment.Equals("pointer")) address = 3 + index;
					if (segment.Equals("temp")) address = 5 + index;
					myWriter("@" + address);
                    myWriter("M=D");     //M[base + index]=D
                }
                else if (segment.Equals("constant"))
                {
                    //即値にDを入れるなんてありえない。読み捨てる用途？
                }
                else if (segment.Equals("static"))
                {
                    myWriter("@" + m_FunctionName + "." + index);    // @ラベル
                    myWriter("M=D");     //M[ラベルのアドレス] = D
                }
			}
		}


        public void writeLabel(string label)
        {
            myWriter("("+ m_FunctionName + "$" + label + ")");
        }

        public void writeGoto(string label)
        {
            myWriter("@" + m_FunctionName + "$" + label);
            myWriter("0;JMP");
        }

        public void writeIf(string label)
        {
			//スタックポインタを１つ下げる
			myWriter("@SP");
			myWriter("M=M-1");   //M[SP] = M[SP] - 1

			//スタックからDに読みだす
			myWriter("@SP");
			myWriter("A=M"); // A = M[SP]
			myWriter("D=M"); // D = M[M[SP]]

			
			myWriter("@" + m_FunctionName + "$" + label);
			//0以外ならジャンプ、0ならジャンプしない
			myWriter("D;JNE");
		}
		public void writeFunction(string functionName, int numLocals)
		{
			//関数のlabelを作る。関数名はグローバル（かぶらない）
			myWriter("(" + functionName + ")");
			
			//numLocalsの数だけ
			//SPとLCLは同じアドレスを指している。これでローカル変数が初期化され、SPが正しい位置にいく
			for (int i = 0; i < numLocals; i++)
			{
				writePushPop(Parser.CommandTypes.C_PUSH, "constant", 0);

			}
		}

		public void writeReturn()
		{
			//HACK:教科書p179通りにやったけど大分短縮できそう
			//ただpush,popがR13,R14を使うので教科書と順番入れ替えてる。まずポップ


			//*ARG = pop()　
			writePushPop(Parser.CommandTypes.C_POP, "argument", 0);

			//M[LCL-5]にFRAMEがあるので、とってくる	
			myWriter("@LCL");
			myWriter("D=M");     // D = M[LCL]
			//FRAMEは一時退避
			myWriter("@R13");
			myWriter("M=D");     // M[R13] = D 
			//RETは一時退避
			myWriter("@5");		// A = 5
			myWriter("A=D-A");   // A = M[LCL]-5
			myWriter("D=M");     // D = M[M[LCL]-5]
			myWriter("@R14");
			myWriter("M=D");     // M[R14]=D

			//SP = ARG+1
			myWriter("@ARG");	// A = ARG
			myWriter("D=M+1");	// D = M[ARG+1
			myWriter("@SP");     // A = SP
			myWriter("M=D");     // M[SP]=D

			//THAT = *(FRAM-1)
			myWriter("@R13");
			myWriter("AM=M-1");		// A,M[R13]=M[R13] - 1
			myWriter("D=M");		// D = M[ M[R13] - 1 ]
			myWriter("@THAT");
			myWriter("M=D");     // M[THAT] = D

			//THIS = *(FRAM-2)
			myWriter("@R13");
			myWriter("AM=M-1");     // A,M[R13]=M[R13] - 1
			myWriter("D=M");        // D = M[ M[R13] - 1 ]
			myWriter("@THIS");
			myWriter("M=D");     // M[THIS] = D

			//ARG = *(FRAM-3)
			myWriter("@R13");
			myWriter("AM=M-1");     // A,M[R13]=M[R13] - 1
			myWriter("D=M");        // D = M[ M[R13] - 1 ]
			myWriter("@ARG");
			myWriter("M=D");     // M[ARG] = D

			//ARG = *(FRAM-4)
			myWriter("@R13");
			myWriter("AM=M-1");     // A,M[R13]=M[R13] - 1
			myWriter("D=M");        // D = M[ M[R13] - 1 ]
			myWriter("@LCL");
			myWriter("M=D");     // M[LCL = D

			//リターンする
			myWriter("@R14");
			myWriter("A=M");     // A=M[R14]
			myWriter("0;JMP");	

		}
		public void writeCall(string functionName, int numArgs)
		{
			callnum++;
			//リターンアドレスを保存する
				myWriter("@" + functionName + "_FUNC_CALL" + callnum);
				myWriter("D=A"); // D = RET
				//スタックにDをロードする
				myWriter("@SP");
				myWriter("A=M"); // A = M[SP]
				myWriter("M=D"); // M[M[SP]] = D
				//スタックポインタを１つ上げる
				myWriter("@SP");
				myWriter("M=M+1");   //M[SP] = M[SP] + 1

			//LCLを保存する
				myWriter("@LCL");
				myWriter("D=M"); // D=M[xxx]　xxxの値が入る
				// スタックにDをロードする
				myWriter("@SP");
				myWriter("A=M"); // A = M[SP]
				myWriter("M=D"); // M[M[SP]] = D
				//スタックポインタを１つ上げる
				myWriter("@SP");
				myWriter("M=M+1");   //M[SP] = M[SP] + 1

			//ARGを保存する
				myWriter("@ARG");
				myWriter("D=M"); // D=M[xxx]　xxxの値が入る
				// スタックにDをロードする
				myWriter("@SP");
				myWriter("A=M"); // A = M[SP]
				myWriter("M=D"); // M[M[SP]] = D
				//スタックポインタを１つ上げる
				myWriter("@SP");
				myWriter("M=M+1");   //M[SP] = M[SP] + 1

			//THISを保存する
				myWriter("@THIS");
				myWriter("D=M"); // D=M[xxx]　xxxの値が入る
				// スタックにDをロードする
				myWriter("@SP");
				myWriter("A=M"); // A = M[SP]
				myWriter("M=D"); // M[M[SP]] = D
				//スタックポインタを１つ上げる
				myWriter("@SP");
				myWriter("M=M+1");   //M[SP] = M[SP] + 1

			//THATを保存する
				myWriter("@THAT");
				myWriter("D=M"); // D=M[xxx]　xxxの値が入る
												// スタックにDをロードする
				myWriter("@SP");
				myWriter("A=M"); // A = M[SP]
				myWriter("M=D"); // M[M[SP]] = D
												//スタックポインタを１つ上げる
				myWriter("@SP");
				myWriter("M=M+1");   //M[SP] = M[SP] + 1

			//ARGを引数の先頭に移す。ARG=SP - (numArgs + 5)
			myWriter("@SP");
			myWriter("D=A"); // D=SP
			int temp = numArgs + 5;
			myWriter("@"+temp); // A =numArgs + 5
			myWriter("D=D-A"); // D=SP - A
			myWriter("@ARG");
			myWriter("M=D"); // M[ARG] = D

			//LCLをSPのところに移す
			myWriter("@SP");
			myWriter("D=A"); // HACK:上の処理と合体できそう
			myWriter("@LCL");
			myWriter("M=D"); // M[LCL] = D

			//関数へジャンプする
			writeGoto(functionName);

			//リターンアドレスのためのラベルを宣言
			myWriter("(" + functionName + "_FUNC_CALL" + callnum + ")");
		}

		int romRowNum = 0;
		public void myWriter(string line)
		{
			//呼び出し元のメソッドを取得する 
			StackFrame callerFrame = new StackFrame(1);
			string methodName1 = callerFrame.GetMethod().Name;

			callerFrame = new StackFrame(2);
			string methodName2 = callerFrame.GetMethod().Name;

			m_file_writer.WriteLine(line + "\t\t\t//#{0:d4}\t<-\t" + methodName1 + "\t<-\t"+ methodName2,romRowNum);

			//L令ならROM行数カウントアップしない
			Regex r = new Regex(@"^\(([^;=\(\)]+)\)$");
			Match m = r.Match(line);
			if (!m.Success)
			{

				romRowNum++;
			}
		}

	}
}
