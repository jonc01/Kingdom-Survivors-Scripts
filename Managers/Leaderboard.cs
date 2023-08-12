using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LootLocker.Requests;
using TMPro;

public class Leaderboard : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI playerNames;
    [SerializeField] TextMeshProUGUI playerScores;
    int leaderboardID = 16807;
    string leaderboardIDstr = "16807";

    //https://www.youtube.com/watch?v=u8llsk7FoYg
    public IEnumerator SubmitScoreCO(int scoreToUpload)
    {
        bool done = false;
        string playerID = PlayerPrefs.GetString("PlayerID");
        // LootLockerSDKManager.SubmitScore(playerID, scoreToUpload, leaderboardID, (response)=>
        LootLockerSDKManager.SubmitScore(playerID, scoreToUpload, leaderboardIDstr, (response)=>
        {
            if(response.success)
            {
                Debug.Log("Successfully uploaded score");
                done = true;
            }
            else
            {
                Debug.Log("Failed" + response.Error);
            }
            
        });
        yield return new WaitWhile(()=>done == false);
    }

    public IEnumerator FetchTopHighScoresCO()
    {
        bool done = false;
        LootLockerSDKManager.GetScoreList(leaderboardIDstr, 10, 0, (response) => 
        {
            if(response.success)
            {
                string tempPlayerNames = "Names\n";
                string tempPlayerScores = "Scores\n";

                LootLockerLeaderboardMember[] members = response.items;

                for(int i=0; i<members.Length; i++)
                {
                    tempPlayerNames += members[i].rank + ". ";
                    if(members[i].player.name != "")
                    {
                        tempPlayerNames += members[i].player.name;
                    }
                    else
                    {
                        tempPlayerNames += members[i].player.id;
                    }
                    tempPlayerScores += members[i].score + "\n";
                    tempPlayerNames += "\n";
                }
                done = true;
                playerNames.text = tempPlayerNames;
                playerScores.text = tempPlayerScores;
            }
            else
            {
                Debug.Log("Failed" + response.Error);
                done = true;
            }
        });
        yield return new WaitWhile(()=> done == false);
    }
}
