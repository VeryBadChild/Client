using UnityEngine;
using System.Net.Sockets;
using System.Collections;
using System.Threading.Tasks;
using Unity.VisualScripting;
using System.IO;
using UnityEditor.PackageManager;
using SocketIOClient;
using UnityEngine.SceneManagement;

public class OutGameServerManager : MonoBehaviour
{
#region PrivateVariables
    private string serverIP = string.Empty;
    private int serverPort = 0;
    private SocketIOUnity socket;
#endregion

#region PublicVariables
    public static OutGameServerManager instance = null;
#endregion


#region PrivateMethod
    private void Awake()
    {
        if (instance != null)
            Destroy(instance);
        instance = this;
    }

    void Start()
    {
        Init();
    }

    private void Init()
    {
        // serverIP = "localhost"; // ���� �׽�Ʈ ��
        serverIP = SecretLoader.outgameServer.ip;
        serverPort = SecretLoader.outgameServer.port;
        socket = new SocketIOUnity("http://" + serverIP +":"+serverPort);

        socket.OnConnected += (sender, e) =>
        {
            Debug.LogFormat("[OutGameServerManager] ���� ���� ���� {0}:{1}", serverIP, serverPort);
        };

        // ���� ���� �̺�Ʈ �ڵ鷯
        socket.OnDisconnected += (sender, e) =>
        {
            Debug.LogFormat("[OutGameServerManager] ���� ���� ���� {0}:{1}", serverIP, serverPort);
        };

        // ���� �̺�Ʈ �ڵ鷯
        socket.OnError += (sender, e) =>
        {
            Debug.LogError("[OutGameServerManager] ���� : " + e);
        };

        // �α��� ���� �̺�Ʈ �ڵ鷯
        socket.On("loginSucc", (res) =>
        {
            Debug.Log("Login success: " + res);
            SceneManager.LoadScene("Topdown");
            Debug.Log("�� �� ���� ���� ��!");
        });

        socket.On("loginFail", (res) =>
        {
            Debug.Log("Login fail: " + res);
        });

        // ȸ������ ���� �̺�Ʈ �ڵ鷯
        socket.On("signupSucc", (res) =>
        {
            Debug.Log("Signup success: " + res);
        });

        socket.On("signupFail", (res) =>
        {
            Debug.Log("Signup fail: " + res);
        });

        socket.On("inquiryPlayer", (res) =>
        {
            Debug.Log("inquiryPlayer: " + res);
            string jsonString = res.GetValue<string>();
            userInfo userInfo = JsonUtility.FromJson<userInfo>(jsonString);
            Debug.Log("inquiryPlayer: " + userInfo);
            Debug.Log("inquiryPlayer: " + userInfo.name);
            Debug.Log("inquiryPlayer: " + userInfo.cart);
            Debug.Log("inquiryPlayer: " + userInfo.email);
        });

        socket.On("enterRoomFail", (res) =>
        {
            Debug.Log(res);
        });

        socket.On("enterRoomSucc", (res) =>
        {
            Debug.Log(res);
        });

        socket.On("moveInGameScene", (res) =>
        {
            Debug.Log(res);
        });

        // ���� ����
        socket.Connect();

    }
#endregion

#region PublicMethod
    public static OutGameServerManager Instance()
    {
        if (instance == null)
        {
            Debug.LogError("[OutGameServerManager] �ν��Ͻ��� �������� �ʽ��ϴ�.");
            return null;
        }

        return instance;
    }

    public void LoginSucc(string email)
    {
        packet sendPacket = new packet();
        sendPacket.email = email;
        Debug.Log("������."+sendPacket);
        string jsonData = JsonUtility.ToJson(sendPacket);
        socket.Emit("loginSucc", jsonData);
    }
#endregion
}