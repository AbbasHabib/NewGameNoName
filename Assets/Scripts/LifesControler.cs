using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LifesControler : MonoBehaviour
{
    public GameObject FHeart;
    public GameObject Hrt;


    private GameObject[] Hearts;
    private CameraController CC;

    public int PlayerLifes;

    public bool FirstHeart = true;



    private void Start()
    {
        CC = CameraController.Instance;
        PlayerLifes = (int) this.gameObject.GetComponent<PlayerController>().Get_Health();
        print(this.gameObject.GetComponent<PlayerController>().gameObject.name);
        print(PlayerLifes);


        Hearts = new GameObject[PlayerLifes];



        GameObject IG;

        
        for (int i = 0; i < PlayerLifes; i++)
        {
            if (FirstHeart)
            {
                IG = Instantiate(Hrt, FHeart.transform.position, FHeart.transform.rotation) as GameObject;
                FHeart = IG;
                IG.transform.parent = gameObject.transform;
                FirstHeart = false;
                Destroy(Hrt);

            }
            else
            {
                IG = Instantiate(Hrt, new Vector3 (FHeart.transform.position.x + FHeart.transform.localScale.x * 3,FHeart.transform.position.y , 1), FHeart.transform.rotation) as GameObject;
                IG.transform.parent = gameObject.transform;
                FHeart = IG;
            }

            Hearts[i] = IG;
        }
    }

    
    public void LoseHearts(float Damage)
    {

        for (int i = 0; i < (int)Damage; ++i)
        {
            PlayerLifes -= 1;
            if (PlayerLifes < 0)
                return;

            GameObject LastHeart = this.gameObject.transform.GetChild(PlayerLifes).gameObject;
            Destroy(LastHeart);
        }

    }
}
