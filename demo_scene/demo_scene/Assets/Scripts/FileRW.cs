using UnityEngine;  
using System.Collections;  
using System.Collections.Generic;  
using System.IO;  
using System.Linq;

public class FileRW : MonoBehaviour
{

    void Start()
    {
        string str = "hello wor";
        OverWriteFileByLine(Application.dataPath,"filetest",str);
        Debug.Log(Application.dataPath);
        
    }

    public void WriteFileByLine(string filePath,string fileName,string strInfo)  
    { 
        StreamWriter sw;  
        FileInfo file_info = new FileInfo (filePath+"//"+fileName);  
        if(!file_info.Exists)  
        {  
            sw=file_info.CreateText();//创建一个用于写入 UTF-8 编码的文本  
            Debug.Log("文件创建成功！");  
        }  
        else  
        {  
            sw=file_info.AppendText();//打开现有 UTF-8 编码文本文件以进行读取  
        }  
        sw.WriteLine(strInfo);  
        sw.Close ();  
        sw.Dispose ();//文件流释放  
    }  


    public void WriteFileByLine(string filePath,string fileName,List<string> strInfo)  
    { 
        StreamWriter sw;  
        FileInfo file_info = new FileInfo (filePath+"//"+fileName);  
        if(!file_info.Exists)  
        {  
            sw=file_info.CreateText();//创建一个用于写入 UTF-8 编码的文本  
            Debug.Log("文件创建成功！");  
        }  
        else  
        {  
            sw=file_info.AppendText();//打开现有 UTF-8 编码文本文件以进行读取  
        }

        foreach(string str in strInfo)
            sw.WriteLine(str);  
        sw.Close ();  
        sw.Dispose ();//文件流释放  
    }

    public void OverWriteFileByLine(string filePath,string fileName,string strInfo)  
    { 
        StreamWriter sw;  
        FileInfo file_info = new FileInfo (filePath+"//"+fileName);  
        sw = file_info.CreateText();
        sw.WriteLine(strInfo);  
        sw.Close ();  
        sw.Dispose ();//文件流释放  
    }  


    public void OverWriteFileByLine(string filePath,string fileName,List<string> strInfo)  
    { 
        StreamWriter sw;  
        FileInfo file_info = new FileInfo (filePath+"//"+fileName);  
        
        sw=file_info.CreateText();//创建一个用于写入 UTF-8 编码的文本  
            
        foreach(string str in strInfo)
            sw.WriteLine(str);  
        sw.Close ();  
        sw.Dispose ();//文件流释放  
    }

    public List<string> ReadFile(string filePath, string fileName,string strInfo)
    {
        StreamReader sr;

        FileInfo fileInfo = new FileInfo(filePath + "/" + fileName);
        if(fileInfo.Exists)  
        {
            sr=fileInfo.OpenText();  
        }  
        else  
        {  
            Debug.LogWarning("Not find files!");  
            return null;  
        }  

        List<string> list = new List<string>();
        string str;  
        while((str=sr.ReadLine())!=null)  
            list.Add(str);//加上str的临时变量是为了避免sr.ReadLine()在一次循环内执行两次  
        sr.Close ();  
        sr.Dispose ();  
        return list;
    }


}
