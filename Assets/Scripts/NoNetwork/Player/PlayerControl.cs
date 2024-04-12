using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControl : MonoBehaviour
{
    public CharacterItem playerCharacter;

    public float moveSpeed = 7f;
    [SerializeField] private float speedMultiplier = 1f;
    [SerializeField] private float speedBoost = 1f;
    public float rotationSpeed = 1f;
    public Animator anim;
    public GameObject trailBoost;

    [SerializeField] private bool autoRun = false;

    private float movementV = 0;
    private float movementH = 0;

    private Countdown countdownTimer;
    private FinishController finishController;
    private GoalController goalController;
    private FloatingJoystick joystick;
    private Rigidbody rB;
    private Camera mainCamera;
    private int playerSPIndex = -1;
    private Transform charModel;
    private CreateBots botSpawner;
    private GameObject goal;
    private bool isFrozen = false;
    private Vector3 gravityDirection;
    private bool isRecovering = false;
    private bool isEffectApplied = false;
    private bool faceDetected = true;

    SaveData saveData;
    [SerializeField] List<CharacterItem> charactersList;

    public bool testing; //to move the character on editor



    private PlayerMovement playerMovement = new PlayerMovement(0, 0, Vector3.zero);

    public struct PlayerMovement
    {
        public float _moveV;
        public float _moveH;
        public Vector3 _position;

        public PlayerMovement(float moveV, float moveH, Vector3 position)
        {
            _moveV = moveV;
            _moveH = moveH;
            _position = position;
        }
    }


    private void SendSpawnpointDataClientRpc(int spIndex, Vector3 spPosition)
    {
        transform.position = new Vector3(spPosition.x, spPosition.y + 1, spPosition.z);
        playerSPIndex = spIndex;
        Debug.Log("[CLIENT RPC]-Player spawned at SP: " + spIndex);
    }

    private void StartCountdownClientRpc()
    {
        countdownTimer.StartCountdown();
    }


    void ReadCharacterSaved()
    {
        saveData.ReadFromJson();

        CharacterItem actualCharacter = new CharacterItem();
        //Buscar la skin
        string savedSkin = saveData.player.playerCharacterData.characterName;
        if (savedSkin == null)
        {
            Debug.LogError("Error: No hay personaje guardado");
            return;
        }

        //Buscar en qué índice de la lista de personajes está, segun el NOMBRE de la skin
        int characterActive = charactersList.FindIndex(character => character.characterName == savedSkin);
        actualCharacter = charactersList[characterActive];

        //Asignar colores guardados
        Color color = Color.black; //si falla saldrá negro
        if (ColorUtility.TryParseHtmlString(saveData.player.playerCharacterData.hairColor, out color))
        {
            actualCharacter.hairColor = color;
        }
        if (ColorUtility.TryParseHtmlString(saveData.player.playerCharacterData.skinColor, out color))
        {
            actualCharacter.skinColor = color;
        }
        if (ColorUtility.TryParseHtmlString(saveData.player.playerCharacterData.bottomColor, out color))
        {
            actualCharacter.bottomColor = color;
        }
        if (ColorUtility.TryParseHtmlString(saveData.player.playerCharacterData.topColor, out color))
        {
            actualCharacter.topColor = color;
        }

        //scriptable object con estos datos
        playerCharacter.characterName = actualCharacter.characterName;
        playerCharacter.prefab = actualCharacter.prefab;
        playerCharacter.hair = actualCharacter.hair;
        playerCharacter.skin = actualCharacter.skin;
        playerCharacter.top = actualCharacter.top;
        playerCharacter.bottom = actualCharacter.bottom;
        playerCharacter.hairColor = actualCharacter.hairColor;
        playerCharacter.skinColor = actualCharacter.skinColor;
        playerCharacter.topColor = actualCharacter.topColor;
        playerCharacter.bottomColor = actualCharacter.bottomColor;
    }

    private void Awake()
    {
        saveData = FindAnyObjectByType<SaveData>();
        ReadCharacterSaved();

        countdownTimer = FindObjectOfType<Countdown>();
        finishController = FindObjectOfType<FinishController>();
        botSpawner = FindObjectOfType<CreateBots>();
        goalController = FindObjectOfType<GoalController>();
        goal = goalController.gameObject;


        FaceTrackingToMovement.onCaraDetectadaEvent += caraDetectada;
        FaceTrackingToMovement.onCaraNoDetectadaEvent += caraNoDetectada;

        //Asignar personaje guardado
        GameObject playerContainer = GameObject.FindGameObjectWithTag("Player");
        GameObject characterInPrefab = GameObject.FindGameObjectWithTag("Character");
        GameObject newCharacter = Instantiate(playerCharacter.prefab, characterInPrefab.transform.position, Quaternion.identity, playerContainer.transform);
        newCharacter.transform.localScale = new Vector3(0.3f, 0.3f, 0.3f);
        playerContainer.GetComponent<PlayerControl>().anim = newCharacter.GetComponent<Animator>();
        Destroy(newCharacter.GetComponent<CapsuleCollider>());
        Destroy(newCharacter.GetComponent<RotateCharacter>());
        
        newCharacter.transform.SetSiblingIndex(0);

        Destroy(characterInPrefab);


    }


    private void OnDisable()
    {
        FaceTrackingToMovement.onCaraDetectadaEvent -= caraDetectada;
        FaceTrackingToMovement.onCaraNoDetectadaEvent -= caraNoDetectada;
    }

    void caraDetectada()
    {
        faceDetected = true;
        anim.SetBool("isRunning", true);
    }

    void caraNoDetectada()
    {
        faceDetected = false;
        anim.SetBool("isRunning", false);
    }

    private void Start()
    {

            joystick = FindObjectOfType<FloatingJoystick>();
            joystick.enabled = true;
            joystick.GetComponentInChildren<Transform>().gameObject.SetActive(true);

            mainCamera = Camera.main;

            if (goalController != null) goalController.Reset();

        charModel = anim.transform;

        rB = GetComponent<Rigidbody>();

        rB.isKinematic = false;
        rB.useGravity = false;
        gravityDirection = new Vector3(0, Physics.gravity.y * 2, 0);
        rB.interpolation = RigidbodyInterpolation.None;
        rB.collisionDetectionMode = CollisionDetectionMode.Continuous;
        //-------------------------------------------------------------------------------------------

        if (countdownTimer == null) countdownTimer = FindObjectOfType<Countdown>();
        if (finishController == null) finishController = FindObjectOfType<FinishController>();
        if (botSpawner == null) botSpawner = FindObjectOfType<CreateBots>();

        countdownTimer.Reset();


            //When a Player spawn, the Server searches for a not occupied spawnpoint from the NetworkList for him.
            SearchNextFreeSpawnPoint();

            //Each time a Player joins the lobby his Server side checks num of players connected to that lobby and starts the countdown when full
            //CheckConnectedPlayers();

    }

    private void SearchNextFreeSpawnPoint()
    {
        Debug.Log("[SERVER]-SP Count: " + botSpawner.spawnpointData.Count);

        for (int i = 0; i <= botSpawner.spawnpointData.Count; i++)
        {
            Debug.Log("[SERVER]-SP Current: " + i);

            /*if (i == botSpawner.spawnpointData.Count)
            {
                Debug.Log("[SERVER]-The room is full, exiting...");
                //matchMaker.DestroyOrExitLobby();
                //mainCamera.enabled = true;
                break;
            }*/

            if (!botSpawner.spawnpointData[i]._isOccupied)
            {
                //Updates the spawnpoint NetworkList with this spawnpoint as isOccupied and the playerId on it
                botSpawner.UpdateSpawnpointsList(i, botSpawner.spawnpointData[i]._spPosition, true, "Player");

                transform.position = new Vector3(botSpawner.spawnpointData[i]._spPosition.x, botSpawner.spawnpointData[i]._spPosition.y + 1, botSpawner.spawnpointData[i]._spPosition.z);
                //playerMovement.Value = new PlayerMovement { _moveV = 0, _moveH = 0, _position = transform.position };
                playerSPIndex = i;
                Debug.Log("[SERVER]-Player spawned at SP: " + i);

                //Calls a ClientRPC to tell their relative Client Player his spawnpoint position
                SendSpawnpointDataClientRpc(i, botSpawner.spawnpointData[i]._spPosition);

                botSpawner.InitializeBots();

                break;
            }
        }
    }

   /* private void CheckConnectedPlayers()
    {
        if (netData.GetPlayerCount() == netData.GetCurrentMaxPlayersPerLobby())/// ToDo: Add 60s timer and fill with bots if it ends
        {
            Debug.Log("START");
            botSpawner.FillSpawnpointsWithBots(); //Spawn Bots
            StartCountdownClientRpc();//Send RPC to clients to start countdown aswell
        }
    }*/


    private void Update()
    {

        if (MovementAllowed())
        {
            if (goalController != null) goalController.AddPlayerToList(transform);
        }

        //The client owner controls his player with the input

        if (MovementAllowed()) goalController.UpdatePosition(transform);

        if (!testing)
        {
            movementV = Math.Clamp(Mathf.Round(joystick.Vertical * 10) / 10 + Mathf.Round(10) / 10, 0, 1);
            movementH = Math.Clamp(Mathf.Round(joystick.Horizontal * 10) / 10 + Mathf.Round(10) / 10, -1, 1);
        }
        else
        {
            movementV = Math.Clamp(Mathf.Round(joystick.Vertical * 10) / 10 + Mathf.Round(Input.GetAxis("Vertical") * 10) / 10, 0, 1);
            movementH = Math.Clamp(Mathf.Round(joystick.Horizontal * 10) / 10 + Mathf.Round(Input.GetAxis("Horizontal") * 10) / 10, -1, 1);
        }

            movementV = Mathf.Clamp01(movementV);
            if (autoRun) movementV = 1f;

            playerMovement = new PlayerMovement { _moveV = movementV, _moveH = movementH, _position = playerMovement._position };

        if (speedBoost > 1f)
        {
            trailBoost.GetComponent<TrailRenderer>().emitting = true;
            trailBoost.GetComponentInChildren<ParticleSystem>().Play();
            speedBoost -= 0.01f;
            speedBoost = Mathf.Clamp(speedBoost, 1f, 10f);
        }
        else
        {
            trailBoost.GetComponent<TrailRenderer>().emitting = false;
            trailBoost.GetComponentInChildren<ParticleSystem>().Stop();
        }

    }


    private void FixedUpdate()
    {
        //Applys physical movement to the player

        //If isOwner, apply movement instantly from the input

        
            if (MovementAllowed())
            {
                Vector3 fwd = transform.forward.normalized * movementV * moveSpeed * speedMultiplier * speedBoost;

                if (testing)
                {
                    Vector3 rotation = new Vector3(0, movementH * rotationSpeed, 0);

                    Vector3 currentRotation = transform.rotation.eulerAngles;
                    //Rotate Player based on Horizontal input
                    rB.MoveRotation(Quaternion.Euler(currentRotation + rotation));
                }

                rB.velocity = fwd + new Vector3(0, rB.velocity.y, 0);   
                anim.SetBool("isRunning", true);

            }

            if(movementH <0.1f && movementV < 0.1f)
        {
            anim.SetBool("isRunning", false);
        }


            if (Input.GetKeyDown(KeyCode.Space) && MovementAllowed())
            {
                LockMovement(true);

                rB.AddForce(transform.up * 8, ForceMode.VelocityChange);
            }

            //Apply extra gravity to the Player to force fast falling
            rB.AddForce(gravityDirection, ForceMode.Acceleration);

        

    }

    public void LockMovement(bool state)
    {
        isFrozen = state;
    }

    private bool MovementAllowed()
    {
        return (!isFrozen && countdownTimer.HasFinished() && !finishController.IsFinished() && faceDetected);
    }

    #region CollisionChecks
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Floor"))
        {
            //gravityDirection = -collision.GetContact(0).normal;
            LockMovement(false); //Unfreeze movement when the player touches the floor
        }
    }

    private void OnTriggerEnter(Collider collider)
    {
        SpeedBoost sB = collider.gameObject.GetComponent<SpeedBoost>();
        if (sB != null)
        {
            SetBoost(sB.speedBoost);
            sB.FadeAndRespawn();
        }

        if (goal != null && goal.CompareTag(collider.gameObject.tag))
        {
            Debug.Log("FINISHED!");
            finishController.Finish();
            if (goalController != null) goalController.RemovePlayerFromList(transform);
            if (goalController != null) goalController.PlayerFinish();
            StopCharacterOnFinish();
        }
    }

    public void StopCharacterOnFinish()
    {
        rB.isKinematic = true;
        rB.collisionDetectionMode = CollisionDetectionMode.Discrete;
        rB.velocity = Vector3.zero;
        rB.rotation = Quaternion.identity;
        rB.detectCollisions = false;
        // charModel.gameObject.SetActive(false);
      //  GetComponent<PlayerNameHolder>().textHolder.SetActive(false);
    }

    private void OnTriggerStay(Collider collider)
    {
        if (collider.gameObject.GetComponent<SpeedFloor>() != null)
        {
            SpeedFloor sF = collider.gameObject.GetComponent<SpeedFloor>();
            speedMultiplier = sF.speedMultiplier;
        }
        Effect effect = collider.GetComponent<Effect>();
        if (effect != null && !isEffectApplied && !isRecovering)
        {
            StartCoroutine(EffectTimer(effect));
        }
    }

    private void OnTriggerExit(Collider collider)
    {
        if (collider.gameObject.GetComponent<SpeedFloor>() != null) speedMultiplier = 1f;
    }

    #endregion

    public void SetBoost(float boost)
    {
        speedBoost = boost;
    }

    IEnumerator EffectTimer(Effect effect)
    {
        effect.applyEffect(gameObject);

        isEffectApplied = true;

        yield return new WaitForSeconds(effect.effectDuration);

        StartCoroutine(RecoverTimer(effect));
        isEffectApplied = false;
    }
    IEnumerator RecoverTimer(Effect effect)
    {
        effect.removeEffect(gameObject);

        Debug.Log(name + " is in invencibility time");

        isRecovering = true;

        yield return new WaitForSeconds(effect.recoverDuration);

        Debug.Log("Invencibility time has finished");
        isRecovering = false;
    }
}
