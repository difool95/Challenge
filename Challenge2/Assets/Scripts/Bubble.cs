using System;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(SphereCollider))]
public class Bubble : MonoBehaviour
{
    public float mergeDuration;
    private int number;
    public int Number
    {
        get { return number; }
        set
        {
            number = value;
             ApplyStyle(number);
        }
    }

    private void ApplyStyle(int num)
    {
        if(num > 2048)
        {
            //Hard coded to get 2048 Style
            ApplyStyleFromHolder(10);
        }
        else
        { 
            int tileIndex = gameController.bubbleStyleHolder.GetIndexOfNumber(num);
            if (num >= 0)
            {
                ApplyStyleFromHolder(tileIndex);
            }
            else
                Debug.LogError("Invalid tile number " + num + ". Cannot apply style as this tile is not contained in tile list");
        }
    } 
    private void ApplyStyleFromHolder(int number)
    {
        numberText.text = gameController.bubbleStyleHolder.TileStyles[number].Number.ToString();
        //Update bubble Type with number
        bubbleType = (Type)gameController.bubbleStyleHolder.TileStyles[number].Number;
        sprite.color = gameController.bubbleStyleHolder.TileStyles[number].TileColor;
    }
    public Ease mergeEase;
    private TextMesh numberText;
    private SpriteRenderer sprite;
    [SerializeField]
    private float moveSpeed = 50;
    public enum Type : int
    {
       number2 = 2,
       number4 = 4,
       number8 = 8,
       number16 = 16,
       number32 = 32,
       number64 = 64,
       number128 = 128,
       number256 = 256,
       number512 = 512,
       number1024 =1024,
       number2048 = 2048
    }

    public Type bubbleType;
    public bool isDestroyed;
    public LayerMask bubbleLayer;
    private new Rigidbody2D rigidbody2D;
    private new  CircleCollider2D collider;
    private GameController gameController;
    public bool isPlaced;
    public GameObject explosion = null;
    private Node node = null;
    private void Awake()
    {
        numberText = GetComponentInChildren<TextMesh>();
        sprite = GetComponent<SpriteRenderer>();
        rigidbody2D = GetComponent<Rigidbody2D>();
        collider = GetComponent<CircleCollider2D>();
        gameController = GameController.Instance;
    }

    private void OnCollisionEnter2D(Collision2D col)
    {
        if (isDestroyed || isPlaced)
        {
            return;
        }
        if (col.gameObject.layer == LayerMask.NameToLayer("Boundary"))
        {
            if (col.gameObject.tag == "Ceiling")
            {
                rigidbody2D.velocity = Vector2.zero;
                rigidbody2D.constraints = RigidbodyConstraints2D.FreezeAll;
            }
        }

        if (col.gameObject.layer == LayerMask.NameToLayer("Bubbles"))
        {
            rigidbody2D.velocity = Vector2.zero;
            rigidbody2D.constraints = RigidbodyConstraints2D.FreezeAll;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "death")
            DestroyWithExplosion();
        if (node == null)
            return;
        if (collision.gameObject.tag == "Lost")
        {
            gameController.state = GameController.GameState.Lost;
            gameController.LostMenu();
        }
            
    }

    /*  private void OnTriggerStay2D(Collider2D col)
      {
          if (col.gameObject.layer == LayerMask.NameToLayer("Boundary"))
          {
              if (col.gameObject.tag == "Death" && isPlaced)
              {
                  // EventManager.TriggerEvent("GameOver");
              }
          }
      }
    */

    public void CheckIfCeil()
    {
        if (IsWithoutCeil() && node != null)
        {
            Debug.Log("release");
            ReleaseBubbles();
        }
    }

    
    bool IsWithoutCeil()
    {
        List<Bubble> neighbors = GetNeighbors();
        Stack<Bubble> visited = new Stack<Bubble>();
        Stack<Bubble> mainStack = new Stack<Bubble>();
        for (int i = 0; i < neighbors.Count; ++i)
        {
            //Check if the bubble have neighbors in top directly
            if (neighbors[i].node && neighbors[i].node.isTop)
            {
                return false;
            }
            mainStack.Push(neighbors[i]);
        }

        while (mainStack.Count > 0)
        {
            Bubble b = mainStack.Pop();
            if (b.node && b.node.isTop)
            {
                return false;
            }
            visited.Push(b);
            neighbors = b.GetNeighbors();
            for (int i = 0; i < neighbors.Count; i++)
            {
                if (!mainStack.Contains(neighbors[i]) && !visited.Contains(neighbors[i]))
                {
                    mainStack.Push(neighbors[i]);
                }
            }
        }
        return true;
    }


    void ReleaseBubbles()
    {
        if (isDestroyed) return;
        rigidbody2D.constraints &= ~(RigidbodyConstraints2D.FreezePositionY | RigidbodyConstraints2D.FreezePositionX);
        rigidbody2D.gravityScale = 2;
        isPlaced = false;
        node = null;
       /* isDestroyed = true;
        isPlaced = false;
        gameController.BubbleRemoved(this);
        Destroy(gameObject, 1.0f);
       */
    }

    [Obsolete]
    void DestroyWithExplosion()
    {
        if (isDestroyed) return;
        if (explosion != null)
        {
             var fx = Instantiate(explosion, transform.position, Quaternion.identity);
             fx.GetComponentInChildren<ParticleSystem>().startColor = sprite.color;
             Destroy(fx, 1.5f);
        }
        isDestroyed = true;
        isPlaced = false;
        gameController.BubbleRemoved(this);
        //HARD CODED TO SYNC WITH BUBBLE TWEEN
        Destroy(gameObject, mergeDuration);
    }

