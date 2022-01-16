// Decompiled with JetBrains decompiler
// Type: GHIElectronics.TinyCLR.Drivers.Azure.SAS.SharedAccessSignatureBuilder
// Assembly: GHIElectronics.TinyCLR.Drivers.Azure.SAS, Version=2.1.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 32DD7DD8-C378-4CFF-8F2C-50F528CFE2C5
// Assembly location: D:\experiment\sqliteapp1\sqliteapp1\bin\Debug\GHIElectronics.TinyCLR.Drivers.Azure.SAS.dll

//using GHIElectronics.TinyCLR.Cryptography;
//using GHIElectronics.TinyCLR.Networking.Net;
using AzureIoTDemo;
using System;
using System.Text;
using System.Web;

namespace Azure.SAS
{
  public class SharedAccessSignatureBuilder
  {
    private string key;

    public SharedAccessSignatureBuilder() => this.TimeToLive = TimeSpan.FromMinutes(60);

    public string KeyName { get; set; }

    public string Key
    {
      get => this.key;
      set => this.key = value;
    }

    public string Target { get; set; }

    public TimeSpan TimeToLive { get; set; }

    public string ToSignature() => this.BuildSignature(this.KeyName, this.Key, this.Target, this.TimeToLive);

    private string BuildSignature(string keyName, string key, string target, TimeSpan timeToLive)
    {
      string str1 = SharedAccessSignatureBuilder.BuildExpiresOn(timeToLive);
      string str2 = HttpUtility.UrlEncode(target);
      string str3 = this.Sign(str2 + "\n" + str1, key);
      StringBuilder stringBuilder = new StringBuilder();
      stringBuilder.Append(string.Format("{0} {1}={2}&{3}={4}&{5}={6}", (object) "SharedAccessSignature", (object) "sr", (object) str2, (object) "sig", (object) HttpUtility.UrlEncode(str3), (object) "se", (object) HttpUtility.UrlEncode(str1)));
      if (!this.IsNullOrWhiteSpace(keyName))
        stringBuilder.Append(string.Format("&{0}={1}", new object[2]
        {
          (object) "skn",
          (object) HttpUtility.UrlEncode(keyName)
        }));
      return stringBuilder.ToString();
    }

    private static string BuildExpiresOn(TimeSpan timeToLive) => ((long) DateTime.UtcNow.Add(timeToLive).Subtract(SharedAccessSignatureConstants.EpochTime).TotalSeconds).ToString();

    protected virtual string Sign(string requestString, string key)
    {
      HMACSHA256 hmacshA256 = new HMACSHA256(Convert.FromBase64String(key));
      //bool useRfC4648Encoding = Convert.UseRFC4648Encoding;
      //Convert.UseRFC4648Encoding = true;
      byte[] bytes = Encoding.UTF8.GetBytes(requestString);
      string base64String = Convert.ToBase64String(hmacshA256.ComputeHash(bytes));
      //Convert.UseRFC4648Encoding = useRfC4648Encoding;
      return base64String;
    }

    private bool IsNullOrWhiteSpace(string s) => s == null || s.IndexOf(" ") >= 0;
  }
}
