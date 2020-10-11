using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Script for distraction. Distraction is a singleton as only one distraction can exist in a scene (AI can only be distracted by one event/object)
/// </summary>
public class DistractionScript : MonoBehaviour
{
    public bool IsActive;
    public static DistractionScript instance;
    private void Start()
    {
        IsActive = false;
        if (instance == null)
            instance = this;
        else
            Destroy(this.gameObject);
    }

    /// <summary>
    /// Activates the distraction for 10 seconds
    /// </summary>
    /// <param name="pos"></param>
    public void Activate(Vector3 pos)
    {
        IsActive = true;
        StopAllCoroutines();
        transform.position = pos;
        StartCoroutine(ActiveFor(10f));
    }

    /// <summary>
    /// Deactivates the distraction.
    /// </summary>
    public void Deactivate()
    {
        IsActive = false;
        StopAllCoroutines();
    }

    /// <summary>
    /// Coroutine for the distraction.
    /// </summary>
    /// <param name="time"></param>
    /// <returns></returns>
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
