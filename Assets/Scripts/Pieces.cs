using UnityEngine;

public class Pieces : MonoBehaviour
{
    //Prefabs des pièces
    [SerializeField] private GameObject pieces;

    //Liste des lieux où les pièces apparaissent
    [SerializeField] private Vector3[] listeEmplacementsAleatoiresPieces;

    //Nombre de pièces et leur nombre max 
    private int nbPieces = 0;
    private const int nbPiecesMax = 5;


    //Est appelée au lancement de la scène
    void Start()
    {
        CreerAleatoirePieces();
    }


    //Fait en sorte que les pièces apparaissent aléatoirement sur la scène selon leur liste
    void CreerAleatoirePieces()
    {
        //Si le nombre de pièces est inférieur au nombre max, fait apparaitre une pièce aléatoirement
        if (nbPieces < nbPiecesMax)
        {
            //Augmente le nombre total de pièces de 1
            nbPieces++;

            //Valeur aléatoire dans la liste des endroits d'apparition 
            int nbAleatoire = Random.Range(0, listeEmplacementsAleatoiresPieces.Length);

            //Fait apparaitre la pièce de manière aléatoire
            GameObject unePiece = Instantiate(pieces, new Vector3(0, 0, 0), Quaternion.identity);
            unePiece.transform.position = listeEmplacementsAleatoiresPieces[nbAleatoire];

            //Met la pièce dans le parent attitré et lui donne le bon nom
            unePiece.transform.parent = GameObject.Find("Pieces").transform;
            unePiece.name = "Piece " + nbPieces;

            //Invoque le délai entre les apparitions des pièces
            Invoke("CreerAleatoirePieces", 7);
        }
    }
}
