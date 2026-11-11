using UnityEngine;
using UnityEngine.SceneManagement;


public class BougePerso : MonoBehaviour
{
    [Header("Player")]
    [Tooltip("Vitesse de marche du perso")]
    [SerializeField] private float vitesseMarche = 6.0f;

    [Tooltip("Vitesse de course du perso")]
    [SerializeField] private float vitesseCourse = 15.0f;

    [Tooltip("Vitesse de rotation du perso")]
    [SerializeField] private float vitesseRotation = 1.0f;

    [Tooltip("Accélération et décélération du perso")]
    [SerializeField] private float accelerationDeceleration = 10.0f;

    [Tooltip("Vitesse de décélération du perso")]
    [SerializeField] private float deceleration = 30.0f;


    [Space(5)]
    [Header("Saut")]
    [Tooltip("Hauteur du saut du perso")]
    [SerializeField] private float hauteurSaut = 1.2f;

    [Tooltip("Valeur de gravité (défaut = -9.81f)")]
    [SerializeField] private float Gravite = -15.0f;

    [Tooltip("Temps nécessaire pour sauter à nouveau")]
    [SerializeField] private float delaiSaut = 0.1f;

    // Délai nécessaire pour sauter à nouveau
    private float sautTimeout;

    // Booléen pour savoir le début de l'animation de saut (avec et sans arme)
    private bool debutAnim = false;


    [Space(5)]
    [Header("Perso au sol")]
    [Tooltip("Détermine si le perso touche le sol ou non")]
    [SerializeField] private bool toucheSol = true;

    [Tooltip("Distance du perso du sol")]
    [SerializeField] private float distanceDuSol = -0.14f;

    [Tooltip("Radius du Character controller pour savoir si le perso touche au sol ou non")]
    [SerializeField] private float GroundedRadius = 0.5f;

    [Tooltip("Layer du sol")]
    [SerializeField] private LayerMask layerSol;


    [Space(5)]
    [Header("Cinemachine")]
    [Tooltip("GameObject que la Cinemachine va suivre")]
    [SerializeField] private GameObject cibleCinemachine;

    [Tooltip("Nb de degrés de la caméra pour se déplacer vers le haut")]
    [SerializeField] private float maxMouvCamHaut = 90.0f;

    [Tooltip("Nb de degrés de la caméra pour se déplacer vers le bas")]
    [SerializeField] private float maxMouvCamBas = -90.0f;

    [Tooltip("Largeur de la zone de détection aux bords de l'écran")]
    [SerializeField] private float zoneBordEcran = 20f;

    [Tooltip("Vitesse de rotation de la caméra lorsque la souris atteint un bord")]
    [SerializeField] private float vitesseRotationBord = 100f;

    // Cinémachines pour la 1re et 3e personne (cameraTP) ainsi que la caméra principale (cinemachineTP)
    [SerializeField] private GameObject cinemachineFP;
    [SerializeField] private GameObject cinemachineTP;
    [SerializeField] private GameObject cameraTP;

    // Valeurs permettant d'ajuster la vélocité et le temps de rotation de la caméra 3e personne
    [SerializeField] private float ajusteVelocityRotation = 0.1f;
    [SerializeField] private float ajusteTempsRotation = 0.1f;

    // Seuil de la caméra
    private const float seuil = 0.01f;

    // Rotation X de la Cinemachine
    private float niveauCinemachine;


    [Space(5)]
    // L'Animator du perso ainsi que son arme et la mire
    [Header("Arme")]
    [SerializeField] private Animator animator;
    [SerializeField] private GameObject arme;
    [SerializeField] GameObject mire;

    // Délai pour ranger l'arme et booléen pour savoir si l'arme est équipée ou non
    private float cooldown;
    private bool equipe = false;


    [Space(5)]
    [Header("Panneaux")]
    // Panneau quand le perso tombe dans la crevasse
    [SerializeField] private GameObject panneauChute;

    // Panneau pour indiquer d'aller voir la maison abandonnée
    [SerializeField] private GameObject panneauMaison;

    // Panneau pour dire que la mission 1 n'est pas encore finie
    [SerializeField] private GameObject panneauPasFini1;

    // Panneau quand on voit Roger
    [SerializeField] private GameObject panneauRoger;


    [Space(5)]
    [Header("Vitesse")]
    // Vitesse verticale
    private float vitesseVerticale;

    // Valeur du changement de vitesse du perso
    private float changementVitesse;

