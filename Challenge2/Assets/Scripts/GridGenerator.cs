using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GridGenerator : MonoBehaviour
{
    public GameObject node;
    public float yoffset = 0.8f;
    [HideInInspector]
    public List<Node> tempNodes = new List<Node>();
    [HideInInspector]
    public List<Node> allNodes = new List<Node>();
    public GameObject nodeParent;

    public void GenerateGrid()
    {
        Vector2 gridpositionrow1 = new Vector2(-2.01f, 4.59f);
        Vector2 gridpositionrow2 = new Vector2(-2.35f, 3.79f);
        float increment = 0;

        for (int k = 0; k < 5; k++)
        {
            GameObject temp = null;
            for (int i = 0; i < 6; i++)
            {
                temp = Instantiate(node, gridpositionrow1, transform.rotation);
                temp.transform.parent = nodeParent.transform;
                gridpositionrow1 += new Vector2(0.85f, 0);

                if (k == 0)
                {
                    temp.GetComponent<Node>().isTop = true;
                }

                // add node to list
                tempNodes.Add(temp.GetComponent<Node>());
                allNodes.Add(temp.GetComponent<Node>());
            }
            for (int i = 0; i < 6; i++)
            {
                temp = Instantiate(node, gridpositionrow2, transform.rotation);
                temp.transform.parent = nodeParent.transform;
                gridpositionrow2 += new Vector2(0.85f, 0);

                if (k == 4)
                {
                    temp.GetComponent<Node>().isBottom = true;
                }

                // add node to list
                tempNodes.Add(temp.GetComponent<Node>());
                allNodes.Add(temp.GetComponent<Node>());
            }
            increment += yoffset;
            gridpositionrow1 = new Vector3(-2.01f, 4.59f - increment * 2, 0);
            gridpositionrow2 = new Vector3(-2.35f, 3.79f - (increment * 2), 0);
        }
    }

    public void GenerateNewRow()
    {
        tempNodes.Clear();
        Vector2 NewRowPosition1 = new Vector2(-2.01f, 6.16f) ;
        Vector2 NewRowPosition2 = new Vector2(-2.35f, 5.375f);
        float increment = 0;
        for (int k = 0; k < 1; k++)
        {
            GameObject temp = null;
            for (int i = 0; i < 6; i++)
            {
                temp = Instantiate(node, NewRowPosition1, transform.rotation);
                temp.transform.parent = nodeParent.transform;
                NewRowPosition1 += new Vector2(0.85f, 0);
                if (k == 0)
                {
                    temp.GetComponent<Node>().isTop = true;
                }
                // add node to list
                tempNodes.Add(temp.GetComponent<Node>());
                allNodes.Add(temp.GetComponent<Node>());
            }
            for (int i = 0; i < 6; i++)
            {
                temp = Instantiate(node, NewRowPosition2, transform.rotation);
                temp.transform.parent = nodeParent.transform;
                NewRowPosition2 += new Vector2(0.85f, 0);
                // add node to list
                tempNodes.Add(temp.GetComponent<Node>());
                allNodes.Add(temp.GetComponent<Node>());
            }
            increment += yoffset;
            NewRowPosition1 = new Vector3(-2.01f, 6.16f - increment * 2, 0);
            NewRowPosition2 = new Vector3(-2.35f, 5.375f - (increment * 2), 0);
        }
    }

    public void SetNewTopNodes()
    {
        List<Node> topNodes = new List<Node>();
        List<Node> nonTopNodes = new List<Node>();
        topNodes =
            (from node in allNodes
            where (node.transform.position.y > 4.5f)
            select node).ToList();
        foreach (var n in topNodes)
        {
            n.isTop = true;
        }
        nonTopNodes =
            (from node in allNodes
             where (node.transform.position.y < 4.5f)
             select node).ToList();
        foreach (var n in nonTopNodes)
        {
            n.isTop = false;
        }
    }

   
    
}
