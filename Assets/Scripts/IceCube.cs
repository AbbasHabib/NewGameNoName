using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IceCube : MonoBehaviour
{
    public void RemoveIce()
    {
        Invoke("DestroyIce", 0.01f);
    }

    public void MeltIce(float time)
    {
        this.GetComponent<SpriteRenderer>().color = new Color(1f, 1f, 1f, time * 0.1f + 0.1f);
    }
    private void DestroyIce()
    {
        Destroy(gameObject);
    }
}
