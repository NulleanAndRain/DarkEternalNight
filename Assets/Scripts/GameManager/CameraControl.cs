using UnityEngine;

public class CameraControl : MonoBehaviour {
    private static Camera mainCamera;
    public GameObject player;
    public float PosYOffset;
    public bool useBorders;

    public float leftBorder;
    public float rightBorder;
    public float topBorder;
    public float bottomBorder;

    private float moveTime = 0;

    public static bool isMoving { get; set; } = true;

    void Start() {
        mainCamera = GetComponent<Camera>();
        _pos = transform.position;
    }

    private Vector3 _pos;
    void Update() {
        if (!isMoving || PauseControl.isPaused) return;
        moveTime %= Time.fixedDeltaTime;
        moveTime += Time.deltaTime * 3;

        if (useBorders) {
            _pos.x = Mathf.Lerp(
                    mainCamera.transform.position.x,
                    Mathf.Clamp(player.transform.position.x, leftBorder, rightBorder),
                    moveTime);
            _pos.y = Mathf.Lerp(
                    mainCamera.transform.position.y,
                    Mathf.Clamp(player.transform.position.y + PosYOffset, bottomBorder, topBorder),
                    moveTime);
        } else {
            _pos.x = Mathf.Lerp(
                    mainCamera.transform.position.x,
                    player.transform.position.x,
                    moveTime);
            _pos.y = Mathf.Lerp(
                    mainCamera.transform.position.y,
                    player.transform.position.y + PosYOffset,
                    moveTime);
        }
        mainCamera.transform.position = _pos;
    }

}