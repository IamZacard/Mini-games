using UnityEngine;
public class InputHandler : MonoBehaviour
{
    public float Horizontal { get; private set; }
    public bool JumpPressed { get; private set; }
    public bool RunTogglePressed { get; private set; }
    public bool DashPressed { get; private set; }
    public bool MeleeAttackPressed { get; private set; }
    public bool RangedAttackPressed { get; private set; }
    private void Update()
    {
        Horizontal = Input.GetAxisRaw("Horizontal");
        if (Input.GetKeyDown(KeyCode.Space))
            JumpPressed = true;
        if (Input.GetKeyDown(KeyCode.LeftShift))
            RunTogglePressed = true;
        if (Input.GetKeyDown(KeyCode.LeftControl))
            DashPressed = true;
        if (Input.GetMouseButtonDown(0))
            MeleeAttackPressed = true;
        if (Input.GetMouseButtonDown(1))
            RangedAttackPressed = true;
    }
    public void ConsumeInputs()
    {
        JumpPressed = false;
        RunTogglePressed = false;
        DashPressed = false;
        MeleeAttackPressed = false;
        RangedAttackPressed = false;
    }
}