﻿using System;
using System.Collections.Generic;
using KRpgLib.Flags;

namespace FlagsUnitTests
{
    public class FakeFlagProvider : IFlagProvider
    {
        public static FakeFlagTemplate FakeFlagTemplate_VariantCount1 = new FakeFlagTemplate(variantCount: 1);

        private readonly List<Flag> _flagsProvided;
        public FakeFlagProvider(params Flag[] flagsProvided)
        {
            _flagsProvided = new List<Flag>(flagsProvided);
        }

        public List<Flag> GetAllFlags()
        {
            return new List<Flag>(_flagsProvided);
        }
    }
    public class FakeFlagProvider_Dynamic : IFlagProvider_Dynamic
    {
        public static readonly FakeFlagTemplate FlagTemplateProvided_InTrueState = new FakeFlagTemplate();
        public static readonly FakeFlagTemplate FlagTemplateProvided_InFalseState = new FakeFlagTemplate();

        private bool _state;

        public event Action OnFlagsChanged;

        public FakeFlagProvider_Dynamic()
        {
            _state = false;
        }

        public void ToggleState()
        {
            _state = !_state;

            OnFlagsChanged?.Invoke();
        }

        public List<Flag> GetAllFlags()
        {
            var templateUsed = _state ? FlagTemplateProvided_InTrueState : FlagTemplateProvided_InFalseState;

            return new List<Flag>() { Flag.Create(templateUsed, 0) };
        }
    }
}
