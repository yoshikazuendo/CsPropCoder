using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CsPropCoder
{
	/// <summary>
	/// 本アプリケーションのルートクラスです。 
	/// </summary>
	class Program
	{
		/// <summary>
		/// アプリケーションを開始します。 
		/// </summary>
		/// <param name="args"></param>
		static void Main(string[] args)
		{
			// 例外を集約する。 
			Thread.GetDomain().UnhandledException += new UnhandledExceptionEventHandler(Program_UnhandledException);

			// コマンドの引数を取得する。 
			string filePath = string.Empty;
			if (args.Length == 1) {
				filePath = args[0];
				Console.Write("args：" + filePath);
			} else {
				throw new Exception(@"本バッチを使用する場合は、引数にプロパティを作成する元のテキストファイル名を指定してくださいです！");
			}

			if (File.Exists(filePath)) {
				string[] lines = File.ReadAllLines(filePath, Encoding.GetEncoding("Shift-JIS"));
				if (lines.Length >= 2) {
					// １行目はヘッダー 
					string[] headers = lines[0].Split('\t');
					if (headers.Length >= 2) {
						// ２行目以降をプロパティ化する情報としてゴリゴリ作っていく。 
						var props = new List<string>();
						foreach (var line in lines.Skip(1)) {
							string[] datas = line.Split('\t');

							// 補足説明を生成する。３列目以降は補足説明として、xmlドキュメントにゴリゴリ載せておく。 
							string tips = null;
							if (headers.Length > 2) {
								for (int i = 2; i < headers.Length; i++) {
									tips += "\r\n/// <para>" + headers[i] + "：" + datas[i] + "</para>";
								}
							}

							props.Add(string.Format(@"/// <summary>
/// {0}を取得・設定します。 {1}
/// </summary>
public string {2} {{ get; set; }}", datas[0], tips, datas[1]));
						}

						File.WriteAllText(Path.ChangeExtension(filePath, ".PropCoder.txt"), string.Join("\r\n\r\n", props.ToArray()), Encoding.UTF8);
					} else {
						// 説明とプロパティ名で最低２列。 
						Console.WriteLine("エラー！説明を示す列と、プロパティ名を示す列で最低２列必要です！何もしません。");
					}
				} else {
					Console.WriteLine("エラー！ファイルの行が１行以下なので何もしません。");
				}
			} else {
				Console.WriteLine("エラー！ファイルが存在しません。");
				return;
			}
		}

		/// <summary>
		/// Program_UnhandledException Event 
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		public static void Program_UnhandledException(object sender, UnhandledExceptionEventArgs e)
		{
			// 例外をコンソールに出力する。 
			var ex = e.ExceptionObject as Exception;
			if (ex != null) {
				Console.Write(ex.ToString());
			}

			Environment.Exit(0);
		}
	}
}