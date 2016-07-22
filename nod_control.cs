using UnityEngine;
using System.Collections;

public class nod_control : MonoBehaviour {

    public int numar;
    public GameObject menu;
    public controller MS;   //main script
    public bool drag;   //daca se face scroll de pe nod drag=true
    public bool adm;
    public int cost;    //costul de la nodul selectat la acest nod memorat ca intreg
    public string costS;    //costul de la nodul selectat la acest nod memorat ca sir de caractere
    public Rect labelPos;
    public GUISkin skin;
    public hilightControl hc;
    int sh;
    int sw;

    void Start()
    {
        //initializari
        sh = Screen.height;
        sw = Screen.width;
        menu = GameObject.Find("menu");
        MS = menu.GetComponent<controller>();
        GetComponent<SpriteRenderer>().color = MS.culoare_nod;
        Vector2 colt = Camera.main.WorldToScreenPoint(transform.position);
        Vector2 size = new Vector2(sw / 10, sh / 10);
        colt = new Vector2(colt.x, sh - colt.y - sh / 15);
        labelPos = new Rect(colt,size);
    }

    void OnGUI()
    {
        if(MS.startDijkstra==true)
        {
            GUI.skin = skin;

            if (cost == MS.inf)
                costS = "inf";
            else
                costS = cost.ToString();

            GUI.Label(labelPos, costS);
        }
    }

    void OnMouseOver()
    {
        MS.ok = false;
        MS.err = "suprapunere";
        adm = true;
        MS.deasupra = numar;
    }

    void OnMouseExit()
    {
        if (MS.deasupra == numar && adm)
        {
            MS.ok = true;
            adm = false;
        }
    }

	void OnMouseDrag()
    {
        drag = true;
    }

    void OnMouseUp()
    {
        Vector2 mousePos;
        mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        if (drag && MS.ok && !MS.overGUI && MS.isInRect())
        {
            MS.ad();
            MS.adauga_muchie(transform.position,mousePos,numar, MS.q);
        }
        if (!MS.ok && MS.deasupra != numar && MS.adiacenta[numar][MS.deasupra] == false)
        {
            MS.adauga_muchie(transform.position, GameObject.Find("nod " + MS.deasupra.ToString()).transform.position, numar, MS.deasupra);
        }
        drag = false;
    }
}
