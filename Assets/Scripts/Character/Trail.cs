using System.Collections;
using UnityEngine;

public class Trail : MonoBehaviour
{
    [SerializeField] float lifeTime;
    IEnumerator Start()
    {
        yield return new WaitForSeconds(lifeTime);
        Delete();
    }
    public void Delete()
    {
        Destroy(gameObject);
    }

}
