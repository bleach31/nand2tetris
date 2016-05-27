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
		FileStream m_file;
		StreamReader m_file_reader;

		public String line;
		/// <summary>
		/// 現コマンドの種類を返す。
		/// ● A_COMMAND は@Xxx を意味し、Xxxはシンボルか 10 進数の数値である
		/// ● C_COMMAND は dest=comp;jumpを意味する
		/// ● L_COMMAND は擬似コマンドであり、(Xxx) を意味する。Xxx はシンボルである
		/// </summary>
		/// <returns></returns>
		public commandtypes commandtype { get;  set; }
		/// <summary>
		/// 現コマンド@Xxx または (Xxx) の Xxxを 返 す 。Xxx は シ ン ボ ル ま た は 10進 数 の 数 値 で あ る 。
		/// こ の ル ー チ ン はcommandType() が A_COMMAND または L_COMMAND のときだけ呼ぶようにする
		/// </summary>
		/// <returns></returns>
		public String symbol { get;  set; }
		/// <summary>
		/// 現 C 命令の dest ニーモニックを返す（候補として 8 つの可能性がある）。
		/// このルーチンは commandType() がC_COMMAND のときだけ呼ぶようにする
		/// </summary>
		/// <returns></returns>
		public String dest { get; set; }
		/// <summary>
		/// 現 C 命令の comp ニーモニックを返す（候補として 28 個の可能性がある）。
		/// このルーチンは commandType() がC_COMMAND のときだけ呼ぶようにする
		/// </summary>
		/// <returns></returns>
		public String comp { get; set; }
		/// <summary>
		/// 現 C 命令の jump ニーモニックを返す（候補として 8 つの可能性がある）。
		/// このルーチンは commandType() がC_COMMAND のときだけ呼ぶようにする
		/// </summary>
		/// <returns></returns>
		public String jump { get; set; }
		enum commandtypes
		{
			A_COMMAND, C_COMMAND, L_COMMAND, OTHER
		}

		/// <summary>
		/// 入力ファイル/ストリームを開きパースを行う準備をする
		/// </summary>
		/// <param name="file">入力ファイルパス</param>
		public Parser(String filepath)
		{
			m_file = new FileStream(filepath,FileMode.Open);
			m_file_reader = new StreamReader(m_file);

			//ファイルの先頭から読み始める
			m_file.Seek(0,SeekOrigin.Begin);
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
			

			while ((line = m_file_reader.ReadLine()) != null)
			{				
				//コメントの削除
				Regex r = new Regex(@"//.*$");
				line = r.Replace(line,"");
				//空白の削除
				r = new Regex(@"\s*");
				line = r.Replace(line,"");

				//HACK: \wがHack言語と厳密に一致しないけど省略。[a-zA-Z0-9_]が正確か？
				//		\wだと日本語をラベルに使えるのでむしろ上位互換

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
		
	}
}
