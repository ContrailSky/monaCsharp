using System;
using System.Collections.Generic;

namespace monaCsharp
{
    public class methods
    {

        #region サーバー情報


        //デフォルトのmonacoin.confデータ
        public string[] mona_info = { "user", "password", "127.0.0.1", "25535", "1", "1", "1" };
        //その他の認識しなかったデータ
        //=で区切られている
        public string mona_info_other = "";
        /*
         * mona_info[0] = "user";      //サーバーに入るためのユーザー名
         * mona_info[1] = "password";  //サーバーに入るためのパスワード名
         * mona_info[2] = "127.0.0.1"; //サーバーが接続を許可するアドレス
         * mona_info[3] = "25535";      //サーバーのポート番号
         * mona_info[4] = "1";         //server
         * mona_info[5] = "1";         //daemon
         * mona_info[6] = "1";         //listen
         */


         #endregion

        #region メソッド


        //monacoin.confを作成またはチェックしてmona_infoに代入する
        //成功→null
        //不成功→エラーメッセージ
        public string CreateConfigFile(string path = "")
        {
            string configpath = "", tmp4entire_mona_info = "";
            string[] tmp_mona_info = {""};
            int limit = 0;

            configpath = (path != "") ?
                path
                :
                @System.Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)
                + "\\Monacoin\\monacoin.conf"
                ;
            if(System.IO.File.Exists(configpath))
            {
                System.IO.StreamReader sr = new System.IO.StreamReader(
                    configpath,
                    System.Text.Encoding.UTF8);
                while (sr.Peek() > -1)
                {
                    tmp4entire_mona_info += sr.ReadLine() + "=";
                }
                sr.Close();
                tmp4entire_mona_info = tmp4entire_mona_info.TrimEnd('=');
                tmp_mona_info = tmp4entire_mona_info.Split('=');
                try
                {
                    limit = (tmp4entire_mona_info.Length
                        - tmp4entire_mona_info.Replace("=", "").Length)
                        + 1;

                    for (int i = 0; i < limit;)
                    {
                        switch (tmp_mona_info[i])
                        {
                            case "rpcuser":
                                mona_info[0] = tmp_mona_info[i+1];
                                break;
                            case "rpcpassword":
                                mona_info[1] = tmp_mona_info[i+1];
                                break;
                            case "rpcallowip":
                                mona_info[2] = tmp_mona_info[i+1];
                                break;
                            case "rpcport":
                                mona_info[3] = tmp_mona_info[i+1];
                                break;
                            case "server":
                                mona_info[4] = tmp_mona_info[i+1];
                                break;
                            case "daemon":
                                mona_info[5] = tmp_mona_info[i+1];
                                break;
                            case "listen":
                                mona_info[6] = tmp_mona_info[i+1];
                                break;
                            default:
                                mona_info_other += tmp_mona_info[i+1] + "=";
                                break;
                        }
                        i += 2;
                    }
                    mona_info_other = mona_info_other.TrimEnd('=');
                    return null;
                }
                catch (Exception ex)
                {
                    return "[MESSAGE]" + Environment.NewLine + ex.Message + Environment.NewLine + Environment.NewLine
                        + "[SOURCE]" + Environment.NewLine + ex.Source + Environment.NewLine + Environment.NewLine
                        + "[STACK_TRACE]" + Environment.NewLine + ex.StackTrace + Environment.NewLine + Environment.NewLine
                        + "[TARGET_SITE]" + Environment.NewLine + ex.TargetSite + Environment.NewLine;
                }
            }
            else
            {
                try
                {
                    System.IO.StreamWriter sw = new System.IO.StreamWriter(
                        configpath,
                        false,
                        System.Text.Encoding.UTF8);
                    sw.Write(
                        "rpcuser=" + mona_info[0] + Environment.NewLine +
                        "rpcpassword=" + mona_info[1] + Environment.NewLine +
                        "rpcallowip=" + mona_info[2] + Environment.NewLine +
                        "rpcport=" + mona_info[3] + Environment.NewLine +
                        "server=" + mona_info[4] + Environment.NewLine +
                        "daemon=" + mona_info[5] + Environment.NewLine +
                        "listen=" + mona_info[6]
                        );
                    sw.Close();
                    return null;
                }
                catch (Exception ex)
                {
                    return "[MESSAGE]" + Environment.NewLine + ex.Message + Environment.NewLine + Environment.NewLine
                        + "[SOURCE]" + Environment.NewLine + ex.Source + Environment.NewLine + Environment.NewLine
                        + "[STACK_TRACE]" + Environment.NewLine + ex.StackTrace + Environment.NewLine + Environment.NewLine
                        + "[TARGET_SITE]" + Environment.NewLine + ex.TargetSite + Environment.NewLine;
                }
            }

        }

        //アドレスが正規かのチェック
        //正規→null
        //不正規→文字
        public string IsRightAddress(string address)
        {
            if (address.Length != 34)
            {
                return "認められないアドレス" + address.Length.ToString();
            }
            if (address.Substring(0, 1) != "M")
            {
                return "認められないアドレス";
            }
            return null;
        }

        //コマンド実行メソッド
        //成功→受信json
        //不成功→エラーメッセージ
        public string RunCommand(string method, object[] param)
        {
            string url="", json="";
            System.Web.Script.Serialization.JavaScriptSerializer jss 
                = new System.Web.Script.Serialization.JavaScriptSerializer();
            url = 
                (mona_info[2].IndexOf("http") == -1)
            ?
                "http://" + mona_info[2] + ":" + mona_info[3] + "/"
            :
                mona_info[2] + ":" + mona_info[3] + "/";
            try
            {
                //Refer for Bitcoin-Tool
                Dictionary<string, object> req = new Dictionary<string, object>();
                req.Add("jsonrpc", "2.0");
                req.Add("id", "1");
                req.Add("method", method);
                req.Add("params", param);
                json = jss.Serialize(req);
                System.Net.WebClient wc = new System.Net.WebClient();
                wc.Credentials = new System.Net.NetworkCredential(mona_info[0], mona_info[1]);
                wc.Headers.Add("Content-type", "application/json-rpc");
                json = wc.UploadString("http://" + mona_info[2] + ":" + mona_info[3], json);
                return json;
                
            }
            catch (Exception ex)
            {
                return "[MESSAGE]" + Environment.NewLine + ex.Message + Environment.NewLine + Environment.NewLine
                +"[SOURCE]" + Environment.NewLine + ex.Source + Environment.NewLine + Environment.NewLine
                + "[STACK_TRACE]" + Environment.NewLine + ex.StackTrace + Environment.NewLine + Environment.NewLine
                + "[TARGET_SITE]" + Environment.NewLine + ex.TargetSite + Environment.NewLine;
            }
        }


        #endregion

    }
}
