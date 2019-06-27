using UnityEngine;  
using System.Collections;  
using System.Collections.Generic;  
using System.IO;  
using System.Linq;

public class FileRW : MonoBehaviour
{
    public enum Type {
        R,
        W,
        RW
    }

    private FileInfo fileInfo;
    private bool isExist;
    private Type type;
    private string filePath;
    private string fileName;

    public FileRW(string filePath,string fileName, Type t)
    {
        this.filePath = filePath;
        this.fileName = fileName;
        this.type = t;

        fileInfo = new FileInfo (filePath + fileName);
        
        switch(t){
            case Type.R:
                isExist = fileInfo.Exists;
                break;
            case Type.W:
            case Type.RW:
                if(!fileInfo.Exists)  
                    fileInfo.CreateText();//创建一个用于写入 UTF-8 编码的文本  
                break;
        }
    }

    ~FileRW()
    {
        
    }

    public void WriteFileByLine(string strInfo)  
    { 
        if(type == Type.R){
            Debug.LogError("The Write Operation is Limited");
            return;
        }

        StreamWriter sw;  
        sw = fileInfo.AppendText();
        sw.WriteLine(strInfo);  
        sw.Close ();  
        sw.Dispose ();//文件流释放  
    }  


    public void WriteFileByLine(List<string> strInfo)  
    { 
        if(type == Type.R){
            Debug.LogError("The Write Operation is Limited");
            return;
        }

        StreamWriter sw;  
        sw = fileInfo.AppendText();

        foreach(string str in strInfo)
            sw.WriteLine(str);  
        sw.Close ();  
        sw.Dispose ();//文件流释放  
    }

    public void OverWriteFileByLine(string strInfo)  
    { 
        if(type == Type.R){
            Debug.LogError("The Write Operation is Limited");
            return;
        }

        StreamWriter sw;   
        sw = fileInfo.CreateText();
        sw.WriteLine(strInfo);  
        sw.Close ();  
        sw.Dispose ();//文件流释放  
    }  


    public void OverWriteFileByLine(List<string> strInfo)  
    { 
        if(type == Type.R){
            Debug.LogError("The Write Operation is Limited");
            return;
        }

        StreamWriter sw;  
        sw=fileInfo.CreateText();//创建一个用于写入 UTF-8 编码的文本  
            
        foreach(string str in strInfo)
            sw.WriteLine(str);  
        sw.Close ();  
        sw.Dispose ();//文件流释放  
    }

    public List<string> ReadFile()
    {
        if(type == Type.W){
            Debug.LogError("The Read Operation is Limited");
            return null;
        }

        if(!isExist){
            Debug.LogError("file not found : " + filePath + fileName);
            return null;
        }

        StreamReader sr;
        sr = fileInfo.OpenText();

        List<string> list = new List<string>();
        string str;  
        while((str = sr.ReadLine()) != null)  
            list.Add(str);
        sr.Close ();  
        sr.Dispose ();  
        return list;

    }


}
