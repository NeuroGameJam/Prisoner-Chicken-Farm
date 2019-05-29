using UnityEngine;

public class Chicken: MonoBehaviour
{
    Animator anim;

    private void Start()
    {
        anim = GetComponent<Animator>();
    }
    public void Kill()
    {
        anim.SetBool("die", true);
    }
}