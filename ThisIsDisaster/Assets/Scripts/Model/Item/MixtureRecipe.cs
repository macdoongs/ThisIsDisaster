using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MixtureRecipe
{

    public  List<long> MaterialID = new List<long>();
    public List<int> MaterialNum = new List<int>();

    public long resultID;

    public MixtureRecipe(long resultID, Dictionary<long, int> matDic)
    {
        this.resultID = resultID;

        foreach (var mat in matDic)
        {
            MaterialID.Add(mat.Key);
            MaterialNum.Add(mat.Value);
        }
    }

}
