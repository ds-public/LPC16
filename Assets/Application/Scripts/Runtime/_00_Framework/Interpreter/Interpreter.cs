using System ;
using System.Collections ;
using System.Collections.Generic ;

using UnityEngine ;

using Cysharp.Threading.Tasks ;

using uGUIHelper ;

namespace DSW
{
	/// <summary>
	/// インタプリタクラス Version 2022/11/27 0
	/// </summary>
	public partial class Interpreter : ExMonoBehaviour
	{
		// シングルトンインスタンス
		private static Interpreter m_Instance = null ; 

		/// <summary>
		/// ブロッキングマスクのインスタンス
		/// </summary>
		public  static Interpreter   Instance
		{
			get
			{
				return m_Instance ;
			}
		}

		//-----------------------------------------------------------
#if false
		// キャンバス部分のインスタンス
		[SerializeField]
		protected UICanvas		m_Canvas ;

		/// <summary>
		/// ＶＲ対応用にキャンバスを取得できるようにする
		/// </summary>
		public UICanvas Canvas
		{
			get
			{
				return m_Canvas ;
			}
		}

		/// <summary>
		/// キャンバスの仮想解像度を設定する
		/// </summary>
		/// <param name="width"></param>
		/// <param name="height"></param>
		/// <returns></returns>
		public static bool SetCanvasResolution( float width, float height )
		{
			if( m_Instance == null || m_Instance.m_Canvas == null )
			{
				return false ;
			}

			m_Instance.m_Canvas.SetResolution( width, height, true ) ;

			return true ;
		}
#endif
		//---------------------------------------------------------------------------

		internal void Awake()
		{
			m_Instance = this ;
#if false
			// そのシーンをシーンが切り替わっても永続的に残すようにする場合、そのシーン側で DontDestroyOnLoad を実行してはならない。
			// DontDestroyOnLoad は、呼び出し側のシーンで、そのシーンに対して実行すること。

			//----------------------------------------------------------

			// キャンバスの解像度を設定する
			float width  =  960 ;
			float height =  540 ;

			Settings settings =	ApplicationManager.LoadSettings() ;
			if( settings != null )
			{
				width  = settings.BasicWidth ;
				height = settings.BasicHeight ;
			}

			SetCanvasResolution( width, height ) ;
#endif
		}

		//-------------------------------------------------------------------------------------------
		// 実行系の情報

		// 変数
		private List<Dictionary<string,object>>	m_Variables ;


		//-------------------------------------------------------------------------------------------

		/// <summary>
		/// 実行する
		/// </summary>
		/// <param name="text"></param>
		/// <returns></returns>
		public static bool Run( string text )
		{
			if( m_Instance == null )
			{
				return false ;
			}

			return m_Instance.Run_Private( text ) ;
		}

		private bool Run_Private( string text )
		{
			// トークン化する
			var tokens = Tokenize( text ) ;

			if( tokens == null || tokens.Count == 0 )
			{
				Debug.LogWarning( "トークンが存在しません" ) ;
				return false ;
			}

			// パースする
			Parse( tokens ) ;

			//----------------------------------------------------------

			// 変数領域を確保する
			m_Variables = new List<Dictionary<string, object>>
			{
				new Dictionary<string, object>()
			} ;

			//----------------------------------------------------------

			return true ;
		}
	}
}
