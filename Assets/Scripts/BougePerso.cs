using UnityEngine;
using UnityEngine.SceneManagement;


public class BougePerso : MonoBehaviour
{
    [Header("Player")]
    [Tooltip("Vitesse de marche du perso")]
    [SerializeField] private float vitesseMarche = 2.0f;

    [Tooltip("Vitesse de course du perso")]
    [SerializeField] private float vitesseCourse = 6.0f;

    [Tooltip("Vitesse de rotation du perso")]
    [SerializeField] private float vitesseRotation = 1.0f;

    [Tooltip("Acc�l�ration et d�c�l�ration du perso")]
    [SerializeField] private float accelerationDeceleration = 10.0f;


    [Space(5)]
    [Header("Saut")]
    [Tooltip("Hauteur du saut du perso")]
    [SerializeField] private float hauteurSaut = 1.2f;

    [Tooltip("Valeur de gravit� (d�faut = -9.81f)")]
    [SerializeField] private float Gravite = -15.0f;

    [Tooltip("Temps n�cessaire pour sauter � nouveau")]
    [SerializeField] private float delaiSaut = 0.1f;

    [Tooltip("Temps n�cessaire pour �tre consid�r� dans l'�tat de chute")]
    [SerializeField] private float delaiChute = 0.15f;

    //D�lai n�cessaire pour sauter � nouveau
    private float sautTimeout;

    //D�lai n�cessaire pour �tre consid�r� dans l'�tat de chute
    private float chuteTimeout;

    //Bool�en pour savoir le d�but de l'animation de saut (avec et sans arme)
    private bool debutAnim = false;


    [Space(5)]
    [Header("Perso au sol")]
    [Tooltip("D�termine si le perso touche le sol ou non")]
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

    [Tooltip("Nb de degr�s de la cam�ra pour se d�placer vers le haut")]
    [SerializeField] private float maxMouvCamHaut = 90.0f;

    [Tooltip("Nb de degr�s de la cam�ra pour se d�placer vers le bas")]
    [SerializeField] private float maxMouvCamBas = -90.0f;

    //Cin�machine pour la 1re et 3e personne (cameraTP) ainsi que la cam�ra principale (cinemachineTP)
    [SerializeField] private GameObject cinemachineFP;
    [SerializeField] private GameObject cinemachineTP;
    [SerializeField] private GameObject cameraTP;

    //Valeur permettant d'ajuster la v�locit� et le temps de rotation de la cam�ra 3e personne
    [SerializeField] private float ajusteVelocityRotation = 0.1f;
    [SerializeField] private float ajusteTempsRotation = 0.1f;

    //Seuil de la cam�ra
    private const float seuil = 0.01f;

    //Rotation X de la Cinemachine
    private float niveauCinemachine;


    [Space(5)]
    //L'Animator du perso ainsi que son arme et la mire
    [Header("Arme")]
    [SerializeField] private Animator animator;
    [SerializeField] private GameObject arme;
    [SerializeField] GameObject mire;

    //D�lai pour ranger l'arme et bool�en pour savoir si l'arme est �quip�e ou non
    private float cooldown;
    private bool equipe = false;


    [Space(5)]
    [Header("Panneaux")]
    //Panneau quand le perso tombe dans la crevasse
    [SerializeField] private GameObject panneauChute;

    //Panneau pour indiquer d'aller voir la maison abandonn�e
    [SerializeField] private GameObject panneauMaison;

    //Panneau pour dire que la mission 1 n'est pas encore finie
    [SerializeField] private GameObject panneauPasFini1;

    //Panneau quand on voit Roger
    [SerializeField] private GameObject panneauRoger;


    [Space(5)]
    [Header("Vitesse")]
    //Vitesse
    private float vitesse;

    //Vitesse (v�locit�) de rotation
    private float velociteRotation = 53.0f;

    //Vitesse verticale
    private float vitesseVerticale;

    //Vitesse finale
    private float vitesseFinale = 53.0f;

