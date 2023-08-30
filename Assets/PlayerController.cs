    using System.Collections;
    using UnityEngine;
    using UnityEngine.UI;
    using UnityEngine.SceneManagement;


    [RequireComponent(typeof(Rigidbody))]
    public class PlayerController : MonoBehaviour
    {
        public float speed = 5.0f;
        public Transform initialCameraTransform;
        public GameObject playerCamera;
        public Transform alternateCameraTransform;
        public AudioClip prizeSound;
        public AudioClip deathSound;
        public AudioClip startSound;
        public Text scoreText;
        public Text livesText;
        public Renderer playerRenderer;

        private Rigidbody rb;
        private Vector3 targetDirection;
        private bool isTurning = false;
        private int score = 0;
        private int lives = 3;
        private bool isRainbowMode = false;

    void Start()
    {
        AudioSource.PlayClipAtPoint(startSound, transform.position);
        rb = GetComponent<Rigidbody>();
        targetDirection = transform.forward;
        UpdateScoreText();
        UpdateLivesText();

        // Deshabilitar la cámara durante 4 segundos al inicio del juego
        StartCoroutine(DisableCameraTemporarily(4.0f));
    }

    IEnumerator DisableCameraTemporarily(float duration)
    {
        playerCamera.SetActive(false); // Deshabilitar la cámara
        yield return new WaitForSeconds(duration); // Esperar durante 'duration' segundos
        playerCamera.SetActive(true); // Habilitar la cámara nuevamente
    }

    void Update()
        {
            if (Input.GetKeyDown(KeyCode.RightAlt)) // AltGr
            {
                if (Camera.main.transform.position == initialCameraTransform.position)
                {
                    Camera.main.transform.position = alternateCameraTransform.position;
                    Camera.main.transform.rotation = alternateCameraTransform.rotation;
                }
                else
                {
                    Camera.main.transform.position = initialCameraTransform.position;
                    Camera.main.transform.rotation = initialCameraTransform.rotation;
                }
            }
        }

        void UpdateScoreText()
        {
            scoreText.text = "Puntuación: " + score;
        }

        void UpdateLivesText()
        {
            livesText.text = "Vidas: " + lives;
        }

        void OnTriggerEnter(Collider other)
        {
            Debug.Log("Colisión detectada con: " + other.gameObject.tag);
            if (other.gameObject.CompareTag("Premio"))
            {
                AudioSource.PlayClipAtPoint(prizeSound, transform.position);
                score += 10;
                Destroy(other.gameObject);
                UpdateScoreText();
            }
            else if (other.gameObject.CompareTag("Cereza"))
            {
                if (!isRainbowMode)
                {
                    StartCoroutine(ActivateRainbowMode());
                }
                Destroy(other.gameObject);
            }
            else if (other.gameObject.CompareTag("Enemigo"))
            {
                if (isRainbowMode)
                {
                    other.gameObject.transform.position = new Vector3(0, 0, 0);
                    score += 250;
                    UpdateScoreText();
                }
                else
                {
                    AudioSource.PlayClipAtPoint(deathSound, transform.position);
                    transform.position = new Vector3(1.2f, 1.29f, 38.8f);
                    lives--;
                    UpdateLivesText();
                }
            }
            CheckGameOver();
        }


        void CheckGameOver()
        {
            if (lives <= 0)
            {
                // Cargar la escena GameOver
                SceneManager.LoadScene("GameOver");
            }

        if (score >= 600)
        {
            // Cargar la escena GameOver
            SceneManager.LoadScene("WinScene");
        }
    }
    

        IEnumerator ActivateRainbowMode()
        {
            isRainbowMode = true;
            gameObject.tag = "JugadorAtaque";

            float elapsedTime = 0f;
            float duration = 10f;

            while (elapsedTime < duration)
            {
                playerRenderer.material.color = new Color(Random.value, Random.value, Random.value);
                elapsedTime += Time.deltaTime;
                yield return new WaitForSeconds(0.5f);
            }

            gameObject.tag = "Jugador";
            playerRenderer.material.color = Color.white;
            isRainbowMode = false;
        }

        void FixedUpdate()
        {
            // Obtener entrada del usuario solo si no estamos en medio de un giro
            if (!isTurning)
            {
                float horizontalInput = Input.GetAxisRaw("Horizontal");
                float verticalInput = Input.GetAxisRaw("Vertical");

                // Calcular la dirección de movimiento relativa a la cámara
                Vector3 forward = initialCameraTransform.forward;
                Vector3 right = initialCameraTransform.right;

                // Hacer que el movimiento sea plano (ignorar el eje y)
                forward.y = 0;
                right.y = 0;
                forward.Normalize();
                right.Normalize();

                if (Mathf.Abs(horizontalInput) > 0.5f)
                {
                    targetDirection = right * horizontalInput;
                    isTurning = true;
                }
                else if (Mathf.Abs(verticalInput) > 0.5f)
                {
                    targetDirection = forward * verticalInput;
                    isTurning = true;
                }
                else
                {
                    targetDirection = Vector3.zero;
                }
            }

            // Realizar el movimiento usando Rigidbody para mejor manejo de colisiones
            if (targetDirection != Vector3.zero)
            {
                rb.MovePosition(transform.position + targetDirection * speed * Time.deltaTime);

                // Rotar el personaje para que mire en la dirección en la que se está moviendo
                Quaternion toRotation = Quaternion.LookRotation(targetDirection, Vector3.up);
                if (Quaternion.Angle(transform.rotation, toRotation) > 1)
                {
                    transform.rotation = Quaternion.RotateTowards(transform.rotation, toRotation, speed * Time.deltaTime * 50);
                }
                else
                {
                    transform.rotation = toRotation;
                    isTurning = false;
                }
            }
        }

    }
