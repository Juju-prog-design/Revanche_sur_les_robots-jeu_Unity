using UnityEngine;


public class Ennemis : MonoBehaviour
{
    // Prefabs des ennemis
    [SerializeField] private GameObject ennemiGros;
    [SerializeField] private GameObject ennemiPetit;

    // Liste des lieux où les ennemis apparaissent
    [SerializeField] private Vector3[] listeEmplacementsAleatoiresGros;
    [SerializeField] private Vector3[] listeEmplacementsAleatoiresPetits;

    // Nb de Gros ennemis et leur nb max 
    private int nbGrosEnnemis = 0;
    private const int nbGrosEnnemisMax = 5;

    // Nb de Petits ennemis et leur nb max 
    private int nbPetitsEnnemis = 0;
    private const int nbPetitsEnnemisMax = 5;


    // Est appelée au lancement de la scène
    void Start()
    {
        CreerAleatoireGros();
        CreerAleatoirePetits();
    }


    // Fait en sorte que les Gros ennemis apparaissent aléatoirement sur la scène selon leur liste
    void CreerAleatoireGros()
    {
        //Si le nb de Gros ennemis est inférieur au nb max, fait apparaitre cet ennemi aléatoirement
        if (nbGrosEnnemis < nbGrosEnnemisMax)
        {
            // Augmente le nb total de cet ennemi de 1
            nbGrosEnnemis++;

            // Valeur aléatoire dans la liste des endroits d'apparition 
            int nbAleatoire = Random.Range(0, listeEmplacementsAleatoiresGros.Length);

            // Fait apparaitre l'ennemi de manière aléatoire
            GameObject unEnnemiGros = Instantiate(ennemiGros, new Vector3(0, 0, 0), Quaternion.identity);
            unEnnemiGros.transform.position = listeEmplacementsAleatoiresGros[nbAleatoire];

            // Met l'ennemi dans le parent attitré et lui donne le bon nom
            unEnnemiGros.transform.parent = GameObject.Find("EnnemisGros").transform;
            unEnnemiGros.name = "EnnemiGros " + nbGrosEnnemis;

            // Invoque le délai entre les apparitions d'ennemis
            Invoke(nameof(CreerAleatoireGros), 7f);
        }
    }


    // Fait en sorte que les Petits ennemis apparaissent aléatoirement sur la scène selon leur liste
    void CreerAleatoirePetits()
    {
        // Si le nb de Petits ennemis est inférieur au nb max, fait apparaitre cet ennemi aléatoirement
        if (nbPetitsEnnemis < nbPetitsEnnemisMax)
        {
            // Augmente le nb total de cet ennemi de 1
            nbPetitsEnnemis++;

            // Valeur aléatoire dans la liste des endroits d'apparition 
            int nbAleatoire = Random.Range(0, listeEmplacementsAleatoiresPetits.Length);

            // Fait apparaitre l'ennemi de manière aléatoire
            GameObject unEnnemiPetit = Instantiate(ennemiPetit, new Vector3(0, 0, 0), Quaternion.identity);
            unEnnemiPetit.transform.position = listeEmplacementsAleatoiresPetits[nbAleatoire];

            // Met l'ennemi dans le parent attitré et lui donne le bon nom
            unEnnemiPetit.transform.parent = GameObject.Find("EnnemisPetits").transform;
            unEnnemiPetit.name = "EnnemiPetit " + nbPetitsEnnemis;

            // Invoque le délai entre les apparitions d'ennemis
            Invoke(nameof(CreerAleatoirePetits), 7f);
        }
    }
}