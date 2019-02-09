using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface INpc
{
    void Talk();
}

public interface IMerchantNPC
{
    List<int> GetSellingItemIDList();
    void SetSellingItemIDList(List<int> ids);
}