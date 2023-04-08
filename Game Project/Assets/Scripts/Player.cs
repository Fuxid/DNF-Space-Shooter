using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField]
    private float _velocity;
    [SerializeField]
    private GameObject _laserPrefab;
    [SerializeField]
    private float _fireRate = 0.2f;
    private float _nextFire;
    [SerializeField]
    private int _health = 3;
    private SpawnManager _spawnManager;
    [SerializeField]
    private bool _isTripleShotActive = false;
    [SerializeField]
    private GameObject _tripleShotPrefab;
    [SerializeField]
    private bool _isSpeedBoostActive = false;
    [SerializeField]
    private float _speedBoostMultiplier = 1.5f;
    [SerializeField]
    private bool _isShieldActive = false;
    [SerializeField]
    private GameObject _shieldObject;
    [SerializeField]
    private int _score;
    private UIManager _uiManager;


    void Start()
    {
        transform.position = new Vector3(0, -1, 0);
        _spawnManager = GameObject.Find("Spawn_Manager").GetComponent<SpawnManager>();
        _uiManager = GameObject.Find("Canvas").GetComponent<UIManager>();

        if (_spawnManager == null)
        {
            Debug.LogError("The Spawn Manager is NULL.");
        }

        if (_uiManager == null)
        {
            Debug.LogError("The UI Manager is NULL");
        }
    }

    void Update()
    {
        CalculateMovement();
        if (Input.GetKey(KeyCode.Space) && Time.time > _nextFire)
        {
            FireLaser();
        }
    }

    void CalculateMovement()
    {
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");
        Vector3 direction = new Vector3(horizontalInput, verticalInput, 0);
        transform.Translate(direction * _velocity * Time.deltaTime);


        if (transform.position.y >= 0)
        {
            transform.position = new Vector3(transform.position.x, 0, transform.position.z);
        }
        else if (transform.position.y <= -4.1f)
        {
            transform.position = new Vector3(transform.position.x, -4.1f, transform.position.z);
        }

        if (transform.position.x >= 10.5f)
        {
            transform.position = new Vector3(-10.5f, transform.position.y, transform.position.z);
        }
        else if (transform.position.x <= -10.5f)
        {
            transform.position = new Vector3(10.5f, transform.position.y, transform.position.z);
        }
    }

    void FireLaser()
    {
    
        _nextFire = Time.time + _fireRate;
        if (_isTripleShotActive == false)
        {
            Instantiate(_laserPrefab, transform.position + new Vector3(0, 1.3f, 0), Quaternion.identity);
        }
        else if (_isTripleShotActive == true)
        {

            Instantiate(_tripleShotPrefab, transform.position, Quaternion.identity);
        }
    


    }

    public void Damage()
    {
        if (_isShieldActive == false)
        {
            _health --;
            _uiManager.UpdateLives(_health);
        }
        else
        {
            _isShieldActive = false;
            _shieldObject.SetActive(false);
            return;
        }

        if (_health <= 0)
        {
            _spawnManager.OnPlayerDeath();
            Destroy(this.gameObject);
        }
    }

    public void TripleShotActive()
    {
        _isTripleShotActive = true;
        StartCoroutine(TripleShotPowerDownRoutine());
    }

     IEnumerator TripleShotPowerDownRoutine()
    {
        yield return new WaitForSeconds(5.0f);
        _isTripleShotActive = false;
    }

    public void SpeedBoostActive()
    {
        _isSpeedBoostActive = true;
        _velocity *= _speedBoostMultiplier;
        StartCoroutine(SpeedBoostPowerDownRoutine());
    }
    IEnumerator SpeedBoostPowerDownRoutine()
    {
        yield return new WaitForSeconds(5.0f);
        _isSpeedBoostActive = false;
        _velocity /= _speedBoostMultiplier;
    }

    public void ShieldActive()
    {
        _isShieldActive = true;
        _shieldObject.SetActive(true);
    }

    public void AddScore(int points)
    {
        _score += points;
        _uiManager.UpdateScore(_score);
    }
}
