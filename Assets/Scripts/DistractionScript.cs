using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DistractionScript : MonoBehaviour
{
    public bool IsActive;

    private void Start()
    {
        IsActive = false;
    }

    public void Activate(Vector3 pos)
    {
        Debug.Log("Activated distraction");
        IsActive = true;
        StopAllCoroutines();
        transform.position = pos;
        StartCoroutine(ActiveFor(10f));
    }

    public void Deactivate()
    {
        IsActive = false;
        StopAllCoroutines();
    }

    IEnumerator ActiveFor(float time)
    {
        while (time > 0)
        {
            time--;
            yield return new WaitForSeconds(1f);
        }
        Deactivate();
    }
}
