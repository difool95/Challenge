using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class popup : MonoBehaviour
{
    Animator anim;
    public Color backgroundColor = new Color(10.0f / 255.0f, 10.0f / 255.0f, 10.0f / 255.0f, 0.6f);
    private GameObject m_background;
    private void Start()
    {
        anim = GetComponent<Animator>();
    }
    private void OnEnable()
    {
        AddBackground();
    }

    private void OnDisable()
    {
        anim.SetTrigger("close");
    }

    public void close()
    {
        anim.SetTrigger("close");
        //Destroy(gameObject, 1);
        RemoveBackground();
        StartCoroutine(RunPopupDestroy());
    }

    private void AddBackground()
    {
        var bgTex = new Texture2D(1, 1);
        bgTex.SetPixel(0, 0, backgroundColor);
        bgTex.Apply();

        m_background = new GameObject("PopupBackground");
        var image = m_background.AddComponent<Image>();
        var rect = new Rect(0, 0, bgTex.width, bgTex.height);
        var sprite = Sprite.Create(bgTex, rect, new Vector2(0.5f, 0.5f), 1);
        image.material.mainTexture = bgTex;
        image.sprite = sprite;
        var newColor = image.color;
        image.color = newColor;
        image.canvasRenderer.SetAlpha(0.0f);
        image.CrossFadeAlpha(1.0f, 0.4f, false);

        var canvas = GameObject.Find("Canvas");
        m_background.transform.localScale = new Vector3(1, 1, 1);
        m_background.GetComponent<RectTransform>().sizeDelta = canvas.GetComponent<RectTransform>().sizeDelta;
        m_background.transform.SetParent(canvas.transform, false);
        m_background.transform.SetSiblingIndex(canvas.transform.GetSiblingIndex()+1);
    }
  private IEnumerator RunPopupDestroy()
    {
        yield return new WaitForSeconds(1);
        Destroy(m_background);
        Destroy(gameObject);
    }
    private void RemoveBackground()
    {
        var image = m_background.GetComponent<Image>();
        if (image != null)
            image.CrossFadeAlpha(0.0f, 0.2f, false);
    }
}
