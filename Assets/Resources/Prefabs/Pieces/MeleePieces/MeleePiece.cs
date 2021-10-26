using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleePiece : PieceBase
{
    [Header("Will the piece move?")]
    public bool _stationary = false;

    public override void Awake()
    {
        base.Awake();
    }

    public override void Start()
    {
        base.Start();
    }

    public override void Update()
    {
        base.Update();
    }

    public override void HandleActions()
    {

        if (targetPosition == (Vector2)this.transform.position || _stationary)
        {
            if (_target != null)
            {
                if (_fm.GetNumStepsToTarget(targetPosition, _target.targetPosition) == 1)
                {
                    if ((Vector2)_target.transform.position == _target.targetPosition && _actionTimer <= 0 && _target.CanBeAttacked())
                    {
                        _actionTimer = Stats.attackSpeed;
                        ChangeAnimationState(ePieceAction.Attack);
                    }
                    else if ((Vector2)_target.transform.position == _target.targetPosition)
                    {
                        _actionTimer -= Time.deltaTime;
                        ChangeAnimationState(ePieceAction.Idle);
                    }
                    else
                    {
                        ChangeAnimationState(ePieceAction.Idle);
                    }
                }
                else
                {
                    if (!_stationary)
                    {
                        previousPosition = targetPosition;
                        targetPosition = GetStepTowardsTarget();
                    }
                    _actionTimer = Stats.attackSpeed / 2f;
                }
            }
            else
            {
                ChangeAnimationState(ePieceAction.Idle);
                _actionTimer = Stats.attackSpeed / 2f;
            }
        }
        else
        {
            _status = ePieceStatus.Moving;
            _actionTimer = Stats.attackSpeed / 2f;
            ChangeAnimationState(ePieceAction.Move);
            this.transform.position = Vector2.MoveTowards(this.transform.position, targetPosition, this.Stats.moveSpeed * Time.deltaTime * GameManager.Instance.gameSpeed);
        }
    }
}