    // Vélocité horizontale actuelle du perso
    private Vector3 vitesseHorizontaleActuelle;


    [Space(5)]
    [Header("Sons")]
    // GameObject avec le son des pièces
    [SerializeField] private GameObject pieces;

    // GameObject avec le son de la victoire
    [SerializeField] private GameObject victoire;

    // GameObjects avec les effets sonores du perso
    [SerializeField] private GameObject ouch;
    [SerializeField] private GameObject tire;
    [SerializeField] private GameObject recharge;

    // AudioSource du tir, de la recharge, de la caméra principale et de la douleur
    private AudioSource audioTir;
    private AudioSource audioRecharge;
    private AudioSource audioPrincipal;
    private AudioSource audioOuch;

    // Indique si le perso est en train de recharger
    private bool rechargementEnCours = false;


    [Space(5)]
    [Header("Autres")]
    // Character controller du perso
    private CharacterController controller;

    // Script "StarterAssetsInputs" du perso
    private StarterAssetsInputs lesInputs;

    // SO "InfosPerso" du perso
    [SerializeField] private InfosPerso infosPerso;

    // Murs invisibles pour empêcher le perso d'entrer dans le village s'il n'a pas fini la mission 1
    [SerializeField] private GameObject mursInvisibles;

    // Murs invisibles pour empêcher le perso d'aller trop loin s'il n'a pas lu la mission 1
    [SerializeField] private GameObject murMaison1;
    [SerializeField] private GameObject murMaison2;


    // --------------------------------------------------------------------------------------------------------------------------------------------------------------------------

    // Est appelée au lancement de la scène
    void Start()
    {
        // Réinitialise les valeurs du nb de pièces et des ennemis tués au lancement de la scène
        infosPerso.nbPieces = 0;
        infosPerso.nbEnnemis = 0;

        // Permet de savoir les valeurs du nb de pièces et d'ennemis tués par le perso dans la console
        Debug.Log(infosPerso.nbEnnemis + " ennemis");
        Debug.Log(infosPerso.nbPieces + " pièces");

        // Trouve les composantes du Character controller et le script "StarterAssetsInputs" du perso
        controller = GetComponent<CharacterController>();
        lesInputs = GetComponent<StarterAssetsInputs>();

        // Récupère les AudioSources du tir, de la recharge et de la douleur
        audioTir = tire.GetComponent<AudioSource>();
        audioRecharge = recharge.GetComponent<AudioSource>();
        audioOuch = ouch.GetComponent<AudioSource>();

        // Récupère l'AudioSource de la caméra principale
        GameObject cameraPrincipale = GameObject.Find("Main Camera");
        audioPrincipal = cameraPrincipale.GetComponent<AudioSource>();

        // Réinitialise les délais de saut
        sautTimeout = delaiSaut;
    }


    // Est appelée à chaque frame
    void Update()
    {
        // Si le booléen "equipe" est à vrai (quand on est en 1re personne)
        if (equipe)
            Bouge();

        // Sinon (quand on est 3e personne)
        else
            DirectionTP();

        // Exécute les méthodes suivantes à chaque frame
        Attaque();
        RetourCameraTP();
        VerifierSol();
        SautEtGravite();
        DebutAnim();
        Chute();
        Victoire();
    }


    // Est appelée après que les fonctions dans Update soient exécutées
    void LateUpdate()
    {
        // Si le booléen "equipe" est à vrai (quand on est en 1re personne)
        if (equipe)
            RotationCamera();
    }


