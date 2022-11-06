using System ;
using System.Collections ;
using System.Collections.Generic ;
using System.Text ;

using UnityEngine ;

using Cysharp.Threading.Tasks ;

using uGUIHelper ;
using MathHelper ;

namespace DSW.Screens
{
	public partial class Sample : ScreenBase
	{


		//-------------------------------------------------------------------------------------------

		override protected void  OnAwake()
		{
		}

		override protected async UniTask OnStart()
		{

			//----------------------------------------------------------

			// フェードインを許可する
			Scene.Ready = true ;

			// フェード完了を待つ
			await Scene.WaitForFading() ;

			//----------------------------------------------------------

			Parse() ;
		}

		//-------------------------------------------------------------------------------------------

		public class Token
		{
			public string	Word ;		// 文字
			public int		Line ;		// 行
			public int		Column ;	// 列
		}

		// 連続トークン
		private static char[] m_TokenWords =
		{
			'0', '1', '2', '3', '4', '5', '6', '7', '8', '9',
			'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l', 'm', 'n', 'o', 'p', 'q', 'r', 's', 't', 'u', 'v', 'w', 'x', 'y', 'z',
			'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M', 'N', 'O', 'P', 'Q', 'R', 'S', 'T', 'U', 'V', 'W', 'X', 'Y', 'Z',
			'_'
		} ;

		// 単独トークン
		private static char[] m_TokenSigns =
		{
			'=', 
			'+', '-', '*', '/', '!', '#', '$', '%', '&', '^', '~', '|', '@', '<', '>', '.', ',', '?',
			'(', ')', '{', '}', '[', ']', ':', ';',
		} ;


