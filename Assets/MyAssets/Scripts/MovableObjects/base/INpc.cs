using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface INpc
{
    void Talk();
}

public interface IMerchantNPC : INpc
{
    List<int> GetSellingItemIDList();
    void SetSellingItemIDList(List<int> ids);
}