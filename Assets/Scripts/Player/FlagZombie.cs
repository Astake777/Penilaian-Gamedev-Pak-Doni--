using UnityEngine;

// INHERITANCE + POLYMORPHISM
// FlagZombie mewarisi Enemy, lalu meng-override Serang().
public class FlagZombie : Enemy
{
    public bool flag = true;

    public override void Serang()
    {
        Debug.Log("FlagZombie Gigit");
    }
}
