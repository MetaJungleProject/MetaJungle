using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;

public class OnlineFightManager : MonoBehaviourPunCallbacks
{
    public static OnlineFightManager insta;

    private void Awake()
    {
        insta = this;
    }



    public override void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps)
    {
        //base.OnPlayerPropertiesUpdate(targetPlayer, changedProps);
    }

    [System.Serializable]
    public class FighterInfo {
        public static string _uid;
        public bool isFighting;
    }
}
