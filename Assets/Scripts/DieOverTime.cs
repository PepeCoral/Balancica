using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DieOverTime : MonoBehaviour
{
    [SerializeField] float lifeTime;
    float t = 0;
    // Update is called once per frame
    void Update()
    {
        t += Time.deltaTime;
        if (t >= lifeTime)
        {
            Destroy(this.gameObject);
        }
    }
}