		private void Parse()
		{
			string path = "js_sample" ;

			var ta = Resources.Load<TextAsset>( path ) ;
			if( ta == null || ta.bytes == null || ta.bytes.Length == 0 )
			{
				Debug.Log( "Can not load : Path = " + path ) ;
				return ;
			}

			//------------------------------------------------------------------------------------------

			Debug.Log( "パース開始" ) ;


			byte[] bytes = ta.bytes ;

			string texts ;

			// UTF-8のシグネチャがあれば除去する
			if( Match( ref bytes, 0, 0xEF, 0xBB, 0xBF ) == true )
			{
				// UTF-8のシグネチャがある
				texts = Encoding.UTF8.GetString( bytes, 3, bytes.Length - 3 ) ;
			}
			else
			{
				// UTF-8のシグネチャがない
				texts = ta.text ;
			}

			int inString = 0 ;			// 文字列の外側か内側か(0=外側・1=シングルの内側・2=ダブルの内側)
			int inComment = 0 ;			// コメントの内側か外側か(0=外側・1=シングルの内側・2=マルチの内側)

			int	line = 1 ;				// 現在の行
			int	column = 1 ;
			int tabCount = 4 ;

			int word_Offset = -1 ;			// 文字列の開始位置の場所
			int word_Column	=  1 ;			// 文字列の開始位置の列数

			char code ;

			int i, l = texts.Length ;
			int o = 0 ;


			//------------------------------------------------------------------------------------------
			// トークン単位にパースする

			List<Token> tokens = new List<Token>() ;

			for( i  = o ; i <  l ; i ++ )
			{
				if( inComment == 0 )
				{
					// コメント外

					if( inString == 0 )
					{
						// 文字列外

						code = texts[ i ] ;

						//-------------------------------

						if( Match( ref texts, i, "//" ) == true )
						{
							// シングルのコメント開始(改行まで有効)
							inComment = 1 ;
							i += 1 ;	// ２文字なので補正
							continue ;
						}
						
						if( Match( ref texts, i, "/*" ) == true )
						{
							// マルチのコメント開始(終端記号まで有効)
							inComment = 2 ;
							i += 1 ;	// ２文字なので補正
							continue ;
						}
						
						if( code == '\'' )
						{
							// シングルの文字列開始
							inString = 1 ;

							word_Offset = i ;
							word_Column = column ;

							column ++ ;

							continue ;
						}
						
						if( code == '"' )
						{
							// ダブルの文字列開始
							inString = 2 ;

							word_Offset = i ;
							word_Column = column ;

							column ++ ;

							continue ;
						}

						//-------------------------------
						// 改行

						int isReturn = 0 ;
						if( Match( ref texts, i, ( char )0x0D, ( char )0x0A ) == true )
						{
							isReturn = 2 ;
						}
						else
						if( ( code == 0x0A ) || ( code == 0x0D ) )
						{
							isReturn = 1 ;
						}

						if( isReturn >  0 )
						{
							// 改行を発見したので文字列取得中であれば終了する
							if( word_Offset >= 0 )
							{
								AddToken( ref tokens, ref texts, word_Offset, i - 1, line, word_Column ) ;
								word_Offset = -1 ;
							}

							//------------------------------

							line ++ ;
							column = 1 ;

							i += ( isReturn - 1 ) ;

							continue ;
						}

						//-------------------------------------------------------
						// 英数＿が集合とみなす

						if( word_Offset <  0 )
						{
							// トークン外

							if( code != ' ' && code != '\t' )
							{
								// スペースとタブではない
								if( m_TokenWords.Contains( code ) == true )
								{
									// 英数
									word_Offset = i ;	// トークンチェック開始
									word_Column = column ;

									column ++ ;

									continue ;
								}
								else
								if( m_TokenSigns.Contains( code ) == true )
								{
									// 記号

									// 複合記号とマッチする場合はそちらで記録する
									// == != && || ++ -- += -= *= /= %= ~= << >> 

									string sign = Match( ref texts, i, "==", "!=", "&&", "||", "++", "--", "+=", "-=", "*=", "/=", "%=", "~=", "^=", "<<", ">>" ) ;

									if( string.IsNullOrEmpty( sign ) == false )
									{
										// 複合で終了
										AddToken( ref tokens, sign, line, column ) ;

										column += sign.Length ;

										i += ( sign.Length - 1 ) ;	// ２文字以上なので補正
									}
									else
									{
										// 単体で終了
										AddToken( ref tokens, ref texts, i, i, line, column ) ;

										column ++ ;
									}

									continue ;
								}
								else
								{
									// 非対応文字であっても一応扱う
									word_Offset = i ;	// トークンチェック開始
									word_Column = column ;

									if( code >= 0x0020 && code <= 0x00FF )
									{
										// 半角
										column ++ ;
									}
									else
									if( code >= 0x0100 )
									{
										// 全角
										column += 2 ;
									}

									continue ;
								}
							}
							else
							{
								if( code == ' ')
								{
									// スペース
									column ++ ;
								}
								else
								if( code == '\t' )
								{
									// タブ
									int tab = ( column - 1 ) % tabCount ;
									column += ( tabCount - tab ) ;
								}

								continue ;
							}
						}
						else
						{
							// トークン内

							if( code != ' ' && code != '\t' )
							{
								// スペースとタブではない
								if( m_TokenWords.Contains( code ) == true )
								{
									// 英数
									column ++ ;

									continue ;
								}
								else
								if( m_TokenSigns.Contains( code ) == true )
								{
									// 記号

									// 英数トークン記録
									AddToken( ref tokens, ref texts, word_Offset, i - 1, line, word_Column ) ;
									word_Offset = -1 ;

									//----------------------------

									string sign = Match( ref texts, i, "==", "!=", "&&", "||", "++", "--", "+=", "-=", "*=", "/=", "%=", "~=", "^=", "<<", ">>" ) ;

									if( string.IsNullOrEmpty( sign ) == false )
									{
										// 複合で終了
										AddToken( ref tokens, sign, line, column ) ;

										column += sign.Length ;

										i += ( sign.Length - 1 ) ;	// ２文字以上なので補正
									}
									else
									{
										// 単体で終了
										AddToken( ref tokens, ref texts, i, i, line, column ) ;

										column ++ ;
									}

									continue ;
								}
								else
								{
									// その他
									if( code >= 0x0020 && code <= 0x00FF )
									{
										// 半角
										column ++ ;
									}
									else
									if( code >= 0x0100 )
									{
										// 全角
										column += 2 ;
									}

									continue ;
								}
							}
							else
							{
								// スペースまたはタブの場合

								// トークン記録
								AddToken( ref tokens, ref texts, word_Offset, i - 1, line, word_Column ) ;
								word_Offset = -1 ;

								if( code == ' ')
								{
									// スペース
									column ++ ;
								}
								else
								if( code == '\t' )
								{
									// タブ
									int tab = ( column - 1 ) % tabCount ;
									column += ( tabCount - tab ) ;
								}
							}
						}
					}
					else
					if( inString == 1 )
					{
						// シングルの文字列中

						code = texts[ i ] ;

						//-------------------------------
						// 改行

						int isReturn = 0 ;
						if( Match( ref texts, i, ( char )0x0D, ( char )0x0A ) == true )
						{
							isReturn = 2 ;
						}
						else
						if( ( code == 0x0A ) || ( code == 0x0D ) )
						{
							isReturn = 1 ;
						}

						if( isReturn >  0 )
						{
							// 閉じられる前の改行はシンタックスエラー
							Debug.LogWarning( "文字列が確定する前に改行になっています" ) ;
							return ;
						}

						//-------------------------------

						if( code == '\'' )
						{
							// シングルの文字列終了
							inString = 0 ;

							// トークンに文字列を格納する
							AddToken( ref tokens, ref texts, word_Offset, i, line, word_Column ) ;
							word_Offset = -1 ;

							column ++ ;

							continue ;
						}
						
						if( code == '\t' )
						{
							// タブ
							int tab = ( column - 1 ) % tabCount ;
							column += ( tabCount - tab ) ;

							continue ;
						}

						if( code >= 0x0020 && code <= 0x00FF )
						{
							// 半角
							column ++ ;
						}
						else
						if( code >= 0x0100 )
						{
							// 全角
							column += 2 ;
						}

						// コントロールコードは無視する

						continue ;
					}
					else
					if( inString == 2 )
					{
						// ダブルの文字列中

						code = texts[ i ] ;

						//-------------------------------
						// 改行

						int isReturn = 0 ;
						if( Match( ref texts, i, ( char )0x0D, ( char )0x0A ) == true )
						{
							isReturn = 2 ;
						}
						else
						if( ( code == 0x0A ) || ( code == 0x0D ) )
						{
							isReturn = 1 ;
						}

						if( isReturn >  0 )
						{
							// 閉じられる前の改行はシンタックスエラー
							Debug.LogWarning( "文字列が確定する前に改行になっています" ) ;
							return ;
						}

						//-------------------------------

						if( code == '"' )
						{
							// ダブルの文字列終了
							inString = 0 ;

							// トークンに文字列を格納する
							AddToken( ref tokens, ref texts, word_Offset, i, line, word_Column ) ;
							word_Offset = -1 ;

							column ++ ;

							continue ;
						}

						if( code == '\t' )
						{
							// タブ
							int tab = ( column - 1 ) % tabCount ;
							column += ( tabCount - tab ) ;

							continue ;
						}

						if( code >= 0x0020 && code <= 0x00FF )
						{
							// 半角
							column ++ ;
						}
						else
						if( code >= 0x0100 )
						{
							// 全角
							column += 2 ;
						}

						// コントロールコードは無視する

						continue ;
					}
				}
				else
				if( inComment == 1 )
				{
					// シングルのコメント中

					code = texts[ i ] ;

					//--------------------------------
					// 改行

					int isReturn = 0 ;
					if( Match( ref texts, i, ( char )0x0D, ( char )0x0A ) == true )
					{
						isReturn = 2 ;
					}
					else
					if( ( code == 0x0A ) || ( code == 0x0D ) )
					{
						isReturn = 1 ;
					}

					if( isReturn >  0 )
					{
						// 改行を発見したのでシングルのコメントを終了する
						inComment = 0 ;

						line ++ ;
						column = 1 ;

						i += ( isReturn - 1 ) ;

						continue ;
					}
				}
				else
				if( inComment == 2 )
				{
					// マルチのコメント中

					code = texts[ i ] ;

					//--------------------------------

					if( Match( ref texts, i, "*/" ) == true )
					{
						// 終端を発見したのでマルチのコメントを終了する
						inComment = 0 ;

						column += 2 ;

						i += 1 ;

						continue ;
					}

					//--------------------------------
					// 改行

					int isReturn = 0 ;
					if( Match( ref texts, i, ( char )0x0D, ( char )0x0A ) == true )
					{
						isReturn = 2 ;
					}
					else
					if( ( code == 0x0A ) || ( code == 0x0D ) )
					{
						isReturn = 1 ;
					}

					if( isReturn >  0 )
					{
						line ++ ;
						column = 1 ;

						i += ( isReturn - 1 ) ;

						continue ;
					}

					//--------------------------------
					// タブ

					if( code == '\t' )
					{
						int tab = ( column - 1 ) % tabCount ;
						column += ( tabCount - tab ) ;
					}
				}
			}

			if( inString >  0 )
			{
				Debug.LogWarning( "文字列が確定していません" ) ;
				return ;
			}

			if( inComment == 2 )
			{
				Debug.LogWarning( "コメントが確定していません" ) ;
				return ;
			}

			Debug.Log( "パース終了 : " + tokens.Count ) ;

			if( tokens.Count >  0 )
			{
				foreach( var token in tokens )
				{
					Debug.Log( token.Line + "行 " + token.Column + "列 : Token = <color=#FFFF00>" + token.Word + "</color>" ) ;
				}
			}
		}