    //Valeur du changement de vitesse du perso
    private float changementVitesse;


    [Space(5)]
    [Header("Sons")]
    //GameObject avec le son des pi�ces
    [SerializeField] private GameObject pieces;

    //GameObject avec le son de la victoire
    [SerializeField] private GameObject victoire;

    //GameObject avec le son de la douleur du perso
    [SerializeField] private GameObject ouch;

    //GameObject avec le son du perso qui tire avec son arme
    [SerializeField] private GameObject tire;

    //GameObject avec le son du perso qui recharge son arme
    [SerializeField] private GameObject recharge;


    [Space(5)]
    [Header("Autres")]
    //Character controller du perso
    private CharacterController controller;

    //Script "StarterAssetsInputs" du perso
    private StarterAssetsInputs lesInputs;

    //SO "InfosPerso" du perso
    [SerializeField] private InfosPerso infosPerso;

    //Murs invisibles pour emp�cher le perso d'entrer dans le village s'il n'a pas fini la mission 1
    [SerializeField] private GameObject mursInvisibles;

    //Murs invisibles pour emp�cher le perso d'aller trop loin s'il n'a pas lu la mission 1
    [SerializeField] private GameObject murMaison1;
    [SerializeField] private GameObject murMaison2;


    //--------------------------------------------------------------------------------------------------------------------------------------------------------------------------

    //Est appel�e au lancement de la sc�ne
    void Start()
    {
        //R�initialise les valeurs du nb de pi�ces et des ennemis tu�s au lancement de la sc�ne
        infosPerso.nbPieces = 0;
        infosPerso.nbEnnemis = 0;

        //Permet de savoir les valeurs du nb de pi�ces et d'ennemis tu�s par le perso dans la console
        Debug.Log(infosPerso.nbEnnemis + " ennemis");
        Debug.Log(infosPerso.nbPieces + " pi�ces");

        //Trouve les composantes du Character controller et le script "StarterAssetsInputs" du perso
        controller = GetComponent<CharacterController>();
        lesInputs = GetComponent<StarterAssetsInputs>();

        //R�initialise les d�lais de saut et de chute au lancement de la sc�ne
        sautTimeout = delaiSaut;
        chuteTimeout = delaiChute;
    }


    //Est appel�e � chaque frame
    void Update()
    {
        //Si le bool�en "equipe" est � vrai (quand on est en 1re personne)
        if (equipe)
        {
            //Ex�cute la m�thode suivante
            Bouge();
        }

        //Sinon (quand on est 3e personne)
        else
        {
            //Ex�cute la m�thode suivante
            DirectionTP();
        }

        //Ex�cute les m�thodes suivantes � chaque frame
        Attaque();
        SautEtGravite();
        VerifierSol();
        DebutAnim();
        Chute();
        Victoire();
    }


    //Est appel�e apr�s que les fonctions dans Update soient ex�cut�es
    void LateUpdate()
    {
        //Si le bool�en "equipe" est � vrai (quand on est en 1re personne)
        if (equipe)
        {
            //Ex�cute la m�thode suivante
            RotationCamera();
        }
    }


    //-----------------------------------------------------------------------CINEMACHINES---------------------------------------------------------------------------------------

