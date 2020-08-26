using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;

public class LineManager : MonoBehaviour
{
    Color c1 = Color.blue;
    Color c2 = new Color(1, 1, 1, 0);
    [HideInInspector]
    public bool didAnError, canCheckResult;
    private int answered = 0;
    Vector3 startPos;
    Vector3 endPos;
    new Camera camera;
    LineRenderer lr;
    Vector3 cameraOffset = new Vector3(0, 0, 10);
    [SerializeField] AnimationCurve ac;
    GraphicRaycaster m_Raycaster;
    PointerEventData m_PointerEventData;
    EventSystem m_EventSystem;
    List<RaycastResult> results = new List<RaycastResult>();
    RaycastResult lastResult,currentResult;

    void Start()
    {
        camera = Camera.main;
        m_Raycaster = GetComponent<GraphicRaycaster>();
        m_EventSystem = GetComponent<EventSystem>();
        m_PointerEventData = new PointerEventData(m_EventSystem);
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            m_PointerEventData.position = Input.mousePosition;
            m_Raycaster.Raycast(m_PointerEventData, results);
            //Check if we wlicked on an image
            if (results.Count != 0 && results[results.Count - 1].gameObject.CompareTag("img"))
            {
                lastResult = results[results.Count - 1];
                if (lr == null)
                    lr = gameObject.AddComponent<LineRenderer>();
                CreateLine();
                results.Clear();
            }
        }
        if (Input.GetMouseButton(0))
        {
            if (lr == null)
                return;
            endPos = camera.ScreenToWorldPoint(Input.mousePosition) + cameraOffset;
            //Set Second Point of the line
            lr.SetPosition(1, endPos);
        }
        if (Input.GetMouseButtonUp(0))
        {
            m_PointerEventData.position = Input.mousePosition;
            m_Raycaster.Raycast(m_PointerEventData, results);
            //Check if we released on a text
            if (results.Count != 0 && results[results.Count - 1].gameObject.CompareTag("choice") && lastResult.gameObject.CompareTag("img"))
            {
                answered += 1;
                if (answered == 3)
                {
                    canCheckResult = true;
                    answered = 0;
                }
                
                currentResult = results[results.Count - 1];
                //Untag both img and animal name to stop creating LineRenderers
                lastResult.gameObject.tag = "Untagged";
                currentResult.gameObject.tag = "Untagged";
                //Create a static Line
                CreateChildWithLineRenderer();
                //Check for errors
                didAnError = CheckWrongAnswer(lastResult.gameObject, currentResult.gameObject);
                lr.enabled = false;
            }
            else if(lr != null)
                lr.enabled = false;
          
            results.Clear();
        }
    }

    private bool CheckWrongAnswer(GameObject g1, GameObject g2)
    {
        if (g1.GetComponent<Item>().id != g2.GetComponent<Item>().id)
            return true || didAnError;
        return didAnError;
    }

    private void CreateChildWithLineRenderer()
    {
        GameObject a = new GameObject("A");
        Instantiate(a, transform.position, Quaternion.identity);
        a.transform.parent = transform;
        LineRenderer staticLr = a.gameObject.AddComponent<LineRenderer>();
        staticLr.enabled = true;
        staticLr.material = new Material(Shader.Find("Sprites/Default"));
        staticLr.SetColors(c1,c1);
        staticLr.positionCount = 2;
        staticLr.SetPosition(0, startPos);
        staticLr.SetPosition(1, endPos);
        staticLr.useWorldSpace = true;
        staticLr.widthCurve = lr.widthCurve;
        staticLr.numCapVertices = 10;
        
    }

    private void CreateLine()
    {
        lr.enabled = true;
        lr.material = new Material(Shader.Find("Sprites/Default"));
        lr.SetColors(c1, c1);
        lr.positionCount = 2;
        startPos = camera.ScreenToWorldPoint(Input.mousePosition) + cameraOffset;
        //Set First Point of the line
        lr.SetPosition(0, startPos);
        lr.useWorldSpace = true;
        lr.widthCurve = ac;
        lr.numCapVertices = 10;
    }
    public void ResetLineRenderers()
    {
        LineRenderer[] lineRenderers = FindObjectsOfType<LineRenderer>();
        foreach (var item in lineRenderers)
        {
            Destroy(item);
        }
    }

}