    [Obsolete]
    void DestroyCluster(List<Bubble> cluster, Bubble bubbleDntDestroy)
    {
        for (int i = 0; i < cluster.Count; i++)
        {
            if (cluster[i] != bubbleDntDestroy && explosion != null)
            {
                cluster[i].DestroyWithExplosion();
            }
        }  
        /*    else
            {
                bubble.PrepareDestruction();
            }
        */
    }
    internal void ThrowBubble(Vector2 direction)
    {
        gameController.state = GameController.GameState.BubbleMidAir;
        transform.parent = null;
        direction.Normalize();
        rigidbody2D.velocity = direction * moveSpeed;
        collider.enabled = true;
    }

    [Obsolete]
    public void SnapToGrid(Node node)
    {
        this.node = node;
        rigidbody2D.constraints = RigidbodyConstraints2D.FreezeAll;
        gameController.BubbleAdded(this);
        if (isPlaced) return;
        isPlaced = true;
        // Merge if cluster when shooting
        MergeIfCluster();

        // Send Event
        //   EventManager.TriggerEvent("Reload");
    }

    [Obsolete]
    private void MergeIfCluster()
    {
        List<Bubble> cluster = CheckForCluster();
        if (cluster.Count >= 2)
        {
            MergeBubbles(cluster, (int)bubbleType);
        }
        else
        {
            gameController.NewBubble();
        }
    }

    [Obsolete]
    private void MergeBubbles(List<Bubble> cluster, int bubbleScore)
    {
        //TODO EVENT

        int clusterScore = GetClusterScore(cluster.Count, bubbleScore);
        Bubble bubbleToMergeTo = GetBubbleToMergeTo(clusterScore, cluster);
        DestroyCluster(cluster, bubbleToMergeTo);
        UpdateBubble(bubbleToMergeTo, clusterScore);
        TweenToBubble(bubbleToMergeTo, cluster);
        if (clusterScore >= 2048)
        {
            DestroyNeighbors(bubbleToMergeTo);
            gameController.NewBubble();
        }
    }

    [Obsolete]
    private void DestroyNeighbors(Bubble bubbleToMergeTo)
    {
        List<Bubble> neighbors = bubbleToMergeTo.GetNeighbors();
        foreach (var b in neighbors)
        {
            b.DestroyWithExplosion();
        }
    }

    private void UpdateBubble(Bubble bubbleToUpdate, int score)
    {
        bubbleToUpdate.Number = score;   
    }

    [Obsolete]
    private void TweenToBubble(Bubble bubbleToMergeTo, List<Bubble> otherBubbles)
    {
        for (int i = 0; i < otherBubbles.Count; i++)
        {
            if(otherBubbles[i] != bubbleToMergeTo)
                otherBubbles[i].GetComponent<Animator>().enabled = true;
                otherBubbles[i].transform.DOMove(bubbleToMergeTo.transform.position, mergeDuration + 0.1f)
               .SetEase(mergeEase)
               .OnComplete(() => bubbleToMergeTo.MergeIfCluster());
        }
    }


    private Bubble GetBubbleToMergeTo(int clusterScore, List<Bubble> cluster)
    {
        //Automatic Top bubble if no merge
        Bubble autoBubble = GetTopBubble(cluster);
        //The next bubble have automatic merge
        for (int i = 0; i < cluster.Count; i++)
        {
            if (HaveAutoMerge(cluster[i], clusterScore))
            {
                return cluster[i];
            }
        }
        return autoBubble;
    }

    private Bubble GetTopBubble(List<Bubble> cluster)
    {
        cluster = cluster.OrderBy(e => e.transform.position.y).ToList();
        return cluster[cluster.Count - 1];
    }

    private bool HaveAutoMerge(Bubble bubble, int clusterScore)
    {
        var neighbors = bubble.GetNeighbors();
        for (int i = 0; i < neighbors.Count; i++)
        {
            if ((int)neighbors[i].bubbleType == clusterScore)
                return true;
        }
        return false;
    }

    private int GetClusterScore(int clusterLength, int bubbleScore)
    {
        var finalScore = bubbleScore;
        for (int i = 0; i < clusterLength -1; i++)
        {
            int intermediateScore;
                intermediateScore = finalScore * 2;
            finalScore = intermediateScore;
        }
        return finalScore;
    }

    List<Bubble> CheckForCluster()
    {
        List<Bubble> neighbors = GetNeighbors();
        List<Bubble> visited = new List<Bubble>();
        Stack<Bubble> mainStack = new Stack<Bubble>();

        for (int i = 0; i < neighbors.Count; i++)
        {
            if (neighbors[i].bubbleType == bubbleType)
            {
                mainStack.Push(neighbors[i]);
            }
        }
        while (mainStack.Count > 0)
        {
            Bubble b = mainStack.Pop();
            visited.Add(b);
            neighbors = b.GetNeighbors();
            for (int i = 0; i < neighbors.Count; i++)
            {
                if (neighbors[i].bubbleType == b.bubbleType)
                {
                    if (!mainStack.Contains(neighbors[i]) && !visited.Contains(neighbors[i]))
                    {
                        mainStack.Push(neighbors[i]);
                    }
                }
            }
        }
        return visited;
    }
   
    List<Bubble> GetNeighbors()
    {
        Collider2D[] arr = Physics2D.OverlapCircleAll(transform.position, 0.9f, bubbleLayer);
        List<Bubble> ret = new List<Bubble>();
        for (int i = 0; i < arr.Length; i++)
        {
            ret.Add(arr[i].gameObject.GetComponent<Bubble>());
        }

        return ret;
    }

}


