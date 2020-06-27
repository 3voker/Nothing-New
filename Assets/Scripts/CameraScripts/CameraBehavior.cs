using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class CameraBehavior : MonoBehaviour
{

    // Use this for initialization
    [SerializeField]
    GameObject objectToFollow;
    [SerializeField]
    Vector3 cameraOffset;

    void Start()
    {
        cameraOffset = new Vector3(0, 0, 0);
        cameraOffset.z = transform.position.z;
    }

    //// Update is called once per frame
    //void Update () {


    //}
    void LateUpdate()
    {
        transform.position = objectToFollow.transform.position + cameraOffset;
    }
}