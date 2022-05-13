using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameOverAllController : MonoBehaviour
{
    [Header("GameControls")]
    public bool IS_SINGLE_PLAYER_GAME;
           
    public bool IS_TWO_PLAYER_GAME;

    public bool IS_TWO_PLAYER_GAME_COOP;
         
    public bool IS_TWO_PLAYER_GAME_ENEMIES;
          
    public static GameOverAllController Instance;


    [Header("ExternalGameObjects")]
    [SerializeField] private GameObject[] P2GOBS;

    private void Awake()
    {
        Instance = this;
    }
    private void Start()
    {
        if (IS_SINGLE_PLAYER_GAME)
        {
            for (uint i = 0; i < P2GOBS.Length; i++) {
                Destroy(P2GOBS[i]);
            }
        }
    }

}
