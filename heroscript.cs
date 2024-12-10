using System.Collections;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Управление персонажем
/// </summary>
public class heroscript : MonoBehaviour
{
    /// <summary>
    /// Что мы считаем за землю
    /// </summary>
    public LayerMask whatIsGround;
    
    /// <summary>
    /// Индикатор прикосневния с землёй
    /// </summary>
    public Transform groundCheck;
    
    /// <summary>
    /// Значение дополнительных прыжков
    /// </summary>
    public int extraJumpsValue;
    
    /// <summary>
    /// Магазин
    /// </summary>
    private GameObject usualShop;

    /// <summary>
    /// Радиус проверки земли
    /// </summary>
    public float checkRadius;
    
    /// <summary>
    /// Сила прыжка
    /// </summary>
    public float jumpForce;    
    
    /// <summary>
    /// Скорость
    /// </summary>
    [HideInInspector] public float run;
    
    /// <summary>
    /// Дополнительные прыжки
    /// </summary>
    private int extraJumps;
    
    /// <summary>
    /// Поворот персонажа
    /// </summary>
    private bool whenlook = true;
    
    /// <summary>
    /// Показатель того, касаемся ли земли
    /// </summary>
    private bool isGrounded;
    
    /// <summary>
    /// Rigidbody
    /// </summary>
    private Rigidbody2D rb;
    
    /// <summary>
    /// Камера
    /// </summary>
    private CameraControlla MyCamera;

    /// <summary>
    /// Очередь для снарядов
    /// </summary>
    private QueueManager queueManager;

    /// <summary>
    /// Активное оружие 
    /// </summary>
    public Weapons currentWeapon;

    /// <summary>
    /// Кнопка базовой атаки
    /// </summary>
    private Button basicAttack;

    /// <summary>
    /// Монетный менеджер
    /// </summary>
    private MoneyManager money;

    /// <summary>
    /// Прицел
    /// </summary>
    [SerializeField] private GameObject aimObject;

    /// <summary>
    /// Джостик
    /// </summary>
    private AroundJoystick joyStick;
    
    /// <summary>
    /// Кнопка джостика
    /// </summary>
    private BtnJoystick btnJoystick;
    
    /// <summary>
    /// Спрайт
    /// </summary>
    private SpriteRenderer sprite;
    
    /// <summary>
    /// Прыжок в обуви
    /// </summary>
    private bool bootsJump = false;
    
    /// <summary>
    /// Направо
    /// </summary>
    public bool right = false;
    
    /// <summary>
    /// Налево
    /// </summary>
    public bool left = false;

    /// <summary>
    /// Боксёрская перчатка
    /// </summary>
    [Header("Weapons")]
    [SerializeField]
    private GameObject boxingHand;

    /// <summary>
    /// Метла
    /// </summary>
    [SerializeField] private GameObject broom;

    /// <summary>
    /// Перчатки
    /// </summary>
    [SerializeField] private GameObject gloves;

    /// <summary>
    /// Ядовитое йяцо
    /// </summary>
    [SerializeField] private GameObject egg;

    /// <summary>
    /// Молот
    /// </summary>
    [SerializeField] private GameObject hammer;

    /// <summary>
    /// Тип джойстика
    /// </summary>
    private string weaponManaging;

    /// <summary>
    /// Старт
    /// </summary>
    private void Start()
    {
        weaponManaging = "joyStick";
        queueManager = GameObject.Find("Queue").GetComponent<QueueManager>();
        usualShop = GameObject.Find("Shops").transform.GetChild(0).gameObject;
        joyStick = GameObject.Find("Joysticks").transform.GetChild(0).GetComponent<AroundJoystick>();
        joyStick.SetPlayer(GetComponent<heroscript>());
        btnJoystick = GameObject.Find("Joysticks").transform.GetChild(1).GetComponent<BtnJoystick>();
        btnJoystick.SetPlayer(GetComponent<heroscript>());
        sprite = GetComponent<SpriteRenderer>();
        MyCamera = Camera.main.GetComponent<CameraControlla>();
        MyCamera.SetPlayer(transform);
        foreach (var item in FindObjectsOfType<Buttons>())
        {
            item.Setter(GetComponent<heroscript>());
        }
        rb = GetComponent<Rigidbody2D>();
        money = GameObject.Find("SilverMoney").GetComponent<MoneyManager>();
    }

