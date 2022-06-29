using UnityEngine;
using UnityEngine.UI;

using ExitGames.Client.Photon;
using Photon.Pun;

namespace Photon.Pun.UtilityScripts
{
  public class CountdownEndGame : MonoBehaviourPunCallbacks
  {
    public const string CountdownStartTime = "StartTimeEndGame";

    private bool isTimerRunning;

    private float startTime;

    public CanvasManager canvasManager;
    //CanvasManager canvasManager = CanvasManager.GetInstance();

    [Header("Reference to a Text component for visualizing the countdown")]
    public Text Text;

    [Header("Countdown time in seconds")]
    public float Countdown = 5.0f;

    public void Start()
    {
      if (Text == null)
      {
        Debug.LogError("Reference to 'Text' is not set. Please set a valid reference.", this);
        return;
      }
    }

    public void Update()
    {
      if (!isTimerRunning)
      {
        return;
      }

      float timer = (float)PhotonNetwork.Time - startTime;
      float countdown = Countdown - timer;

      string minutes = ((int)countdown/60).ToString();
      string seconds = (countdown % 60).ToString("00");

      if (seconds.Equals("60"))
      {
        return;
      }

      string timeInText = minutes + ":" + seconds;
      Text.text = timeInText;

      if (timeInText.Equals("0:00"))
      {
        Debug.Log("Fim de jogo");
        isTimerRunning = false;
        canvasManager.ShowTheWinner();
        //canvasManager.ShowTheWinner();
      }
    }

    public override void OnRoomPropertiesUpdate(Hashtable propertiesThatChanged)
    {
      object startTimeFromProps;

      if (propertiesThatChanged.TryGetValue(CountdownStartTime, out startTimeFromProps))
      {
        isTimerRunning = true;
        startTime = (float)startTimeFromProps;
      }
    }



    /*Esse bloco é do código novo que está sendo usado em uma Demo do Photon, só deixei aqui para não gritar erro atoa*/
    public static bool TryGetStartTime(out int startTimestamp)
    {
      startTimestamp = PhotonNetwork.ServerTimestamp;

      object startTimeFromProps;
      if (PhotonNetwork.CurrentRoom.CustomProperties.TryGetValue(CountdownStartTime, out startTimeFromProps))
      {
        startTimestamp = (int)startTimeFromProps;
        return true;
      }

      return false;
    }

    /*Esse bloco é do código novo que está sendo usado em uma Demo do Photon, só deixei aqui para não gritar erro atoa*/
    public static void SetStartTime()
    {
      int startTime = 0;
      bool wasSet = TryGetStartTime(out startTime);

      Hashtable props = new Hashtable
            {
                {CountdownTimer.CountdownStartTime, (int)PhotonNetwork.ServerTimestamp}
            };
      PhotonNetwork.CurrentRoom.SetCustomProperties(props);


      Debug.Log("Set Custom Props for Time: " + props.ToString() + " wasSet: " + wasSet);
    }
  }
}
