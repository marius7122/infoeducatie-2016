using UnityEngine;
using System.Collections;

public class hilightControl : MonoBehaviour {

    public float[] lineY;   //memoreaza coordonata y a fiecarei linii de cod
    public float speed; //viteza de tranzitie
    public controller MS;      //main script
    public Color culoare;
    public Vector3 destinatie;
    public Vector3 drum;
    public int sel;
    public Color normal;
    public Color selectat;
    public Color vizitat;
    public Color adiacent;
    public int step=0;
    public int q;
    public bool[] viz;
    public int[] d;
    public GameObject[] noduri;
    public nod_control[] ns;
    public SpriteRenderer[] sr;
    public Color hlTrue;
    public Color hlFalse;
    public Color hlColor;   //culoare hilight 
    public GameObject aux;
    public SpriteRenderer cc;
    public Rect[] but;  //pozitii butoane redare
    public Texture2D[] butT;    //texturi butoane redare
    public int PP;
    public GUISkin skin;
    public Rect label;  //pozitie label viteza de rulare
    public Rect textField;  //pozitie textield viteza de rulare
    public string textFieldS;   //viteza de rulare memorata ca sir de caractere
    public float textFieldN=1;  //viteza de rulare memorata ca numar real
    public int play;
    public int pas=1;
    public float st;    //start time
    public Rect dinNou; //pozitie buton restart
    
    int sw;
    int sh;

    public struct actiune
    {
        public Vector2 moveTo;
        public Color[] culoareNod;
        public int[] cost;
        public Color hlColor;
    }

    actiune[] steps=new actiune[1000];

    void Start()
    {
        MS = GameObject.Find("menu").GetComponent<controller>();
        destinatie = transform.position;
        destinatie += new Vector3(0, 0, 2);
        drum = destinatie;
        transform.position = destinatie;
        cc = GetComponent<SpriteRenderer>();
        cc.color = culoare;
        but = new Rect[3];
        PP = 1;

        //atribuiri
        sw = Screen.width;
        sh = Screen.height;
        Vector2 size = new Vector2(sw / 20, sw / 20);
        but[0] = new Rect(new Vector2(sw * 0.7f, sh * 0.8f), size);
        but[1] = new Rect(new Vector2(sw * 0.8f, sh * 0.8f), size);
        but[2] = new Rect(new Vector2(sw * 0.9f, sh * 0.8f), size);
        label = new Rect(sw * 0.7f, sh * 0.65f, 1000, 100);
        textField = new Rect(sw * 0.85f, sh * 0.65f, sw * 0.08f, sh * 0.05f);
        dinNou = new Rect(sw * 0.79f, sh * 0.9f, sw * 0.07f, sh * 0.07f);

        skin.label.fontSize = skin.textField.fontSize = skin.button.fontSize = sw *3/ 200;

        //initializari
        viz = new bool[MS.q + 1];
        d = new int[MS.q + 1];
        noduri = new GameObject[MS.q + 1];
        ns = new nod_control[MS.q + 1];
        sr = new SpriteRenderer[MS.q + 1];

        for (int i = 1; i <= MS.q; i++)
        {
            noduri[i] = GameObject.Find("nod " + i.ToString());
            ns[i] = noduri[i].GetComponent<nod_control>();
            sr[i] = noduri[i].GetComponent<SpriteRenderer>();
        }

        for(int i=0;i<1000;i++)
        {
            steps[i].culoareNod = new Color[MS.q + 1];
            steps[i].cost = new int[MS.q + 1];
        }

        for (int i = 1; i <= MS.q; i++) 
            GameObject.Find("nod " + i.ToString()).GetComponent<nod_control>().skin.label.normal.textColor = Color.red;
        hlColor = hlTrue;


        dijkstra();
    }

    void setStep(int I) //seteaza pasul cu numarul I
    {
        destinatie = steps[I].moveTo;
        hlColor = steps[I].hlColor;
        for(int i=1;i<=MS.q;i++)
        {
            actualizeazaCost(i, steps[I].cost[i]);
            schimbaCuloare(i, steps[I].culoareNod[i]);
        }
    }

