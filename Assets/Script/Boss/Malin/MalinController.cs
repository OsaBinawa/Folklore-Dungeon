using UnityEngine;
public enum MalinPhase
{
    Hidden,
    Stunned,
    Active,
    FinalPhase,
    SelfDestruct
}
public class MalinController : MonoBehaviour
{
    public MainBody body;
    public LeftHand leftHand;
    public RightHand rightHand;

    private bool leftDead;
    private bool rightDead;

    private int stunTurns;
    private int selfDestructTurns;

    public void HandDestroyed(MainHand hand)
    {
        if (hand is LeftHand)
        {
            leftDead = true;
            body.SetWeakness(ElementType.Typo);
        }

        if (hand is RightHand)
        {
            rightDead = true;
            body.SetWeakness(ElementType.Miss);
        }

        stunTurns = 3;
        body.EnterStun();

        if (leftDead && rightDead)
        {
            body.RemoveWeakness();
            body.EnterFinalPhase();
            selfDestructTurns = 10;
        }
    }

    public void OnPlayerTurn()
    {
        if (stunTurns > 0)
        {
            stunTurns--;
            if (stunTurns == 0)
                body.ExitStun();
        }

        if (selfDestructTurns > 0)
        {
            selfDestructTurns--;
            if (selfDestructTurns == 0)
                body.TriggerSelfDestruct();
        }
    }
}

