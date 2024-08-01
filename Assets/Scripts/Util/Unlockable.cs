using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Unlockable : MonoBehaviour
{
    public static bool bypass = true;

    [SerializeField]
    private GoalManager goalManager;

    [SerializeField]
    private string feature;

    private Dictionary<string, int> featureToGoal = new Dictionary<string, int>{
        {"virus",           0},
        {"manual",          0},

        {"worm",            1},
        {"drain",           1},
        {"firewall",        1},

        {"recon",           2},
        {"rootkit",         2},
        {"encryption",      2},

        {"adware",          3},
        {"phishing",        3},

        {"structure",       4},
        {"apt",             4},

        {"polymorphism",    5},
        {"email-filter",    5},

        {"zero-day",        6},
        {"backdoor",        6},

        {"scan",            7},
        {"patch",           7},

        {"trojan",          8},

        {"ids",             9},

        {"sabotage",        10},
        {"dlp",             10},
        
        {"cuckoo-egg",      11},

        {"obfuscation",     12},

        {"botnet",          13},

        {"steganography",   14},

        {"disable",         15},

        {"ips",             16},

        {"ransomware",      17},

        {"end-1",           18},

        {"end-2",           19}};

    /// <summary>
    /// 
    /// </summary>
    public void Start()
    {
        UnlockCheck();
    }

    /// <summary>
    /// Sets this game object to active if this feature is unlocked and inactive otherwise.
    /// </summary>
    public void UnlockCheck() {
        if (IsUnlocked() || bypass) {
            gameObject.SetActive(true);
        } else {
            gameObject.SetActive(false);
        }
    }

    /// <summary>
    /// Returns true if this feature is unlocked and returns false otherwise.
    /// </summary>
    /// <returns>Feature is Unlocked</returns>
    private bool IsUnlocked() {
        return PlayerManager.GetPlayer(GameManager.GetTurnPlayer()).GetUnlocks()[featureToGoal[feature]] >=
            goalManager.GetGoal(featureToGoal[feature]).GetWorkRequired();
    }
}
