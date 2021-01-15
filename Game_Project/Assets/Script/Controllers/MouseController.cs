using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

public class MouseController : MonoBehaviour
{
    // Start is called before the first frame update
    public Texture2D circleCursor;
    public Texture2D ax;
    public Texture2D deconstructure;
    public Texture2D hammer;

    public static MouseController Instance { get; protected set; }

    public GUIStyle MouseDragSkin;

    // The world-position of the mouse last frame.
    Vector3 lastFramePosition;
    Vector3 currFramePosition;

    // The world-position start of our left-mouse drag operation
    Vector3 dragStartPosition;
    Vector3 dragStartPosition_GUI;

    int cursor;
    int old_cursor;

    void Start()
    {
        Cursor.SetCursor(circleCursor, Vector2.zero, CursorMode.ForceSoftware);
        cursor = 0;
        old_cursor = 0;
    }

    // Update is called once per frame
    void Update()
    {
        currFramePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        currFramePosition.z = 0;

        if (old_cursor != cursor)
        {
            if(cursor == 0)
            {
                Cursor.SetCursor(circleCursor, Vector2.zero, CursorMode.ForceSoftware);
                old_cursor = cursor;
            }
            if (cursor == 1)
            {
                Cursor.SetCursor(ax, Vector2.zero, CursorMode.ForceSoftware);
                old_cursor = cursor;
            }
            if (cursor == 2)
            {
                Cursor.SetCursor(deconstructure, Vector2.zero, CursorMode.ForceSoftware);
                old_cursor = cursor;
            }
            if (cursor == 3)
            {
                Cursor.SetCursor(hammer, Vector2.zero, CursorMode.ForceSoftware);
                old_cursor = cursor;
            }
        }

        
        UpdatePressing();
       
        //UpdateDragging();
       
        //UpdateCameraMovement();

        lastFramePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        lastFramePosition.z = 0;
    }

  
    void UpdatePressing()
    {
        if (Input.GetMouseButtonDown(0))
        {
            dragStartPosition = currFramePosition;
        }

        if (Input.GetMouseButtonUp(0))
        {
            int start_x = Mathf.FloorToInt(dragStartPosition.x);
            int start_y = Mathf.FloorToInt(dragStartPosition.y);
            int end_x = Mathf.FloorToInt(currFramePosition.x);
            int end_y = Mathf.FloorToInt(currFramePosition.y);
            if(start_x == end_x && start_y == end_y)
            {
                Tile t = WorldController.Instance.world.GetTileAt(start_x, start_y);
                if (t != null)
                {
                    BuildModeController bmc = GameObject.FindObjectOfType<BuildModeController>();
                    //call buildmodecontroller
                    bmc.DoBuild(t);
                }
            }
        }

    }



    void UpdateDragging()
    {
        if (EventSystem.current.IsPointerOverGameObject())
        {
            return;
        }
        //Need fix
        //if (currFramePosition.x < 0 || currFramePosition.x > 12 || currFramePosition.y <0 || currFramePosition.y > 12)
        //{
        //    return;
        //}
        // Start Drag
        if (Input.GetMouseButtonDown(0))
        {
            dragStartPosition = currFramePosition;
            dragStartPosition_GUI = Input.mousePosition;
        }

        int start_x = Mathf.FloorToInt(dragStartPosition.x);
        int end_x = Mathf.FloorToInt(currFramePosition.x);
        int start_y = Mathf.FloorToInt(dragStartPosition.y);
        int end_y = Mathf.FloorToInt(currFramePosition.y);

        // We may be dragging in the "wrong" direction, so flip things if needed.
        if (end_x < start_x)
        {
            int tmp = end_x;
            end_x = start_x;
            start_x = tmp;
        }
        if (end_y < start_y)
        {
            int tmp = end_y;
            end_y = start_y;
            start_y = tmp;
        }


        // End Drag
        if (Input.GetMouseButtonUp(0))
        {
            // Loop through all the tiles
            for (int x = start_x; x <= end_x; x++)
            {
                for (int y = start_y; y <= end_y; y++)
                {
                    Tile t = WorldController.Instance.world.GetTileAt(x, y);
                    if (t != null)
                    {

                        //FIXME
                            t.Type = TileType.Floor;
                       
                    }
                }
            }
        }
    }

    void UpdateCameraMovement()
    {
        // Handle screen panning
        if (Input.GetMouseButton(1) || Input.GetMouseButton(2))
        {   // Right or Middle Mouse Button

            Vector3 diff = lastFramePosition - currFramePosition;
            Camera.main.transform.Translate(diff);

        }

        Camera.main.orthographicSize -= Input.GetAxis("Mouse ScrollWheel")*2f;

        Camera.main.orthographicSize = Mathf.Clamp(Camera.main.orthographicSize, 3f, 20f);

    }


    public void UpdateCursor(int i)
    {
        if(cursor == i)
        {
            if(cursor != 0)
            {
                cursor = 0;
                return;
            }
        }
        cursor = i;
    }


}
