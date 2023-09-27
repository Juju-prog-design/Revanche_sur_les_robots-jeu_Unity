using UnityEngine;


//Permet de créer un SO dans le dossier Scripts
[CreateAssetMenu(fileName = "InfosPerso", menuName = "SO/NouveauPerso")]


public class InfosPerso : ScriptableObject
{
    //Nb de pièces
    public int nbPieces = 0;

    //Nb d'ennemis tués
    public int nbEnnemis = 0;
}