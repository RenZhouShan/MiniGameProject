using System.Collections;
using System.Collections.Generic;
using System;
using System.Diagnostics;
using System.IO;
using Debug = UnityEngine.Debug;
 
public class FileTools
{
    /// <summary>
    /// 确保文件夹存在
    /// </summary>
    public static void EnsureDirectoryExist(string targetDir, bool isHide = false)
    {
        if (!Directory.Exists(targetDir))
        {
            Directory.CreateDirectory(targetDir);
            if (isHide)
            {
                File.SetAttributes(targetDir, FileAttributes.Hidden);
            }
        }
    }
    /// <summary>
    /// 尝试着读取文件,小文件
    /// </summary>
    /// <param name="path"></param>
    /// <returns></returns>
    public static byte[] TryReadFile(string path)
    {
        //文件路径要正确
        if (path.Length < 1)
        {
            return null;
        }
        if (!File.Exists(path))
        {
            Debug.LogError(path+"这个文件没找到");
            return null;
        }
        FileStream fs=new FileStream(path, FileMode.Open, FileAccess.Read);
        if (fs.CanRead)
        {
            byte[]array=new byte[fs.Length];
            fs.Read(array, 0, array.Length);
            fs.Close();
            return array;
        }
        fs.Close();
        return null;
    }
    /// <summary>
    /// 尝试着写东西进一个文件，覆盖原文件
    /// </summary>
    /// <param name="path"></param>
    /// <param name="content"></param>
    public static void TryWriteFile(string path,string content)
    {
        if (path.Length<1)
        {
            return;
        }
        using (StreamWriter sw=new StreamWriter(path,false))
        {
           sw.Write(content);
        }
    }
 
    public static long TryGetFileSize(string path)
    {
        if (path.Length<1)
        {
            return 0;
        }
 
        FileInfo fi = new FileInfo(path);
        if (fi==null)
        {
            return 0;
        }
        return fi.Length;
    }
}