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
	/// <summary>
	/// 変数
	/// </summary>
	public partial class Sample
	{
		private static Sample m_Instance ;

		// 変数
		private List<Dictionary<string,object>>	m_Variables ;


		private void Initialize()
		{
			m_Instance = this ;

			m_Variables = new List<Dictionary<string, object>>
			{
				new Dictionary<string, object>()
			} ;
		}

		// 値を設定する(グローバル)
		public void SetVariableToGlobal( string variableName, object variableValue )
		{
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

		// 値を設定する(ローカル)
		public void SetVariableToLocal( string variableName, object variableValue )
		{
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

		//-------------------------------------------------------------------------------------------

		/// <summary>
		/// 変数読み出し
		/// </summary>
		public class VariableGetter
		{
			/// <summary>
			/// 名前
			/// </summary>
			public string	Name ;

			/// <summary>
			/// 取得する
			/// </summary>
			/// <returns></returns>
			public System.Object Get()
			{
				return null ;
			}
		}

		/// <summary>
		/// 変数書き込み(グローバル)
		/// </summary>
		public class VariableSetterToGlobal
		{
			/// <summary>
			/// 名前
			/// </summary>
			public string	Name ;

			/// <summary>
			/// 書き込み対象
			/// </summary>
			public System.Object Node ;

			/// <summary>
			/// 実行する
			/// </summary>
			public void Run()
			{			
			}
		}

		/// <summary>
		/// 変数書き込み(ローカル)
		/// </summary>
		public class VariableSetterToLocal
		{
			/// <summary>
			/// 名前
			/// </summary>
			public string	Name ;

			/// <summary>
			/// 書き込み対象
			/// </summary>
			public object	Node ;

			/// <summary>
			/// 実行する
			/// </summary>
			public void Run()
			{
				if
				(
					Node is System.Boolean		||
					Node is System.Int32		||
					Node is System.Double		||
					Node is System.String
				)
				{
					// 値をそのまま格納する
					m_Instance.SetVariableToLocal( Name, Node ) ;
				}
				else
				if
				(
					Node is Calculation
				)
				{
					// 計算結果を格納する
					m_Instance.SetVariableToLocal( Name, ( ( Calculation )Node ).Get() ) ;
				}
				else
				if
				(
					Node is Comparing
				)
				{
					// 比較結果を格納する
					m_Instance.SetVariableToLocal( Name, ( ( Comparing )Node ).Get() ) ;
				}

			}
		}


		//-------------------------------------------------------------------------------------------

		/// <summary>
		/// 配列の値を設定する
		/// </summary>
		public class VariableArraySetter
		{
			/// <summary>
			/// 配列のキー
			/// </summary>
			public object	Key ;

			/// <summary>
			/// 入力ピン
			/// </summary>
			public object	Target ;
		}

		/// <summary>
		/// 配列の値を取得する
		/// </summary>
		public class VariableArrayGetter
		{
			/// <summary>
			/// 配列のキー
			/// </summary>
			public object	Key ;

			/// <summary>
			/// 配列の値を取得する
			/// </summary>
			/// <returns></returns>
			public object Get()
			{
				return null ;
			}
		}

		/// <summary>
		/// 変数グループの値を設定する
		/// </summary>
		public class VariableGroupSetter
		{
			/// <summary>
			/// 設定対象の変数群
			/// </summary>
			public List<VariableSetterToGlobal>		Setters ;

			/// <summary>
			/// 入力ピン
			/// </summary>
			public object	Target ;
		}

		/// <summary>
		/// 変数グループの値を取得する
		/// </summary>
		public class VariableGroupGetter
		{
			/// <summary>
			/// 取得対象の変数群
			/// </summary>
			public List<VariableGetter>				Getters ;

			/// <summary>
			/// 取得する
			/// </summary>
			/// <returns></returns>
			public List<object>	Get()
			{
				return null ;
			}
		}

		/// <summary>
		/// 関数実行
		/// </summary>
		public class FunctionCaller
		{
			/// <summary>
			/// 入力値
			/// </summary>
			public List<object>		Paraneters ;

			/// <summary>
			/// 実行して結果を取得する
			/// </summary>
			/// <returns></returns>
			public List<object> Execute()
			{
				return null ;
			}
		}


	}
}
