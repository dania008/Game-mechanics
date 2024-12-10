using UnityEngine;
using UnityEngine.UI;

public class BirdHelperForPhone : MonoBehaviour
{
    /// <summary>
    /// Текст прыжка
    /// </summary>
    [SerializeField] private Text textJump;
    
    /// <summary>
    /// Количество алмазов
    /// </summary>
    [SerializeField] private Text GemsText;
    
    /// <summary>
    /// Количество алмазов
    /// </summary>
    [SerializeField] private Text GemsText_2;
    
    /// <summary>
    /// Сила
    /// </summary>
    public int force;
    
    /// <summary>
    /// Кнопка fire
    /// </summary>
    public GameObject buttonOne;
    
    /// <summary>
    /// Кнопка ability
    /// </summary>
    public GameObject buttonTwo;
    
    /// <summary>
    /// Кнопка Jump
    /// </summary>
    public GameObject buttonThird;
    
    /// <summary>
    /// RigidBody
    /// </summary>
    private new Rigidbody2D rigidbody;
    
    /// <summary>
    /// Проигрыш
    /// </summary>
    public GameObject Loose;
    
    /// <summary>
    /// Проигрыш с чекпоинтом
    /// </summary>
    public GameObject LooseWithCheckPoint;
    
    /// <summary>
    /// Сила прыжка
    /// </summary>
    [SerializeField] private GameObject[] JumpForce;
    
    /// <summary>
    /// Кнопка атаки
    /// </summary>
    [SerializeField] private GameObject FireBallButton;
    
    /// <summary>
    /// Кнопка способности
    /// </summary>
    [SerializeField] private Button ability;
    
    /// <summary>
    /// CheckMode
    /// </summary>
    private int CheckMode;

    /// <summary>
    /// При включении
    /// </summary>
    private void OnEnable()
    {
        CheckMode = PlayerPrefs.GetInt("ModeInGame1");
        if (PlayerPrefs.GetString("Skill_3_1") == "ON" && (CheckMode == 2 || CheckMode == 1)) // СИЛА ПРЫЖКА
        {
            JumpForce[0].gameObject.SetActive(true);
            JumpForce[1].gameObject.SetActive(true);
            JumpForce[2].gameObject.SetActive(true);
        }
        textJump.text = force.ToString();
        if (PlayerPrefs.GetString("Skill_3_2") == "ON" && (CheckMode == 2 || CheckMode == 1)) // БОНУСЫ
        {
            ability.gameObject.SetActive(true);
        }
        if (PlayerPrefs.GetString("Skill_3_3") == "ON" && (CheckMode == 2 || CheckMode == 1)) // ОГОНЕК
        {
            FireBallButton.SetActive(true);
        }
    }
    
    /// <summary>
    /// Удаление сохранения чекпоинта
    /// </summary>
    public void DeleteKeyCheckPoint()
    {
        PlayerPrefs.DeleteKey("CheckPointValue");
        PlayerPrefs.DeleteKey("GemCollectInGame_1");
    }
    
    /// <summary>
    /// Перед стартом
    /// </summary>
    private void Awake()
    {
        rigidbody = GetComponent<Rigidbody2D>();
    }
    
    /// <summary>
    /// Прибавить очки прыжка
    /// </summary>
    public void PlusJump()
    {
        if (force != 9)
        {
            force++;
            textJump.text = force.ToString();
        }
    }
    
    /// <summary>
    /// Убавить очки прыжка
    /// </summary>
    public void MinusJump()
    {
        if (force != 1)
        {
            force--;
            textJump.text = force.ToString();
        }
    }
    
    /// <summary>
    /// Прыжок
    /// </summary>
    public void Jump()
    {
        rigidbody.AddForce(Vector2.up * (force - rigidbody.velocity.y), ForceMode2D.Impulse);
        rigidbody.MoveRotation(rigidbody.velocity.y * 2.0F);
    }
    
    /// <summary>
    /// Сохранение алмазов при выходе
    /// </summary>
    public void SaveGemsExit()
    {
        PlayerPrefs.SetInt("Gem", PlayerPrefs.GetInt("Gem") + System.Convert.ToInt32(GemsText.text));
    }
    
    /// <summary>
    /// Сохранение алмазов при выходе
    /// </summary>
    public void SaveGemsExit_2()
    {
        PlayerPrefs.SetInt("Gem", PlayerPrefs.GetInt("Gem") + System.Convert.ToInt32(GemsText_2.text));
    }

    /// <summary>
    /// Обработчик столкновений
    /// </summary>
    /// <param name="collision">Коллизия</param>
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (PlayerPrefs.HasKey("CheckPointValue"))
        {
            LooseWithCheckPoint.SetActive(true);
        }
        else
        {
            Loose.SetActive(true);
        }
        buttonOne.SetActive(false);
        buttonTwo.SetActive(false);
        buttonThird.SetActive(false);
        Time.timeScale = 0.0F;
    }
    
    /// <summary>
    /// Обработчик столкновений
    /// </summary>
    /// <param name="collision">Коллизия</param>
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.name == "AbilityObject(Clone)")
        {
            Destroy(collision.gameObject);
            ability.interactable = true;
        }
    }
}
