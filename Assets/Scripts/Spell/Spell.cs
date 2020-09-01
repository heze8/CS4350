﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Spell
{
    [SerializeField]
    public SpellBaseType _spellBaseType;
    [SerializeField]
    public List<SpellModifier> _spellModifiers =  new List<SpellModifier>() ;
    
    //Modifiable properties
    public class SpellProperties
    {
        public int iterations = 1;
    }

    [SerializeField] public SpellProperties _spellProperties =  new SpellProperties();

    public void CastSpell(Vector3 mouseDirection = new Vector3())
    {
        _spellBaseType._posDiff = mouseDirection;
        SpellModifier.SpellChain spell = new SpellModifier.SpellChain(()   => _spellBaseType.SpellBehaviour(this));
        
        if (_spellModifiers != null && _spellModifiers.Count != 0)
        {
            foreach (var modifier in _spellModifiers)
            {
                modifier.ModifySpell(this);
                spell = modifier.ModifyBehaviour(spell);
            }

        }
        spell.spell.Invoke();
        
    }
    
}