using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

namespace Assembler
{
	class Parser
	{
		StreamReader m_file;

		public String line;

		public commandtypes commandtype { get;  set; }
		public String symbol { get;  set; }
		public String dest { get; set; }
		public String comp { get; set; }
		public String jump { get; set; }
		public enum commandtypes
		{
	         A_COMMAND,C_COMMAND,L_COMMAND,OTHER
		}

		/// <summary>
		/// 入力ファイル/ストリームを開きパースを行う準備をする
		/// </summary>
		/// <param name="file">入力ファイルパス</param>
		public Parser(String filepath)
		{
			m_file = new StreamReader(filepath);
		}

		/// <summary>
		/// 入力にまだコマンドが存在するか？
		/// </summary>
		/// <returns></returns>
		public bool hasMoreCommands() {
			commandtype = commandtypes.OTHER;
			symbol = "";
			dest = "";
			comp = "";
			jump = "";
			while ((line = m_file.ReadLine()) != null)
			{				
				//コメントの削除
				Regex r = new Regex("//.*$");
				line = r.Replace(line,"");
				//前後の空白の削除
				//HACK:文中含め、ての空白を削除したほうがいい
				line = line.Trim();

				//HACK: \wがHack言語と厳密に一致しないけど省略。むしろ日本語をラベルに使えるので上位互換

				//A命令
				r = new Regex(@"^@([\w]+)$");
				Match m = r.Match(line);
				if(m.Success)
				{
					symbol  = m.Groups[1].Value;
					commandtype = commandtypes.A_COMMAND;
					return true;
				}
				
				//C命令
				r = new Regex(@"^(([^;=]+)=)?([^;=]+)(;([^;=]+))?$");
				m = r.Match(line);
				if (m.Success)
				{
					//(0|1|-1|D|A|!D|!A|-D|-A|D+1|A+1|D-1|A-1|D+A|D-A|A-D|D&A|D\|A|M|!M|M+1|M-1|D+M|D-M|M-D|D&M|D\|M|)
					//HACK:dest,comp,jump中身チェック
					dest = m.Groups[2].Value;
					comp = m.Groups[3].Value;
					jump = m.Groups[5].Value;
					commandtype = commandtypes.C_COMMAND;
					return true;
				}
				
				//ラベルシンボル
				r = new Regex(@"^(\w+)$");
				m = r.Match(line);
				if (m.Success) {
					symbol = m.Groups[1].Value;
					commandtype = commandtypes.L_COMMAND;
					return true;
				}
			}
			return false;
		}
		/*
		/// <summary>
		/// 入力から次のコマンドを読み、それを現在のコマンドにする。
		/// このルーチンはhasMoreCommands() が true の場合のみ呼ぶようにする。最初は現コマンドは空である
		/// </summary>
		public void advance() {
			//これいらねぇ・・・
			}
		}

		/// <summary>
		/// 現コマンドの種類を返す。
		/// ● A_COMMAND は@Xxx を意味し、Xxxはシンボルか 10 進数の数値である
		/// ● C_COMMAND は dest=comp;jumpを意味する
		/// ● L_COMMAND は擬似コマンドであり、(Xxx) を意味する。Xxx はシンボルである
		/// </summary>
		/// <returns></returns>
		public commandtypes commandType()
		{
			return m_commandtype;
		
		}
		/// <summary>
		/// 現コマンド@Xxx または (Xxx) の Xxxを 返 す 。Xxx は シ ン ボ ル ま た は 10進 数 の 数 値 で あ る 。
		/// こ の ル ー チ ン はcommandType() が A_COMMAND または L_COMMAND のときだけ呼ぶようにする
		/// </summary>
		/// <returns></returns>
		public string symbol() {
		
		}
		/// <summary>
		/// 現 C 命令の dest ニーモニックを返す（候補として 8 つの可能性がある）。
		/// このルーチンは commandType() がC_COMMAND のときだけ呼ぶようにする
		/// </summary>
		/// <returns></returns>
		public string dest() {
		
		}
		/// <summary>
		/// 現 C 命令の comp ニーモニックを返す（候補として 28 個の可能性がある）。
		/// このルーチンは commandType() がC_COMMAND のときだけ呼ぶようにする
		/// </summary>
		/// <returns></returns>
		public string comp()
		{

		}
		/// <summary>
		/// 現 C 命令の jump ニーモニックを返す（候補として 8 つの可能性がある）。
		/// このルーチンは commandType() がC_COMMAND のときだけ呼ぶようにする
		/// </summary>
		/// <returns></returns>
		public string jump()
		{

		}
		*/
	}
}
