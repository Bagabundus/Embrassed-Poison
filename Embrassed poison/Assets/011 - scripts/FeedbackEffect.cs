using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class FeedbackEffect : MonoBehaviour
{
    public Color c;
    public float speed = 1;
    bool BOOL = true;

    private TextMeshPro TMP;

    private void Awake()
    {
        TMP = GetComponent<TextMeshPro>();
        StartCoroutine(ChangeColor(1));
        Destroy(this.gameObject, 1);
    }
    void Update()
    {
        TMP.rectTransform.position += new Vector3(0, speed * Time.deltaTime, 0);
    }
    public IEnumerator ChangeColor(float timer)
    {
        timer = Mathf.Clamp(timer, 0, 1);

        if(BOOL == true)
        {
        TMP.color = c;
        }
        else
        {
        TMP.color = Color.white;
        }

        yield return new WaitForSeconds(.16f);
        BOOL = !BOOL;
        StartCoroutine(ChangeColor(timer));
        
    }

}
