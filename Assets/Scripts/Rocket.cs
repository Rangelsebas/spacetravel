using UnityEngine;
using UnityEngine.SceneManagement;

public class Rocket : MonoBehaviour
{

    Rigidbody rigidBody;
    AudioSource audioSource;
    [SerializeField] float rcsThrust = 100f;
    [SerializeField] float mainThrust = 100f;
    [SerializeField] float levelLoadDelay = 1f;
    [SerializeField] 
    KeyCode keyPositive;
    [SerializeField]
    KeyCode keyNegative;


    [SerializeField] AudioClip mainEngine;
    [SerializeField] AudioClip success;
    [SerializeField] AudioClip death;

    [SerializeField] ParticleSystem mainEngineParticles;
    [SerializeField] ParticleSystem succesParticles;
    [SerializeField] ParticleSystem deathParticles;

    bool isTransitioning = false;

    // Start is called before the first frame update
    void Start()
    {
        rigidBody = GetComponent<Rigidbody>();
        audioSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!isTransitioning) 
        {
        RespondToThrustInput();
        RespondToRotateInput();
        }
    }

    void OnCollisionEnter(Collision collision) 
    {
        if (isTransitioning) {
            return;
        }
        switch (collision.gameObject.tag) 
        {   
            case "Friendly":
                break;
            case "Finish":
                StartSuccessSequence();
                break;
            default:
                StartDeathSequence();
                break;
        }
    }

    private void StartSuccessSequence() 
    {
        isTransitioning = true;
        audioSource.Stop();
        audioSource.PlayOneShot(success);
        succesParticles.Play();
        Invoke("LoadNextLevel", levelLoadDelay);
    }

    private void StartDeathSequence() 
    {
        isTransitioning = true;
        audioSource.Stop();
        audioSource.PlayOneShot(death);
        deathParticles.Play();
        Invoke("LoadFirstLevel", levelLoadDelay);
    }

    private void LoadNextLevel() 
    {
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        int nextSceneIndex = currentSceneIndex + 1;
        if (nextSceneIndex == SceneManager.sceneCountInBuildSettings)
        {
            nextSceneIndex = 0;
        }
        SceneManager.LoadScene(nextSceneIndex);
    }

    private void LoadFirstLevel() 
    {
        SceneManager.LoadScene(0);
    }


    private void RespondToThrustInput() {
        if (Input.GetKey(KeyCode.Space)) 
        {
            ApplyThrust();
        }
        else 
        {
            StopApplyingThrust();
        }
    }

    private void StopApplyingThrust()
    {
        audioSource.Stop();
        mainEngineParticles.Stop();
    }

    private void ApplyThrust() 
    {
        rigidBody.AddRelativeForce(Vector3.up * mainThrust * Time.deltaTime);
        if (!audioSource.isPlaying) 
        {
            audioSource.PlayOneShot(mainEngine);
        }
        mainEngineParticles.Play();
    }

    private void RespondToRotateInput() 
    {
        rigidBody.angularVelocity = Vector3.zero;

        float rotationThisFrame = rcsThrust * Time.deltaTime;
        if (Input.GetKey(KeyCode.A)) 
        {
            transform.Rotate(0, 0, -1.5f * rotationThisFrame);
        }
        else if (Input.GetKey(KeyCode.D)) 
        {
            transform.Rotate(0, 0, 1.5f * rotationThisFrame);
        }
        else if (Input.GetKey(keyPositive)) 
        {
            transform.Rotate(0, 0, 1.5f * rotationThisFrame);
        }
        else if (Input.GetKey(keyNegative)) 
        {
            transform.Rotate(0, 0, -1.5f * rotationThisFrame);
        }
    }
}
