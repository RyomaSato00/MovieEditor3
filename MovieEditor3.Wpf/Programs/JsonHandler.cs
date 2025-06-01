using System.IO;

using Newtonsoft.Json;

namespace MovieEditor3.Wpf.Programs;

internal static class JsonHandler
{
    /// <summary> JSONファイルの相対パス </summary>
    public static readonly string JsonFilePath = "UserSetting.json";

    /// <summary> ユーザー設定データ </summary>
    public static UserSetting Workspace { get; private set; } = new();

    /// <summary>
    /// JSONファイルからユーザー設定を読み込む
    /// </summary>
    public static void Load()
    {
        try
        {
            var text = File.ReadAllText(JsonFilePath);
            var obj = JsonConvert.DeserializeObject<UserSetting>(text);

            if (obj is not null)
            {
                // 正常にユーザー設定を読み込めた場合はワークスペースに保存
                Workspace = obj;
            }
        }
        catch (FileNotFoundException e)
        {
            System.Diagnostics.Debug.WriteLine($"\"{e.FileName}\" is not found. A new one will be created.");
        }
        catch (Exception e)
        {
            System.Diagnostics.Debug.WriteLine(e);
        }
    }

    /// <summary>
    /// ユーザー設定をJSONファイルに保存する
    /// </summary>
    public static void Save()
    {
        var text = JsonConvert.SerializeObject(Workspace, Formatting.Indented);
        File.WriteAllText(JsonFilePath, text);
    }
}

internal class UserSetting
{
    /// <summary>
    /// 出力先ディレクトリのパス
    /// </summary>
    public string OutputDirectory { get; set; } = Environment.CurrentDirectory;

    /// <summary>
    /// 最近使用したメディアのディレクトリパス
    /// </summary>
    public string RecentryUsedDirPath { get; set; } = Environment.CurrentDirectory;

    public static void Copy(UserSetting src, UserSetting dst)
    {
        dst.OutputDirectory = src.OutputDirectory;
        dst.RecentryUsedDirPath = src.RecentryUsedDirPath;
    }
}
