using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControl : MonoBehaviour
{
    public CharacterItem playerCharacter;

    [SerializeField] public float speedMultiplier = 1f;
    [SerializeField] public float speedBoost = 1f;
    Animator anim;
    public GameObject trailBoost;

    [SerializeField] private bool autoRun = false;


    private Countdown countdownTimer;
    private FinishController finishController;
    private GoalController goalController;
    private Rigidbody rB;
    private int playerSPIndex = -1;
    private CreateBots botSpawner;
    private GameObject goal;
    private bool isFrozen = false;
    private bool isRecovering = false;
    private bool isEffectApplied = false;
    private bool faceDetected = true;

    SaveData saveData;
    [SerializeField] List<CharacterItem> charactersList;


    //-----EVENTS-----

    private void OnEnable()
    {
        GoalController.onRaceFinishEvent += EndRace;
        FaceTrackingToMovement.onCaraDetectadaEvent += caraDetectada;
        FaceTrackingToMovement.onCaraNoDetectadaEvent += caraNoDetectada;
        anim = GetComponentInChildren<Animator>();
    }

    private void OnDisable()
    {
        GoalController.onRaceFinishEvent -= EndRace;
        FaceTrackingToMovement.onCaraDetectadaEvent -= caraDetectada;
        FaceTrackingToMovement.onCaraNoDetectadaEvent -= caraNoDetectada;
    }

    void caraDetectada()
    {
        faceDetected = true;
        if (MovementAllowed()) anim.SetBool("isRunning", true);
    }

    void caraNoDetectada()
    {
        faceDetected = false;
        anim.SetBool("isRunning", false);
    }
    //If the player looses
    void EndRace()
    {
        LockMovement(true);
        StopCharacterOnFinish();
        anim.SetBool("isRunning", false);
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
        rB = GetComponent<Rigidbody>();
        saveData = FindAnyObjectByType<SaveData>();
        ReadCharacterSaved();

        countdownTimer = FindObjectOfType<Countdown>();
        finishController = FindObjectOfType<FinishController>();
        botSpawner = FindObjectOfType<CreateBots>();
        goalController = FindObjectOfType<GoalController>();
        goal = goalController.gameObject;

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


    private void Start()
    {
        if (goalController != null) goalController.Reset();

        LockMovement(false);
        //-------------------------------------------------------------------------------------------

        if (countdownTimer == null) countdownTimer = FindObjectOfType<Countdown>();
        if (finishController == null) finishController = FindObjectOfType<FinishController>();
        if (botSpawner == null) botSpawner = FindObjectOfType<CreateBots>();

        countdownTimer.Reset();

        //When a Player spawn, the Server searches for a not occupied spawnpoint from the NetworkList for him.
        SearchNextFreeSpawnPoint();

    }

    private void SearchNextFreeSpawnPoint()
    {
        Debug.Log("se ejecuta PLAYER CONTROL");
        for (int i = 0; i <= botSpawner.spawnpointData.Count; i++)
        {
            if (!botSpawner.spawnpointData[i]._isOccupied)
            {
                //Updates the spawnpoint NetworkList with this spawnpoint as isOccupied and the playerId on it
                botSpawner.UpdateSpawnpointsList(i, botSpawner.spawnpointData[i]._spPosition, true, "Player");

                transform.position = new Vector3(botSpawner.spawnpointData[i]._spPosition.x, botSpawner.spawnpointData[i]._spPosition.y + 1, botSpawner.spawnpointData[i]._spPosition.z);
                playerSPIndex = i;

                botSpawner.InitializeBots();

                break;
            }
        }
    }

    private void Update()
    {

        if (MovementAllowed())
        {
            if (goalController != null)
            {
                goalController.AddPlayerToList(transform);
                goalController.UpdatePosition(transform);
            }
        }

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


    public void LockMovement(bool state)
    {
        isFrozen = state;
    }

    public bool MovementAllowed()
    {
        return (!isFrozen && countdownTimer.HasFinished() && !finishController.IsFinished() && faceDetected);
    }

    #region CollisionChecks
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Floor"))
        {
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
        anim.SetBool("isRunning", false);
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
