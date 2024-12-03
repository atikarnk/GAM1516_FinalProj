using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MarioCamera : MonoBehaviour
{
    public BoxCollider2D levelBoundsCollider;

    // for shaking
    private bool isShaking;
    private float shakeTimer;

    public Vector2 ViewSize
    {
        get
        {
            float viewHeight = Camera.main.orthographicSize;
            float viewWidth = viewHeight * Camera.main.aspect;
            return new Vector2(viewWidth, viewHeight);
        }
    }

    public bool CheckShaking
    {
        get { return isShaking; }
        set { isShaking = value; }
    }

    // Start is called before the first frame update
    void Start()
    {
        shakeTimer = GameConstants.CameraShakeMaxTime;
    }

    // Update is called once per frame
    void Update()
    {
        if (Game.Instance.GetMarioState.State != EMarioState.Dead)
        {
            // Get mario and the camera locations
            Vector2 marioLocation = Game.Instance.MarioGameObject.transform.position;
            Vector2 cameraLocation = transform.position;

            // Set the X value
            cameraLocation.x = Mathf.MoveTowards(cameraLocation.x, marioLocation.x, Time.deltaTime * GameConstants.MaxCameraDelta);
            cameraLocation.y = 3.5f;

            // Set the camera location
            SetCameraLocation(cameraLocation);

            if (isShaking)
            {
                Shake(marioLocation);
            }
        }
    }

    public void SetCameraLocation(Vector2 location)
    {
        Vector2 viewSize = ViewSize;
        Bounds levelBounds = levelBoundsCollider.bounds;

        float maxCameraX = levelBounds.max.x - viewSize.x;
        float minCameraX = levelBounds.min.x + viewSize.x;
        float maxCameraY = levelBounds.max.y - viewSize.y;
        float minCameraY = levelBounds.min.y + viewSize.y;

        Vector3 cameraLocation = Vector3.zero;
        cameraLocation.x = Mathf.Clamp(location.x, minCameraX, maxCameraX);
        cameraLocation.y = Mathf.Clamp(location.y, minCameraY, maxCameraY);
        cameraLocation.z = transform.position.z;

        transform.position = cameraLocation;
    }

    public void Shake(Vector2 location)
    {
        if (isShaking)
        {
            float x = UnityEngine.Random.Range(-0.25f, 0.25f);
            float y = UnityEngine.Random.Range(-0.25f, 0.25f);

            SetCameraLocation(new Vector2(location.x + x, location.y));
        }
    }
}