    // -----------------------------------------------------------------------CINEMACHINES---------------------------------------------------------------------------------------
    // Accélération, décélération et déplacement du perso
    void Deplacer(Vector3 direction, float vitesseVoulue)
    {
        // Détermine la vitesse horizontale souhaitée
        Vector3 vitesseVoulueHorizontale = direction * vitesseVoulue;

        // Détermine si le perso change de direction
        bool changeDirection = vitesseVoulueHorizontale.sqrMagnitude > 0.01f && vitesseHorizontaleActuelle.sqrMagnitude > 0.01f &&
            Vector3.Dot(vitesseHorizontaleActuelle.normalized, vitesseVoulueHorizontale.normalized) < 0.95f;

        // Détermine la vitesse de changement
        float vitesseChangement;

        if (vitesseVoulueHorizontale == Vector3.zero || changeDirection)
            vitesseChangement = deceleration;

        else
            vitesseChangement = accelerationDeceleration;

        // Accélère, ralentit ou change la direction du perso
        vitesseHorizontaleActuelle = Vector3.MoveTowards(vitesseHorizontaleActuelle, vitesseVoulueHorizontale, vitesseChangement * Time.deltaTime);

        // Calcule la vitesse actuelle du perso
        float vitesse = vitesseHorizontaleActuelle.magnitude;

        // Rend plus fluide le changement de vitesse de l'animation
        changementVitesse = Mathf.Lerp(changementVitesse, vitesse, Time.deltaTime * accelerationDeceleration);

        // Si le changement de vitesse est plus petit que 0.01, met sa valeur à 0
        if (changementVitesse < 0.01f)
            changementVitesse = 0;

        // Donne la valeur du changement de vitesse à l'Animator
        animator.SetFloat("Vitesse", changementVitesse);

        // Déplace le perso avec son Character Controller
        controller.Move(vitesseHorizontaleActuelle * Time.deltaTime + Vector3.up * (vitesseVerticale * Time.deltaTime));
    }


    // Permet à la caméra 3e personne de bouger
    void DirectionTP()
    {
        // Valeurs des mouvements horizontaux et verticaux de la caméra
        float gaucheDroite = lesInputs.bouge.x;
        float avantArriere = lesInputs.bouge.y;

        // Détermine si le perso marche ou cours
        float vitesseVoulue = lesInputs.cours ? vitesseCourse : vitesseMarche;

        // Si y'a pas de mouvement, mettre la vitesse du perso à 0
        if (lesInputs.bouge == Vector2.zero)
            vitesseVoulue = 0.0f;

        // Normalise les valeurs de mouvements
        Vector3 direction = new Vector3(gaucheDroite, 0.0f, avantArriere).normalized;

        // Si le perso se déplace
        if (direction != Vector3.zero)
        {
            // Angle voulu pour faire rotater le perso
            float angleVoulu = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + cinemachineTP.transform.eulerAngles.y;

            // Angle de rotation du perso
            float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, angleVoulu, ref ajusteVelocityRotation, ajusteTempsRotation);

            // Rotate le perso
            transform.rotation = Quaternion.Euler(0f, angle, 0f);

            // Direction du mouvement selon l'angle de rotation
            direction = (Quaternion.Euler(0f, angleVoulu, 0f) * Vector3.forward).normalized;
        }

