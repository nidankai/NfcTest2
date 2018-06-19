using Android.App;
using Android.Widget;
using Android.OS;
using Android.Nfc;
using Android.Content;
using System;
using Android.Text;
using System.Reflection;

namespace NfcTest2
{
    // LaunchMode = Android.Content.PM.LaunchMode.SingleTopはインテント受信時にOnCreate()ではなくOnNewIntent()を実行させるために必要
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme", MainLauncher = true, LaunchMode = Android.Content.PM.LaunchMode.SingleTop)]
    public class MainActivity : Activity
    {
        private NfcAdapter mNfcAdapter;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.activity_main);
            Console.WriteLine(MethodBase.GetCurrentMethod());
        }
        protected override void OnResume()
        {
            base.OnResume();

            mNfcAdapter = NfcAdapter.GetDefaultAdapter(this);

            //NFC機能なし機種
            if (mNfcAdapter == null)
            {
                Toast.MakeText(this, "no Nfc feature", ToastLength.Short).Show();
                Finish();
                return;
            }

            //NFC通信OFFモード
            if (!mNfcAdapter.IsEnabled)
            {
                Toast.MakeText(this, "off Nfc feature", ToastLength.Short).Show();
                Finish();
                return;
            }

            //NFCを見つけたときに反応させる
            //PendingIntent→タイミング（イベント発生）を指定してIntentを発生させる
            PendingIntent pendingIntent = PendingIntent.GetActivity(this, 0, new Intent(this, typeof(MainActivity)), 0);
            //タイミングは、タグ発見時とする。
            IntentFilter[] intentFilter = new IntentFilter[]{
                new IntentFilter(NfcAdapter.ActionTagDiscovered)
            };

            //反応するタグの種類を指定。
            String[][] techList = new String[][]{
                new string[] {
                    typeof(Android.Nfc.Tech.NfcA).FullName,
                    typeof(Android.Nfc.Tech.NfcB).FullName,
                    typeof(Android.Nfc.Tech.IsoDep).FullName,
                    typeof(Android.Nfc.Tech.MifareClassic).FullName,
                    typeof(Android.Nfc.Tech.MifareUltralight).FullName,
                    typeof(Android.Nfc.Tech.NdefFormatable).FullName,
                    typeof(Android.Nfc.Tech.NfcV).FullName,
                    typeof(Android.Nfc.Tech.NfcF).FullName,
                }
            };

            NfcManager manager = (NfcManager)GetSystemService(NfcService);
            mNfcAdapter.EnableForegroundDispatch(this, pendingIntent, intentFilter, techList);
        }

        protected override void OnPause()
        {
            base.OnPause();

            //アプリが表示されてない時は、NFCに反応しなくてもいいようにする
            //mNfcAdapter.EnableForegroundDispatch(this, null, null, null);
        }

        protected override void OnNewIntent(Intent intent)
        {
            base.OnNewIntent(intent);

            String action = intent.Action;
            if (TextUtils.IsEmpty(action))
            {
                return;
            }

            if (!action.Equals(NfcAdapter.ActionTagDiscovered))
            {
                return;
            }

            //成功！と表示してみる
            Toast.MakeText(this, "Success！", ToastLength.Short).Show();
        }

    }
}