    void actualizeazaCost(int i,int cost)
    {
        noduri[i].GetComponent<nod_control>().cost = cost;
    }

    void schimbaCuloare(int nod,Color culoare)
    {
        noduri[nod].GetComponent<SpriteRenderer>().color = culoare;
    }

    Vector2 goToLine(int i) //returneaza pozitia randului i
    {
        return new Vector2(drum.x, lineY[i-1]);
    }

    void getStep(int movePos,Color c)   //memoreaza pasul actual
    {
        step++;
        for (int i = 1; i <= MS.q; i++)
        {
            aux = noduri[i];
            steps[step].cost[i] = ns[i].cost;
            steps[step].culoareNod[i] = aux.GetComponent<SpriteRenderer>().color;
        }
        steps[step].hlColor = c;
        steps[step].moveTo = goToLine(movePos);
    }


    int i, j, N, min,ok;
    void dijkstra() //algoritm dijkstra
    {
        getStep(1, hlTrue);

        for(i=1;i<=MS.q;i++)
        {
            d[i] = MS.inf;
            ns[i].cost = MS.inf;
            
        }
        d[sel] = 0;
        ns[sel].cost = 0;

        getStep(5, hlTrue);

        while(min!=MS.inf)
        {
            min = MS.inf;
            for(i=1;i<=MS.q;i++)
                if(d[i]<min && !viz[i])
                {
                    min = d[i];
                    N = i;
                }
            

            if(min!=MS.inf)
            {
                sr[N].color = selectat;
                getStep(11, hlTrue);

                viz[N] = true;
                sr[N].color = vizitat;
                getStep(12, hlTrue);

                for (i = 1; i <= MS.q; i++)
                    if (MS.adiacenta[N][i] && !viz[i])
                    {
                        sr[i].color = adiacent;
                        if (!viz[i] && d[i] > d[N] + MS.graf[N][i])
                        {
                            getStep(14, hlTrue);
                            d[i] = d[N] + MS.graf[N][i];
                            ns[i].cost = d[i];
                            getStep(15, hlTrue);
                        }
                        else
                            getStep(14, hlFalse);
                        sr[i].color = normal;
                    }
            }


        }
        getStep(5, hlFalse);

        for (i = 1; i <= MS.q; i++)
            Debug.Log(i + "->" + d[i]);
    }

    void OnGUI()
    {
        GUI.skin = skin;
        if (GUI.Button(but[0],butT[0]))
        {
            PP = 1;
            play = 0;
            if (pas > 1)
                setStep(--pas);
        }
        if (GUI.Button(but[1], butT[PP]))
        {
            if (PP == 1)
            {
                PP = 2;
                play = 1;
            }
            else
            {
                PP = 1;
                play = 0;
                st = Time.time;
            }
        }
        if (GUI.Button(but[2], butT[3]))
        {
            PP = 1;
            play = 0;
            if (pas <= step)
                setStep(pas++);
        }

        if (GUI.Button(dinNou, "Restart!"))
            Application.LoadLevel(0);

        GUI.Label(label, "Viteza de rulare=");
        textFieldS = GUI.TextField(textField, textFieldS);
        float.TryParse(textFieldS, out textFieldN);
        if (textFieldN > 20)
            textFieldN /= 10;
        textFieldS = textFieldN.ToString();
    }

    void Update()
    {
        if (pas == 1)
            setStep(pas++);

        if(play==1)
        {
            if(Time.time > 1/textFieldN+st && pas<=step)
            {
                setStep(pas++);
                st = Time.time;
            }
        }

        transform.position = Vector2.Lerp(transform.position, destinatie, Time.deltaTime * textFieldN * 5); //miscare hilight
        cc.color = Color.Lerp(cc.color, hlColor, Time.deltaTime * textFieldN * 5);  //transformare culoare hilight
    }

    

}