    //Permet � la cam�ra 3e personne de bouger
    void DirectionTP()
    {
        //Valeurs des mouvements horizontaux et verticaux de la cam�ra
        float gaucheDroite = lesInputs.bouge.x;
        float avantArriere = lesInputs.bouge.y;

        //D�termine si le perso marche ou cours (si marche = vitesse de marche, si course = vitesse de course)
        float vitesseVoulue = lesInputs.cours ? vitesseCourse : vitesseMarche;

        //Si y'a pas de mouvement, mettre la vitesse du perso � 0
        if (lesInputs.bouge == Vector2.zero) vitesseVoulue = 0.0f;


        //Vitesse horizontale de la cam�ra et marge de vitesse (droit � l'erreur � 0.1)
        float vitessseHorizontale = new Vector3(controller.velocity.x, 0.0f, controller.velocity.z).magnitude;
        float ecartVitesse = 0.1f;

        //Si la vitesse horizontale est plus petite que la vitesse voulue - l'�cart de vitesse OU l'inverse
        if (vitessseHorizontale < vitesseVoulue - ecartVitesse || vitessseHorizontale > vitesseVoulue + ecartVitesse)
        {
            vitesse = Mathf.Lerp(vitessseHorizontale, vitesseVoulue, Time.deltaTime * accelerationDeceleration);
            vitesse = Mathf.Round(vitesse * 1000f) / 1000f;
        }

        //Sinon, la vitesse est �gale � la vitesse voulue
        else
        {
            vitesse = vitesseVoulue;
        }


        //Rend plus fluide le chngement de vitesse entre "Idle", "Marche" et "Course"
        changementVitesse = Mathf.Lerp(changementVitesse, vitesseVoulue, Time.deltaTime * accelerationDeceleration);

        //Si le changement de vitesse est plus petit que 0.1, met sa valeur � 0 (pour pas que la valeur soit n�gative)
        if (changementVitesse < 0.01f) changementVitesse = 0;

        //Donne la valeur du changement de vitesse � celle du float "Vitesse" de l'Animator du perso
        animator.SetFloat("Vitesse", changementVitesse);


        //Normalise les valeurs de mouvements
        Vector3 direction = new Vector3(gaucheDroite, vitesseVerticale, avantArriere).normalized;


        //Si la magnitude des valeurs de mouvements normalis�es sont plus petites ou �gales � 0.1 ET que les valeurs de d�placements du perso ne sont pas �gales � 0
        if (direction.magnitude >= 0.1f && lesInputs.bouge != Vector2.zero)
        {
            //Angle voulu pour faire rotater le perso ((transform.eulerAngles.y), car pivot perso = sur axe des Y)
            float angleVoulu = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + cinemachineTP.transform.eulerAngles.y;

            //Angle de rotation du perso
            float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, angleVoulu, ref ajusteVelocityRotation, ajusteTempsRotation);

            //Rotate le perso
            transform.rotation = Quaternion.Euler(0f, angle, 0f);

            //Direction du mouvement (normalis�e) du perso selon l'angle de rotation
            Vector3 moveDir = (Quaternion.Euler(0f, angleVoulu, 0f) * Vector3.forward).normalized;

            //Bouge le perso selon les valeurs de mouvements normalis�es (avec la rotation tout en prenant en compte s'il saute) avec son Character controller
            controller.Move(moveDir.normalized * (vitesseVoulue * Time.deltaTime) + new Vector3(0.0f, vitesseVerticale, 0.0f) * Time.deltaTime);
        }

