using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpenLinks : MonoBehaviour
{
    public void OpenWishList()
    {
        Application.OpenURL("https://store.steampowered.com/app/1244300/The_Astro_Parcel_Service/");
    }

    public void OpenTwitter()
    {
        Application.OpenURL("https://twitter.com/AstroParcel");
    }
}
