using UnityEngine;
using System.Collections;

public class muchie_control : MonoBehaviour {

    public Vector2 A;   //pozitie nod pornire
    public Vector2 B;   //pozitie nod destinatie
    public int a;   //nod pornire
    public int b;   //nod destinatie
    public float unghi; //unghiul dintre noduri
    public Vector2 M;   //mijloc
    public Vector2 P;   //pozitie text field
    public float dist;
    public float alfa;
    public Vector2 dim; //dimensiune text field
    public Rect TextBoxPos;
    public int cost;
    public string costTxt;
    public float dif;
    public GUISkin skin;
    public controller MS;   //main script
    public Vector2 mouse;
    public int sh;

    void Start()
    {
        MS = GameObject.Find("menu").GetComponent<controller>();
        sh = Screen.height;
        MS.adiacenta[a][b] = true;

        //calculeaza pozitie text field
        dist = Screen.height / 13;
        dif = dist / 3;
        unghi = (unghi / 180) * Mathf.PI;
        alfa = unghi + (Mathf.PI / 2);
        M = (A + B) / 2;
        P.x = M.x + dist*Mathf.Cos(alfa) - dif;
        P.y = Screen.height - (M.y + dist*Mathf.Sin(alfa)) - dif;
        dim = new Vector2(Screen.width/20, Screen.height/20);
        TextBoxPos = new Rect(P, dim);

        costTxt = "0";
    }

    void Update()
    {
        mouse = Input.mousePosition;
        mouse.y = sh - mouse.y;
        if (TextBoxPos.Contains(mouse))
        {
            MS.overGUI = true;
            if (MS.err != gameObject.transform.name)
                MS.err = gameObject.transform.name;
        }
        else if (MS.err == gameObject.transform.name)
            MS.overGUI = false;
        
    }

    void OnGUI()
    {
        GUI.skin = skin;
        costTxt = GUI.TextField(TextBoxPos, costTxt);
        cost = int.Parse(costTxt);
        if (cost < 0)
            cost = 0;
        if (cost > 9999)
            cost /=10;
        costTxt = cost.ToString();
        MS.graf[a][b] = cost;
    }

}
