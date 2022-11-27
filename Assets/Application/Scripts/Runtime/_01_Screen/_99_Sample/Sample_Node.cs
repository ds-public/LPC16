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
	/// ノード
	/// </summary>
	public partial class Sample
	{
		/// <summary>
		/// 計算記号
		/// </summary>
		public enum CalculationSigns
		{
			/// <summary>
			/// +
			/// </summary>
			Addition,

			/// <summary>
			/// -
			/// </summary>
			Subtraction,

			/// <summary>
			/// *
			/// </summary>
			Multiplication,

			/// <summary>
			/// /
			/// </summary>
			Division,

			/// <summary>
			/// %
			/// </summary>
			Residue,

			/// <summary>
			/// ^
			/// </summary>
			Raise,

			/// <summary>
			/// &
			/// </summary>
			And,

			/// <summary>
			/// |
			/// </summary>
			Or,

			/// <summary>
			/// ~
			/// </summary>
			ExclusiveOr,

			/// <summary>
			/// !
			/// </summary>
			Not,

			/// <summary>
			/// ++
			/// </summary>
			Increase,

			/// <summary>
			/// --
			/// </summary>
			Decrease,

			/// <summary>
			/// (
			/// </summary>
			ParenthesizedOpen,

			/// <summary>
			/// )
			/// </summary>
			ParenthesizedClose,
		}

		/// <summary>
		/// 計算
		/// </summary>
		public class Calculation
		{
			/// <summary>
			/// 実行ノード群
			/// </summary>
			public List<System.Object>	Nodes ;

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
		/// 比較記号
		/// </summary>
		public enum ComparingSigns
		{
			/// <summary>
			/// ==
			/// </summary>
			Equal,

			/// <summary>
			/// !=
			/// </summary>
			NotEqual,

			/// <summary>
			/// >
			/// </summary>
			Large,

			/// <summary>
			/// >=
			/// </summary>
			LargeEqual,

			/// <summary>
			/// <
			/// </summary>
			Small,

			/// <summary>
			/// <=
			/// </summary>
			SmallEqual,
		}
	
		/// <summary>
		/// 条件記号
		/// </summary>
		public enum ComditionSigns
		{
			/// <summary>
			/// &&
			/// </summary>
			And,

			/// <summary>
			/// ||
			/// </summary>
			Or,
		}

		/// <summary>
		/// 比較
		/// </summary>
		public class Comparing
		{
			/// <summary>
			/// 左辺値
			/// </summary>
			public System.Object	Node_L ;

			/// <summary>
			/// 右辺値
			/// </summary>
			public System.Object	Node_R ;

			/// <summary>
			/// 比較記号
			/// </summary>
			public ComparingSigns	ComparingSign ;

			/// <summary>
			/// 取得する
			/// </summary>
			/// <returns></returns>
			public bool Get()
			{
				return false ;
			}
		}


		/// <summary>
		/// ブロック
		/// </summary>
		public class Block
		{
			/// <summary>
			/// 実行ノード群
			/// </summary>
			public List<System.Object>	Nodes ;

			/// <summary>
			/// 実行する
			/// </summary>
			public void Run()
			{
			}
		}

		/// <summary>
		/// スコープ
		/// </summary>
		public class Scope : Block
		{
		}

		/// <summary>
		/// 関数
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

		//------------------------------------------------------------

		/// <summary>
		/// 構文(if)
		/// </summary>
		public class Syntax_if
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
		public class Syntax_for : Scope
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