        //Sinon, bouge le perso selon ses valeurs de mouvements normalis�es (tout en prenant en compte s'il saute) avec son Character controller
        else
        {
            controller.Move(direction.normalized * (vitesse * Time.deltaTime) + new Vector3(0.0f, vitesseVerticale, 0.0f) * Time.deltaTime);
        }
    }


    //Rotation de la cam�ra quand le perso est en mode 1re personne
    void RotationCamera()
    {
        //D�tecte si il y a un mouvement de la cam�ra
        if (lesInputs.regarder.sqrMagnitude >= seuil)
        {
            //Emp�che les mouvements de la souris de multiplier la vitesse de la cam�ra (axes X et Y)
            float deltaTimeMultiplier = 1;
            niveauCinemachine += lesInputs.regarder.y * vitesseRotation * deltaTimeMultiplier;
            velociteRotation = lesInputs.regarder.x * vitesseRotation * deltaTimeMultiplier;

            //Met � jour les valeurs des angles possibles que la Cinemachine peut faire
            niveauCinemachine = GrandeurAngle(niveauCinemachine, maxMouvCamBas, maxMouvCamHaut);

            //Met � jour les valeurs de la Cinemachine
            cibleCinemachine.transform.localRotation = Quaternion.Euler(niveauCinemachine, 0.0f, 0.0f);

            //Permet au perso d'effectuer une rotation � gauche et � droite
            transform.Rotate(Vector3.up * velociteRotation);
        }
    }


    //D�termine les angles possibles que la Cinemachine peut faire
    static float GrandeurAngle(float angleGauche, float minGauche, float maxGauche)
    {
        //Emp�che la Cinemachine d'avoir une rotation trop grande ou trop faible (- que -360 degr�s ou + que +360 degr�s)
        if (angleGauche < -360f) angleGauche += 360f;
        if (angleGauche > 360f) angleGauche -= 360f;
        return Mathf.Clamp(angleGauche, minGauche, maxGauche);
    }


    //Permet au perso de bouger quand il est en mode 1re personne
    void Bouge()
    {
        //D�termine si le perso marche ou cours (si marche = vitesse de marche, si course = vitesse de course)
        float vitesseVoulue = lesInputs.cours ? vitesseCourse : vitesseMarche;

        //Si y'a pas de mouvement, mettre la vitesse du perso � 0
        if (lesInputs.bouge == Vector2.zero) vitesseVoulue = 0.0f;


        //Vitesse horizontale du perso
        float vitesseHorizontale = new Vector3(controller.velocity.x, 0.0f, controller.velocity.z).magnitude;

        //D�calage de vitesse (fluidit�) et magnitude des vecteurs de d�placement
        float ecartVitesse = 0.1f;
        float inputMagnitude = 1f;

        //Si la vitesse horizontale est plus petite que la vitesse voulue - l'�cart de vitesse OU l'inverse
        if (vitesseHorizontale < vitesseVoulue - ecartVitesse || vitesseHorizontale > vitesseVoulue + ecartVitesse)
        {
            vitesse = Mathf.Lerp(vitesseHorizontale, vitesseVoulue * inputMagnitude, Time.deltaTime * accelerationDeceleration);
            vitesse = Mathf.Round(vitesse * 1000f) / 1000f;
        }

        //Sinon, la vitesse est �gale � la vitesse voulue
        else
        {
            vitesse = vitesseVoulue;
        }

        //Normalise les valeurs de mouvements
        Vector3 inputDirection = new Vector3(lesInputs.bouge.x, 0.0f, lesInputs.bouge.y).normalized;

        //Si les valeurs de d�placements du perso ne sont pas �gales � 0
        if (lesInputs.bouge != Vector2.zero)
        {
            //D�place le perso
            inputDirection = transform.right * lesInputs.bouge.x + transform.forward * lesInputs.bouge.y;
        }

        //Permet de bouger le perso selon son Character controller
        controller.Move(inputDirection.normalized * (vitesse * Time.deltaTime) + new Vector3(0.0f, vitesseVerticale, 0.0f) * Time.deltaTime);
    }


    //--------------------------------------------------------------------ANIMS-------------------------------------------------------------------------------------------------

    //D�but de l'animation de saut du perso
    void DebutAnim()
    {
        //Si on appuie sur la touche "espace" pour faire sauter le perso ET que celui-ci touche au sol (valeur � vrai) ET que le bool�en "debutAnim" est � faux
        if (lesInputs.saute && toucheSol && debutAnim == false)
        {
            //Rend le bool�en "debutAnim" � vrai
            debutAnim = true;

            //Enclenche le trigger "Saut" de l'Animator du perso
            animator.SetTrigger("Saut");
        }
    }


    //Fin de l'animation de saut du perso
    public void FinAnim(float valeur)
    {
        //La valeur de la hauteur du saut du perso est la valeur (valeur) donn�e dans le script "GestionAnims"
        hauteurSaut = valeur;

        //Rend le bool�en "debutAnim" � faux (pour que la m�thode "DebutAnim" puisse �tre ex�cut�e � nouveau)
        debutAnim = false;
    }


    //------------------------------------------------------------------------ATTAQUE-------------------------------------------------------------------------------------------

    void Attaque()
    {
        //Si le perso tire
        if (lesInputs.tire)
        {
            //Rend le bool�en "equipe" � vrai
            equipe = true;

            //Active la cam�ra 1re personne
            cinemachineFP.SetActive(true);

            //D�sactive la cam�ra 3e personne
            cameraTP.SetActive(false);

            //Active la mire
            mire.SetActive(true);

            //Rend le bool�en "Arme" de l'Animator du perso � vrai
            animator.SetBool("Arme", true);

            //Rend l'arme visible
            arme.SetActive(true);


            //Active le son de l'arme qui tire
            tire.GetComponent<AudioSource>().enabled = true;

            //Invoque les m�thodes "FiniTir" et "Recharge" dans 1 seconde
            Invoke("FiniTir", 1);
            Invoke("Recharge", 1);

            //Rend le bool�en "Arme" de l'Animator du perso � faux (pour pouvoir tirer � nouveau)
            lesInputs.tire = false;

            //Position de la mire
            Vector3 positionDepart = mire.transform.position;

            //D�lai pour retourner vers la cam�ra 3e personne (valeur mise � 5 secondes)
            cooldown = 5;


            //Collisions avec ce qui est touch�
            GameObject objetCollisions;

            //Trajectoire du tir
            RaycastHit hit;

            //Si la trajectoire du tir (selon la vis�e de la mire) (qui va vers l'avant) vise qq � une distance de 20m ou moins
            if (Physics.Raycast(positionDepart, mire.transform.TransformDirection(Vector3.forward), out hit, 20))
            {
                //Fait en sorte que ce qui est touch� soit touch� par la trajectoire du tir
                objetCollisions = hit.collider.transform.gameObject;

                //Si ce qui est touch� est un ennemi
                if (objetCollisions.tag == "Ennemi")
                {
                    Debug.Log("OK");
                    //D�truit l'ennemi
                    Destroy(objetCollisions);

                    //Augmente de 1 le nb d'ennemis tu�s par le perso
                    infosPerso.nbEnnemis++;

                    //Permet de savoir le nb d'ennemis tu�s par le perso dans la console
                    Debug.Log(infosPerso.nbEnnemis + " ennemis");
                }
            }
        }

        //Enclenche le d�lai pour passer � la cam�ra 3e personne (si le perso ne tire plus)
        cooldown -= Time.deltaTime;

        //Si le d�lai est plus petit ou �gal � 0
        if (cooldown <= 0)
        {
            //Rend le bool�en "equipe" � faux
            equipe = false;

            ////Rend le bool�en "Arme" de l'Animator du perso � faux
            animator.SetBool("Arme", false);

            //Rend l'arme invisible
            arme.SetActive(false);

            //Met la valeur du d�lai � -1 seconde (pour pas que celle-ci diminue � l'infini)
            cooldown = -1;

            //D�sactive la cam�ra 1re personne
            cinemachineFP.SetActive(false);

            //Active la cam�ra 3e personne
            cameraTP.SetActive(true);

            //D�sactive la mire
            mire.SetActive(false);
        }
    }


    //Recharger l'arme du perso
    void Recharge()
    {
        //Si l'Audio source de tirer est � faux
        if (tire.GetComponent<AudioSource>().enabled == false)
        {
            //Active l'Audio source de recharger
            recharge.GetComponent<AudioSource>().enabled = true;

            //Invoque la m�thode "DesactiveRecharge" dans 1 seconde
            Invoke("DesactiveRecharge", 1);
        }
    }


    //D�sactive l'Audio source de recharger
    void DesactiveRecharge()
    {
        recharge.GetComponent<AudioSource>().enabled = false;
    }


    //D�sactive l'Audio source de tirer
    void FiniTir()
    {
        tire.GetComponent<AudioSource>().enabled = false;
    }


    //-------------------------------------------------------------------CHUTE + T�L�PORTATION----------------------------------------------------------------------------------

    //Si le perso tombe dans la crevasse
    void Chute()
    {
        if (transform.position.y < 5 && transform.position.z > 30 && transform.position.z < 45)
        {
            //Active l'Audio source du perso
            GetComponent<AudioSource>().enabled = true;

            //Invoque la m�thode "RetourEnHaut" dans 1 seconde
            Invoke("RetourEnHaut", 1);
        }

        //Sinon
        else
        {
            //D�sactive l'Audio source du perso
            GetComponent<AudioSource>().enabled = false;
        }
    }


    //T�l�porte le perso � une position apr�s qu'il soit tomb� dans la crevasse
    void RetourEnHaut()
    {
        transform.position = new Vector3(15, 5.078046f, -90);

        //Active le panneau de chute
        panneauChute.SetActive(true);

        //Invoque la m�thode "EnleverPanneaux" dans 3 secondes
        Invoke("EnleverPanneaux", 3);
    }


    //-------------------------------------------------------------------D�SACTIVATION------------------------------------------------------------------------------------------

    //Permet de d�sactiver les panneaux actifs apr�s qu'ils aient �t� actifs
    void EnleverPanneaux()
    {
        panneauMaison.SetActive(false);

        panneauChute.SetActive(false);

        panneauRoger.SetActive(false);

        panneauPasFini1.SetActive(false);
    }


    //D�sactive l'Audio source de la douleur du perso (quand il est touch� par les ennemis)
    void DesactiveOuch()
    {
        ouch.GetComponent<AudioSource>().enabled = false;
    }


    //D�sactive l'Audio source des pi�ces
    void DesactivePieces()
    {
        pieces.GetComponent<AudioSource>().enabled = false;
    }


    //------------------------------------------------------------------------COLLISIONS----------------------------------------------------------------------------------------

    //Quand le perso entre en collision avec qq
    void OnTriggerEnter(Collider collision)
    {
        //Pi�ces
        if (collision.tag == "Piece")
        {
            //Active l'Audio source des pi�ces
            pieces.GetComponent<AudioSource>().enabled = true;

            //D�truit la pi�ce
            Destroy(collision.gameObject);

            //Augmnte le nb de pi�ces du perso de 1
            infosPerso.nbPieces++;

            //Permet de savoir le nb de pi�ces par le perso dans la console
            Debug.Log(infosPerso.nbPieces + " pi�ces");

            //Invoque la m�thode "DesactivePieces" dans 1 seconde
            Invoke("DesactivePieces", 1);
        }


        //Collision des ennemis (entre le cercle de d�tection de leur NavMesh et leur collider pour pas que le perso passe � travers eux)
        if (collision.tag == "CollisionsEnnemis")
        {
            //Active l'Audio source de la douleur du perso
            ouch.GetComponent<AudioSource>().enabled = true;

            //Active le panneau de chute
            panneauChute.SetActive(true);

            //Invoque la m�thode "EnleverPanneaux" dans 3 secondes
            Invoke("EnleverPanneaux", 3);

            //Invoque les m�thodes suivantes apr�s un certain d�lai
            Invoke("DesactiveOuch", 2);
            Invoke("EnleverPanneaux", 2);
        }


        //Collider si le perso a pas fini la mission 1 (- de 10 ennemis)
        if (collision.tag == "PasFini" && infosPerso.nbEnnemis < 10)
        {
            //Active le panneau PasFini1
            panneauPasFini1.SetActive(true);

            //Active le collider du mur invisible pour que le perso ne puisse pas entrer dans le village
            mursInvisibles.SetActive(true);

            //Invoque la m�thode "EnleverPanneaux" dans 2 secondes
            Invoke("EnleverPanneaux", 2);
        }


        //Collider si le perso a fini la mission 1 (10 ennemis)
        if (collision.tag == "PasFini" && infosPerso.nbEnnemis == 10)
        {
            //Dit � la cam�ra principale d'arr�ter sa musique
            GameObject.Find("Main Camera").GetComponent<AudioSource>().enabled = false;

            //Active l'Audio source du village
            GameObject.Find("Village").GetComponent<AudioSource>().enabled = true;
        }


        //Collider des murs invisbles si le perso n'a pas visit� la maison abandonn�e
        if (collision.tag == "Maison")
        {
            //Rend actif les murs invisibles
            murMaison1.GetComponent<BoxCollider>().enabled = true;
            murMaison2.GetComponent<BoxCollider>().enabled = true;

            //Rend le panneau indiquant d'aller voir la maison abandonn�e actif
            panneauMaison.SetActive(true);

            //Invoque la m�thode "EnleverPanneaux" dans 2 secondes
            Invoke("EnleverPanneaux", 3);
        }


        //Collider du pauvre Roger (squelette)
        if (collision.tag == "Roger")
        {
            //Active le panneau de Roger
            panneauRoger.SetActive(true);

            //Invoque la m�thode "EnleverPanneaux" dans 2 secondes
            Invoke("EnleverPanneaux", 2);
        }
    }


    //------------------------------------------------------------------------VICTOIRE------------------------------------------------------------------------------------------

    //Quand le perso a tu�s tous les ennemis ET a ramass� toutes les pi�ces
    void Victoire()
    {
        if (infosPerso.nbPieces == 5 && infosPerso.nbEnnemis == 10)
        {
            //Active l'Audio source de la victoire
            victoire.GetComponent<AudioSource>().enabled = true;

            //Invoque la m�thode "SceneSuivante" dans 2 secondes
            Invoke("SceneSuivante", 2);
        }
    }


    //Aller � la sc�ne suivante
    void SceneSuivante()
    {
        //Dit � la cam�ra principale d'arr�ter sa musique
        GameObject.Find("Main Camera").GetComponent<AudioSource>().enabled = false;

        //Charge la sc�ne "Base"
        SceneManager.LoadScene("Base");
    }


    //------------------------------------------------------------------------SAUT----------------------------------------------------------------------------------------------

    //D�termine si le perso touche au sol ou non (cr�ation d'une sph�re sous le perso)
    void VerifierSol()
    {
        Vector3 spherePosition = new Vector3(transform.position.x, transform.position.y - distanceDuSol, transform.position.z);
        toucheSol = Physics.CheckSphere(spherePosition, GroundedRadius, layerSol, QueryTriggerInteraction.Ignore);
    }


    //Permet au perso de sauter
    void SautEtGravite()
    {
        //Applique la gravit� au fil du temps si la vitesse verticale est sous la vitesse finale
        if (vitesseVerticale < vitesseFinale)
        {
            vitesseVerticale += Gravite * Time.deltaTime;
        }

        //Si le perso touche au sol
        if (toucheSol)
        {
            //R�initialise le d�lai de chute
            chuteTimeout = delaiChute;

            //Emp�che la v�locit� de baisser � l'infini quand le perso touche au sol
            if (vitesseVerticale < 0.0f)
            {
                vitesseVerticale = -2f;
            }

            //Si on appuie sur la barre d'espace ET que le d�lai pour sauter � nouveau est de 0 : Saute
            if (lesInputs.saute && sautTimeout <= 0.0f)
            {
                vitesseVerticale = Mathf.Sqrt(hauteurSaut * -2f * Gravite);
            }

            //Enclenche le d�lai pour sauter � nouveau
            if (sautTimeout >= 0.0f)
            {
                sautTimeout -= Time.deltaTime;
            }
        }

        //Sinon
        else
        {
            //R�initialise le d�lai pour sauter � nouveau
            sautTimeout = delaiSaut;

            //R�initialise le d�lai de chute
            if (chuteTimeout >= 0.0f)
            {
                chuteTimeout -= Time.deltaTime;
            }

            //Si le perso ne touche pas au sol : ne Saute PAS
            lesInputs.saute = false;
        }
    }
}