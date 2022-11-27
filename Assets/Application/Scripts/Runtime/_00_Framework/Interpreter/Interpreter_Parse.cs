using System ;
using System.Collections ;
using System.Collections.Generic ;
using System.Text ;

using UnityEngine ;

using Cysharp.Threading.Tasks ;

using uGUIHelper ;
using MathHelper ;

namespace DSW
{
	/// <summary>
	/// パース
	/// </summary>
	public partial class Interpreter
	{
		/// <summary>
		/// 予約語
		/// </summary>
		public class SyntaxDescriptor
		{
			public string									SyntaxName ;
			public Func<List<Token>,int,List<object>,int>	SyntaxAdaptor ;

			public SyntaxDescriptor( string syntaxName, Func<List<Token>,int,List<object>,int> syntaxAdaptor )
			{
				SyntaxName		= syntaxName ;
				SyntaxAdaptor	= syntaxAdaptor ;
			}
		}


		private List<SyntaxDescriptor>	m_SyntaxDescriptors = new List<SyntaxDescriptor>()
		{
			new SyntaxDescriptor( "function", SyntaxAdaptor_Function )
		} ;

		//-------------------------------------------------------------------------------------------

		// パースする
		private List<object> Parse( List<Token> tokens )
		{
			Debug.Log( "トークンを解析する : " + tokens.Count ) ;

			// ノード格納
			List<object> nodes = new List<object>() ;

			int offset = 0 ;

			bool isSyntax ;

			while( true )
			{
				// 構文(予約語)かどうか判定する
				isSyntax = false ;
				foreach( var syntaxDescriptor in m_SyntaxDescriptors )
				{
					if( tokens[ offset ].Word == syntaxDescriptor.SyntaxName )
					{
						// 構文である
						offset = syntaxDescriptor.SyntaxAdaptor( tokens, offset, nodes ) ;
						isSyntax = true ;
						break ;
					}
				}

				if( isSyntax == false )
				{
					// 変数格納
					offset = SyntaxAdaptor_VariableSetter( tokens, offset, nodes ) ;
				}

				if( offset <  0 )
				{
					// 問題発生
					Debug.LogWarning( "構文のパースで問題が発生しました" ) ;
					break ;
				}

				offset ++ ;

				if( offset >= tokens.Count )
				{
					// パース終了
					break ;
				}
			}

			Debug.Log( "ノードの数:" + nodes.Count ) ;

			return nodes ;
		}

		//-------------------------------------------------------------------------------------------

		// 構文(変数格納)
		private static int SyntaxAdaptor_VariableSetter( List<Token> tokens, int offset, List<object> nodes )
		{
			int syntaxOffset = offset ;

			string variableName = tokens[ offset ].Word ;
			if( IsVariableName( variableName ) == false )
			{
				Debug.LogWarning( "変数名が正しくありません : " + tokens[ syntaxOffset ].Line + "行 : " + variableName ) ;
				return -1 ;
			}

			offset ++ ;
			if( offset >= tokens.Count || tokens[ offset ].Word != "=" )
			{
				// エラー(項目が足りない)
				Debug.LogWarning( "変数定義のパラメータが足りません : " + tokens[ syntaxOffset ].Line + "行" ) ;
				return -1 ;
			}




			return offset ;
		}








		//-------------------------------------------------------------------------------------------

		// 構文(関数定義)
		private static int SyntaxAdaptor_Function( List<Token> tokens, int offset, List<object> nodes )
		{
			int syntaxOffset = offset ;

			offset ++ ;
			if( offset >= tokens.Count )
			{
				// エラー(項目が足りない)
				Debug.LogWarning( "function のパラメータが足りません : " + tokens[ syntaxOffset ].Line + "行" ) ;
				return -1 ;
			}

			string variableName = tokens[ offset ].Word ;
			if( IsVariableName( variableName ) == false )
			{
				Debug.LogWarning( "function の関数名が正しくありません : " + tokens[ syntaxOffset ].Line + "行 : " + variableName ) ;
				return -1 ;
			}

			offset ++ ;
			if( offset >= tokens.Count || tokens[ offset ].Word != "(" )
			{
				// エラー(項目が足りない)
				Debug.LogWarning( "function の構文異常です : " + tokens[ syntaxOffset ].Line + "行" ) ;
				return -1 ;
			}


			//----------------------------------------------------------
			// 引数名を取得する

			List<string> parameterNames = new List<string>() ;

			int state = 0 ;
			while( true )
			{
				offset ++ ;
				if( offset >= tokens.Count )
				{
					// エラー(項目が足りない)
					Debug.LogWarning( "function の構文異常です : " + tokens[ syntaxOffset ].Line + "行" ) ;
					return -1 ;
				}

				string word = tokens[ offset ].Word ;

				if( word == ")" )
				{
					if( state == 0 || state == 1 )
					{
						// 終了
						break ;
					}
					else
					{
						Debug.LogWarning( "function の引数構文に問題があります : " + tokens[ syntaxOffset ].Line + "行"  ) ;
						return -1 ;
					}
				}

				if( state == 0 || state == 2 )
				{
					if( IsVariableName( word ) == false )
					{
						Debug.LogWarning( "function の引数名が正しくありません : " + tokens[ syntaxOffset ].Line + "行 : " + word ) ;
						return -1 ;
					}

					parameterNames.Add( word ) ;

					state = 1 ;
				}
				else
				if( state == 1 )
				{
					if( word != "," )
					{
						Debug.LogWarning( "function の引数構文に問題があります : " + tokens[ syntaxOffset ].Line + "行"  ) ;
						return -1 ;
					}

					state = 2 ;
				}
			}

			//----------------------------------------------------------


			// 構文(変数格納)
			var variableSetter = new Node.Syntax.VariableSetter()
			{
				Name	= variableName
			} ;

			// 構文(関数定義)
			var function = new Node.Syntax.Function()
			{
				ParameterNames = parameterNames
			} ;

			variableSetter.Value = function ;

			// ノードに追加する
			nodes.Add( variableSetter ) ;

			//----------------------------------------------------------

			// 成功
			return offset ;
		}







		//-------------------------------------------------------------------------------------------

		// 変数の先頭の文字
		private static char[] m_VariableHeadCodes = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUWXYZ_".ToCharArray() ;

		/// <summary>
		/// 変数名として妥当か確認する
		/// </summary>
		/// <param name="word"></param>
		/// <returns></returns>
		private static bool IsVariableName( string word )
		{
			return m_VariableHeadCodes.Contains( word[ 0 ] ) ;
		}
	}
}
