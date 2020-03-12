using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;

public class TcpClientScript : MonoBehaviour
{
    public string alias;
    public string ip;
    public static string roomName;
    public static string createdRoom;

    public static string roomData;

    public int respLength;

    public static Tuple<string, string> dataStruct;

    public Tuple<NetworkStream, TcpClient> TcpTuple;

    public TcpClient client;

    public NetworkStream stream;

    public bool plsConnect = false;
    public bool firstTime = false;
    public byte[] data = new byte[1024];

    public int _port = 5005;

    public static bool newCommandFlag = false;
    public bool newCountdown = false;
    public bool newOutcome = false;

    public string countData = null;
    public string outcomeData = null;

    public static List<string> roomList = new List<string>();

    public Text nameInput;
    public Text ipInput;

    public GameObject rockButton = null;
    public GameObject paperButton = null;
    public GameObject scissorsButton = null;


    void Start()
    {
        nameInput = GameObject.Find("NameInput").GetComponentInChildren<Text>();
        ipInput = GameObject.Find("IPInput").GetComponentInChildren<Text>();

        Thread looking = new Thread(() => Looking());
        looking.Start();
    }

    // Update is called once per frame
    void Update()
    {
        if(rockButton == null && paperButton == null && scissorsButton == null)
        {
            try
            {
                rockButton = GameObject.Find("RockButton");
                paperButton = GameObject.Find("PaperButton");
                scissorsButton = GameObject.Find("ScissorsButton");
            }
            catch(Exception ex)
            {
                Debug.Log(ex);
            }
        }

        if (newOutcome == true)
        {
            GameObject.Find("CountText").GetComponent<Text>().text = outcomeData;
        }
        if (newCountdown == true)
        {
            GameObject.Find("CountText").GetComponent<Text>().text = countData;
            rockButton.SetActive(true);
            paperButton.SetActive(true);
            scissorsButton.SetActive(true);
        }
    }

    public void Looking()
    {
        while (true)
        {
            try
            {
                Debug.Log("Connecting...");
                Thread TcpThread = new Thread(() => TcpStart(ipInput.text, nameInput.text));
                TcpThread.Start();
                TcpStart(ipInput.text, nameInput.text);
                Debug.Log("Connected!");
                break;
            }
            catch (Exception ex)
            {
                Debug.Log(ex);
            }
            Thread.Sleep(1);
        }
    }


    public void TcpStart(string ip, string alias)
    {
        while (true)
        {
            try
            {
                TcpClient client = new TcpClient(ip, _port);
                NetworkStream stream = client.GetStream();
                Debug.Log("Connection started");
                Thread sendingThread = new Thread(() => SendThread(client, stream));
                Thread receivingThread = new Thread(() => ReceiveThread(client, stream));
                sendingThread.Start();
                receivingThread.Start();
                break;
                //receivingThread.Join();
                //sendingThread.Join();
            }
            catch (Exception ex)
            {
                Debug.Log(ex);
            }
        }
    }

    public void AcceptButton()
    {

        firstTime = true;
        SceneChangerScript.SceneChanger(1);
    }

    public void SendThread(TcpClient client, NetworkStream stream)
    {
        while (true)
        {
            try
            {
                if (firstTime == true)
                    FirstTime(stream);
                else
                {
                    if (newCommandFlag == true)
                    {
                        NewCommand(stream);
                        newCommandFlag = false;
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.Log("Eception: " + ex);
            }
            Thread.Sleep(1);
        }
    }

    public void ReceiveThread(TcpClient client, NetworkStream stream)
    {
        byte[] data = new byte[512];
        while (true)
        {
            try
            {
                int respLength = stream.Read(data, 0, data.Length);
                string stringData = Encoding.ASCII.GetString(data, 0, respLength);
                Debug.Log(stringData);
                string[] splitter = { ": " };
                string[] splitter2 = { ", " };
                string[] splitData = stringData.Split(splitter, System.StringSplitOptions.RemoveEmptyEntries);
                if (splitData[0] == "Outcome")
                {
                    string[] splittedPlayers = splitData[1].Split(splitter2, System.StringSplitOptions.RemoveEmptyEntries);
                    int i;
                    for (i = 0; i <= splittedPlayers.Length - 1; i++)
                    {
                        if (splittedPlayers[i] == alias)
                        {
                            outcomeData = "You Won!";
                        }
                        else if (i == splittedPlayers.Length - 1 && splittedPlayers[i] != alias)
                        {
                            outcomeData = "You Lost!";
                        }
                        newOutcome = true;
                    }
                }
                else if (splitData[0] == "Countdown")
                {
                    countData = splitData[1] + " !";
                    newCountdown = true;
                }
            }
            catch (Exception ex)
            {
                Debug.Log(ex);
            }
            Thread.Sleep(1);
        }
    }

    public void FirstTime(NetworkStream stream)
    {
        dataStruct = new Tuple<string, string>("alias", alias);
        var message = System.Text.Encoding.ASCII.GetBytes(dataStruct.Item1 + ": " + dataStruct.Item2);
        stream.Write(message, 0, message.Length);
        firstTime = false;
    }

    public void NewCommand(NetworkStream stream)
    {
        switch (dataStruct.Item1)
        {
            // Rock / Paper / Scissors //
            case "answer":
                {   
                    //msgtype: TYPE; alias: MESSAGE //
                    var message = System.Text.Encoding.ASCII.GetBytes("msgtype: "+ dataStruct.Item1 + "; "+ alias + ": " + dataStruct.Item2);
                    stream.Write(message, 0, message.Length);
                    break;
                }
        }
        newCommandFlag = false;
    }

    public void Rock()
    {
        dataStruct = new Tuple<string, string>("answer", "rock");
        rockButton.SetActive(false);
        paperButton.SetActive(false);
        scissorsButton.SetActive(false);
        newCommandFlag = true;
    }
    public void Paper()
    {
        dataStruct = new Tuple<string, string>("answer", "paper");
        rockButton.SetActive(false);
        paperButton.SetActive(false);
        scissorsButton.SetActive(false);
        newCommandFlag = true;
    }
    public void Scissors()
    {
        dataStruct = new Tuple<string, string>("answer", "scissors");
        rockButton.SetActive(false);
        paperButton.SetActive(false);
        scissorsButton.SetActive(false);
        newCommandFlag = true;
    }


}
