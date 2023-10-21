using Core.AI.Brain;
using Core.GS.Enums;

namespace Core.GS.AI.Brains;

/// <summary>
/// Interface for controllable brains
/// </summary>
public interface IControlledBrain
{
    EWalkState WalkState { get; }
    EAggressionState AggressionState { get; set; }
    GameNpc Body { get; }
    GameLiving Owner { get; }
    void Attack(GameObject target);
    void Disengage();
    void Follow(GameObject target);
    void FollowOwner();
    void Stay();
    void ComeHere();
    void Goto(GameObject target);
    void UpdatePetWindow();
    GamePlayer GetPlayerOwner();
    GameNpc GetNPCOwner();
    GameLiving GetLivingOwner();
    void SetAggressionState(EAggressionState state);
    bool IsMainPet { get; set; }
}