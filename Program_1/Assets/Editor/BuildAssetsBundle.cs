using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using NUnit.Framework.Constraints;
using UnityEditor;
using UnityEngine;
using LitJson;
 
public class BuildAssetsBundle : Editor
{
    private const string BuildAssetsBundleMenuAll = "Tools/BuildAssetsBundle";
    private const string BuildAssetsBundleMenu = "Assets/BuildAssetsBundle";
    private static List<string> assetNames = new List<string>();
    /// <summary>
    /// 打包assetsbundle的源文件的文件夹
    /// </summary>
    private static string OriginalDirectory = Application.dataPath + "/Resources/";
    /// <summary>
    /// 打出包的输出文件夹
    /// </summary>
    private static string OutDirectory = "Assets/StreamingAssets/AssetsBundle/";
    private const string ConfigFile = "Assets/StreamingAssets/AssetsBundle/AssetsBundleConfig.Config";
    //配置文件json，对应的对象实例    
    private static AssetsBundleConfig config = null;
    #region  打assets包
    /// <summary>
    /// 打包Assets/package目录下的所有文件为bundle包
    /// </summary>
    [MenuItem(BuildAssetsBundleMenuAll)]
    public static void BuildAssetsBundleAll()
    {
        DirectoryInfo info = new DirectoryInfo(OriginalDirectory);
        DirectoryInfo[] infos = info.GetDirectories();
        GetConfig();
        for (int i = 0; i < infos.Length; i++)
        {
            var name = infos[i].Name;
            FindAllFile(OriginalDirectory + name + "/", name);
        }
        OutPutConfig();
    }
    //根据具体文件夹打包assetsbundle
    [MenuItem(BuildAssetsBundleMenu, false, 1)]
    public static void BuildAssetsB()
    {
        //
        //System.Console.WriteLine("haha");
        //
        var paths = Selection.assetGUIDs.Select(AssetDatabase.GUIDToAssetPath).Where(AssetDatabase.IsValidFolder).ToList();
        string[] strs = Selection.assetGUIDs;
        string path = EditorUtility.SaveFolderPanel("Save Resourse", "", "");
        Debug.Log(path);
        if (path.Length != 0)
        {
            Object[] selection = Selection.GetFiltered(typeof(Object), SelectionMode.DeepAssets);
            foreach (Object sel in selection)
            {
                var strName = path.Split('/');
                GetConfig();
                Debug.Log(path);
                Debug.Log(strName[strName.Length - 1]);
                FindAllFile("Assets/BuildAssetsBundle", strName[strName.Length - 1]);
                OutPutConfig();
            }
            Selection.objects = selection;
        }
    }
    static void FindAllFile(string path, string assetsBundlename)
    {
        assetNames.Clear();
        FindFile(path);
        CreateAssetBunlde(assetsBundlename);
    }
    static void FindFile(string assetBundleName)
    {
        DirectoryInfo di = new DirectoryInfo(assetBundleName);
 
        FileInfo[] fis = di.GetFiles();
        for (int i = 0; i < fis.Length; i++)
        {
            if (fis[i].Extension != ".meta")
            {
                string str = fis[i].FullName;
                assetNames.Add(str.Substring(str.LastIndexOf("Assets")));
            }
        }
        DirectoryInfo[] dis = di.GetDirectories();
        for (int j = 0; j < dis.Length; j++)
        {
            FindFile(dis[j].FullName);
        }
    }
    /// <summary>
    /// 每次打包assetsbundle的前要获取整个配置文件
    /// </summary>
    static void GetConfig()
    {
        config = null;
        var bytes = FileTools.TryReadFile(ConfigFile);
        config = (bytes == null) ? new AssetsBundleConfig() : JsonMapper.ToObject<AssetsBundleConfig>(Encoding.UTF8.GetString(bytes));
    }
    /// <summary>
    /// 打包好了，要输出配置文件
    /// </summary>
    static void OutPutConfig()
    {
        if (config != null && config.AssetsBundles != null & config.AssetsBundles.Count > 0)
        {
            var content = JsonMapper.ToJson(config);
            FileTools.TryWriteFile(ConfigFile, content);
        }
        AssetDatabase.Refresh();
    }
    /// <summary>
    /// 打包资源到StreamingAssets下对应的平台，对应的资源名
    /// </summary>
    /// <param name="assetBundleName"></param>
    static void CreateAssetBunlde(string assetBundleName)
    {
        string[] assetBundleNames = assetNames.ToArray();
        AssetBundleBuild[] abs = new AssetBundleBuild[1];
        abs[0].assetNames = assetBundleNames;
        abs[0].assetBundleName = assetBundleName + ".assetBundle";
        BuildTarget buildTarget;
        string outPutPlat;
#if UNITY_ANDROID   //安卓  
        buildTarget = BuildTarget.Android;
        outPutPlat = "Android";
#elif UNITY_IOS
        buildTarget = BuildTarget.iOS;
        outPutPlat = "IOS";
#else
        buildTarget = BuildTarget.StandaloneWindows;
        outPutPlat = "Windows";
#endif
        var dicName = OutDirectory + outPutPlat;
        FileTools.EnsureDirectoryExist(dicName);
        AssetBundleManifest mainfest = BuildPipeline.BuildAssetBundles(dicName, abs, BuildAssetBundleOptions.None, buildTarget);
        var filepath = dicName + "/" + abs[0].assetBundleName;
        var size = (int)FileTools.TryGetFileSize(filepath);
        var assets = CreateAssets(assetBundleName, outPutPlat, size);
        if (config.AssetsBundles != null && config.AssetsBundles.Count > 0)
        {
            for (int i = 0; i < config.AssetsBundles.Count; i++)
            {
                var havedAssets = config.AssetsBundles[i];
                if ((havedAssets.name == assets.name) && (havedAssets.platform == assets.platform))
                {
                    config.AssetsBundles.Remove(havedAssets);
                    break;
                }
            }
        }
        config.AssetsBundles.Add(assets);
        Debug.LogError(assetBundleName + "打包完成");
    }
    /// <summary>
    /// 新建assetbundle对应的对象实例
    /// </summary>
    /// <param name="assetBundleName"></param>
    /// <param name="platform"></param>
    /// <returns></returns>
    static Assets CreateAssets(string assetBundleName, string platform,int size)
    {
        Assets assets = new Assets();
        assets.name = assetBundleName;
        assets.platform = platform;
        assets.size = size;
        return assets;
    }
    #endregion
}