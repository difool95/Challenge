using UnityEngine;

[RequireComponent(typeof(CircleCollider2D))]
public class Node : MonoBehaviour
{

    public bool isTop = false;
    public bool isBottom = false;

    private Bubble bubble = null;

    private void OnTriggerStay2D(Collider2D col)
    {
        if (bubble != null) return;

        if (col.gameObject.GetComponent<Bubble>().isDestroyed) return;

        if (col.gameObject.layer == LayerMask.NameToLayer("Bubbles"))
        {
            if (col.gameObject.GetComponent<Rigidbody2D>().velocity != Vector2.zero)
            {
                return;
            }

            if ((gameObject.transform.position - col.transform.position).sqrMagnitude < 0.3f)
            {
                Attach(col.gameObject.GetComponent<Bubble>());
            }
        }
    }


    public void Attach(Bubble b)
    {
        b.gameObject.transform.position = transform.position;
        bubble = b;
        bubble.transform.parent = transform;
        bubble.SnapToGrid(this);
    }
}