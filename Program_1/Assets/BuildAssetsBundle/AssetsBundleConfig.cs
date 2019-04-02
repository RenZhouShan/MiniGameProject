using System.Collections.Generic;
using UnityEngine;
 
/// <summary>
/// AssetsBundleConfig配置文件的映射对象
/// </summary>
[SerializeField]
public class AssetsBundleConfig
{
    [SerializeField]
    public List<Assets> AssetsBundles;
    public AssetsBundleConfig()
    {
        AssetsBundles = new List<Assets>();
    }
}
[SerializeField]
public class Assets
{
    /// <summary>
    /// AssetsBundle的名字
    /// </summary>
    [SerializeField]
    public string name;
    /// <summary>
    /// AssetsBundle的大小
    /// </summary>
    [SerializeField]
    public int size;
    /// <summary>
    /// AssetsBundle的所属平台0-window，1-ios，2-android
    /// </summary>
    [SerializeField]
    public string platform;
 
    public Assets()
    {
    }
 
    public Assets(string name, int size, string platform)
    {
        this.name = name;
        this.size = size;
        this.platform = platform;
    }
}