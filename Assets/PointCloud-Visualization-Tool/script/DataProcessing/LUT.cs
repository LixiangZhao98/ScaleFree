using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class LUT   //LookUpTable
{[SerializeField]
    List<int> LUTUnit_;
    public LUT()
    {
        LUTUnit_ = new List<int>();
    }
    public void AddToLUT(int targetint)
    {
        LUTUnit_.Add(targetint);
    }
    public int GetLTUnitNum()
    {
        return LUTUnit_.Count;
    }
    public List<int> GetLTUnit()
    {
        return LUTUnit_;
    }
}