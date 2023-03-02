using UnityEngine;
using Cinemachine;
public class Cam : MonoBehaviour
{

    public CinemachineVirtualCamera cam;
    CinemachineTransposer cTransposer;
    public float minZoom = 0.3f;
    public float maxZoom = 20.0f;
    public float zoom=3.0f;
    public float inc=0.5f;
    public float dampFactor = 1f;
    Vector3 incVector;
    Vector3 maxZoomVector;
    Vector3 minZoomVector;
    // Start is called before the first frame update
    void Start()
    {
        cTransposer = cam.GetCinemachineComponent<CinemachineTransposer>();
        incVector = new Vector3(0, 0, inc);
        maxZoomVector = new Vector3(0, 0, maxZoom);
        minZoomVector = new Vector3(0, 0, minZoom);
        setDamping(); 
    }
    public void ZoomOut()
    {

        cTransposer.m_FollowOffset += incVector* (-cTransposer.m_FollowOffset.z);
        if (cTransposer.m_FollowOffset.z> maxZoom)
        {
            cTransposer.m_FollowOffset = maxZoomVector;
        }
        setDamping();

    }

    public void ZoomIn()
    {

        cTransposer.m_FollowOffset -= incVector *(-cTransposer.m_FollowOffset.z);
        if (cTransposer.m_FollowOffset.z < minZoom)
        {
            cTransposer.m_FollowOffset = minZoomVector; 
        }
        setDamping();

    }
    // Update is called once per frame
    void setDamping()
    {
        float f = cTransposer.m_FollowOffset.z;
        cTransposer.m_XDamping = (1 - (f - minZoom) / (maxZoom - minZoom)) * dampFactor;
        cTransposer.m_YDamping = (1 - (f - minZoom) / (maxZoom - minZoom)) * dampFactor;
    }
}
