using UnityEngine;
using System.Collections;

public class controller : MonoBehaviour {

    public int[][] graf;
    public bool[][] adiacenta;
    public GameObject nod;
    public GameObject nodAux;
    private GameObject cifra;
    public Vector3 mousePos;
    public Vector3 objPos;
    public int q;   //numarul de noduri
    public int deasupra; //deasupra carui nod este/a fost cursorul
    public bool ok; //daca se poate adauga un nod ok=1
    public bool pressed;
    public nod_control NC;
    public muchie_control MC;
    public GameObject[] cif;    //cifrele de la 0 la 9
    public float px=10; 
    public Color culoare_nod;
    public GameObject muchie;
    public GameObject muchieAux;
    public string err;
    public bool overGUI;
    public Rect lim;    //suprafata in care se pot adauga noduri
    public Vector2[] pozNod;    //pozitiile nodurilor
    public Rect labelPos;
    public Rect textFieldPos;
    public GUISkin skin;
    public int sel; //nod de plecare memorat ca intreg
    public string selS="1"; //nod de plecare memorat ca sir de caractere
    public string errMes;   //mesaj de eroare
    public Rect errMesPos;  //pozitie mesaj de eroare
    public Rect buttonPos;  //pozitie buton de pornire
    public bool startDijkstra;  //daca a inceput rularea startDijkstra = true
    public GameObject script;
    public int inf = 989892;    //valoare infinita
    public GameObject hilight;
    int sw; //lungime ecran
    int sh; //latime ecran


    void Start()
    {
        sw = Screen.width;
        sh = Screen.height;


        skin.label.fontSize = skin.textField.fontSize = skin.button.fontSize = sw * 3 / 200; //ajustam marimea fontului in functie de ecran


        //atribuiri pozitii
        lim = new Rect(sw / 30, sh / 12, sw * 5 / 8, sh * 0.85f);
        labelPos = new Rect(0.7f * sw, sh / 12, sw / 5, sh / 10);
        errMesPos = new Rect(sw/30, 0, sw, sh);   
        textFieldPos = new Rect(0.85f * sw, sh / 12, sw / 18, sh / 18);
        buttonPos = new Rect(0.7f * sw, sh / 5, sw / 7, sh / 10);

        //initiere tablouri
        pozNod = new Vector2[100];
        graf = new int[101][];
        adiacenta = new bool[101][];
        for (int i = 0; i < 101; i++)
        {
            graf[i] = new int[101];
            adiacenta[i] = new bool[101];

        }

        skin.label.normal.textColor = Color.black;
    }

    void OnGUI()
    {
        if (!startDijkstra)
        {
            GUI.skin = skin;

            GUI.Label(labelPos, "Nod de plecare(sel) = ");

            //parsare in intreg
            selS = GUI.TextField(textFieldPos, selS);
            int.TryParse(selS, out sel);
            if (sel > q)
                sel /= 10;
            selS = sel.ToString();

            if (GUI.Button(buttonPos, "Porneste!"))
            {
                if (sel < 1)
                    errMes = "Nu exista acest nod!";
                else
                {
                    startDijkstra = true;
                    hilight.GetComponent<hilightControl>().sel = sel;
                    script.SetActive(true);
                }

            }

            GUI.Label(errMesPos, errMes);
        }
    }

    void OnMouseUp()
    {
        if (pressed && ok && isInRect())
            ad();
        else
            pressed = false;
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0) && ok && !overGUI && isInRect())
            ad();

    }

    public void adauga_muchie(Vector2 X,Vector2 Y,int x,int y)
    {
        if (startDijkstra == true)
            return;

        Vector3 mid;
        Vector3 aux;
        Quaternion q;
        float unghi;
        float lungimeM;
        float lungime;
        Vector3 scale;
        float px = 0.3f;
        float dif = 0.1f;
        
        //aflam coordonatele si rotatia muchiei
        mid = X + Y;
        mid = mid / 2;
        aux = Y - X;
        unghi = Mathf.Atan2(aux.y, aux.x)* Mathf.Rad2Deg;   //unghiul dintre cele doua noduri
        q = Quaternion.AngleAxis(unghi, Vector3.forward);   //transformam unghiul in quaternion
        
        muchieAux = Instantiate(muchie, mid, q) as GameObject;

        muchieAux.name = x.ToString() + '-' + y.ToString(); //denumim muchia

        //scalam muchia intre cele doua noduri
        lungimeM = muchie.GetComponent<Renderer>().bounds.size.x;
        lungime = distanta(X, Y) - px;
        scale = muchie.transform.localScale;
        scale.x = lungime * scale.x / lungimeM - dif;
        muchieAux.transform.localScale = scale;

        muchieAux.GetComponent<SpriteRenderer>().color = Color.black;   //culoarea muchiei este neagra

        MC = muchieAux.GetComponent<muchie_control>();  //capatam accesul la variabilele muchiei create
        //initializam variabilele muchiei create
        MC.A = Camera.main.WorldToScreenPoint(X);   
        MC.B = Camera.main.WorldToScreenPoint(Y);
        MC.unghi = unghi;
        MC.a = x;MC.b = y;

    }

    public void ad()    //functia adauga un nod
    {
        if (startDijkstra == true)
            return;
        if(q==98)   //nu mai mult de 99 de noduri
            return;

        q++;    //crestem numarul nodurilor
        mousePos = Input.mousePosition;
        objPos = Camera.main.ScreenToWorldPoint(mousePos);
        objPos += new Vector3(0, 0, 10);
        NC = nod.GetComponent<nod_control>();
        NC.numar = q;

        nodAux=Instantiate(nod, objPos, Quaternion.identity) as GameObject;
        pozNod[q] = objPos; //memoram pozitia nodului

        nodAux.name= "nod " + q.ToString(); //modificam numele nodului
        pressed = true;
        objPos -= new Vector3(0, 0, 1);

        if(q<10)
        {
            cifra = Instantiate(cif[q], objPos , Quaternion.identity) as GameObject;
            cifra.transform.parent = nodAux.transform;
        }

        else
        {
            objPos -= new Vector3(px, 0, 0);
            cifra = Instantiate(cif[q / 10], objPos, Quaternion.identity) as GameObject;
            cifra.transform.parent = nodAux.transform;
            objPos += new Vector3(2*px , 0, 0);
            cifra = Instantiate(cif[q % 10], objPos, Quaternion.identity) as GameObject;
            cifra.transform.parent = nodAux.transform;
        }

    }

    float distanta(Vector2 x, Vector2 y)    // distanta dintre 2 puncte
    {
        return Mathf.Sqrt((x.x - y.x) * (x.x - y.x) + (x.y - y.y) * (x.y - y.y));
    }

    public bool isInRect()  //verifica daca mouseul este in lim
    {
        Vector2 mouse = Input.mousePosition;
        mouse.y = Screen.height - mouse.y;
        return lim.Contains(mouse);
    }
}
