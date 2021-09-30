using Microsoft.VisualStudio.Shell;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Resources;
using System.Runtime.InteropServices;
using System.Windows.Media;


namespace VSIXProjectMultiLang
{

    public static class MyConfig
    {
        public static double FontSize { get; set; } = 12;
        public static bool IsEnable { get; set; } = true;
        public static Color Color { get; set; } = Colors.DarkOrange;

        ///"D:\test\Lang.zh-TW.resx"
        private static string filePath = @"";
        public static string FilePath {
            get
            {
                return filePath;
            }
            set
            {
                bool isExist = File.Exists( value );
                if(isExist == true)
                {
                    //路徑被重置先清空字典
                    dict.Clear( );

                    ResXResourceReader rsxr = new ResXResourceReader(value);
                    foreach (DictionaryEntry d in rsxr)
                        dict.Add( d.Key.ToString( ), d.Value.ToString( ) );

                    rsxr.Close( );

                    filePath = value;
                }
            }
        } 

        private static Dictionary<string, string> dict = new Dictionary<string, string>( );
        public static Dictionary<string, string> Dict {
            get { return dict; }
            private set
            {
                dict = value;
            }
        } 



    }

    [ComVisible(true)]
    public class MyConfigPage : DialogPage
    {
        [Category( "乖乖設定就對了" )]
        [DisplayName( "是否顯示多語系提示" )]
        [Description( "是否顯示多語系提示" )]
        public bool IsEnable { get; set; }

        [Category( "乖乖設定就對了" )]
        [DisplayName( "多語系提示顏色" )]
        [Description( "我是想要有調色盤但是找不到所以直接放棄\r\n推薦用這兩個顏色 #FFFF8C00 #FFFF8CAA" )]
        public Color Color { get; set; }

        [Category( "乖乖設定就對了" )]
        [DisplayName( "字體大小" )]
        [Description( "字體大小\r\n如果要像 vs2019 reference 樣式的話大小應該是 8" )]
        [DefaultValue( 12 )]
        public double FontSize { get; set; }


        [Category( "乖乖設定就對了" )]
        [DisplayName( "多語系檔案路徑" )]
        [Description( "多語系檔案路徑 注意只有.resx 才能生效像這樣 D:\\test\\Lang.zh-TW.resx" )]
        public string FilePath { get; set; }

        protected override void OnApply(PageApplyEventArgs e)
        {
            MyConfig.IsEnable = IsEnable;
            MyConfig.Color = Color;
            MyConfig.FontSize = FontSize;
            MyConfig.FilePath = FilePath;
            base.OnApply( e );
        }
        public override void SaveSettingsToStorage()
        {
            MyConfig.IsEnable = IsEnable;
            MyConfig.Color = Color;
            MyConfig.FontSize = FontSize;
            MyConfig.FilePath = FilePath;
            base.SaveSettingsToStorage( );
        }

        public override void LoadSettingsFromStorage()
        {
            IsEnable = MyConfig.IsEnable;
            Color = MyConfig.Color;
            FontSize = MyConfig.FontSize;
            FilePath = MyConfig.FilePath;
            base.LoadSettingsFromStorage( );
        }

        public override void ResetSettings()
        {
            MyConfig.IsEnable = true;
            MyConfig.Color = Colors.DarkOrange;
            MyConfig.FontSize = 12;
            MyConfig.FilePath = @"";
            base.ResetSettings( );
        }
    }

}
