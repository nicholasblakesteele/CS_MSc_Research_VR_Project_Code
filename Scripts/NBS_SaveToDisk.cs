using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;

public class NBS_SaveToDisk : MonoBehaviour
{
    private string fullSavePath;
    private StreamWriter streamWriter;

    // ********************************
    // Class default methods
    // ********************************

    private void Awake()
    {
        // Not used
    }

    private void Start()
    {
        // Not used
    }

    private void Update()
    {
        // Not used
    }

    // ********************************
    // Save methods
    // ********************************

    public void InitialiseSavePath()
    {
        string fileName = "study_logs_" + System.DateTime.Now.ToString("ddMMyyyy_HHmmss") + ".csv";

        fullSavePath = Application.persistentDataPath + "/" + fileName;

        //print("Log save location: " + fullSavePath);
    }
    
    public void WriteToLogFile(List<String> rowList)
    {
        // Reopen file with append permisisons

        streamWriter = new StreamWriter(fullSavePath, true);

        // Append each item into one string, using built in new line (don't use manual \n)

        string stringToWrite = "";

        foreach (String row in rowList)
        {
            string trimmedRow = row.Trim(); 

            stringToWrite += trimmedRow + streamWriter.NewLine;                 
        }

        // Remove new line at start/end of file to stop log file having blank rows

        stringToWrite = stringToWrite.Trim(); 

        // Write single string containing batch of rows to file

        streamWriter.WriteLine(stringToWrite);

        // Must close file or it won't write to disk

        FinishWritingLogFile();
    }

    private void FinishWritingLogFile()
    {
        // NOTE: nothing will be written to disk until the stram is closed

        streamWriter.Close();
    }  
}