using UnityEngine;


public class GestionAnims : MonoBehaviour
{
    //� un certain point dans les animations de saut (avec et sans arme), il y a un event (�v�nement) o� cette m�thode est appell�e
    public void MessageFin(float valeur)
    {
        //Envoi la valeur d'un float (1.2) � la m�thode "FinAnim" du script "BougePerso" qui va �tre la valeur de la hauteur du saut du perso
        transform.parent.SendMessage("FinAnim", valeur);
    }
}