    /// <summary>
    /// Изменение оружия
    /// </summary>
    /// <param name="weapon"></param>
    public void ChangeWeapon(Weapons weapon)
    {
        currentWeapon = weapon;
        switch (weapon)
        {
            case Weapons.Hammer:
            case Weapons.BoxingHand:
                joyStick.gameObject.SetActive(true);
                break;
            case Weapons.Broom:
            case Weapons.Egg:
                btnJoystick.gameObject.SetActive(true);
                break;
        }
    }

    /// <summary>
    /// Соприкосновение
    /// </summary>
    /// <param name="collision">Коллизия</param>
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Coin")
        {
            collision.transform.parent.GetComponent<CoinSpawner>().CollectCoin();
            money.AddCoin();
        }
        if (collision.tag == "Book")
        {
            collision.transform.parent.parent.GetComponent<BookSpawner>().CollectBook();
        }
        if (collision.tag == "Book")
        {
            switch (collision.GetComponent<SpriteRenderer>().sprite.name)
            {
                case "UsualBook":
                    usualShop.SetActive(true);
                    break;
                default:
                    break;
            }
        }
    }

    /// <summary>
    /// Атака
    /// </summary>
    /// <param name="value">Тип оружия</param>
    public void Attack(int value = 0)
    {
        switch (currentWeapon)
        {
            case Weapons.Broom:
                AttackBroom(value);
                break;
            case Weapons.BoxingHand:
                if (queueManager.BoxHandes.Count > 0)
                {
                    var boxHand = queueManager.BoxHandes.Dequeue();
                    boxHand.transform.localPosition = aimObject.transform.position;
                    boxHand.GetComponent<BoxingHand>().setDir(transform);
                    boxHand.SetActive(true);
                }
                else
                {
                    var boxHand = Instantiate(boxingHand, aimObject.transform.position, Quaternion.identity);
                    boxHand.GetComponent<BoxingHand>().setDir(transform);
                    boxHand.transform.SetParent(queueManager.transform.GetChild(0));
                }
                break;
            case Weapons.BasicAttack:
                gloves.SetActive(true);
                break;
            case Weapons.Egg:
                Instantiate(egg, transform.position, Quaternion.identity);
                break;
            case Weapons.Hammer:
                if (queueManager.Hammers.Count > 0)
                {
                    var hammer = queueManager.Hammers.Dequeue();
                    hammer.transform.rotation = Quaternion.identity;
                    hammer.transform.localPosition = aimObject.transform.position;
                    hammer.GetComponent<HammerAttack>().SetPlayer(this);
                    hammer.SetActive(true);
                }
                else
                {
                    var hammerObj = Instantiate(hammer, transform.position, Quaternion.identity);
                    hammerObj.transform.SetParent(queueManager.transform.GetChild(1));
                    hammerObj.GetComponent<HammerAttack>().SetPlayer(this);
                }
                break;
            case Weapons.Nothing:
            default:
                break;
        }
    }

    /// <summary>
    /// Атака метлой
    /// </summary>
    /// <param name="dir">Направление</param>
    private void AttackBroom(int dir)
    {
        var broomObj = Instantiate(broom, transform.position, Quaternion.identity);
        broomObj.transform.parent = transform;
        if (dir == -1)
            broomObj.GetComponent<Animator>().SetInteger("dir", -1);
        else if (dir == 1)
            broomObj.GetComponent<Animator>().SetInteger("dir", 1);
    }

    /// <summary>
    /// Фикс. вызов 
    /// </summary>
    private void FixedUpdate()
    {
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, checkRadius, whatIsGround);
    }
    
    /// <summary>
    /// Движение влево
    /// </summary>
    public void LeftGo()
    {
        left = true;
        whenlook = false;
        Flip();
    }
    
    /// <summary>
    /// Движение направо
    /// </summary>
    public void RightGo()
    {
        right = true;
        whenlook = true;
        Flip();
    }
    
    /// <summary>
    /// Состояние покоя
    /// </summary>
    public void Idle() // TODO: Velocity проверять не в апдейте, а по кнопкам
    {
        rb.velocity = Vector2.zero;
    }

    /// <summary>
    /// Покадровый вызов
    /// </summary>
    private void Update()
    {
        switch (weaponManaging)
        {
            case "joyStick":
                if (joyStick)
                {
                    aimObject.transform.localPosition = Vector3.zero + Vector3.ClampMagnitude(joyStick.direction * 100, 1.1f);
                    aimObject.transform.LookAt(transform);
                }
                else
                {
                    joyStick = GameObject.FindGameObjectWithTag("Aim").GetComponent<AroundJoystick>();
                }
                break;
            case "basicAttack":
                break;
            case "button":
                break;
            case "vertical":
                break;
            case "horizontal":
                break;
            case "None":
                break;
        }
        if (right || left)
        {
            if (right)
                if (rb.velocity.x > 3)
                {
                    rb.velocity = new Vector2(rb.velocity.x - 0.03f, rb.velocity.y);
                }
                else
                {
                    rb.velocity = new Vector2(rb.velocity.x + 0.18f, rb.velocity.y);
                }
            if (left)
                if (rb.velocity.x < -3)
                {
                    rb.velocity = new Vector2(rb.velocity.x + 0.03f, rb.velocity.y);
                }
                else
                {
                    rb.velocity = new Vector2(rb.velocity.x - 0.18f, rb.velocity.y);
                }
        }
        else
        {
            if (rb.velocity.x > 0.05f)
            {
                rb.velocity = new Vector2(rb.velocity.x - 0.08f, rb.velocity.y);
            }
            else if (rb.velocity.x < -0.05f)
            {
                rb.velocity = new Vector2(rb.velocity.x + 0.08f, rb.velocity.y);
            }
            else if (rb.velocity.x < 0.05f || rb.velocity.x > -0.05f)
            {
                rb.velocity = new Vector2(0, rb.velocity.y);
            }
        }
        if (isGrounded == true)
        {
            extraJumps = extraJumpsValue;
        }
    }
    
    /// <summary>
    /// Прыжок
    /// </summary>
    public void Jump()
    {
        if (isGrounded == true)
        {
            rb.velocity = new Vector2(rb.velocity.x, 0) + Vector2.up * jumpForce;
        }
        else if (extraJumps > 0)
        {
            rb.velocity = Vector2.up * jumpForce;
            extraJumps--;
        }
    }
    
    /// <summary>
    /// Получение урона
    /// </summary>
    public void GetDmg()
    {
        StartCoroutine(GetDamage());
    }

    /// <summary>
    /// Корутина смены цвета при получение урона
    /// </summary>
    /// <returns></returns>
    public IEnumerator GetDamage()
    {
        yield return new WaitForSeconds(0.15f);
        gameObject.GetComponent<CircleCollider2D>().enabled = false;
        gameObject.GetComponent<SpriteRenderer>().color = new Color(1, 0, 0, 1);
        yield return new WaitForSeconds(0.3f);
        gameObject.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 1);
        gameObject.GetComponent<CircleCollider2D>().enabled = true;
    }

    /// <summary>
    /// Поворот
    /// </summary>
    private void Flip()
    {
        if (whenlook == false)
        {
            sprite.flipX = true;
        }
        else if (whenlook == true)
        {
            sprite.flipX = false;
        }
    }
}
