using System.Collections;
using TMPro;
using UnityEngine;

public class LoadingGamePanel : MonoBehaviour
{
    public GameObject loadingPanelGameObject;
    public TMP_Text loadingContext;
    public float typingSpeed = 0.08f;

    private void Start()
    {
        loadingPanelGameObject.SetActive(false);
    }

    public void SetPanelContext(string loadingContext)
    {
        string fullText = loadingContext;
        StartCoroutine(TypeText(fullText));
    }

    private IEnumerator TypeText(string fullText)
    {
        loadingContext.text = "";
        foreach (char letter in fullText.ToCharArray())
        {
            loadingContext.text += letter;
            yield return new WaitForSeconds(typingSpeed);
        }
    }

    public void EnableGamePanel(bool enable)
    {
        loadingPanelGameObject.SetActive(enable);
    }
}
