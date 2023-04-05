using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using Valve.VR;
using Valve.VR.InteractionSystem;

public class Output : MonoBehaviour
{
    public UnityEngine.UI.InputField InputField;//GUI element for entering the Participants Code
    public SteamVR_Action_Vector2 m_MoveValue = null;//magic carpet locomotion is used; controller input needs to be known
    public GameObject Camera = null;//main camera for head movement
    public GameObject Raycast = null;//gaze tracker for event 
    
    private String Name;//label for event file
    private List<string[]> rowData = new List<string[]>();
    //assorted event count(ers)
    private int counterRecording;
    private int counterCalibration;
    private int counterEvent;
    int Checkpoints_remaining;

    private void Awake()
    {
        DateTime localDate = DateTime.Now;
        Name = System.DateTime.Now.ToString("HH-mm-ss");
    }

    void Start()
    {
        //the checkpoints are collected once a raycast hit is registered
        //this part of the example is completely arbitrary but is incudeld because tags are mentioned
        Checkpoints_remaining = 11;
        GameObject[] Checkpoints = GameObject.FindGameObjectsWithTag("Checkpoint");
        Checkpoints_remaining = Checkpoints.Length;

        InputField.gameObject.SetActive(true);//GUI Element for entering the Participants Code
      
        //use informative names for the rows add or delete rows acoording to your needs
        string[] rowDataTemp = new string[18];
        rowDataTemp[0] = "Timestamp";
        rowDataTemp[1] = "Recording_Start";
        rowDataTemp[2] = "Calibration_Start";
        rowDataTemp[3] = "Checkpoint_Collected";
        rowDataTemp[4] = "X_Controller_Input";
        rowDataTemp[5] = "Y_Controller_Input";
        rowDataTemp[6] = "X_Head_Position";
        rowDataTemp[7] = "Y_Head_Position";
        rowDataTemp[8] = "Z_Head_Position";
        rowDataTemp[9] = "X_Head_Transform";
        rowDataTemp[10] = "Y_Head_Transform";
        rowDataTemp[11] = "Z_Head_Transform";
        rowDataTemp[12] = "X_Raycast_Position";
        rowDataTemp[13] = "Y_Raycast_Postion";
        rowDataTemp[14] = "Z_Raycast_Position";
        rowDataTemp[15] = "X_Raycast_Transform";
        rowDataTemp[16] = "Y_Raycast_Transform";
        rowDataTemp[17] = "Z_Raycast_Transform";

        rowData.Add(rowDataTemp);
    }


    void Update()
    {
        if (InputField.interactable == false)
        { 
            Save();//the participants code needs to be known before starting to save events
        }

        if (Input.GetKeyDown(KeyCode.R))
        { 
            counterRecording++;//counter for keycode R 
        }

        if (Input.GetKeyDown(KeyCode.C))
        { 
            counterCalibration++;//counter for keycode C (if used add counter for Validation)
        }

        GameObject[] Checkpoints = GameObject.FindGameObjectsWithTag("Checkpoint");
        if (Checkpoints.Length != Checkpoints_remaining)//counts the remaining checkpoints indicates that a checkpoint was collected
        {
            counterEvent++;
            Checkpoints_remaining = Checkpoints.Length;
            // return;
        }

        if (Input.GetMouseButtonDown(1))//quits the application and event saving (not needed, but nifty)
        {
            Application.Quit();
        }
    }



    void Save()
    {
        string[] rowDataTemp = new string[18];
        rowDataTemp[0] = System.DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss:fffff");//Systemtime
        rowDataTemp[1] = counterRecording.ToString();//Recording Start
        rowDataTemp[2] = counterCalibration.ToString();//Calibration Start
        rowDataTemp[3] = counterEvent.ToString();//Collection of Checkpoint 
        rowDataTemp[4] = m_MoveValue.axis.x.ToString();//Movement along the x-Axis
        rowDataTemp[5] = m_MoveValue.axis.y.ToString();//Movement along the y-Axis
        rowDataTemp[6] = Camera.transform.position.x.ToString();//x Position Head
        rowDataTemp[7] = Camera.transform.position.y.ToString();//y Position Head
        rowDataTemp[8] = Camera.transform.position.z.ToString();//z Position Head
        rowDataTemp[9] = Camera.transform.eulerAngles.x.ToString();//x Transform Head
        rowDataTemp[10] = Camera.transform.eulerAngles.y.ToString();//y Transform Head
        rowDataTemp[11] = Camera.transform.eulerAngles.z.ToString();//z Transform Head
        rowDataTemp[12] = Raycast.transform.position.x.ToString();//x Position Raycast Gaze Tracker
        rowDataTemp[13] = Raycast.transform.position.y.ToString();//y Position Raycast Gaze Tracker
        rowDataTemp[14] = Raycast.transform.position.z.ToString();//z Position Raycast Gaze Tracker
        rowDataTemp[15] = Raycast.transform.eulerAngles.x.ToString();//x Transform Raycast Gaze Tracker
        rowDataTemp[16] = Raycast.transform.eulerAngles.y.ToString();//y Transform Raycast Gaze Tracker
        rowDataTemp[17] = Raycast.transform.eulerAngles.z.ToString();//z Transform Raycast Gaze Tracker

        rowData.Add(rowDataTemp);

        string[][] output = new string[rowData.Count][];

        for (int i = 0; i < output.Length; i++)
        {
            output[i] = rowData[i];
        }

        int length = output.GetLength(0);
        string delimiter = "/"; //Delimter could also be something more common e.g. ; or ,

        StringBuilder sb = new StringBuilder();

        for (int index = 0; index < length; index++)
            sb.AppendLine(string.Join(delimiter, output[index]));


        string filePath = GetPath();

        StreamWriter outStream = System.IO.File.CreateText(filePath);
        outStream.WriteLine(sb);
        outStream.Close();

    }

   
    string GetPath() //saves the event file in the folder your application is build in 
    {
        return Application.dataPath + "/" + DateTime.Now.ToLongDateString() + "_" +  Name + "_" + InputField.text.ToString() + ".csv";
    }
}


