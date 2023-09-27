using UnityEngine;


public class TexteMission1 : MonoBehaviour
{
    //Panneaux de la mission 1 et celui où le perso est en colère après avoir lu celui de la mission 1
    [SerializeField] private GameObject panneauMission1;
    [SerializeField] private GameObject panneauVengeance;

    //Murs invisibles pour empêcher le perso d'aller trop loin s'il n'a pas lu la mission 1
    [SerializeField] private GameObject murMaison1;
    [SerializeField] private GameObject collisionsMurMaison1;
    [SerializeField] private GameObject murMaison2;
    [SerializeField] private GameObject collisionsMurMaison2;

    //GameObject ayant l'Audio source du perso en colère
    [SerializeField] private GameObject enColere;

    //Valeur pour savoir si le perso est en colère ou non
    private float colere;


    //Est appelée au lancement de la scène
    void Start()
    {
        //Permet de réinitialiser la valeur de colère dans le Start
        colere = 0;
    }


    //Si le collider de la mission 1 entre en contact avec le tag "Player"
    void OnTriggerEnter(Collider collision)
    {
        if (collision.tag == "Player")
        {
            //Rend le panneau de la mission 1 actif
            panneauMission1.SetActive(true);

            //Augmente la valeur de colère du perso
            colere++;
        }
    }


    //Si le tag "Player" sort du collider de la mission 1
    void OnTriggerExit(Collider collision)
    {
        //Désactive le panneau de la mission 1
        panneauMission1.SetActive(false);

        //Désactive les murs invisibles (voir dans le script "BougePerso" dans les méthodes OnTriggerEnter)
        murMaison1.GetComponent<BoxCollider>().enabled = false;
        collisionsMurMaison1.SetActive(false);
        murMaison2.GetComponent<BoxCollider>().enabled = false;
        collisionsMurMaison2.SetActive(false);

        //Si la valeur de colère du perso n'est pas égal à 0 (permet d'empêcher la première ligne de code plus bas de s'exécuter à chaque fois que l'on sort du collider)
        if (colere != 0)
        {
            //Active les Audios sources de la colère
            enColere.GetComponent<AudioSource>().enabled = true;

            //Active le panneau vengeance
            panneauVengeance.SetActive(true);

            //Invoque la méthode "EnleverPanneau" dans 2 secondes
            Invoke("EnleverPanneau", 2);
        }
    }


    //Désactiver le panneau vengeance
    void EnleverPanneau()
    {
        panneauVengeance.SetActive(false);
    }
}
