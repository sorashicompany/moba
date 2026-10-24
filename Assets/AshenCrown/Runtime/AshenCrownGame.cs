using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public sealed class AshenCrownGame : MonoBehaviour
{
    sealed class Actor { public GameObject go; public bool enemy; public float hp = 100; public float attack = 12; public float timer; }
    readonly List<Actor> actors = new List<Actor>();
    Actor hero;
    Text hud;
    float waveTimer;
    Vector2 move;
    Camera cam;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    static void Bootstrap()
    {
        if (FindObjectOfType<AshenCrownGame>() == null) new GameObject("Ashen Crown Runtime").AddComponent<AshenCrownGame>();
    }

    void Start()
    {
        Application.targetFrameRate = 60;
        if (FindObjectOfType<EventSystem>() == null) new GameObject("EventSystem", typeof(EventSystem), typeof(StandaloneInputModule));
        cam = Camera.main;
        if (cam == null) { var c = new GameObject("Main Camera"); c.tag = "MainCamera"; cam = c.AddComponent<Camera>(); }
        cam.orthographic = true; cam.orthographicSize = 18;
        cam.transform.position = new Vector3(0, 25, -22); cam.transform.rotation = Quaternion.Euler(50, 0, 0);
        RenderSettings.ambientLight = new Color(.32f, .32f, .38f);
        var lightGo = new GameObject("Moonlight"); var light = lightGo.AddComponent<Light>(); light.type = LightType.Directional; light.intensity = 1.2f; lightGo.transform.rotation = Quaternion.Euler(45, -30, 0);
        Block("Ashen Vale", Vector3.zero, new Vector3(72, .5f, 72), new Color(.12f, .14f, .16f));
        Block("Ancient Road", Vector3.up * .28f, new Vector3(4, .08f, 72), new Color(.28f, .25f, .22f));
        Block("Ember Citadel", new Vector3(-29, 1, -29), new Vector3(7, 2, 7), new Color(.7f, .16f, .06f));
        Block("Gloam Citadel", new Vector3(29, 1, 29), new Vector3(7, 2, 7), new Color(.2f, .25f, .8f));
        hero = Spawn("Ashen Knight", false, new Vector3(-23, 1, -23), 240, new Color(1, .3f, .08f));
        Spawn("Gloam Reaver", true, new Vector3(22, 1, 22), 220, new Color(.3f, .4f, 1));
        Spawn("Grave Oracle", true, new Vector3(24, 1, 20), 140, new Color(.7f, .2f, .8f));
        Spawn("Dusk Warden", false, new Vector3(-25, 1, -20), 260, new Color(.2f, .7f, .8f));
        BuildHud();
        for (int i = 0; i < 18; i++) Block("Broken Monolith", new Vector3(Random.Range(-32,32), 1, Random.Range(-32,32)), new Vector3(1, Random.Range(1,3), 1), new Color(.2f,.21f,.24f));
    }

    GameObject Block(string name, Vector3 pos, Vector3 scale, Color color)
    {
        var go = GameObject.CreatePrimitive(PrimitiveType.Cube); go.name = name; go.transform.position = pos; go.transform.localScale = scale;
        var mat = new Material(Shader.Find("Standard")); mat.color = color; go.GetComponent<Renderer>().material = mat; return go;
    }

    Actor Spawn(string name, bool enemy, Vector3 pos, float hp, Color color)
    {
        var go = GameObject.CreatePrimitive(PrimitiveType.Capsule); go.name = name; go.transform.position = pos;
        var mat = new Material(Shader.Find("Standard")); mat.color = color; go.GetComponent<Renderer>().material = mat;
        var a = new Actor { go = go, enemy = enemy, hp = hp }; actors.Add(a); return a;
    }

    void BuildHud()
    {
        var root = new GameObject("Ashen Crown HUD"); var canvas = root.AddComponent<Canvas>(); canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        root.AddComponent<CanvasScaler>().uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize; root.AddComponent<GraphicRaycaster>();
        hud = Label(root.transform, "ASHEN CROWN  •  ПЕПЕЛ против СУМРАКА", 24, new Vector2(.5f,1), new Vector2(0,-24), new Vector2(800,60), TextAnchor.UpperCenter);
        Label(root.transform, "ПК: WASD/стрелки — движение, Space — атака, Q/W/E — способности\nТелефон: кнопки справа и экранный джойстик", 17, new Vector2(.5f,0), new Vector2(0,20), new Vector2(720,70), TextAnchor.LowerCenter);
        ButtonAt(root.transform,"АТАКА",new Vector2(.88f,.25f),new Vector2(150,75),Attack);
        ButtonAt(root.transform,"Q · РАЗЛОМ",new Vector2(.72f,.12f),new Vector2(145,62),()=>Cast(1));
        ButtonAt(root.transform,"W · ВОЛНА",new Vector2(.85f,.10f),new Vector2(145,62),()=>Cast(2));
        ButtonAt(root.transform,"E · ЗАТМЕНИЕ",new Vector2(.96f,.22f),new Vector2(155,62),()=>Cast(3));
        ButtonAt(root.transform,"◉",new Vector2(.12f,.2f),new Vector2(125,125),()=>{});
    }

    Text Label(Transform parent,string value,int size,Vector2 anchor,Vector2 pos,Vector2 dimensions,TextAnchor alignment)
    {
        var go=new GameObject("Label"); go.transform.SetParent(parent,false); var t=go.AddComponent<Text>(); t.font=Resources.GetBuiltinResource<Font>("Arial.ttf"); t.text=value; t.fontSize=size; t.color=new Color(.96f,.92f,.83f); t.alignment=alignment;
        var rt=t.rectTransform; rt.anchorMin=anchor; rt.anchorMax=anchor; rt.anchoredPosition=pos; rt.sizeDelta=dimensions; return t;
    }

    void ButtonAt(Transform parent,string title,Vector2 anchor,Vector2 size,UnityEngine.Events.UnityAction action)
    {
        var go=new GameObject(title); go.transform.SetParent(parent,false); var image=go.AddComponent<Image>(); image.color=new Color(.08f,.09f,.14f,.94f);
        var button=go.AddComponent<Button>(); button.targetGraphic=image; button.onClick.AddListener(action);
        var rt=go.GetComponent<RectTransform>(); rt.anchorMin=anchor; rt.anchorMax=anchor; rt.anchoredPosition=Vector2.zero; rt.sizeDelta=size;
        var text=Label(go.transform,title,16,Vector2.zero,Vector2.zero,size,TextAnchor.MiddleCenter); text.rectTransform.anchorMax=Vector2.one; text.raycastTarget=false;
    }

    void Update()
    {
        if (hero == null || hero.hp <= 0) return;
        float x=Input.GetAxisRaw("Horizontal"), z=Input.GetAxisRaw("Vertical");
        hero.go.transform.position += new Vector3(x,0,z).normalized * 8f * Time.deltaTime;
        var p=hero.go.transform.position; p.x=Mathf.Clamp(p.x,-34,34); p.z=Mathf.Clamp(p.z,-34,34); hero.go.transform.position=p;
        if(Input.GetKeyDown(KeyCode.Space)) Attack();
        if(Input.GetKeyDown(KeyCode.Q)) Cast(1); if(Input.GetKeyDown(KeyCode.W)) Cast(2); if(Input.GetKeyDown(KeyCode.E)) Cast(3);
        waveTimer+=Time.deltaTime; if(waveTimer>12){waveTimer=0; Spawn("Ember Wraithling",false,new Vector3(-23,1,-23),65,new Color(1,.35f,.1f)); Spawn("Gloam Thrall",true,new Vector3(23,1,23),65,new Color(.3f,.4f,1));}
        foreach(var a in actors) if(a!=hero && a.hp>0){ a.timer-=Time.deltaTime; var d=hero.go.transform.position-a.go.transform.position; if(d.magnitude>2) a.go.transform.position+=d.normalized*2.5f*Time.deltaTime; else if(a.timer<=0){hero.hp-=8;a.timer=1.4f;} }
        cam.transform.position=Vector3.Lerp(cam.transform.position,hero.go.transform.position+new Vector3(0,25,-22),Time.deltaTime*4); cam.transform.LookAt(hero.go.transform.position);
        hud.text="ASHEN CROWN  •  HP "+Mathf.Max(0,Mathf.CeilToInt(hero.hp))+"  •  Противников: "+actors.FindAll(a=>a.enemy&&a.hp>0).Count;
    }

    void Attack()
    {
        Actor target=null; float best=3.5f;
        foreach(var a in actors) if(a.enemy&&a.hp>0){float d=Vector3.Distance(hero.go.transform.position,a.go.transform.position); if(d<best){best=d;target=a;}}
        if(target!=null){target.hp-=45;if(target.hp<=0)target.go.SetActive(false);}
    }
    void Cast(int skill)
    {
        float radius=skill==1?5:skill==2?8:11; float damage=skill==1?75:skill==2?55:130;
        foreach(var a in actors) if(a.enemy&&a.hp>0&&Vector3.Distance(hero.go.transform.position,a.go.transform.position)<radius){a.hp-=damage;if(a.hp<=0)a.go.SetActive(false);}
        if(skill==3) hero.hp=Mathf.Min(240,hero.hp+35);
    }
}