        // Valeurs de déplacement
        Deplacer(direction, vitesseVoulue);
    }


    // Rotation de la caméra quand le perso est en mode 1re personne
    void RotationCamera()
    {
        // Détecte si il y a un mouvement de la caméra
        if (lesInputs.regarder.sqrMagnitude >= seuil)
        {
            // Rotation verticale de la caméra
            niveauCinemachine += lesInputs.regarder.y * vitesseRotation;

            // Rotation horizontale du perso
            float rotationHorizontale = lesInputs.regarder.x * vitesseRotation;

            // Met à jour les valeurs des angles possibles que la Cinemachine peut faire
            niveauCinemachine = GrandeurAngle(niveauCinemachine, maxMouvCamBas, maxMouvCamHaut);

            // Met à jour les valeurs de la Cinemachine
            cibleCinemachine.transform.localRotation = Quaternion.Euler(niveauCinemachine, 0.0f, 0.0f);

            // Permet au perso d'effectuer une rotation à gauche et à droite
            transform.Rotate(Vector3.up * rotationHorizontale);
        }

        // Rotation supplémentaire lorsque la souris atteint les bords de l'écran
        if (Cursor.lockState == CursorLockMode.Confined)
        {
            // Bord gauche
            if (Input.mousePosition.x <= zoneBordEcran)
                transform.Rotate(Vector3.up * (-vitesseRotationBord * Time.deltaTime));

            // Bord droit
            else if (Input.mousePosition.x >= Screen.width - zoneBordEcran)
                transform.Rotate(Vector3.up * (vitesseRotationBord * Time.deltaTime));
        }
    }


    // Détermine les angles possibles que la Cinemachine peut faire
    static float GrandeurAngle(float angle, float angleMin, float angleMax)
    {
        return Mathf.Clamp(angle, angleMin, angleMax);
    }


    // Permet au perso de bouger quand il est en mode 1re personne
    void Bouge()
    {
        // Détermine si le perso marche ou cours
        float vitesseVoulue = lesInputs.cours ? vitesseCourse : vitesseMarche;

        // Si y'a pas de mouvement, mettre la vitesse du perso à 0
        if (lesInputs.bouge == Vector2.zero)
            vitesseVoulue = 0.0f;

        // Direction du déplacement selon l'orientation du perso
        Vector3 direction = transform.right * lesInputs.bouge.x + transform.forward * lesInputs.bouge.y;

        // Normalise la direction pour conserver une vitesse constante en diagonale
        direction = direction.normalized;

        // Valeurs de déplacement
        Deplacer(direction, vitesseVoulue);
    }


    // --------------------------------------------------------------------ANIMS-------------------------------------------------------------------------------------------------

    // Début de l'animation de saut du perso
    void DebutAnim()
    {
        // Si on appuie sur la touche "espace" pour faire sauter le perso ET que celui-ci touche au sol (valeur à vrai) ET que le booléen "debutAnim" est à faux
        if (lesInputs.saute && toucheSol && debutAnim == false)
        {
            // Rend le booléen "debutAnim" à vrai
            debutAnim = true;

            // Enclenche le trigger "Saut" de l'Animator du perso
            animator.SetTrigger("Saut");
        }
    }


    // Fin de l'animation de saut du perso
    public void FinAnim(float valeur)
    {
        // La valeur de la hauteur du saut du perso est la valeur (valeur) donnée dans le script "GestionAnims"
        hauteurSaut = valeur;

        // Rend le booléen "debutAnim" à faux (pour que la méthode "DebutAnim" puisse être exécutée à nouveau)
        debutAnim = false;
    }


    // ------------------------------------------------------------------------ATTAQUE-------------------------------------------------------------------------------------------

    // Attaque avec l'arme du perso
    void Attaque()
    {
        // Si le perso ne tire pas, ne rien faire
        if (!lesInputs.tire)
            return;

        // Réinitialise l'input pour éviter un tir automatique après la recharge
        lesInputs.tire = false;

        // Empêche de tirer pendant la recharge
        if (rechargementEnCours)
            return;

        // Commence la séquence de tir et de recharge
        rechargementEnCours = true;

        // Active le mode 1re personne
        equipe = true;

        // Active la caméra 1re personne
        cinemachineFP.SetActive(true);

        // Désactive la caméra 3e personne
        cameraTP.SetActive(false);

        // Active la mire
        mire.SetActive(true);

        // Modifie l'animation du perso
        animator.SetBool("Arme", true);

        // Rend l'arme visible
        arme.SetActive(true);

        // Verrouille le curseur
        Cursor.lockState = CursorLockMode.Locked;

        // Active l'AudioSource du tir
        audioTir.enabled = true;

        // Programme la fin du tir
        Invoke(nameof(FiniTir), audioTir.clip.length);

        // Délai pour retourner à la caméra 3e personne
        cooldown = 5f;

        // Position et direction de la mire
        Vector3 positionDepart = mire.transform.position;
        Vector3 directionTir = mire.transform.forward;

        // Récupère tous les colliders touchés par le tir
        RaycastHit[] impacts = Physics.RaycastAll(positionDepart, directionTir, 20f, Physics.DefaultRaycastLayers, QueryTriggerInteraction.Collide);

        // Trie les impacts du plus proche au plus éloigné
        System.Array.Sort(impacts, (a, b) => a.distance.CompareTo(b.distance));

        // Parcourt les impacts dans l'ordre de distance
        foreach (RaycastHit impact in impacts)
        {
            // Collisions
            GameObject objetCollisions = impact.collider.gameObject;

            // Ignore la zone de contact de l'ennemi
            if (objetCollisions.CompareTag("Ennemi"))
                continue;

            // Si le tir touche le collider vulnérable d'un ennemi
            if (objetCollisions.CompareTag("CollisionsEnnemis"))
            {
                // Détruit l'ennemi
                Destroy(objetCollisions);

                // Augmente de 1 le nb d'ennemis tués du perso
                infosPerso.nbEnnemis++;

                // Permet de savoir le nb d'ennemis tués par le perso dans la console
                Debug.Log(infosPerso.nbEnnemis + " ennemis");

                // Arrête la boucle
                break;
            }

            // Si un autre objet est touché, le tir est bloqué
            break;
        }
    }

    // Gestion du retour à la caméra 3e personne
    void RetourCameraTP()
    {
        // Si le perso est en 1re personne
        if (equipe)
        {
            // Diminue le délai de retour
            cooldown -= Time.deltaTime;

            // Si le délai est terminé
            if (cooldown <= 0f)
            {
                // Désactive le mode 1re personne
                equipe = false;

                // Désactive la caméra 1re personne
                cinemachineFP.SetActive(false);

                // Réactive la caméra 3e personne
                cameraTP.SetActive(true);

                // Désactive la mire
                mire.SetActive(false);

                // Déséquipe l'arme
                arme.SetActive(false);

                // Modifie l'animation du perso
                animator.SetBool("Arme", false);

                // Déverrouille le curseur
                Cursor.lockState = CursorLockMode.Confined;
            }
        }
    }


    // Termine le son du tir
    void FiniTir()
    {
        // Arrête le son du tir
        audioTir.enabled = false;

        // Commence la recharge
        Recharge();
    }


    // Recharger l'arme du perso
    void Recharge()
    {
        // Active l'AudioSource de recharge
        audioRecharge.enabled = true;

        // Attend la durée complète du son
        Invoke(nameof(DesactiveRecharge), audioRecharge.clip.length);
    }


    // Termine la recharge
    void DesactiveRecharge()
    {
        // Arrête le son de recharge
        audioRecharge.enabled = false;

        // Autorise un nouveau tir
        rechargementEnCours = false;
    }


    // -------------------------------------------------------------------CHUTE + TÉLÉPORTATION----------------------------------------------------------------------------------

    // Si le perso tombe dans la crevasse
    void Chute()
    {
        if (transform.position.y < 5 && transform.position.z > 30 && transform.position.z < 45)
        {
            // Active l'Audio source du perso
            GetComponent<AudioSource>().enabled = true;

            // Invoque la méthode "RetourEnHaut" dans 1 seconde
            Invoke(nameof(RetourEnHaut), 1f);
        }

        // Sinon
        else
        {
            // Désactive l'Audio source du perso
            GetComponent<AudioSource>().enabled = false;
        }
    }


    // Téléporte le perso à une position après qu'il soit tombé dans la crevasse
    void RetourEnHaut()
    {
        transform.position = new Vector3(15, 5.078046f, -90);

        // Active le panneau de chute
        panneauChute.SetActive(true);

        // Invoque la méthode "EnleverPanneaux" dans 3 secondes
        Invoke(nameof(EnleverPanneaux), 3f);
    }


    // -------------------------------------------------------------------DÉSACTIVATION------------------------------------------------------------------------------------------

    // Permet de désactiver les panneaux actifs après qu'ils aient été actifs
    void EnleverPanneaux()
    {
        panneauMaison.SetActive(false);
        panneauChute.SetActive(false);
        panneauRoger.SetActive(false);
        panneauPasFini1.SetActive(false);
    }


    // Désactive l'Audio source de la douleur du perso (quand il est touché par les ennemis)
    void DesactiveOuch()
    {
        ouch.GetComponent<AudioSource>().enabled = false;
    }


    // Désactive l'Audio source des pièces
    void DesactivePieces()
    {
        pieces.GetComponent<AudioSource>().enabled = false;
    }


    // ------------------------------------------------------------------------COLLISIONS----------------------------------------------------------------------------------------

    // Quand le perso entre en collision avec qq
    void OnTriggerEnter(Collider collision)
    {
        // Pièces
        if (collision.CompareTag("Piece"))
        {
            // Active l'Audio source des pièces
            pieces.GetComponent<AudioSource>().enabled = true;

            // Détruit la pièce
            Destroy(collision.gameObject);

            // Augmnte le nb de pièces du perso de 1
            infosPerso.nbPieces++;

            // Permet de savoir le nb de pièces par le perso dans la console
            Debug.Log(infosPerso.nbPieces + " pièces");

            // Invoque la méthode "DesactivePieces" dans 1 seconde
            Invoke(nameof(DesactivePieces), 1f);
        }


        // Collision des ennemis (entre le cercle de détection de leur NavMesh et leur collider pour pas que le perso passe à travers eux)
        if (collision.CompareTag("CollisionsEnnemis"))
        {
            // Active l'Audio source de la douleur du perso
            ouch.GetComponent<AudioSource>().enabled = true;

            // Active le panneau de chute
            panneauChute.SetActive(true);

            // Invoque la méthode "EnleverPanneaux" dans 3 secondes
            Invoke(nameof(EnleverPanneaux), 3f);

            // Invoque les méthodes suivantes après un certain délai
            Invoke(nameof(DesactiveOuch), 2f);
            Invoke(nameof(EnleverPanneaux), 2f);
        }


        // Collider si le perso a pas fini la mission 1 (- de 10 ennemis)
        if (collision.CompareTag("PasFini") && infosPerso.nbEnnemis < 10)
        {
            // Active le panneau PasFini1
            panneauPasFini1.SetActive(true);

            // Active le collider du mur invisible pour que le perso ne puisse pas entrer dans le village
            mursInvisibles.SetActive(true);

            // Invoque la méthode "EnleverPanneaux" dans 2 secondes
            Invoke(nameof(EnleverPanneaux), 2f);
        }


        // Collider si le perso a fini la mission 1 (10 ennemis)
        if (collision.CompareTag("PasFini") && infosPerso.nbEnnemis == 10)
        {
            // Dit à la caméra principale d'arrêter sa musique
            audioPrincipal.enabled = false;

            // Active l'Audio source du village
            GameObject.Find("Village").GetComponent<AudioSource>().enabled = true;
        }


        // Collider des murs invisbles si le perso n'a pas visité la maison abandonnée
        if (collision.CompareTag("Maison"))
        {
            // Rend actif les murs invisibles
            murMaison1.GetComponent<BoxCollider>().enabled = true;
            murMaison2.GetComponent<BoxCollider>().enabled = true;

            // Rend le panneau indiquant d'aller voir la maison abandonnée actif
            panneauMaison.SetActive(true);

            // Invoque la méthode "EnleverPanneaux" dans 2 secondes
            Invoke(nameof(EnleverPanneaux), 3f);
        }


        // Collider du pauvre Roger (squelette)
        if (collision.CompareTag("Roger"))
        {
            // Active le panneau de Roger
            panneauRoger.SetActive(true);

            // Invoque la méthode "EnleverPanneaux" dans 2 secondes
            Invoke(nameof(EnleverPanneaux), 2f);
        }
    }


    // ------------------------------------------------------------------------VICTOIRE------------------------------------------------------------------------------------------

    // Quand le perso a tués tous les ennemis ET a ramassé toutes les pièces
    void Victoire()
    {
        if (infosPerso.nbPieces == 5 && infosPerso.nbEnnemis == 10)
        {
            // Active l'Audio source de la victoire
            victoire.GetComponent<AudioSource>().enabled = true;

            // Invoque la méthode "SceneSuivante" dans 2 secondes
            Invoke(nameof(SceneSuivante), 2f);
        }
    }


    // Aller à la scène suivante
    void SceneSuivante()
    {
        // Dit à la caméra principale d'arrêter sa musique
        audioPrincipal.enabled = false;

        // Charge la scène "Base"
        SceneManager.LoadScene("Base");
    }


    // ------------------------------------------------------------------------SAUT----------------------------------------------------------------------------------------------

    // Détermine si le perso touche au sol ou non (création d'une sphère sous le perso)
    void VerifierSol()
    {
        // Position de la sphère sous le perso
        Vector3 spherePosition = transform.position + Vector3.up * distanceDuSol;

        // Vérifie si la sphère touche le sol
        toucheSol = Physics.CheckSphere(spherePosition, GroundedRadius, layerSol, QueryTriggerInteraction.Ignore);
    }


    // Permet au perso de sauter
    void SautEtGravite()
    {
        // Vitesse finale
        float vitesseFinale = -53.0f;

        // Applique la gravité sans dépasser la vitesse maximale de chute
        if (vitesseVerticale > vitesseFinale)
            vitesseVerticale = Mathf.Max(vitesseVerticale + Gravite * Time.deltaTime, vitesseFinale);

        // Si le perso touche au sol
        if (toucheSol)
        {
            // Empêche la vélocité de baisser à l'infini quand le perso touche au sol
            if (vitesseVerticale < 0.0f)
                vitesseVerticale = -2f;

            // Si on appuie sur la barre d'espace ET que le délai pour sauter à nouveau est de 0 : Saute
            if (lesInputs.saute && sautTimeout <= 0.0f)
                vitesseVerticale = Mathf.Sqrt(hauteurSaut * -2f * Gravite);

            // Enclenche le délai pour sauter à nouveau
            if (sautTimeout >= 0.0f)
                sautTimeout -= Time.deltaTime;
        }

        // Sinon
        else
        {
            // Réinitialise le délai pour sauter à nouveau
            sautTimeout = delaiSaut;

            // Si le perso ne touche pas au sol : ne Saute PAS
            lesInputs.saute = false;
        }
    }
}