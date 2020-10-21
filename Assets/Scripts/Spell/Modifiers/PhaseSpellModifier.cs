﻿using System;
using UnityEngine;

class PhaseSpellModifier : SpellModifier
{
    [SerializeField]
    private int phaseAmount = 3;
    public override SpellBase ModifyBehaviour(SpellBase action)
    {
        //important to make sure it doesnt cast a recursive method
        Action oldBehavior = action.behaviour;
        
        Action spell = () =>
        {
            oldBehavior.Invoke();
            var existingPhase = action._objectForSpell.GetComponent<Phasing>();
            if(existingPhase == null)
                action._objectForSpell.AddComponent<Phasing>()._damage = action._damage;
            else
            {
                existingPhase.AddPhaseAmount(phaseAmount);
            }
        };
        
        action.behaviour = spell;
        return action;
    }

    public override void UseValue()
    {
        phaseAmount = Mathf.RoundToInt(phaseAmount * value);
    }

    public override Tooltip GetTooltip()
    {
        return new Tooltip("Phase <Modifier>", $"Causes affected entities to pass through solid objects up to {phaseAmount} times");
    }
}