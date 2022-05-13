using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndPoint : MonoBehaviour
{
    [SerializeField]GameObject FinalMessage;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            Debug.Log("End Point is reached");
            if(FinalMessage != null)
            {
                FinalMessage.SetActive(true);
            }
        }
    }
}
