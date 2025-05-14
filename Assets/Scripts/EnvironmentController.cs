
using UnityEngine;


public class EnvironmentController : MonoBehaviour
{
    [SerializeField] float xspeed;
    [SerializeField] float ySpeed;
    [SerializeField] float maxHeight;
    [SerializeField] float maxWidth = 15.4f;



    public void Move(float vel, float yVel)
    {

        float finalYSpeed = Mathf.Clamp(transform.localPosition.y + (yVel * Time.deltaTime * ySpeed),0, maxHeight);
        float finalXSpeed = transform.position.x - (vel * Time.deltaTime * xspeed);
        transform.position = new Vector3(finalXSpeed, transform.position.y, transform.position.z);
        transform.localPosition= new Vector3(transform.localPosition.x ,finalYSpeed, transform.localPosition.z);
        if (transform.localPosition.x < -maxWidth)
        {
            transform.localPosition = new Vector3(15.3f, transform.localPosition.y, transform.localPosition.z);
        }
        if (transform.localPosition.x > maxWidth)
        {
            transform.localPosition = new Vector3(-15.3f, transform.localPosition.y, transform.localPosition.z);
        }



    }

}
