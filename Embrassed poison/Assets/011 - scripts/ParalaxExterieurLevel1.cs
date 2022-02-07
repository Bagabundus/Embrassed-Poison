using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParalaxExterieurLevel1 : MonoBehaviour
{
    public float speed_parallax1;
    public float speed_parallax2;
    public float speed_parallax3;
    public float speed_parallax4;
    public float speed_parallax5;

    private Material Parallax1;
    private Material Parallax2;
    private Material Parallax3;
    private Material Parallax4;
    private Material Parallax5;

    private Player player;

    private float Offset_Parallax1;
    private float Offset_Parallax2;
    private float Offset_Parallax3;
    private float Offset_Parallax4;
    private float Offset_Parallax5;



    public void Start()
    {
        initiate();
    }

    void initiate()
    {
        Parallax1 = transform.GetChild(0).GetComponent<Renderer>().material;
        Parallax2 = transform.GetChild(1).GetComponent<Renderer>().material;
        Parallax3 = transform.GetChild(2).GetComponent<Renderer>().material;
        Parallax4 = transform.GetChild(3).GetComponent<Renderer>().material;
        Parallax5 = transform.GetChild(4).GetComponent<Renderer>().material;

        player = FindObjectOfType<Player>();
    }

    private void Update()
    {
        Scroll();
        Follow();

    }
    void Scroll()
    {
        Offset_Parallax1 += speed_parallax1 * Time.deltaTime;
        Offset_Parallax2 += speed_parallax2 * Time.deltaTime;
        Offset_Parallax3 += speed_parallax3 * Time.deltaTime;
        Offset_Parallax4 += speed_parallax4 * Time.deltaTime;
        Offset_Parallax5 += speed_parallax5 * Time.deltaTime;

        Parallax1.SetTextureOffset("_MainTex", new Vector2(Offset_Parallax1, 0));
        Parallax2.SetTextureOffset("_MainTex", new Vector2(Offset_Parallax2, 0));
        Parallax3.SetTextureOffset("_MainTex", new Vector2(Offset_Parallax3, 0));
        Parallax4.SetTextureOffset("_MainTex", new Vector2(Offset_Parallax4, 0));
        Parallax5.SetTextureOffset("_MainTex", new Vector2(Offset_Parallax5, 0));
    }
    void Follow()
    {
        transform.position = new Vector3(player.transform.position.x, 0, 0);
    }
}
