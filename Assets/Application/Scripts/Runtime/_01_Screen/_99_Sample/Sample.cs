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

			//------------------------------------------------------------------------------------------
			// コードをロードする
			
			string path = "js_sample" ;

			var ta = Resources.Load<TextAsset>( path ) ;
			if( ta == null || ta.bytes == null || ta.bytes.Length == 0 )
			{
				Debug.Log( "Can not load : Path = " + path ) ;
				return ;
			}

			byte[] bytes = ta.bytes ;

			string text ;

			// UTF-8のシグネチャがあれば除去する
			if( Match( ref bytes, 0, 0xEF, 0xBB, 0xBF ) == true )
			{
				// UTF-8のシグネチャがある
				text = Encoding.UTF8.GetString( bytes, 3, bytes.Length - 3 ) ;
			}
			else
			{
				// UTF-8のシグネチャがない
				text = ta.text ;
			}

			//------------------------------------------------------------------------------------------
			// インタプリタを実行する

			Interpreter.Run( text ) ;
		}


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
	}
}
