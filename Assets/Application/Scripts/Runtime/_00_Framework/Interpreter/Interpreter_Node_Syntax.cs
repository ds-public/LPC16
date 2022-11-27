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
	/// ノード
	/// </summary>
	public partial class Interpreter
	{
		/// <summary>
		/// スコープ対象
		/// </summary>
		public enum Scopes
		{
			Global,
			Local,
		}

		// 値を設定する(グローバル)
		public void SetVariable( string variableName, object variableValue, Scopes scope )
		{
			if( scope == Scopes.Global )
			{
				// グローバル対象
				if( m_Variables[ 0 ].ContainsKey( variableName ) == false )
				{
					// 追加
					m_Variables[ 0 ].Add( variableName, variableValue ) ;
				}
				else
				{
					// 更新
					m_Variables[ 0 ][ variableName ] = variableValue ;
				}
			}
			else
			if( scope == Scopes.Local )
			{
				// ローカル対象
				int l = m_Variables.Count - 1 ;

				if( m_Variables[ l ].ContainsKey( variableName ) == false )
				{
					// 追加
					m_Variables[ l ].Add( variableName, variableValue ) ;
				}
				else
				{
					// 更新
					m_Variables[ l ][ variableName ] = variableValue ;
				}
			}
		}

		//-----------------------------------------------------------

		/// <summary>
		/// ノード
		/// </summary>
		public partial class Node
		{
			/// <summary>
			/// 構文(予約語)
			/// </summary>
			public partial class Syntax
			{
				/// <summary>
				/// 変数書き込み
				/// </summary>
				public class VariableSetter
				{
					/// <summary>
					/// 名前
					/// </summary>
					public string	Name ;

					/// <summary>
					/// 書き込み対象
					/// </summary>
					public object	Value ;

					/// <summary>
					/// スコープ対象
					/// </summary>
					public Scopes	Scope ;


					//--------------------------------------------------------

					/// <summary>
					/// 実行する
					/// </summary>
					public void Run( Interpreter interperter )
					{
						if
						(
							Value is System.Boolean		||
							Value is System.Int32		||
							Value is System.Double		||
							Value is System.String		||
							Value is Function
						)
						{
							// 値をそのまま格納する
							interperter.SetVariable( Name, Value, Scope ) ;
						}
						else
						if
						(
							Value is Calculation
						)
						{
							// 計算結果を格納する
							interperter.SetVariable( Name, ( ( Calculation )Value ).Get(), Scope ) ;
						}
						else
						if
						(
							Value is Comparing
						)
						{
							// 比較結果を格納する
							interperter.SetVariable( Name, ( ( Comparing )Value ).Get(), Scope ) ;
						}

					}
				}

				/// <summary>
				/// 構文(関数定義)
				/// </summary>
				public class Function : Scope
				{
					/// <summary>
					/// 入力引数の名前群
					/// </summary>
					public List<string>	ParameterNames ;

					/// <summary>
					/// 実行する
					/// </summary>
					/// <param name="parameters"></param>
					/// <returns></returns>
					public List<System.Object> Run( List<System.Object> parameters )
					{
						return null ;
					}
				}


				/// <summary>
				/// 構文(if)
				/// </summary>
				public class If
				{
					/// <summary>
					/// 比較
					/// </summary>
					public Comparing	Compare ;

					//-----------------------------------

					/// <summary>
					/// 分岐(真)
					/// </summary>
					public Scope		Branch_True ;

					/// <summary>
					/// 分岐(偽)
					/// </summary>
					public Scope		Branch_False ;

					//-----------------------------------

					/// <summary>
					/// 実行する
					/// </summary>
					public void Run()
					{
					}
				}

				/// <summary>
				/// 構文(for)
				/// </summary>
				public class For : Scope
				{
					/// <summary>
					/// 初期化
					/// </summary>
					public Block		Initialize ;

					/// <summary>
					/// 比較
					/// </summary>
					public Comparing	Compare ;

					/// <summary>
					/// 変化
					/// </summary>
					public Block		Modify ;

					//-----------------------------------
				}





			}

		}
	}
}