		//-------------------------------------------------------------------------------------------

		// 指定したコードとマッチするか判定する
		private bool Match( ref byte[] bytes, int o, params byte[] codes )
		{
			int l = bytes.Length ;
			int m = codes.Length ;

			if( ( o + m ) >  l )
			{
				// オーバーするのでマッチはしない
				return false ;
			}

			int i ;
			for( i  = 0 ; i <  m ; i ++ )
			{
				if( bytes[ o + i ] != codes[ i ] )
				{
					// マッチはしない
					return false ;
				}
			}

			// マッチした
			return true ;
		}

		// 指定したコードとマッチするか判定する
		private bool Match( ref string texts, int o, params char[] codes )
		{
			int l = texts.Length ;
			int m = codes.Length ;

			if( ( o + m ) >  l )
			{
				// オーバーするのでマッチはしない
				return false ;
			}

			int i ;
			for( i  = 0 ; i <  m ; i ++ )
			{
				if( texts[ o + i ] != codes[ i ] )
				{
					// マッチはしない
					return false ;
				}
			}

			// マッチした
			return true ;
		}

		// 指定したコードとマッチするか判定する
		private bool Match( ref string texts, int o, string codes )
		{
			int l = texts.Length ;
			int m = codes.Length ;

			if( ( o + m ) >  l )
			{
				// オーバーするのでマッチはしない
				return false ;
			}

			int i ;
			for( i  = 0 ; i <  m ; i ++ )
			{
				if( texts[ o + i ] != codes[ i ] )
				{
					// マッチはしない
					return false ;
				}
			}

			// マッチした
			return true ;
		}

		// 指定したコードとマッチするか判定する
		private string Match( ref string texts, int o, params string[] words )
		{
			int l = texts.Length ;
			int i, m ;

			for( i  = 0 ; i <  words.Length ; i ++ )
			{
				m = words[ i ].Length ;

				if( ( o + m ) >  l )
				{
					// オーバーするのでマッチはしない
					continue ;
				}

				if( texts.Substring( o, m ) == words[ i ] )
				{
					// マッチした
					return words[ i ] ;
				}
			}

			// マッチしなかった
			return null ;
		}


		private void AddToken( ref List<Token> tokens, ref string texts, int os, int oe, int line, int column )
		{
			var token = new Token() ;

			token.Word		= texts.Substring( os, oe - os + 1 ) ;
			token.Line		= line ;
			token.Column	 = column ;

			tokens.Add( token ) ;
		}

		private void AddToken( ref List<Token> tokens, string word, int line, int column )
		{
			var token = new Token() ;

			token.Word		= word ;
			token.Line		= line ;
			token.Column	 = column ;

			tokens.Add( token ) ;
		}

	}
}
