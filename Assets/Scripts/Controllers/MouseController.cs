using UnityEngine;
using System.Collections.Generic;
using UnityEngine.EventSystems;

public class MouseController : MonoBehaviour
{
    [SerializeField]
    private GameObject crossheirPrefab;
    private Vector3 _lastFramePosition;
    private Vector3 _currentFramePosition;
    private List<GameObject> _dragPreviewGameObjects;
    private TileType _currentType = TileType.Floor;


    private bool _isDragging;
    private bool _buildModeIsObject = false;
    private string _buildModeObjType;

    private Vector3 _dragStartPosition;
    // Use this for initialization
    void Start()
    {
        _isDragging = false;
        _dragPreviewGameObjects = new List<GameObject>();
        SimplePool.Preload(crossheirPrefab, 1000);
    }

    // Update is called once per frame
    void Update()
    {
        _currentFramePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);//ensure the camera is tagged as the main camera
        _currentFramePosition.z = 0;
        //update cursor position
        /*UpdateCursor();*/
        UpdateDragging();
        UpdateCameraMovement();
    }
    //todo check for nullity at this call


    /*   private void UpdateCursor()
       {
           Tile tileUnderCursor = WorldController.Instance.GetTileAtWorldCoord(_currentFramePosition);
           if (tileUnderCursor != null)
           {
               crossheir.SetActive(true);
               Vector3 cursorPosition = new Vector3(tileUnderCursor.X, tileUnderCursor.Y, 0);
               crossheir.transform.position = cursorPosition;
           }
           else
           {
               crossheir.SetActive(false);
           }
       }*/

    private void UpdateDragging()
    {
        //Start drag
        if (Input.GetMouseButtonDown(0) && !EventSystem.current.IsPointerOverGameObject()) {
            _isDragging = true;
            _dragStartPosition = _currentFramePosition;
        }
        int startX = Mathf.FloorToInt(_dragStartPosition.x);
        int endX = Mathf.FloorToInt(_currentFramePosition.x);
        /*Ensure that if the user drags backward, will still work*/
        if (endX < startX) {
            int tmp = endX;
            endX = startX;
            startX = tmp;
        }

        int startY = Mathf.FloorToInt(_dragStartPosition.y);
        int endY = Mathf.FloorToInt(_currentFramePosition.y);
        /*Ensure that if the user drags backward, will still work**/
        if (endY < startY) {
            int tmp = endY;
            endY = startY;
            startY = tmp;
        }
        while (_dragPreviewGameObjects.Count > 0) {
            GameObject go = _dragPreviewGameObjects[0];
            _dragPreviewGameObjects.RemoveAt(0);
            SimplePool.Despawn(go);
        }

        //while dragging
        if (Input.GetMouseButton(0) && _isDragging) {
            for (int x = startX; x <= endX; x++) {
                for (int y = startY; y <= endY; y++) {
                    Tile t = WorldController.Instance.World.GetTileAt(x, y);
                    if (t != null) {
                        GameObject go = SimplePool.Spawn(crossheirPrefab, new Vector3(x, y, 0), Quaternion.identity);
                        go.transform.SetParent(this.transform, true);
                        _dragPreviewGameObjects.Add(go);
                    }
                }
            }
        }
        //End drag
        if (Input.GetMouseButtonUp(0) && _isDragging) {
            for (int x = startX; x <= endX; x++) {
                for (int y = startY; y <= endY; y++) {
                    Tile t = WorldController.Instance.World.GetTileAt(x, y);
                    if (t != null) {
                        if (_buildModeIsObject) {
                            WorldController.Instance.World.PlaceInstalledObject(_buildModeObjType, t);
                        } else {
                            t.Type = _currentType;
                        }
                    }
                }
            }
            _dragStartPosition = _currentFramePosition;
            _isDragging = false;
        }
    }

    private void UpdateCameraMovement()
    {
        //handle screen dragging
        if (Input.GetMouseButton(2) || Input.GetMouseButton(1)) //right or middle mouse button
        {
            Vector3 diff = _lastFramePosition - _currentFramePosition;
            Camera.main.transform.Translate(diff);
        }
        Camera.main.orthographicSize -= Camera.main.orthographicSize * Input.GetAxis("Mouse ScrollWheel");
        Camera.main.orthographicSize = Mathf.Clamp(Camera.main.orthographicSize, 3f, 20f);
        _lastFramePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        _lastFramePosition.z = 0;

    }

    public void SetMode_BuildFloor()
    {
        _buildModeIsObject = false;
        _currentType = TileType.Floor;
    }
    public void SetMode_Bulldoze()
    {
        _buildModeIsObject = false;
        _currentType = TileType.Empty;
    }

    public void SetMode_BuildInstalledObject(string objType)
    {
        _buildModeIsObject = true;
        _buildModeObjType = objType;

    }
}
