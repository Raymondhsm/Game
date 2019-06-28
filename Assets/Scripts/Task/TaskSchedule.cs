using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TaskSchedule : MonoBehaviour
{
    

    private string filePath;
    public string FilePath
    {
        set { filePath = value; }
    }
    private string fileName;
    public string FileName
    {
        set { fileName = value; }
    }

    public void ReadTaskSchedule(List<string> notStart, Dictionary<string,OnGoingTask> OnGoing, List<string> finished, List<string> taskList)
    {
        FileRW file = new FileRW(filePath,fileName,FileRW.Type.R);
        List<string> data = file.ReadFile();

        if(data == null) return;

        for(int i=0; i<data.Count; i++)
        {
            string str = data[i];
            if(str == "#NotStart") 
            { 
                while(true)
                {
                    str = data[i+1];
                    if(str == ""){
                        i++;
                        continue;
                    }
                    else if(str[0] == '#') break;
                    else
                    {
                        notStart.Add(str);
                        taskList.Add(str);
                        i++;
                    }
                }
            }
            else if(str == "#OnGoing")
            {
                while(true)
                {
                    str = data[i+1];
                    if(str == ""){
                        i++;
                        continue;
                    }
                    else if(str[0] == '#') break;
                    else
                    {
                        OnGoingTask ogt = new OnGoingTask();
                        ogt.taskName = str;
                        ogt.taskNodeIndex = int.Parse(data[i+2]);
                        ogt.taskNodeNum = int.Parse(data[i+3]);
                        ogt.taskNodeLayer = int.Parse(data[i+4]);

                        OnGoing.Add(str,ogt);

                        taskList.Add(str);

                        i = i + 4;
                    }
                }
            }
            else if(str == "#Finished")
            {
                while(true)
                {
                    if(i >= data.Count - 2)break;
                    str = data[i+1];
                    if(str == ""){
                        i++;
                        continue;
                    }
                    else if(str[0] == '#') break;
                    else
                    {
                        finished.Add(str);
                        taskList.Add(str);
                        i++;
                    }
                }
            }
        }
    }

    public void WriteTaskSchedule(List<string> notStart, Dictionary<string,OnGoingTask> OnGoing, List<string> finished)
    {
        FileRW file = new FileRW(filePath,fileName,FileRW.Type.W);

        List<string> data = new List<string>();

        data.Add("#NotStart");
        foreach(string na in notStart)
            data.Add(na);
        data.Add("");
        
        data.Add("#OnGoing");
        foreach(OnGoingTask ta in OnGoing.Values)
        {
            data.Add(ta.taskName);
            data.Add(ta.taskNodeIndex.ToString());
            data.Add(ta.taskNodeNum.ToString());
            data.Add(ta.taskNodeLayer.ToString());
        }
        data.Add("");

        data.Add("#Finished");
        foreach(string na in finished)
            data.Add("na");

        file.OverWriteFileByLine(data);
    }
}



public class OnGoingTask
{
    public string taskName;
    public int taskNodeIndex;
    public int taskNodeNum;

    public int taskNodeLayer;
    
}
