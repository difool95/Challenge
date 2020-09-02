using UnityEngine;
using System.Collections.Generic;
using DG.Tweening;

public class RayCastShooter : MonoBehaviour
{
	public LayerMask nodeLayer;
	GameObject currentBubble, nextBubble;

	public GameObject dotPrefab;
	Vector2 direction;
	private bool mouseDown = false;
	private List<Vector3> dots;
	private List<GameObject> dotsPool;
	private int maxDots = 50;
	private float dotGap = 0.32f;
	private GameObject nextBubbleParent;
	public GameObject nodeParent;
	public Ease gridEase;
	GameController gc;
	int OneTimeOnTwo;
	Rect touchRectangle;
	bool inRect;
    // Use this for initialization

    void Start()
	{
		touchRectangle = new Rect(0, 0, Screen.width, 150);
		nextBubbleParent = GameObject.Find("Next");
		gc = GameController.Instance;
		GenerateFirstBubbles();
		dots = new List<Vector3>();
		dotsPool = new List<GameObject>();
		var i = 0;
		var alpha = 1.0f / maxDots;
		var startAlpha = 1.0f;
		while (i < maxDots)
		{
			var dot = Instantiate(dotPrefab);
			var sp = dot.GetComponent<SpriteRenderer>();
			var c = sp.color;
			c.a = startAlpha - alpha;
			startAlpha -= 2 * alpha;
			sp.color = c;
			dot.SetActive(false);
			dotsPool.Add(dot);
			i++;

		}
	}


	private void GenerateFirstBubbles()
    {
		currentBubble = Instantiate(gc.bubbles[Random.Range(0, 6)], transform.position, Quaternion.identity);
		nextBubble = Instantiate(gc.bubbles[Random.Range(0, 6)], nextBubbleParent.transform.position, Quaternion.identity);
		currentBubble.transform.parent = transform;
	}



	public void MoveGrid()
	{
		nodeParent.transform.DOMoveY(nodeParent.transform.position.y - 0.2f, 1)
			.SetEase(gridEase)
			.OnComplete(MoveFinished);
	}

	private void MoveFinished()
	{
		OneTimeOnTwo += 1;
		gc.grid.SetNewTopNodes();
		gc.state = GameController.GameState.bubbleReset;
		if(OneTimeOnTwo%8 == 0)
        {
			gc.grid.GenerateNewRow();
			gc.GenerateBubbles(gc.grid.tempNodes);
		}
	}

	public void GenerateNextBubble()
    {
		if(transform.childCount < 1)
        {
			nextBubble.transform.position = transform.position;
			currentBubble = nextBubble;
			nextBubble = Instantiate(gc.bubbles[Random.Range(0, 6)], nextBubbleParent.transform.position, Quaternion.identity);
			currentBubble.transform.parent = transform;
			gc.state = GameController.GameState.gridIsMoving;
			MoveGrid();
		}
	}

	void HandleTouchDown(Vector2 touch)
	{

	}
	void HandleTouchUp(Vector2 touch)
	{
		if (dots == null || dots.Count < 2)
			return;

		foreach (var d in dotsPool)
			d.SetActive(false);
		if(currentBubble != null && GameController.Instance.state == GameController.GameState.bubbleReset)
		currentBubble.GetComponent<Bubble>().ThrowBubble(direction);
	}

    void HandleTouchMove(Vector2 touch)
	{
		if (dots == null)
		{
			return;
		}
		dots.Clear();
		foreach (var d in dotsPool)
			d.SetActive(false);
		Vector2 point = Camera.main.ScreenToWorldPoint(touch);
		direction = new Vector2(transform.position.x - point.x, transform.position.y - point.y);

		RaycastHit2D hit = Physics2D.Raycast(transform.position, direction);
		if (hit.collider != null)
		{
			dots.Add(transform.position);
			if (hit.collider.tag == "SideWall")
			{
				DoRayCast(hit, direction);
			}
			else
			{
				dots.Add(hit.point);
				DrawPaths();
			}
		}
	}

	void DoRayCast(RaycastHit2D previousHit, Vector2 directionIn)
	{
		dots.Add(previousHit.point);
	/*	var normal = Mathf.Atan2(previousHit.normal.y, previousHit.normal.x);
		var newDirection = normal + (normal - Mathf.Atan2(directionIn.y, directionIn.x));
		var reflection = new Vector2(-Mathf.Cos(newDirection), -Mathf.Sin(newDirection));
		var newCastPoint = previousHit.point + (2 * reflection);
	*/
			directionIn.Normalize ();
			var	newCastPoint = new Vector2(previousHit.point.x + 2 * (-directionIn.x), previousHit.point.y + 2 * (directionIn.y));
			var	reflection = new Vector2 (-directionIn.x, directionIn.y);

		var hit2 = Physics2D.Raycast(newCastPoint, reflection);
		if (hit2.collider != null)
		{
			if (hit2.collider.tag == "SideWall")
			{
				//shoot another cast
				DoRayCast(hit2, reflection);
			}
			else
			{
				dots.Add(hit2.point);
				DrawPaths();
			}
		}
		else
		{
			DrawPaths();
		}
	}

	// Update is called once per frame
	void Update()
	{
		if (dots == null)
			return;

# if PLATFORM_ANDROID

		if (touchRectangle.Contains(Input.mousePosition))
			inRect = true;
		else
			inRect = false;
		if (Input.touches.Length > 0)
		{
			Touch touch = Input.touches[0];
			if (touchRectangle.Contains(touch.position))
				inRect = true;
			else inRect = false;

				if (touch.phase == TouchPhase.Began && inRect)
			{
				HandleTouchDown(touch.position);
			}
			else if (touch.phase == TouchPhase.Ended && inRect)
			{
				HandleTouchUp(touch.position);
			}
			else if (touch.phase == TouchPhase.Moved && inRect)
			{
				HandleTouchMove(touch.position);
			}
		}
#endif
#if UNITY_EDITOR_OSX
		else if (Input.GetMouseButtonDown(0) && inRect)
		{
			mouseDown = true;
			HandleTouchDown(Input.mousePosition);
		}
		else if (Input.GetMouseButtonUp(0) && inRect)
		{
			mouseDown = false;
			HandleTouchUp(Input.mousePosition);
		}
		else if (mouseDown && inRect)
		{
			HandleTouchMove(Input.mousePosition);
		}
#endif
	}

	void DrawPaths()
	{
		if (dots.Count > 1)
		{
			foreach (var d in dotsPool)
			d.SetActive(false);
			int index = 0;
			for (var i = 1; i < dots.Count; i++)
			{
				DrawSubPath(i - 1, i, ref index);
			}
		}
	}

    private void DrawSubPath(int start, int end, ref int index)
    {
		var pathLength = Vector2.Distance(dots[start], dots[end]);
		int numDots = Mathf.RoundToInt((float)pathLength / dotGap);
		float dotProgress = 1.0f / numDots;
		var p = 0.0f;

		while (p < 1)
		{
			var px = dots[start].x + p * (dots[end].x - dots[start].x);
			var py = dots[start].y + p * (dots[end].y - dots[start].y);
			if (index < maxDots)
			{
				var d = dotsPool[index];
				d.transform.position = new Vector2(px, py);
				d.SetActive(true);
				index++;
			}
			p += dotProgress;
		}
	}
}
