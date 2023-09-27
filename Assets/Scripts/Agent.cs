using UnityEngine;
using UnityEngine.AI;


public class Agent : MonoBehaviour
{
    //Composante NavMeshAgent des ennemis ainsi que leur cible (le perso)
    [SerializeField] NavMeshAgent agent;

    //Bool�en qui d�termine si les ennemis peuvent ou non suivrent le perso (de base � faux)
    private bool peut = false;


    //Est appel�e � chaque frame
    void Update()
    {
        //Si le bool�en "peut" est � vrai
        if (peut)
        {
            //Donne la destination (perso) aux ennemis
            agent.destination = GameObject.Find("Perso").transform.position;

            //Permet que les ennemis suivent du regard le perso
            agent.updateRotation = true;
        }
    }


    //Si le collider des ennemis entre en contact avec le tag "Player"
    void OnTriggerEnter(Collider collision)
    {
        if (collision.tag == "Player")
        {
            //Met le bool�en "peut" � vrai
            peut = true;

            //Active l'Audio source des ennemis
            GetComponent<AudioSource>().enabled = true;

            //Active la composante NavMeshAgent des ennemis (pour qu'ils suivent le perso)
            GetComponent<NavMeshAgent>().enabled = true;
        }
    }


    //Si le tag "Player" sort du collider des ennemis
    void OnTriggerExit(Collider collision)
    {
        //Met le bool�en "peut" � faux
        peut = false;

        //D�sactive l'Audio source des ennemis
        GetComponent<AudioSource>().enabled = false;

        //D�sactive la composante NavMeshAgent des ennemis (pour qu'ils arr�tent de suivre le perso)
        GetComponent<NavMeshAgent>().enabled = false;
    }
}
