using UnityEngine;


public class TexteMission1 : MonoBehaviour
{
    //Panneaux de la mission 1 et celui o� le perso est en col�re apr�s avoir lu celui de la mission 1
    [SerializeField] private GameObject panneauMission1;
    [SerializeField] private GameObject panneauVengeance;

    //Murs invisibles pour emp�cher le perso d'aller trop loin s'il n'a pas lu la mission 1
    [SerializeField] private GameObject murMaison1;
    [SerializeField] private GameObject collisionsMurMaison1;
    [SerializeField] private GameObject murMaison2;
    [SerializeField] private GameObject collisionsMurMaison2;

    //GameObject ayant l'Audio source du perso en col�re
    [SerializeField] private GameObject enColere;

    //Valeur pour savoir si le perso est en col�re ou non
    private float colere;


    //Est appel�e au lancement de la sc�ne
    void Start()
    {
        //Permet de r�initialiser la valeur de col�re dans le Start
        colere = 0;
    }


    //Si le collider de la mission 1 entre en contact avec le tag "Player"
    void OnTriggerEnter(Collider collision)
    {
        if (collision.tag == "Player")
        {
            //Rend le panneau de la mission 1 actif
            panneauMission1.SetActive(true);

            //Augmente la valeur de col�re du perso
            colere++;
        }
    }


    //Si le tag "Player" sort du collider de la mission 1
    void OnTriggerExit(Collider collision)
    {
        //D�sactive le panneau de la mission 1
        panneauMission1.SetActive(false);

        //D�sactive les murs invisibles (voir dans le script "BougePerso" dans les m�thodes OnTriggerEnter)
        murMaison1.GetComponent<BoxCollider>().enabled = false;
        collisionsMurMaison1.SetActive(false);
        murMaison2.GetComponent<BoxCollider>().enabled = false;
        collisionsMurMaison2.SetActive(false);

        //Si la valeur de col�re du perso n'est pas �gal � 0 (permet d'emp�cher la premi�re ligne de code plus bas de s'ex�cuter � chaque fois que l'on sort du collider)
        if (colere != 0)
        {
            //Active les Audios sources de la col�re
            enColere.GetComponent<AudioSource>().enabled = true;

            //Active le panneau vengeance
            panneauVengeance.SetActive(true);

            //Invoque la m�thode "EnleverPanneau" dans 2 secondes
            Invoke("EnleverPanneau", 2);
        }
    }


    //D�sactiver le panneau vengeance
    void EnleverPanneau()
    {
        panneauVengeance.SetActive(false);
    }
}
