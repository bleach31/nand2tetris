using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VMtranslator
{
	class CodeWriter
	{
		StreamWriter m_file_writer;
		string m_filenamewithoutExtention;
		int callnum;
		/// <summary>
		/// 出力ファイル/ストリームを開きパースを行う準備をする
		/// </summary>
		/// <param name="file">入力ファイルパス</param>
		public CodeWriter(String filepath)
		{
			callnum = 0;
			m_filenamewithoutExtention = Path.GetFileNameWithoutExtension(filepath);
			m_file_writer = new StreamWriter(filepath);

		}

		public void close() {
			m_file_writer.Close();
		}
		/// <summary>
		/// 与えられた算術コマンドをアセンブリコードに変換し、それを書き込む
		/// </summary>
		/// <param name="command"></param>
		public void WriteArithmetic(string command)
		{
			callnum++;
			//一つ目をR13に取り出す negとnot以外は2項演算なので必要
			if (!command.Equals("neg") && !command.Equals("not")) {
				m_file_writer.WriteLine("@SP");		//SP=258						//M[258]=null.M[257]=x,SP[256]=y
				m_file_writer.WriteLine("M=M-1");	//SPの更新　M[SP] = M[SP] - 1	//SP=257
				m_file_writer.WriteLine("A=M");		//A=M[SP]
				m_file_writer.WriteLine("D=M");		//D=M[M[SP]-1]  = y
				//xをR13へ退避
				m_file_writer.WriteLine("@R13");
				m_file_writer.WriteLine("M=D");		//M[R13]= y
			}
			//二つ目をDに取り出す
			m_file_writer.WriteLine("@SP");		//SP=257
			//m_file_writer.WriteLine("M=M-1");	//どうせpushするのでSPを更新しない
			m_file_writer.WriteLine("A=M-1");	//A=M[SP]-1		
			m_file_writer.WriteLine("D=M");		//D=M[M[SP]-1
			
			//要注意　二項演算　M[R13]=y,D=x　単項演算　D=y
			switch (command)
			{
				case "add":
					m_file_writer.WriteLine("@R13");
					m_file_writer.WriteLine("D=D+M");	//D= x + y
					break;
				case "sub":
					m_file_writer.WriteLine("@R13");
					m_file_writer.WriteLine("D=D-M");	//D= x - y
					break;
				case "neg":
					m_file_writer.WriteLine("D=-D");	//D= !y
					break;
				case "eq":
					m_file_writer.WriteLine("@R13");
					m_file_writer.WriteLine("D=D-M");	//D= x - y
					m_file_writer.WriteLine("@" + m_filenamewithoutExtention + "_EQ_FALSE_CALL"+callnum);	
					//HACK: なぜ偽でジャンプするように書いてしまったんだ・・・普通逆だろ
					m_file_writer.WriteLine("D;JNE");	//if D!=0 jump
					//真の場合
					m_file_writer.WriteLine("D=-1");	//D= true
					m_file_writer.WriteLine("@" + m_filenamewithoutExtention + "_EQ_END_CALL" + callnum);	
					m_file_writer.WriteLine("0;JMP");	//無条件ジャンプ
					//偽の場合
					m_file_writer.WriteLine("(" + m_filenamewithoutExtention + "_EQ_FALSE_CALL" + callnum + ")");	
					m_file_writer.WriteLine("D=0");		//D= false 

					m_file_writer.WriteLine("(" + m_filenamewithoutExtention + "_EQ_END_CALL" + callnum + ")");	
					break;
				case "gt":
					m_file_writer.WriteLine("@R13");
					m_file_writer.WriteLine("D=D-M");	//D= x - y
					m_file_writer.WriteLine("@" + m_filenamewithoutExtention + "_GT_FALSE_CALL"+callnum);	
					m_file_writer.WriteLine("D;JLE");	//if D <= 0 jump

					m_file_writer.WriteLine("D=-1");	//D= true
					m_file_writer.WriteLine("@" + m_filenamewithoutExtention + "_GT_END_CALL"+callnum);	
					m_file_writer.WriteLine("0;JMP");	//無条件ジャンプ

					m_file_writer.WriteLine("(" + m_filenamewithoutExtention + "_GT_FALSE_CALL" + callnum + ")");	
					m_file_writer.WriteLine("D=0");	//D= false

					m_file_writer.WriteLine("(" + m_filenamewithoutExtention + "_GT_END_CALL" + callnum + ")");	
					break;
				case "lt":
					m_file_writer.WriteLine("@R13");
					m_file_writer.WriteLine("D=D-M");	//D= x - y
					m_file_writer.WriteLine("@" + m_filenamewithoutExtention + "_LT_FALSE_CALL"+callnum);	
					m_file_writer.WriteLine("D;JGE");	//if D >= 0 偽

					m_file_writer.WriteLine("D=-1");	//D= true
					m_file_writer.WriteLine("@" + m_filenamewithoutExtention + "_LT_END_CALL"+callnum);	
					m_file_writer.WriteLine("0;JMP");	//無条件ジャンプ

					m_file_writer.WriteLine("(" + m_filenamewithoutExtention + "_LT_FALSE_CALL" + callnum + ")");	
					m_file_writer.WriteLine("D=0");	//D= false

					m_file_writer.WriteLine("(" + m_filenamewithoutExtention + "_LT_END_CALL" + callnum + ")");	
					break;
				case "and":
					m_file_writer.WriteLine("@R13");
					m_file_writer.WriteLine("D=D&M");	//D= x And y
					break;
				case "or":
					m_file_writer.WriteLine("@R13");
					m_file_writer.WriteLine("D=D|M");	//D= x + y
					break;
				case "not":
					m_file_writer.WriteLine("D=!D");	//D= !y
					break;

				default:
                        break;
			}
			//スタックへ戻す
			m_file_writer.WriteLine("@SP");										//SP=257
			m_file_writer.WriteLine("A=M-1");	//A=M[SP]-1						//A=256
			m_file_writer.WriteLine("M=D");		//M[M[SP]-1]=D			
		}

		/// <summary>
		/// C_PUSH または C_POP コマンドをアセンブリコードに変換し、それを書き込む
		/// </summary>
		/// <param name="command"></param>
		/// <param name="segment"></param>
		/// <param name="index"></param>
		public void writePushPop(VMtranslator.Parser.CommandTypes command, string segment, int index)
		{
			callnum++;
			if (command == VMtranslator.Parser.CommandTypes.C_PUSH)
			{
				//データをDにロードする
				List<string> memMappedSegments = new List<string>(){"local","argument","this","that","pointer","temp"}; 
				if (memMappedSegments.Contains(segment))
				{
					//segmentの値がそのままマップされてないので変換
					if (segment.Equals("local")) segment = "LCL";
					if (segment.Equals("argument")) segment = "ARG";
					if (segment.Equals("pointer")) segment = "3";
					if (segment.Equals("temp")) segment = "5";
					else segment = segment.ToUpper();


					m_file_writer.WriteLine("@" + segment);
					m_file_writer.WriteLine("D=A");		//D = ベースアドレス
					m_file_writer.WriteLine("@" + index);
					m_file_writer.WriteLine("A=D+A");	//A = ベースアドレス + インデックス
					m_file_writer.WriteLine("D=M");		//D = M[base + index]
				}
				else if (segment.Equals("constant"))
				{
					m_file_writer.WriteLine("@" + index);
					m_file_writer.WriteLine("D=A");		// D = index
				}
				else if (segment.Equals("static")) 
				{
					m_file_writer.WriteLine("@" + m_filenamewithoutExtention + "."+ index);	//Aにラベルのアドレスを入れる
					m_file_writer.WriteLine("D=M");		//D = M[ラベルのアドレス]
				}

				//スタックにDをロードする
				m_file_writer.WriteLine("@SP");
				m_file_writer.WriteLine("A=M");	// A = M[SP]
				m_file_writer.WriteLine("M=D");	// M[M[SP]] = D
				
				//スタックポインタを１つ上げる
				m_file_writer.WriteLine("@SP");
				m_file_writer.WriteLine("M=M+1");	//M[SP] = M[SP] + 1

			}
			else if (command == VMtranslator.Parser.CommandTypes.C_POP)
			{
				//スタックポインタを１つ下げる
				m_file_writer.WriteLine("@SP");
				m_file_writer.WriteLine("M=M-1");	//M[SP] = M[SP] - 1

				//スタックからDに読みだす
				m_file_writer.WriteLine("@SP");
				m_file_writer.WriteLine("A=M");	// A = M[SP]
				m_file_writer.WriteLine("D=M");	// D = M[M[SP]]

				//データをDから所定の場所へロードする。　Dは使えないので注意
				List<string> memMappedSegments = new List<string>() { "local", "argument", "this", "that", "pointer", "temp" };
				if (memMappedSegments.Contains(segment))
				{
					//segmentの値がそのままマップされてないので変換
					if (segment.Equals("local")) segment = "LCL";
					if (segment.Equals("argument")) segment = "ARG";
					if (segment.Equals("pointer")) segment = "3";
					if (segment.Equals("temp")) segment = "5";
					else segment = segment.ToUpper();

					//すごく冗長になってしまった。M[base+index] = Dがしたいだけ。
					//segment -> base addressを持っといてindexをこの場で加算が早い
					//DをM[R13]に退避
					m_file_writer.WriteLine("@R13");
					m_file_writer.WriteLine("M=D");	//M[R13]=D

					//書き込み先アドレスの作成
					m_file_writer.WriteLine("@" + segment);
					m_file_writer.WriteLine("D=A");		//D = ベースアドレス
					m_file_writer.WriteLine("@" + index);
					m_file_writer.WriteLine("D=D+A");	//D = ベースアドレス + インデックス

					//書き込み先アドレスをR14に退避
					m_file_writer.WriteLine("@R14");
					m_file_writer.WriteLine("M=D");		//M[R14]=D(base + index)

					//Dに書き込む値を入れる
					m_file_writer.WriteLine("@R13");
					m_file_writer.WriteLine("M=D");		//D=M[R13]

					//Aにアドレスを入れる
					m_file_writer.WriteLine("@R14");
					//書き込む
					m_file_writer.WriteLine("M=D");		//M[base + index] = D

				}
				else if (segment.Equals("constant"))
				{
					//即値にDを入れるなんてありえない。読み捨てる用途？
				}
				else if (segment.Equals("static"))
				{
					m_file_writer.WriteLine("@" + m_filenamewithoutExtention + "." + index);	// @ラベル　A ← ラベルのアドレス
					m_file_writer.WriteLine("M=D");		//M[ラベルのアドレス] = D
				}
			}
		}

	}
}
