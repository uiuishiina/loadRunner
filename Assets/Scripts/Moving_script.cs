using System.Collections;
using UnityEngine;

public class Moving_script : MonoBehaviour
{
   public bool Move(Vector2 vec,Vector3 pos)
   {
        var target = new Vector3(pos.x + vec.x, pos.y + vec.y, pos.z);
        Vector3.MoveTowards(pos, target, 0.2f);
        return true;
   }
}
