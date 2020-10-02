﻿using UnityEngine;

class SpeedSpellModifier : SpellModifier
{
    public override void ModifySpell(SpellBase spell)
    {
        spell._speed += 1f;
        spell._speed *= 2;
        spell._cooldown /= 1.4f;
    }
}