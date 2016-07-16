using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace VMtranslator
{
	class Parser
	{
		FileStream m_file;
		StreamReader m_file_reader;
		public String line;
		public enum CommandTypes
		{
			C_ARITHMETIC, C_PUSH, C_POP, C_LABEL, C_GOTO, C_IF, C_FUNCTION, C_RETURN, C_CALL, OTHER
		};
		public CommandTypes commandtype;
		public String arg1;
		public int arg2;

		/// <summary>
		/// 入力ファイル/ストリームを開きパースを行う準備をする
		/// </summary>
		/// <param name="file">入力ファイルパス</param>
		public Parser(String filepath)
		{
			m_file = new FileStream(filepath, FileMode.Open);
			m_file_reader = new StreamReader(m_file);

		}
		public bool hasMoreCommands()
		{
			commandtype = CommandTypes.OTHER;
			//コマンドを見つけるorファイル末尾までいく、まで1行ずつ読む
			while ((line = m_file_reader.ReadLine()) != null)
			{
				//コメントの削除
				Regex r = new Regex(@"//.*$");
				line = r.Replace(line, "");
				//先頭、末尾の空白の削除
				line = line.Trim();

				//命令全体のパース
				//基本形は：command arg1 arg2
				//arg1とarg2はない場合があり、arg2は数字
				r = new Regex(@"^(\S+)( (\S+))?( ([0-9]+))?$");
				Match m = r.Match(line);
				if (m.Success) 
				{ 
					String command = m.Groups[1].Value;
					arg1 = m.Groups[3].Value;
					int.TryParse(m.Groups[5].Value,out arg2);

					//算術命令
					List<string> arithmetics = new List<string>(){"add","sub","neg","eq","gt","lt","and","or","not"}; 
					if (arithmetics.Contains(command))
					{
						commandtype = CommandTypes.C_ARITHMETIC;
						arg1 = command;	//。C_ARITHMETIC の場合、コマンド自体（add、sub など）が返される。
						return true;
					}
					else if (command.Contains("push"))
					{
						commandtype = CommandTypes.C_PUSH;
						return true;
					}
					else if (command.Contains("pop"))
					{
						commandtype = CommandTypes.C_POP;
						return true;
                    }
                    else if (command.Contains("label"))
                    {
                        commandtype = CommandTypes.C_LABEL;
                        //
                        return true;
                    }
                    else if (command.Contains("if-goto"))
                    {
                        commandtype = CommandTypes.C_IF;
                        //
                        return true;
                    }
                    else if (command.Contains("goto"))
                    {
                        commandtype = CommandTypes.C_GOTO;
                        //
                        return true;
                    }
                    else if (command.Contains("function"))
                    {
                        commandtype = CommandTypes.C_FUNCTION;
                        //
                        //
                        return true;
                    }
                    else if (command.Contains("return"))
                    {
                        commandtype = CommandTypes.C_RETURN;
                        return true;
                    }
                    else if (command.Contains("call"))
                    {
                        commandtype = CommandTypes.C_CALL;
                        //
                        //
                        return true;
                    }
                }
			}
			return false;
		}

	}
}
