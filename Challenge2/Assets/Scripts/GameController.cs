
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

[RequireComponent(typeof(GridGenerator))]
public class GameController : MonoBehaviour
{
	public enum GameState
	{
		BubbleMidAir, bubbleReset, gridIsMoving, Lost
	}
	public event System.Action<int> onBubbleMerged;

	
	new Transform camera;
	public GameObject lostUI;
	public GameObject scoreText;
	public GameState state;
	[SerializeField]
	public GameObject[] bubbles;
	private RayCastShooter Launcher;
	[HideInInspector]
    public GridGenerator grid;
	private int bubbleStartNumber = 12;
	public static GameController Instance;
	public BubbleStyleHolder bubbleStyleHolder;
	List<Bubble> bubblesInGame = new List<Bubble>();
	private void Awake()
    {
        grid = GetComponent<GridGenerator>();
		Instance = this;
	}

	public void MergedBubble(int id)
    {
		onBubbleMerged?.Invoke(id);
    }
    private void Start()
    {
		onBubbleMerged += OnScoreUI;
		camera = Camera.main.transform;
		state = GameState.bubbleReset;
		Launcher = GameObject.Find("Launcher").GetComponent<RayCastShooter>();
		//TODO Change randomization
		grid.GenerateGrid();
		GenerateBubbles(grid.tempNodes);
		grid.GenerateNewRow();
		GenerateBubbles(grid.tempNodes);
		grid.SetNewTopNodes();
	}

    private void OnScoreUI(int strike)
    {
		Debug.Log("score " + strike);
    }

    public void BubbleAdded(Bubble b)
    {
		bubblesInGame.Add(b);
	}

	public void BubbleRemoved(Bubble bubble)
	{
		bubblesInGame.Remove(bubble);
	}

	public void NewBubble()
    {
		CheckAllBubblesCeil();
		Launcher.GenerateNextBubble();
	}

	private void CheckAllBubblesCeil()
    {
        foreach (var bubble in bubblesInGame)
        {
			bubble.CheckIfCeil();
        }
		
    }
	public void GenerateBubbles(List<Node> nodes)
    {
		for (int i = 0; i < bubbleStartNumber; i++)
		{
			var rnd = Random.Range(0, 5);
			var temp = Instantiate(bubbles[rnd], nodes[i].transform.position, Quaternion.identity);
			Bubble b = temp.GetComponent<Bubble>();
			b.isPlaced = true;
			b.GetComponent<CircleCollider2D>().enabled = true;
			nodes[i].Attach(b);
		}
	}

	public void Replay()
	{
		SceneManager.LoadScene(0);
	}

    [System.Obsolete]
    public void LostMenu()
    {
		if(!lostUI.active)
		lostUI.SetActive(true);
	}

    private void OnDestroy()
    {
		onBubbleMerged -= OnScoreUI;
    }


    /*public void ScoreStrike(int strikeNumber)
    {
		//camera.DOShakePosition(0.2f, 0.4f, 10, 90, false, true);
		scoreText.SetActive(true);
		scoreText.GetComponent<Animator>().SetTrigger("strike");
		scoreText.GetComponent<Text>().text = "X" + strikeNumber;
	}
	*/
}